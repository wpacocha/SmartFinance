using Microsoft.EntityFrameworkCore;
using SmartFinance.API.Data;
using SmartFinance.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Adding sqlite
builder.Services.AddDbContext<FinanceDbContext>(options => options.UseSqlite("Data source=finance.db"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// seeder for categories
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();
    DataSeeder.SeedCategories(context);
}

builder.Services.AddHttpClient<ExchangeRateService>();

app.Run();