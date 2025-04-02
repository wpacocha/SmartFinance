using SmartFinance.API.Data;
using SmartFinance.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace SmartFinance.API.Services
{
    public interface ICurrencyConversionService
    {
        Task ConvertUserTransactionsToCurrency(User user, string newCurrency);
    }

    public class CurrencyConversionService : ICurrencyConversionService
    {
        private readonly FinanceDbContext _context;
        private readonly ExchangeRateService _exchangeRateService;

        public CurrencyConversionService(
            FinanceDbContext context,
            ExchangeRateService exchangeRateService)
        {
            _context = context;
            _exchangeRateService = exchangeRateService;
        }

        public async Task ConvertUserTransactionsToCurrency(User user, string newCurrency)
        {
            Console.WriteLine("=== START KONWERSJI ===");

            var transactions = await _context.Transactions
                .Where(t => t.UserId == user.Id)
                .ToListAsync();

            Console.WriteLine($"Znaleziono {transactions.Count} transakcji dla usera {user.Username}");

            foreach (var tx in transactions)
            {
                Console.WriteLine($"[TX {tx.Id}] {tx.Amount} {tx.Currency} → {newCurrency}");

                if (tx.Currency == newCurrency)
                {
                    Console.WriteLine($"[TX {tx.Id}] Pomijam, waluta już ustawiona.");
                    continue;
                }

                var rateFrom = await _exchangeRateService.GetRateAsync(tx.Currency);
                var rateTo = await _exchangeRateService.GetRateAsync(newCurrency);

                if (rateFrom == null || rateTo == null)
                {
                    Console.WriteLine($"[TX {tx.Id}] Brak kursu: {tx.Currency} → {newCurrency}");
                    continue;
                }

                var factor = rateFrom.Rate / rateTo.Rate;
                var originalAmount = tx.Amount;

                tx.Amount = Math.Round(tx.Amount * factor, 2);
                tx.Currency = newCurrency;

                Console.WriteLine($"[TX {tx.Id}] Przeliczono {originalAmount} → {tx.Amount} (kurs {factor})");
            }

            await _context.SaveChangesAsync();
            Console.WriteLine("=== KONIEC KONWERSJI ===");
        }
    }
}
