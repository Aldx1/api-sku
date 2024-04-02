public static class ExtensionMethods
{

    /// <summary>
    /// Add status returns to route
    /// </summary>
    /// <param name="routeHandler"></param>
    /// <param name="args">status codes to add</param>
    public static void AddProduces<T>(this RouteHandlerBuilder routeHandler, params int[] args)
    {
        routeHandler.Produces<T>(StatusCodes.Status200OK);
        foreach (var statusCode in args.EmptyIfNull())
        {
            routeHandler.Produces(statusCode);
        }
    }

    /// <summary>
    /// Return an empty list if collection is null
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <returns>Empty list if null, else original list</returns>
    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> collection)
    {
        if (collection == null) return Enumerable.Empty<T>();

        return collection;
    }

    /// <summary>
    /// Convert product and offer? into store product dto
    /// </summary>
    /// <param name="product"></param>
    /// <param name="offer"></param>
    /// <param name="quantity"></param>
    /// <returns>Store Product DTO</returns>
    public static StoreProductDTO ToStoreProduct(this Product product, Offer? offer, int quantity = 1)
    {
        return new StoreProductDTO(product.Id, product.Name, product.Price, quantity, offer?.Quantity, offer?.OfferPrice, 0);
    }
}