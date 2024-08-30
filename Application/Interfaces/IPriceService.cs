namespace Application.Interfaces
{
    public interface IPriceService
    {
        Task<decimal> Get24hAvgPrice(string symbol);
        Task<decimal> GetSimpleMovingAverage(string symbol, int n, string timePeriod, DateTime? startDate);
    }
}
