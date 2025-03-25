namespace SmartFinance.API.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    
    public int? UserId { get; set; } // null = default
    public User? User { get; set; }
    
    public ICollection<Transaction>? Transactions { get; set; }
}