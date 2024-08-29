namespace Application.Services
{
    #region Usings
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    #endregion

    public class BinanceWebSocketService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public BinanceWebSocketService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var priceDataCollector = scope.ServiceProvider.GetRequiredService<PriceDataCollector>();
                    await priceDataCollector.CollectPriceDataAsync(stoppingToken);
                }
            }
        }
    }
}
