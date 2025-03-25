namespace SmartFinance.API.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }

    public string PrefferedCurrency { get; set; } = "PLN"; // by default
    
    public ICollection<Transaction>? Transactions { get; set; }
}