namespace Application.Services
{
    #region Usings
    using Application.Interfaces;
    using Data.Context;
    using Microsoft.EntityFrameworkCore;
    #endregion

    public class PriceService : IPriceService
    {
        private readonly AppDbContext _context;

        public PriceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> Get24hAvgPrice(string symbol)
        {
            var now = DateTime.UtcNow;
            var start = now.AddDays(-1);

            try
            {
                var prices = await _context.Prices
                    .AsNoTracking()
                    .Where(p => p.Symbol == symbol && p.Timestamp >= start && p.Timestamp <= now)
                    .Select(p => p.Price)
                    .ToListAsync();

                var averagePrice = prices.Average();
                return averagePrice;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<decimal> GetSimpleMovingAverage(string symbol, int n, string timePeriod, DateTime? startDate)
        {
            return 0;
        }
    }
}
