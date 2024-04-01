using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Cart
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? CartProductsJson { get; set; }
    public decimal TotalPrice { get; set; }

    [NotMapped]
    public ICollection<StoreProductDTO>? CartProducts { get; set; }
}

