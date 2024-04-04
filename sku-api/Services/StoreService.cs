using System.Collections;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

public class StoreService : IStoreService
{
    private readonly IStoreDbContext _context;
    private readonly Serilog.ILogger _logger;

    public StoreService(IStoreDbContext context, Serilog.ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<StoreProductDTO>> GetStore()
    {
        try
        {
            var products = await _context.Products.AttemptFullSetFetch(_logger);
            var offers = await _context.Offers.AttemptFullSetFetch(_logger);

            List<StoreProductDTO> storeProducts = new List<StoreProductDTO>();

            foreach (var product in products.EmptyIfNull())
            {
                var offer = offers.EmptyIfNull().FirstOrDefault(o => o.ProductId == product.Id);
                storeProducts.Add(product.ToStoreProduct(offer, 0));
            }

            return storeProducts;
        }
        catch (Exception ex)
        {
            _logger.Error<Exception>(ex.Message, ex);
            return Enumerable.Empty<StoreProductDTO>();
        }
    }

    public async Task<IEnumerable<Product>> GetProducts()
    {
        try
        {
            var products = await _context.Products.AttemptFullSetFetch(_logger);
            return products.EmptyIfNull();
        }
        catch (Exception ex)
        {
            _logger.Error<Exception>(ex.Message, ex);
            return Enumerable.Empty<Product>();
        }
    }

    public async Task<Product?> GetProduct(int id)
    {
        try
        {
            var product = await _context.Products.AttemptItemFetchById(id, _logger);
            return product ?? null;
        }
        catch (Exception ex)
        {
            _logger.Error<Exception>(ex.Message, ex);
            return null;
        }
    }

    public async Task<IEnumerable<Offer>> GetOffers()
    {
        try
        {
            var offers = await _context.Offers.AttemptFullSetFetch(_logger);
            return offers.EmptyIfNull();
        }
        catch (Exception ex)
        {
            _logger.Error<Exception>(ex.Message, ex);
            return Enumerable.Empty<Offer>();
        }
    }

    public async Task<Offer?> GetOffer(int id)
    {
        try
        {
            var offer = await _context.Offers.AttemptItemFetchById(id, _logger);
            return offer ?? null;
        }
        catch (Exception ex)
        {
            _logger.Error<Exception>(ex.Message, ex);
            return null;
        }
    }

    public async Task<IEnumerable<Offer>?> GetOffers(int productId)
    {
        try
        {
            var offers = await _context.Offers.AttemptItemFetchByProp("ProductId", productId, _logger);
            return offers;
        }
        catch (Exception ex)
        {
            _logger.Error<Exception>(ex.Message, ex);
            return null;
        }
    }

    public async Task<UpdateResult> PostProducts(IEnumerable<Product> products)
    {
        try
        {
            var result = await _context.AttemptUpdate(_context.Products, products, ["Name", "Price"], ["Name"], _logger);
            return result;
        }
        catch (Exception ex)
        {
            _logger.Error<Exception>(ex.Message, ex);
            return new UpdateResult() { Success = false, Message = "Internal Error" };
        }
    }

    public async Task<UpdateResult> PostOffers(IEnumerable<Offer> offers)
    {
        try
        {
            var result = await _context.AttemptUpdate(_context.Offers, offers, ["ProductId", "Quantity", "OfferPrice"], ["ProductId"], _logger);
            return result;
        }
        catch (Exception ex)
        {
            _logger.Error<Exception>(ex.Message, ex);
            return new UpdateResult() { Success = false, Message = "Internal Error" };
        }
    }

    public async Task<UpdateResult> DeleteProducts(IEnumerable<int> productIds)
    {
        try
        {
            var result = await DeleteProductsAndOffers(productIds);
            return result;
        }
        catch (Exception ex)
        {
            _logger.Error<Exception>(ex.Message, ex);
            return new UpdateResult() { Success = false, Message = "Internal Error" };
        }
    }

    private async Task<UpdateResult> DeleteProductsAndOffers(IEnumerable<int> productIds)
    {
        try
        {
            var combinedResult = new UpdateResult();

            var result = await _context.AttemptDelete(_context.Products, productIds, _logger);
            if (result.Success)
            {
                try
                {
                    // Find associated offers and remove these too.
                    var offers = await _context.Offers.Where(o => productIds.Contains(o.ProductId)).ToListAsync();
                    var offerIds = offers.Select(o => o.Id);
                    var offerResult = await _context.AttemptDelete(_context.Offers, offerIds, _logger);

                    combinedResult.Message = result.Message;
                    combinedResult.Success = true;
                    combinedResult.UpdateResultObject = await GetStore();
                }
                catch (Exception ex)
                {
                    _logger.Error<Exception>(ex.Message, ex);
                    return new UpdateResult() { Success = false, Message = "Internal Error" };
                }
            }

            return combinedResult;
        }
        catch (Exception ex)
        {
            _logger.Error<Exception>(ex.Message, ex);
            return new UpdateResult() { Success = false, Message = "Internal Error" };
        }
    }

    public async Task<UpdateResult> DeleteOffers(IEnumerable<int> offerIds)
    {
        try
        {
            var result = await _context.AttemptDelete(_context.Offers, offerIds, _logger);
            return result;
        }
        catch (Exception ex)
        {
            _logger.Error<Exception>(ex.Message, ex);
            return new UpdateResult() { Success = false, Message = "Internal Error" };
        }
    }
}