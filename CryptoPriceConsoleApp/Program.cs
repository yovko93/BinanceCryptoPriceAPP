namespace CryptoPriceConsoleApp
{
    #region Usings
    using Application.Interfaces;
    using Application.Services;
    using Data.Context;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    #endregion

    class Program
    {
        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var commandHandler = scope.ServiceProvider.GetRequiredService<CommandHandler>();
                var serviceProvider = scope.ServiceProvider
                          .GetServices<IHostedService>()
                          .OfType<BinanceWebSocketService>()
                          .Single();

                var app = new App(commandHandler, serviceProvider);
                await app.RunAsync();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Warning); // Log only warnings and errors
            }).ConfigureServices((context, services) =>
            {
                var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
                services.AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(connectionString));

                services.AddMemoryCache();

                // Register background service
                services.AddHostedService<BinanceWebSocketService>();

                // Register app service
                services.AddScoped<IPriceService, PriceService>();
                services.AddScoped<App>();
                services.AddScoped<CommandHandler>();
            });
    }
}
