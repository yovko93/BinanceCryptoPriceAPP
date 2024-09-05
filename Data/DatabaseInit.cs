namespace Data
{
    #region Usings
    using Data.Context;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    #endregion

    public static class DatabaseInit
    {
        public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
        {
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("DatabaseInitializer");

            try
            {
                logger.LogInformation("Try Initializing the database.......");
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    //logger.LogInformation("Ensuring database exists and applying migrations...");

                    //if (!await context.Database.CanConnectAsync())
                    //{
                    //    logger.LogInformation("Database does not exist. Creating database...");
                    //    var isCreatedDB = await context.Database.EnsureCreatedAsync();
                    //    logger.LogInformation($"Is db created - {isCreatedDB}");
                    //}
                    //else
                    //{
                    logger.LogInformation("Applying migrations...");
                    await context.Database.MigrateAsync();
                    //}

                    logger.LogInformation("Database migrations completed.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initializing the database.");
                throw;
            }
        }
    }
}
