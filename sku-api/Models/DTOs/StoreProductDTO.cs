/// <summary>
/// Data Transfer object for Get:Store 
/// attaches relevant offer to products
/// Used for cart quantities. 
/// </summary>

public class StoreProductDTO
{
    // Product props
    public int? Id { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    // Offer props
    public int? OfferQuantity { get; set; }
    public decimal? OfferPrice { get; set; }
    public int? OffersApplied { get; set; }

    public StoreProductDTO(int id, string name, decimal price, int quantity, int? offerQuantity, decimal? offerPrice, int? offersApplied)
    {
        Id = id;
        Name = name;
        Price = price;
        Quantity = quantity;
        OfferQuantity = offerQuantity;
        OfferPrice = offerPrice;
        OffersApplied = offersApplied;
    }

    public StoreProductDTO() { }
}