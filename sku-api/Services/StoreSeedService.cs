public class StoreSeedService : ISeedDataService
{
    private readonly IStoreDbContext _context;

    public StoreSeedService(IStoreDbContext context)
    {
        _context = context;
    }

    public void SeedDatabase()
    {
        _context.Products.AddRange(GetProducts());
        _context.Offers.AddRange(GetOffers());
        _context.Users.AddRange(GetUsers());
        _context.SaveChanges();
    }

    private IEnumerable<Product> GetProducts()
    {
        return new List<Product>()
        {
            new Product(){Id = 1, Name = "A", Price = 50},
            new Product(){Id = 2, Name = "B", Price = 30},
            new Product(){Id = 3, Name = "C", Price = 20},
            new Product(){Id = 4, Name = "D", Price = 15}
        };
    }

    private IEnumerable<Offer> GetOffers()
    {
        return new List<Offer>()
        {
            new Offer(){Id = 1, Quantity = 3, OfferPrice = 130, ProductId = 1},
            new Offer(){Id = 2, Quantity = 2, OfferPrice = 45, ProductId = 2}
        };
    }

    private IEnumerable<User> GetUsers()
    {
        return new List<User>()
        {
            new User()
            {
                Username = "admin",
                PasswordHash = "$2a$11$Lwxfpt3IlvbnBtfe5ovCKOVE7aa8SPXFAF79Bbbh92hq5u5dwdH3S"
            }
        };
    }
}