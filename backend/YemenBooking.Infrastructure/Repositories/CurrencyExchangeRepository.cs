using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Infrastructure.Data.Context;

namespace YemenBooking.Infrastructure.Repositories
{
    public class CurrencyExchangeRepository : BaseRepository<CurrencyExchangeRate>, ICurrencyExchangeRepository
    {
        public CurrencyExchangeRepository(YemenBookingDbContext context) : base(context) { }

        public Task<bool> AreRatesExpiredAsync(int maxAgeHours = 24)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkUpdateRatesAsync(List<CurrencyExchangeRate> rates)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> ConvertAmountAsync(decimal amount, string fromCurrency, string toCurrency)
        {
            if (string.Equals(fromCurrency, toCurrency, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(amount);
            }
            // Minimal fallback mapping for demo
            var rate = GetRateFallback(fromCurrency, toCurrency);
            if (rate.HasValue)
            {
                return Task.FromResult(amount * rate.Value);
            }
            throw new InvalidOperationException($"Exchange rate not found for {fromCurrency}->{toCurrency}");
        }

        public Task<List<CurrencyExchangeRate>> GetAllCurrentRatesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<CurrencyExchangeRate?> GetExchangeRateAsync(string fromCurrency, string toCurrency)
        {
            // TODO: replace with real DB lookup; using fallback for now
            var rate = GetRateFallback(fromCurrency, toCurrency);
            if (rate.HasValue)
            {
                return Task.FromResult<CurrencyExchangeRate?>(new CurrencyExchangeRate
                {
                    Id = Guid.NewGuid(),
                    FromCurrency = fromCurrency.ToUpperInvariant(),
                    ToCurrency = toCurrency.ToUpperInvariant(),
                    Rate = rate.Value,
                    LastUpdated = DateTime.UtcNow,
                    Source = "Fallback",
                    IsActive = true
                });
            }
            return Task.FromResult<CurrencyExchangeRate?>(null);
        }

        public Task<DateTime?> GetLastUpdateDateAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<CurrencyExchangeRate>> GetLatestRatesForCurrencyAsync(string baseCurrency)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetSupportedCurrenciesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateExchangeRateAsync(string fromCurrency, string toCurrency, decimal rate)
        {
            throw new NotImplementedException();
        }

        private static decimal? GetRateFallback(string fromCurrency, string toCurrency)
        {
            var from = fromCurrency.ToUpperInvariant();
            var to = toCurrency.ToUpperInvariant();
            if (from == to) return 1m;
            // Example approximate: 1 USD ~ 250 YER (demo). Adjust as needed or fetch from DB.
            if (from == "USD" && to == "YER") return 250m;
            if (from == "YER" && to == "USD") return 1m / 250m;
            return null;
        }

        // public async Task<CurrencyExchangeRate?> GetExchangeRateAsync(string fromCurrency, string toCurrency)
        //     => await _context.CurrencyExchangeRates
        //         .Where(r => r.FromCurrency == fromCurrency && r.ToCurrency == toCurrency && r.IsActive)
        //         .OrderByDescending(r => r.LastUpdated)
        //         .FirstOrDefaultAsync();

        // public async Task<List<CurrencyExchangeRate>> GetLatestRatesForCurrencyAsync(string baseCurrency)
        //     => await _context.CurrencyExchangeRates
        //         .Where(r => r.FromCurrency == baseCurrency && r.IsActive)
        //         .GroupBy(r => r.ToCurrency)
        //         .Select(g => g.OrderByDescending(r => r.LastUpdated).First())
        //         .ToListAsync();

        // public async Task<List<CurrencyExchangeRate>> GetAllCurrentRatesAsync()
        //     => await _context.CurrencyExchangeRates
        //         .Where(r => r.IsActive)
        //         .GroupBy(r => new { r.FromCurrency, r.ToCurrency })
        //         .Select(g => g.OrderByDescending(r => r.LastUpdated).First())
        //         .ToListAsync();

        // public async Task<bool> UpdateExchangeRateAsync(string fromCurrency, string toCurrency, decimal rate)
        // {
        //     var entity = await GetExchangeRateAsync(fromCurrency, toCurrency);
        //     if (entity != null)
        //     {
        //         entity.Rate = rate;
        //         entity.LastUpdated = DateTime.UtcNow;
        //         await _context.SaveChangesAsync();
        //         return true;
        //     }
        //     return false;
        // }

        // public async Task<bool> BulkUpdateRatesAsync(List<CurrencyExchangeRate> rates)
        // {
        //     foreach (var rateEntity in rates)
        //     {
        //         var existing = await _context.CurrencyExchangeRates
        //             .FirstOrDefaultAsync(r => r.FromCurrency == rateEntity.FromCurrency && r.ToCurrency == rateEntity.ToCurrency);
        //         if (existing != null)
        //         {
        //             existing.Rate = rateEntity.Rate;
        //             existing.LastUpdated = DateTime.UtcNow;
        //         }
        //         else
        //         {
        //             await _context.CurrencyExchangeRates.AddAsync(rateEntity);
        //         }
        //     }
        //     await _context.SaveChangesAsync();
        //     return true;
        // }

        // public async Task<DateTime?> GetLastUpdateDateAsync()
        //     => await _context.CurrencyExchangeRates
        //         .Where(r => r.IsActive)
        //         .MaxAsync(r => (DateTime?)r.LastUpdated);

        // public async Task<bool> AreRatesExpiredAsync(int maxAgeHours = 24)
        // {
        //     var last = await GetLastUpdateDateAsync();
        //     return last == null || (DateTime.UtcNow - last.Value).TotalHours > maxAgeHours;
        // }

        // public async Task<decimal> ConvertAmountAsync(decimal amount, string fromCurrency, string toCurrency)
        // {
        //     var rate = await GetExchangeRateAsync(fromCurrency, toCurrency);
        //     if (rate != null)
        //         return amount * rate.Rate;
        //     throw new InvalidOperationException($"Exchange rate not found for {fromCurrency}->{toCurrency}");
        // }

        // public async Task<List<string>> GetSupportedCurrenciesAsync()
        //     => await _context.CurrencyExchangeRates
        //         .Where(r => r.IsActive)
        //         .Select(r => r.FromCurrency)
        //         .Distinct()
        //         .ToListAsync();
    }
}
