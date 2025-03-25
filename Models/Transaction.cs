namespace SmartFinance.API.Models;

public class Transaction
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Currency { get; set; } = "PLN"; // default
    
    public bool IsIncome { get; set; } = false; // default
    public int Month { get; set; }
    public int Year { get; set; }
    public int CategoryId { get; set; } // Foreign Key
    public Category? Category { get; set; }
    public int UserId { get; set; } // Foreign Key
    public User? User { get; set; }
}