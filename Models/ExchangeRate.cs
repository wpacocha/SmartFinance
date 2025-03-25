namespace SmartFinance.API.Models;

public class ExchangeRate
{
    public int Id { get; set; }
    public string Currency { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public DateTime Date { get; set; }
}