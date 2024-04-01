using System.Text.Json.Serialization;

public class OrderDTO
{
    [JsonPropertyName("products")]
    public string? OrderItemsJson { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime OrderDate { get; set; }

    public OrderDTO(Order order)
    {
        OrderItemsJson = order.OrderItemsJson;
        TotalPrice = order.TotalPrice;
        OrderDate = order.OrderDate;
    }
}