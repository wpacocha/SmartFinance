using Microsoft.EntityFrameworkCore;
using SmartFinance.API.Models;

namespace SmartFinance.API.Data;

public class FinanceDbContext : DbContext
{
    public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options) {}
    
    public DbSet<Transaction> Transactions { get; set; }
    
    public DbSet<Category> Categories { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<ExchangeRate> ExchangeRates { get; set; }
}