using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PYBWeb.Infrastructure.Data
{
    public class AmbienteDbContextFactory : IDesignTimeDbContextFactory<AmbienteDbContext>
    {
        public AmbienteDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AmbienteDbContext>();
            var pastaData = Path.Combine(AppContext.BaseDirectory, "..", "DATA_PYB");
            var connectionString = $"Data Source={Path.Combine(pastaData, "dados2025.db")}";
            optionsBuilder.UseSqlite(connectionString);

            return new AmbienteDbContext(connectionString);
        }
    }
}