using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Serilog.Core;

public static class StoreContextAndSetExtensions
{
    public static async Task<IEnumerable<T>?> AttemptFullSetFetch<T>(this DbSet<T> dbSet, Serilog.ILogger logger) where T : class
    {
        int attempt = 1;
        while (attempt <= 10)
        {
            try
            {
                var contextObjects = await dbSet.ToListAsync();
                return contextObjects;
            }
            catch (Exception ex)
            {
                logger?.Error<Exception>(ex.Message, ex);
                Thread.Sleep(100);
                attempt++;
            }
        }

        return null;
    }

    public static async Task<T?> AttemptItemFetchById<T>(this DbSet<T> dbSet, int id, Serilog.ILogger logger) where T : class
    {
        int attempt = 1;
        while (attempt <= 10)
        {
            try
            {
                var contextObject = await dbSet.FindAsync(id);
                return contextObject;
            }
            catch (Exception ex)
            {
                logger?.Error<Exception>(ex.Message, ex);
                Thread.Sleep(100);
                attempt++;
            }
        }

        return null;
    }

    public static async Task<IEnumerable<T>?> AttemptItemFetchByProp<T>(this DbSet<T> dbSet, string propName, object propVal, Serilog.ILogger logger) where T : class
    {
        int attempt = 1;
        while (attempt <= 10)
        {
            try
            {
                var parameter = Expression.Parameter(typeof(T), "item");
                var property = Expression.Property(parameter, propName);
                var constant = Expression.Constant(propVal);
                var equality = Expression.Equal(property, constant);
                var lambda = Expression.Lambda<Func<T, bool>>(equality, parameter);

                // Where (item => item.propName == propVal);

                var contextObjects = await dbSet.Where(lambda).ToListAsync();
                return contextObjects;
            }
            catch (Exception ex)
            {
                logger?.Error<Exception>(ex.Message, ex);
                Thread.Sleep(100);
                attempt++;
            }
        }

        return null;
    }

    private static bool IsValid<T>(this T setObject, string[] validationProps, StringBuilder stringBuilder, Serilog.ILogger logger)
    {
        // Just check if object values are not null or 0..
        if (validationProps != null && validationProps.Length > 0)
        {
            foreach (var propName in validationProps)
            {
                try
                {
                    var objProperty = setObject.GetPropertyInfo(propName, logger);
                    if (objProperty == null) return false;

                    var objPropertyType = objProperty.GetType();
                    var objPropValue = objProperty.GetPropertyValue(setObject, logger);


                    if (objPropertyType == typeof(decimal) || objPropertyType == typeof(double) || objPropertyType == typeof(int))
                    {
                        var valueAsDouble = Convert.ToDouble(objPropValue);
                        if (valueAsDouble == 0)
                        {
                            stringBuilder.AppendLine($"Can't add object with 0 for {objProperty.Name}");
                            return false;
                        }
                    }
                    else if (objPropertyType == typeof(string))
                    {
                        var valueAsString = Convert.ToString(objPropValue);
                        if (string.IsNullOrWhiteSpace(valueAsString))
                        {
                            stringBuilder.AppendLine($"Can't add object with empty/null value for {objProperty.Name}");
                            return false;
                        }
                    }
                    else
                    {
                        if (objPropValue == null)
                        {
                            stringBuilder.AppendLine($"Can't add object with null value for {objProperty.Name}");
                            return false;
                        }
                    }

                }
                catch (Exception ex)
                {
                    logger?.Error<Exception>(ex.Message, ex);
                }
            }
        }

        return true;
    }

