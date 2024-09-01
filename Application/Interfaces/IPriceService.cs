namespace Application.Interfaces
{
    #region Usigns
    using Models;
    #endregion

    public interface IPriceService
    {
        Task<Result<AveragePriceResult>> Get24hAvgPrice(string symbol);
        Task<Result<SMAResult>> GetSimpleMovingAverage(string symbol, int n, string timePeriod, DateTime? startDate);
    }
}
