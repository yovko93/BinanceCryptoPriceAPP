namespace CryptoPriceConsoleApp
{
    public class App
    {
        private readonly CommandHandler _commandHandler;

        public App(CommandHandler commandHandler)
        {
            _commandHandler = commandHandler;
        }

        public async Task RunAsync()
        {
            Console.WriteLine("Enter a command (type 'exit' to quit): ");
            Console.WriteLine("24h {symbol} - Returns the average price for the last 24h of data");
            Console.WriteLine("sma {symbol} {n} {p} {s}(optional)(YYYY-MM-DD) - Return the current Simple Moving average of the symbol's price");
            Console.WriteLine("symbol options - BTCUSDT / ETHUSDT / ADAUSDT");
            Console.WriteLine("p options - 1w / 1d / 30m / 5m / 1m");

            while (true)
            {
                var input = Console.ReadLine();

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
