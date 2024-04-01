/* public class CheckoutService
{
    private readonly Serilog.ILogger _logger;

    public CheckoutService(Serilog.ILogger logger)
    {
        _logger = logger;
    }

    public void UpdateCartTotalAndOffersApplied(Cart cart)
    {
        decimal currentTotal = 0;
        foreach (var cartItem in cart.CartProducts.EmptyIfNull())
        {
            var productPriceAndOffer = GetProductPriceWithOffers(cartItem.Price, cartItem.Quantity, cartItem.OfferQuantity, cartItem.OfferPrice);
            currentTotal += productPriceAndOffer.Item1;

            // Only update how many times the offer is applied if product has an available offer
            if (cartItem.OfferQuantity != null) cartItem.OffersApplied = productPriceAndOffer.Item2;
        }
        cart.TotalPrice = currentTotal;
    }


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
} */