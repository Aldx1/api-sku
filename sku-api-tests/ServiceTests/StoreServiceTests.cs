using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class StoreServiceTests
{
    StoreDbContext _context;
    StoreService _service;


    private List<Product> _products = new List<Product>() {
            new Product { Id = 1, Name = "Product 1", Price = 10 },
            new Product { Id = 2, Name = "Product 2", Price = 20 },
            new Product { Id = 3, Name = "Product 3", Price = 30 },
            new Product { Id = 4, Name = "Product 4", Price = 40 }
        };

    private List<Offer> _offers = new List<Offer>() {
            new Offer { Id = 1, ProductId = 1, Quantity = 5, OfferPrice = 8 },
            new Offer { Id = 2, ProductId = 2, Quantity = 10, OfferPrice = 15 }
        };


    [OneTimeSetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<StoreDbContext>()
                    .UseInMemoryDatabase("StoreTest")
                    .Options;

        _context = new StoreDbContext(options);

        var mockLogger = new Mock<Serilog.ILogger>();

        _context.Products.AddRange(_products);
        _context.Offers.AddRange(_offers);
        _context.SaveChanges();

        _service = new StoreService(_context, mockLogger.Object);
    }

    // GetStore returns a list of StoreProductDTO objects
    [Test]
    public async Task test_get_store_returns_list_of_store_product_dto_objects()
    {
        var result = await _service.GetStore();

        Assert.IsInstanceOf<IEnumerable<StoreProductDTO>>(result);
        Assert.That(_products.Count, Is.EqualTo(result.Count()));
    }

    // GetProducts returns a list of Product objects
    [Test]
    public async Task test_get_products_returns_list_of_product_objects()
    {

        var result = await _service.GetProducts();

        Assert.IsInstanceOf<IEnumerable<Product>>(result);
        Assert.That(_products.Count, Is.EqualTo(result.Count()));
    }

    // GetProduct returns a Product object
    [Test]
    public async Task test_get_product_returns_product_object()
    {

        var result = await _service.GetProduct(1);

        Assert.IsInstanceOf<Product>(result);
    }

    // GetProduct returns a null object
    [Test]
    public async Task test_get_product_returns_null_object()
    {

        var result = await _service.GetProduct(10);

        Assert.That(result, Is.EqualTo(null));
    }


    // GetOffers returns a list of Offer objects
    [Test]
    public async Task test_get_offers_returns_list_of_offer_objects()
    {

        var result = await _service.GetOffers();

        Assert.IsInstanceOf<IEnumerable<Offer>>(result);
        Assert.That(_offers.Count, Is.EqualTo(result.Count()));
    }

    // GetOffer returns an Offer object
    [Test]
    public async Task test_get_offer_returns_null_object()
    {

        var result = await _service.GetOffer(10);

        Assert.That(result, Is.EqualTo(null));
    }

    // GetOffer returns a null object
    [Test]
    public async Task test_get_offer_returns_offer_object()
    {

        var result = await _service.GetOffer(1);

        Assert.IsInstanceOf<Offer>(result);
    }

    // GetOffers returns an empty list of offers object
    [Test]
    public async Task test_get_offers_returns_empty_list_of_offer_object()
    {

        var result = await _service.GetOffers(10);

        Assert.IsInstanceOf<IEnumerable<Offer>>(result);
        Assert.That(0, Is.EqualTo(result.Count()));
    }

    // GetOffers returns a list of offers object with 1 element
    [Test]
    public async Task test_get_offers_returns_list_of_offer_object()
    {

        var result = await _service.GetOffers(1);

        Assert.IsInstanceOf<IEnumerable<Offer>>(result);
        Assert.That(1, Is.EqualTo(result.Count()));
    }


    [OneTimeTearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }

}

