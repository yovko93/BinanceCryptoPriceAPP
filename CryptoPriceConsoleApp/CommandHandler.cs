using Application.Interfaces;

namespace CryptoPriceConsoleApp
{
    public class CommandHandler
    {
        private readonly IPriceService _priceService;

        public CommandHandler(IPriceService priceService)
        {
            _priceService = priceService;
        }

        public async Task HandleCommandAsync(string input)
        {
            // Split the command into parts
            string[] commandParts = input.Split(' ');

            string command = commandParts[0].ToLower();

            switch (command)
            {
                case "24h":
                if (commandParts.Length == 2)
                {
                    string symbol = commandParts[1];
                    if (!await CheckSymbol(symbol))
                    {
                        return;
                    }

                    await Get24hAvgPrice(symbol);
                }
                else
                {
                    Console.WriteLine("Invalid usage of 24h command. Correct format: 24h {symbol}");
                }
                break;

                case "sma":
                if (commandParts.Length < 4 || commandParts.Length > 5)
                {
                    Console.WriteLine("Invalid usage of sma command. Correct format: sma {symbol} {n} {p} {s}");
                }
                else
                {
                    string symbol = commandParts[1];
                    if (!await CheckSymbol(symbol))
                    {
                        return;
                    }

                    var n = int.Parse(commandParts[2]);
                    var p = commandParts[3].ToString();
                    DateTime? s = null;

                    if (commandParts.Length == 5)
                    {
                        if (DateTime.TryParse(commandParts[4], out DateTime startDate))
                        {
                            s = startDate;
                        }
                        else
                        {
                            Console.WriteLine("Invalid date format. Please enter a valid date (YYYY-MM-DD).");
                            return;
                        }
                    }

                    await GetSimpleMovingAverage(symbol, n, p, s);
                }
                break;

                default:
                Console.WriteLine("Unknown command.");
                break;
            }
        }

        private async Task Get24hAvgPrice(string symbol)
        {
            try
            {
                var averagePrice = await _priceService.Get24hAvgPrice(symbol);

                Console.WriteLine($"24h Avg Price for {symbol}: {averagePrice}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private async Task GetSimpleMovingAverage(string symbol, int n, string timePeriod, DateTime? startDate)
        {
            try
            {
                var sma = await _priceService.GetSimpleMovingAverage(symbol, n, timePeriod, startDate);

                Console.WriteLine($"SMA for {symbol} (n={n}, p={timePeriod}, s={startDate ?? DateTime.Now}): {sma}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private async Task<bool> CheckSymbol(string symbol)
        {
            if (symbol != "BTCUSDT" && symbol != "ADAUSDT" && symbol != "ETHUSDT")
            {
                Console.WriteLine("Invalid symbol!");
                return false;
            }

            return true;
        }
    }
}
