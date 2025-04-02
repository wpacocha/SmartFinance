using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartFinance.API.Data;
using SmartFinance.API.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Dodanie klucza JWT z konfiguracji
var jwtKey = builder.Configuration["Jwt:Key"] ?? "supersecretkey123"; // fallback jeśli nie podano

// 💉 AddDbContext
builder.Services.AddDbContext<FinanceDbContext>(options =>
    options.UseSqlite("Data source=finance.db"));

builder.Services.AddHttpClient<ExchangeRateService>();
builder.Services.AddScoped<ICurrencyConversionService, CurrencyConversionService>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
});


// 🔐 Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// 👇 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000", policy =>
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




var app = builder.Build();

// 🌐 Swagger dev mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowLocalhost3000");

// ⛓️ Middleware kolejność ma znaczenie
app.UseAuthentication(); // 👈 musi być przed Authorization!
app.UseAuthorization();

app.MapControllers();

// 🌱 Seeder
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();
    DataSeeder.SeedCategories(context);
}

app.Run();