    private static async Task<bool> EntitiesAlreadyExist<T>(this T setObject, string[] existingPropNames, DbSet<T> dbSet, StringBuilder stringBuilder, Serilog.ILogger logger) where T : class
    {
        if (existingPropNames != null && existingPropNames.Length > 0)
        {
            try
            {
                var expression = BuildExpression(setObject, existingPropNames, logger);
                var existingObjects = await dbSet.Where(expression).ToListAsync();
                if (existingObjects.Count > 0)
                {
                    stringBuilder.AppendLine($"{existingObjects.Count} existing objects exist with crit: ({expression})");
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger?.Error<Exception>(ex.Message, ex);
            }
        }

        return false;
    }

    public static async Task<UpdateResult> AttemptUpdate<T>(this DbContext context, DbSet<T> dbSet, IEnumerable<T> setObjects, string[] validationProps, string[] existingObjectProps, Serilog.ILogger logger) where T : class
    {
        int attempt = 1;

        while (attempt <= 10)
        {
            try
            {
                var result = new UpdateResult();
                StringBuilder stringBuilder = new StringBuilder();
                List<T> itemsToAdd = new List<T>();

                // Loop products and save 1 at a time?
                foreach (var setObject in setObjects)
                {
                    // Make sure object is valid
                    if (!setObject.IsValid(validationProps, stringBuilder, logger))
                    {
                        stringBuilder.AppendLine($"Skipped {setObject} - Invalid properties");
                        continue;
                    }

                    // Find any matching products if we define the prop names
                    if (await setObject.EntitiesAlreadyExist(existingObjectProps, dbSet, stringBuilder, logger))
                    {
                        stringBuilder.AppendLine($"Skipped {setObject} - Existing entities exist");
                        continue;
                    }

                    itemsToAdd.Add(setObject);
                }

                if (itemsToAdd.Count > 0)
                {
                    dbSet.AddRange(itemsToAdd);
                    await context.SaveChangesAsync();
                }

                stringBuilder.AppendLine($"Successfully added {itemsToAdd.Count} to set");

                result.UpdateResultObject = await dbSet.AttemptFullSetFetch(logger);
                result.Success = true;
                result.Message = stringBuilder.ToString().TrimEnd();

                logger?.Information(stringBuilder.ToString());
                return result;
            }
            catch (Exception ex)
            {
                logger?.Error<Exception>(ex.Message, ex);
                Thread.Sleep(100);
                attempt++;
            }
        }

        return new UpdateResult() { Success = false, Message = "Error adding objects" };
    }

    public static async Task<UpdateResult> AttemptDelete<T>(this DbContext context, DbSet<T> dbSet, IEnumerable<int> objectIds, Serilog.ILogger logger) where T : class
    {
        int attempt = 1;

        while (attempt <= 10)
        {
            try
            {
                var result = new UpdateResult();
                StringBuilder stringBuilder = new StringBuilder();
                int deletedItems = 0;

                // Loop object ids and remove 1 at a time
                foreach (var objectId in objectIds)
                {
                    var itemToDelete = await dbSet.AttemptItemFetchById(objectId, logger);
                    if (itemToDelete == null)
                    {
                        stringBuilder.AppendLine($"Cannot find {typeof(T).Name} with id {objectId}");
                        continue;
                    }

                    dbSet.Remove(itemToDelete);
                    deletedItems += 1;
                }

                if (deletedItems > 0)
                {
                    await context.SaveChangesAsync();
                    stringBuilder.AppendLine($"Successfully removed {deletedItems} from set");
                }

                result.UpdateResultObject = await dbSet.AttemptFullSetFetch(logger);
                result.Success = true;
                result.Message = stringBuilder.ToString().TrimEnd();

                logger?.Information(stringBuilder.ToString());
                return result;
            }
            catch (Exception ex)
            {
                logger?.Error<Exception>(ex.Message, ex);
                Thread.Sleep(100);
                attempt++;
            }
        }

        return new UpdateResult() { Success = false, Message = "Error deleting objects" };
    }



    private static PropertyInfo? GetPropertyInfo<T>(this T tObject, string propName, Serilog.ILogger logger)
    {
        try
        {
            var propInfo = tObject?.GetType().GetProperty(propName);
            return propInfo;
        }
        catch (Exception ex)
        {
            logger?.Error<Exception>(ex.Message, ex);
            return null;
        }
    }

    private static object? GetPropertyValue<T>(this PropertyInfo property, T tObject, Serilog.ILogger logger)
    {
        try
        {
            var propValue = property.GetValue(tObject);
            return propValue;
        }
        catch (Exception ex)
        {
            logger?.Error<Exception>(ex.Message, ex);
            return null;
        }
    }

    private static Expression<Func<T, bool>> BuildExpression<T>(T setObject, string[] propNames, Serilog.ILogger logger)
    {
        try
        {
            var parameter = Expression.Parameter(typeof(T), "item");
            Expression? combined = null;

            foreach (var propName in propNames.EmptyIfNull())
            {
                var propInfo = setObject.GetPropertyInfo(propName, logger);
                if (propInfo == null) continue;

                var propValue = propInfo.GetPropertyValue(setObject, logger);
                if (propValue == null) continue;

                var property = Expression.Property(parameter, propName);
                var constant = Expression.Constant(propValue);
                var equality = Expression.Equal(property, constant);

                combined = combined == null ? equality : Expression.AndAlso(combined, equality);
            }

            if (combined == null)
            {
                return x => true;
            }

            return Expression.Lambda<Func<T, bool>>(combined, parameter);
        }
        catch (Exception ex)
        {
            logger?.Error<Exception>(ex.Message, ex);
            return x => true;
        }
    }
}