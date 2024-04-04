using Microsoft.EntityFrameworkCore;

public interface IStoreDbContext
{
    DbSet<Offer> Offers { get; }

    /// <summary>
    /// Gets or sets the DbSet for the Product entity in the store database.
    /// </summary>
    DbSet<Product> Products { get; }

    /// <summary>
    /// Gets or sets the DbSet for the Order entity in the store database.
    /// </summary>
    DbSet<Order> Orders { get; }

    /// <summary>
    /// Gets or sets the DbSet for the Cart entity in the store database.
    /// </summary>
    DbSet<Cart> Carts { get; }

    /// <summary>
    /// Gets or sets the DbSet for the User entity in the store database.
    /// </summary>
    DbSet<User> Users { get; }

    /// <summary>
    /// Saves the changes with the DbContext parent
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves the changes with the DbContext parent
    /// </summary>
    int SaveChanges(bool acceptAllChangesOnSuccess = true);
}