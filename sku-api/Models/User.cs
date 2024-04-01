using System.ComponentModel.DataAnnotations;

public class User
{
    [Key]
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
}