using SmartFinance.API.Models;
using System.Text.Json;

namespace SmartFinance.API.Services;

public class ExchangeRateService
{
    private readonly HttpClient _httpClient;

    public ExchangeRateService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ExchangeRate?> GetRateAsync(string currencyCode)
    {
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