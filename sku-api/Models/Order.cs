using System.ComponentModel.DataAnnotations;

public class Order
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? OrderItemsJson { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime OrderDate { get; set; }

    public Order() { OrderItemsJson = ""; }

    public Order(Cart cart)
    {
        UserId = cart.UserId;
        OrderItemsJson = cart.CartProductsJson;
        TotalPrice = cart.TotalPrice;
        OrderDate = DateTime.Now;
    }
}