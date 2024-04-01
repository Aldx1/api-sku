using System.ComponentModel.DataAnnotations;

public class Offer
{
    [Key]
    public int Id { get; set; }
    public required int ProductId { get; set; }
    public required int Quantity { get; set; }
    public required decimal OfferPrice { get; set; }
}