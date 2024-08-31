namespace Application.Services
{
    #region Usings
    using Application.Interfaces;
    using Data.Context;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;
    #endregion

    public class PriceService : IPriceService
    {
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(1); // Cache duration

        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;

        public PriceService(
            AppDbContext context,
            IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<decimal> Get24hAvgPrice(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException("Symbol cannot be null or empty", nameof(symbol));
            }

            try
            {
                // Create a cache key
                string cacheKey = $"{symbol}_24hAvgPrice";

                // Check if the result is already cached
                if (_cache.TryGetValue(cacheKey, out decimal cachedAveragePrice))
                {
                    return cachedAveragePrice;
                }

                // Get the latest price data timestamp
                var lastPriceData = await _context.Prices
                    .AsNoTracking()
                    .Where(x => x.Symbol == symbol)
                    .OrderByDescending(x => x.Timestamp)
                    .FirstOrDefaultAsync();

                if (lastPriceData == null)
                {
                    throw new ArgumentException("No data available for the symbol", nameof(symbol));
                }

                var now = lastPriceData.Timestamp;
                var start = now.AddDays(-1);

                // Calculate the average price for the symbol
                var averagePrice = await _context.Prices
                    .AsNoTracking()
                    .Where(p => p.Symbol == symbol && p.Timestamp >= start && p.Timestamp <= now)
                    .AverageAsync(p => (decimal)p.Price);

                // Store the result in cache
                _cache.Set(symbol, averagePrice, CacheDuration);

                return averagePrice;
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidOperationException("An error occurred while calculating the 24-hour average price.", e);
            }
            catch (Exception e)
            {
                throw new Exception("An unexpected error occurred. Please try again later.", e);
            }
        }

        public async Task<decimal> GetSimpleMovingAverage(string symbol, int n, string timePeriod, DateTime? startDate)
        {
            try
            {
                // Create a cache key
                string cacheKey = $"{symbol}_{n}_{timePeriod}_{startDate?.ToString("yyyyMMdd")}";

                // Check if the result is already cached
                if (_cache.TryGetValue(cacheKey, out decimal cachedSma))
                {
                    return cachedSma;
                }

                // Validate and Determine the interval for each data point based on the timePeriod
                TimeSpan interval = timePeriod switch
                {
                    "1w" => TimeSpan.FromDays(7),
                    "1d" => TimeSpan.FromDays(1),
                    "30m" => TimeSpan.FromMinutes(30),
                    "5m" => TimeSpan.FromMinutes(5),
                    "1m" => TimeSpan.FromMinutes(1),
                    _ => throw new ArgumentException("Invalid time period")
                };

                // Get the start date; if null, use the current date and time
                DateTime start;
                if (startDate.HasValue)
                {
                    // Get the current UTC time
                    DateTime utcNow = DateTime.UtcNow;
                    start = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day,
                                utcNow.Hour, utcNow.Minute, utcNow.Second, DateTimeKind.Utc);
                }
                else
                {
                    start = DateTime.UtcNow;
                }

                // Calculate the end date based on the start date
                DateTime end = start.Subtract(interval * n);

                // Fetch the price data for the given symbol within the time range
                var prices = await _context.Prices
                    .AsNoTracking()
                    .Where(p => p.Symbol == symbol && p.Timestamp <= start && p.Timestamp > end)
                    .OrderByDescending(p => p.Timestamp)
                    .Take(n)
                    .Select(p => p.Price)
                    .ToListAsync();

                // Validate if we have enough data points
                if (prices.Count < n)
                    throw new InvalidOperationException("Not enough data points to calculate the SMA.");

                // Calculate the SMA
                decimal sma = prices.Average();

                // Store the result in cache
                _cache.Set(cacheKey, sma, CacheDuration);

                return sma;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
