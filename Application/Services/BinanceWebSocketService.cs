namespace Application.Services
{
    #region Usings
    using Data.Context;
    using Data.Models;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json.Linq;
    using System.Net.WebSockets;
    using System.Text;
    #endregion

    public class BinanceWebSocketService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private ClientWebSocket _clientWebSocket;
        private readonly object _lock = new object();
        private CancellationTokenSource _manualCancellationSource;
        private CancellationToken _stoppingToken;
        private readonly ILogger<BinanceWebSocketService> _logger;

        public BinanceWebSocketService(
            IServiceProvider serviceProvider,
            ILogger<BinanceWebSocketService> logger)
        {
            _serviceProvider = serviceProvider;
            _manualCancellationSource = new CancellationTokenSource();
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Store the stoppingToken to be used in the Start/Stop methods
            _stoppingToken = stoppingToken;

            await StartAsync(_manualCancellationSource.Token);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            lock (_lock)
            {
                if (_clientWebSocket != null && _clientWebSocket.State == WebSocketState.Open)
                    return;
            }

            _clientWebSocket = new ClientWebSocket();

            var uri = new Uri("wss://stream.binance.com:9443/ws/" +
                         "btcusdt@kline_1m/btcusdt@kline_5m/btcusdt@kline_30m/btcusdt@kline_1d/btcusdt@kline_1w/" +
                         "adausdt@kline_1m/adausdt@kline_5m/adausdt@kline_30m/adausdt@kline_1d/adausdt@kline_1w/" +
                         "ethusdt@kline_1m/ethusdt@kline_5m/ethusdt@kline_30m/ethusdt@kline_1d/ethusdt@kline_1w");

            await _clientWebSocket.ConnectAsync(uri, cancellationToken);

            _ = Task.Run(async () => await ReceiveDataAsync(cancellationToken), cancellationToken);
        }

        public void Stop()
        {
            lock (_lock)
            {
                if (_clientWebSocket != null && _clientWebSocket.State == WebSocketState.Open)
                {
                    _manualCancellationSource.Cancel();
                    _clientWebSocket?.Dispose();
                    _clientWebSocket = null;
                    _manualCancellationSource = new CancellationTokenSource();
                }
            }
        }

        public async Task RestartWebSocketAsync()
        {
            Stop();
            await StartAsync(_manualCancellationSource.Token);
        }

        private async Task ReceiveDataAsync(CancellationToken token)
        {
            var buffer = new byte[1024 * 4];
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var result = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), token);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, token);
                        break;
                    }

                    var jsonString = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    await HandleMessageAsync(jsonString);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error receiving data: {ex.Message}");
                    break;
                }
            }
        }

        private async Task HandleMessageAsync(string jsonString)
        {
            var jObject = JObject.Parse(jsonString);

            if (jObject["e"]?.ToString() == "kline")
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    try
                    {
                        var klineData = new KlineData
                        {
                            Symbol = jObject["s"]?.ToString(),
                            Interval = jObject["k"]["i"]?.ToString(),
                            KlineStartTime = (long)jObject["k"]["t"],
                            KlineCloseTime = (long)jObject["k"]["T"],
                            ClosePrice = decimal.Parse(jObject["k"]["c"]?.ToString() ?? "0"),
                            NumberOfTrades = (int)jObject["k"]["n"],
                        };

                        //var cryptoPriceDB = dbContext.KlineDatas
                        //    .FirstOrDefault(e => e.KlineCloseTime == klineData.KlineCloseTime
                        //                                         && e.Symbol == klineData.Symbol
                        //                                         && e.Interval == klineData.Interval
                        //                                         && e.ClosePrice == klineData.ClosePrice);

                        dbContext.KlineDatas.Add(klineData);
                        await dbContext.SaveChangesAsync();
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e.Message);
                    }
                }
            }
        }
    }
}