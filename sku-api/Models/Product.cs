using System.ComponentModel.DataAnnotations;

public class Product
{
    [Key]
    public int Id { set; get; }
    public required string Name { get; set; }
    public required decimal Price { get; set; }
}