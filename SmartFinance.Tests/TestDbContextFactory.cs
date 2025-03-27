using Microsoft.EntityFrameworkCore;
using SmartFinance.API.Data;

namespace SmartFinance.Tests
{
    public static class TestDbContextFactory
    {
        public static FinanceDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<FinanceDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var dbContext = new FinanceDbContext(options);
            dbContext.Database.EnsureCreated();

            return dbContext;
        }
    }
}
