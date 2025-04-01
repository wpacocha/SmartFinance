using Microsoft.EntityFrameworkCore;
using SmartFinance.API.Models;

namespace SmartFinance.API.Data;

public static class DataSeeder
{
    public static void SeedCategories(FinanceDbContext context)
    {
        if (!context.Categories.Any())
        {
            var categories = new List<Category>
            {
                // Food
                new Category {Name = "Groceries", Type ="Food"},
                new Category {Name = "Restaurants", Type ="Food"},
                new Category {Name = "Coffe & Snacks", Type ="Food"},
                
                // Transportation
                new Category {Name = "Public transport", Type ="Transport"},
                new Category {Name = "Fuel", Type ="Transport"},
                new Category {Name = "Parking", Type ="Transport"},
                
                // Housing
                new Category {Name = "Rent", Type ="Housing"},
                new Category {Name = "Utilities", Type ="Housing"},
                new Category {Name = "Internet", Type ="Housing"},
                
                // Entertaiment
                new Category {Name = "Events", Type ="Entertaiment"},
                new Category {Name = "Subscriptions", Type ="Entertaiment"},
                
                // Shopping
                new Category {Name = "Clothing", Type ="Shopping"},
                new Category {Name = "Cosmetics", Type ="Shopping"},
                new Category {Name = "Electronics", Type ="Shopping"},
                
                // Health
                new Category {Name = "Medical bills", Type ="Health"},
                new Category {Name = "Pharmacy", Type ="Health"},
                
                // Work & Education
                new Category {Name = "Courses", Type ="Work & Education"},
                new Category {Name = "Books", Type ="Work & Education"},
                
                // Finance
                new Category {Name = "Savings", Type ="Finance"},
                new Category {Name = "Investments", Type ="Finance"},
                
                // Pets
                new Category { Name = "Pet Food", Type = "Pet"},
                new Category { Name = "Vet", Type = "Pet"},
                
                // Family
                new Category {Name = "Toys", Type = "Family"},
                new Category {Name = "School", Type = "Family"},
                
                // Other
                new Category {Name = "Donations", Type = "Other"},
                new Category {Name = "Miscellaneous", Type = "Other"},
            };

            context.Categories.AddRange(categories);
            context.SaveChanges();
        }
    }
}