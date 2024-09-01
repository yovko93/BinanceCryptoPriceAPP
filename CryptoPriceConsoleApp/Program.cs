namespace CryptoPriceConsoleApp
{
    #region Usings
    using Application.Interfaces;
    using Application.Services;
    using Data.Context;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    #endregion

    class Program
    {
        static async Task Main(string[] args)
        {
            // 1. Set up a service collection and register services
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            // 2. Build the service provider
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // 3. Run the application
            using (var scope = serviceProvider.CreateScope())
            {
                var app = scope.ServiceProvider.GetRequiredService<App>();
                await app.RunAsync();
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql("Host=localhost;Database=crypto_price_data_db;Username=myappuser;Password=mysecretpassword"));

            services.AddMemoryCache();

            services.AddScoped<IPriceService, PriceService>();
            services.AddScoped<App>();
            services.AddScoped<CommandHandler>();
        }
    }
}
