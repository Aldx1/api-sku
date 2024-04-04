using Microsoft.EntityFrameworkCore;

/// <summary>
/// Represents the database context for a store application.
/// Provides access to the various entities in the database, such as offers, products, orders, carts, and users.
/// </summary>
public class StoreDbContext : DbContext, IStoreDbContext
{
    /// <summary>
    /// Gets or sets the DbSet for the Offer entity in the store database.
    /// </summary>
    public DbSet<Offer> Offers { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DbSet for the Product entity in the store database.
    /// </summary>
    public DbSet<Product> Products { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DbSet for the Order entity in the store database.
    /// </summary>
    public DbSet<Order> Orders { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DbSet for the Cart entity in the store database.
    /// </summary>
    public DbSet<Cart> Carts { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DbSet for the User entity in the store database.
    /// </summary>
    public DbSet<User> Users { get; set; } = null!;

    /// <summary>
    /// Initializes a new instance of the StoreDbContext class.
    /// </summary>
    /// <param name="options">The options for configuring the context.</param>
    public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options) { }


    /// <summary>
    /// Saves the changes with the DbContext parent
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Saves the changes with the DbContext parent
    /// </summary>
    public override int SaveChanges(bool acceptAllChangesOnSuccess = true)
    {
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }
}