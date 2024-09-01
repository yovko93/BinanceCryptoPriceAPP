namespace BinanceCryptoPriceAPI.Infrastructure.Extensions
{
    #region Usings
    using Application.Interfaces;
    using Application.Services;
    using Data.Context;
    using Microsoft.EntityFrameworkCore;
    #endregion

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            // Add DbContext with PostgreSQL provider
            services.AddDbContext<AppDbContext>(options =>
            {
                string connectionString = configuration.GetConnectionString("DefaultConnection");
                options.UseNpgsql(connectionString);
            });

            return services;
        }

        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddSingleton<BinanceWebSocketService>();
            services.AddHostedService(provider => provider.GetRequiredService<BinanceWebSocketService>());

            services.AddScoped<IPriceService, PriceService>();

            return services;
        }
    }
}
