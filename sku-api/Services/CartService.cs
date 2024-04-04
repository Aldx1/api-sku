
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

public class CartService : ICartService
{
    private readonly IUserService _userService;
    private readonly IStoreDbContext _context;
    private readonly Serilog.ILogger _logger;

    public CartService(IUserService userService, IStoreDbContext context, Serilog.ILogger logger)
    {
        _userService = userService;
        _context = context;
        _logger = logger;
    }


    /// <summary>
    /// Retrieve the cart for that user
    /// </summary>
    /// <returns>The cart, only have 1 cart per user</returns>
    private async Task<Cart> GetCart()
    {
        try
        {
            var userId = _userService.GetUserId();
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId.Value,
                    CartProductsJson = string.Empty,
                    TotalPrice = 0
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }
        catch (Exception ex)
        {
            _logger.Error<Exception>(ex.Message, ex);
            return null;
        }
    }

    public async Task<CartDTO?> GetCartDTO()
    {
        try
        {
            var cart = await GetCart();
            if (cart == null) throw new Exception("null cart");
            return new CartDTO(cart);
        }
        catch (Exception ex)
        {
            _logger.Error<Exception>(ex.Message, ex);
            return null;
        }
    }

    public async Task<IEnumerable<OrderDTO>> GetOrders()
    {
        try
        {
            var userId = _userService.GetUserId();
            var orders = await _context.Orders.Where(o => o.UserId == userId).ToListAsync();
            return orders.EmptyIfNull().Select(o => new OrderDTO(o));
        }
        catch (Exception ex)
        {
            _logger.Error<Exception>(ex.Message, ex);
            return Enumerable.Empty<OrderDTO>();
        }
    }

    public async Task<UpdateResult> PutProducts(IEnumerable<StoreProductDTO> products)
    {
        try
        {
            var cart = await GetCart();

            List<StoreProductDTO> cartProducts;
            if (cart.CartProductsJson?.Length > 0)
            {
                cartProducts = JsonSerializer.Deserialize<List<StoreProductDTO>>(cart.CartProductsJson);
            }
            else
            {
                cartProducts = new List<StoreProductDTO>();
            }


            var currentProducts = await _context.Products.AttemptFullSetFetch(_logger);
            var currentOffers = await _context.Offers.AttemptFullSetFetch(_logger);

            foreach (var product in products.Where(x => x.Quantity >= 0))
            {
                // Only add items which are in inventory 
                var currentProduct = currentProducts?.FirstOrDefault(p => p.Id == product.Id || p.Name == product.Name);
                if (currentProduct == null) continue;

                var existingCartProduct = cartProducts.EmptyIfNull().FirstOrDefault(i => i.Id == currentProduct.Id);
                if (existingCartProduct == null)
                {
                    var currentProductOffer = currentOffers.FirstOrDefault(o => o.ProductId == currentProduct.Id);
                    cartProducts.Add(currentProduct.ToStoreProduct(currentProductOffer, product.Quantity));
                }
                else
                {
                    existingCartProduct.Quantity = product.Quantity;
                }
            }

            cart.CartProducts = cartProducts.EmptyIfNull().Where(p => p.Quantity > 0).ToList();
            UpdateCartTotalAndOffersApplied(cart);
            await _context.SaveChangesAsync();

            return new UpdateResult()
            {
                Success = true,
                Message = "",
                UpdateResultObject = new CartDTO(cart)
            };
        }
        catch (Exception ex)
        {
            _logger.Error<Exception>(ex.Message, ex);
            return new UpdateResult() { Success = false, Message = "Inner error" };
        }
    }

    /// <summary>
    /// Set the total price for the cart
    /// </summary>
    /// <param name="cart"></param>
    private void UpdateCartTotalAndOffersApplied(Cart cart)
    {
        decimal currentTotal = 0;
        foreach (var cartItem in cart.CartProducts.EmptyIfNull())
        {
            var productPriceAndOffer = GetProductPriceWithOffers(cartItem.Price, cartItem.Quantity, cartItem.OfferQuantity, cartItem.OfferPrice);
            currentTotal += productPriceAndOffer.Item1;

            // Only update how many times the offer is applied if product has an available offer
            if (cartItem.OfferQuantity != null) cartItem.OffersApplied = productPriceAndOffer.Item2;
        }

        cart.CartProductsJson = JsonSerializer.Serialize(cart.CartProducts);
        cart.TotalPrice = currentTotal;
    }


    /// <summary>
    /// Set the total price for that item, based on offers
    /// </summary>
    /// <param name="cart"></param>
    /// <returns>Item1 = product total, Item2 = number of times offer has been applied (if any available)</returns>
    private (decimal, int) GetProductPriceWithOffers(decimal productPrice, int productQuantity, int? offerQuantity, decimal? offerPrice)
    {
        decimal priceWithoutOffers = productPrice * productQuantity;
        if (offerQuantity == null || productQuantity < offerQuantity.Value)
        {
            return (priceWithoutOffers, 0);
        }

        // If offer price is null, return the price without offers. We don't want to give away free items
        if (offerPrice == null)
        {
            _logger.Warning($"offerPrice is null - OQ: {offerQuantity}, PP: {productPrice}, PQ: {productQuantity}");
            return (priceWithoutOffers, 0);
        }

        // How many times to apply the offer
        int offersApplied = productQuantity / offerQuantity.Value;
        int remainingQuantity = productQuantity % offerQuantity.Value;

        decimal combinedPrice = offersApplied * offerPrice.Value;

        if (remainingQuantity > 0)
        {
            combinedPrice += productPrice * remainingQuantity;
        }

        return (combinedPrice, offersApplied);
    }

    public async Task<UpdateResult> Checkout()
    {
        try
        {
            var cart = await GetCart();
            if (cart == null || string.IsNullOrWhiteSpace(cart.CartProductsJson))
            {
                return Unsuccessful("No cart items");
            }

            // Create order 
            var order = new Order(cart);

            // Delete cart
            var deleteCartResult = await _context.AttemptDelete(_context.Carts, new List<int> { cart.Id }, _logger);

            // Save order
            if (deleteCartResult.Success)
            {
                var addOrderResult = await _context.AttemptUpdate(_context.Orders, new List<Order> { order }, [], [], _logger);

                if (addOrderResult.Success)
                {
                    return Successful("", new OrderDTO(order));
                }

                return Unsuccessful("Couldn't add order");
            }

            return Unsuccessful("Couldn't delete cart");
        }
        catch (Exception ex)
        {
            _logger.Error<Exception>(ex.Message, ex);
            return new UpdateResult() { Success = false, Message = "Inner error" };
        }
    }


    private UpdateResult Unsuccessful(string message)
    {
        return new UpdateResult()
        {
            Success = false,
            Message = message
        };
    }

    private UpdateResult Successful(string message, object? resultObject)
    {
        return new UpdateResult()
        {
            Success = true,
            Message = message,
            UpdateResultObject = resultObject
        };
    }
}