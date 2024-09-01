using Application.Services;

namespace CryptoPriceConsoleApp
{
    public class App
    {
        private CommandHandler _commandHandler;
        private BinanceWebSocketService _binanceWebSocketService;

        public App(CommandHandler commandHandler, BinanceWebSocketService binanceWebSocketService)
        {
            _commandHandler = commandHandler;
            _binanceWebSocketService = binanceWebSocketService;
        }

        public async Task RunAsync()
        {
            Console.WriteLine("Enter a command (type 'exit' to quit): ");
            Console.WriteLine();
            Console.WriteLine("24h {symbol} - Returns the average price for the last 24h of data");
            Console.WriteLine("sma {symbol} {n} {p} {s}(optional)(YYYY-MM-DD) - Return the current Simple Moving average of the symbol's price. (symbol-BTCUSDT/ETHUSDT/ADAUSDT | p-1w/1d/30m/5m/1m)");
            Console.WriteLine();
            Console.WriteLine("Type 'start' or 'stop' to start/stop the websocket service");

            while (true)
            {
                var input = Console.ReadLine();

                if (input == "start")
                {
                    await _binanceWebSocketService.RestartWebSocketAsync();
                    Console.WriteLine("Web socket started!");
                    continue;
                }

                if (input == "stop")
                {
                    _binanceWebSocketService.Stop();
                    Console.WriteLine("Web socket stoped!");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Invalid command.");
                    continue;
                }


                if (input.Trim().ToLower() == "exit")
                    break;

                await _commandHandler.HandleCommandAsync(input);
            }
        }
    }
}
