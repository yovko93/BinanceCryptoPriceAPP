namespace Application.Services
{
    #region Usings
    using Data.Context;
    using Data.Models;
    using Newtonsoft.Json.Linq;
    using System.Net.WebSockets;
    using System.Text;
    #endregion

    public class PriceDataCollector
    {
        private readonly AppDbContext _context;

        public PriceDataCollector(
            AppDbContext context)
        {
            _context = context;
        }

        public async Task CollectPriceDataAsync(CancellationToken cancellationToken)
        {
            using var clientWebSocket = new ClientWebSocket();

            var uri = new Uri("wss://stream.binance.com:9443/ws/btcusdt@trade/adausdt@trade/ethusdt@trade");

            await clientWebSocket.ConnectAsync(uri, cancellationToken);

            var buffer = new byte[1024 * 4];

            while (clientWebSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                //await Task.Delay(100, cancellationToken);

                var result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationToken);
                }
                else
                {
                    var jsonMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    await ProcessMessageAsync(jsonMessage);
                }
            }
        }

        private async Task ProcessMessageAsync(string jsonMessage)
        {
            try
            {
                var jObject = JObject.Parse(jsonMessage);

                var symbol = jObject["s"].ToString(); // Symbol (BTCUSDT/ETHUSDT/ADAUSDT)
                var price = decimal.Parse(jObject["p"].ToString()); // Price

                var priceData = new PriceData
                {
                    Symbol = symbol,
                    Price = price,
                    Timestamp = DateTime.UtcNow
                };

                _context.Prices.Add(priceData);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle errors (e.g., logging)
                Console.WriteLine($"Error processing message: {ex.Message}");
            }
        }
    }
}
