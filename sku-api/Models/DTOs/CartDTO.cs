using System.Text.Json.Serialization;

public class CartDTO
{
    [JsonPropertyName("products")]
    public string? CartItemsJson { get; set; }
    public decimal TotalPrice { get; set; }

    public CartDTO(Cart cart)
    {
        CartItemsJson = cart.CartProductsJson;
        TotalPrice = cart.TotalPrice;
    }
}