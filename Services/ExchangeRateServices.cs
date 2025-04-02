using SmartFinance.API.Models;
using System;
using System.Text.Json;

namespace SmartFinance.API.Services
{
    public class ExchangeRateService
    {
        private readonly HttpClient _httpClient;

        public ExchangeRateService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public virtual async Task<ExchangeRate?> GetRateAsync(string currencyCode)
        {
            if (currencyCode.ToUpper() == "PLN")
            {
                return new ExchangeRate
                {
                    Currency = "PLN",
                    Rate = 1.0m,
                    Date = DateTime.UtcNow
                };
            }

            var url = $"https://api.nbp.pl/api/exchangerates/rates/A/{currencyCode}/?format=json";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var rate = doc.RootElement
                .GetProperty("rates")[0]
                .GetProperty("mid").GetDecimal();

            var date = doc.RootElement
                .GetProperty("rates")[0]
                .GetProperty("effectiveDate").GetDateTime();

            return new ExchangeRate
            {
                Currency = currencyCode.ToUpper(),
                Rate = rate,
                Date = date
            };
        }
    }
}
