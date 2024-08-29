namespace Data.Context
{
    #region Usings
    using Data.Models;
    using Microsoft.EntityFrameworkCore;
    #endregion

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<PriceData> Prices { get; set; }
    }
}
