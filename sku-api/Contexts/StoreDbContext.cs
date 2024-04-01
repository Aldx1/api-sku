using Microsoft.EntityFrameworkCore;

public class StoreDbContext : DbContext
{
    public DbSet<Offer> Offers { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<Cart> Carts { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options) { }

}