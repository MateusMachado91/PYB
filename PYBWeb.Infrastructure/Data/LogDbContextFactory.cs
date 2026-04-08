using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.IO;

namespace PYBWeb.Infrastructure.Data;

/// <summary>
/// Factory para criação do contexto de logs em design-time (migrations)
/// </summary>
public class LogDbContextFactory : IDesignTimeDbContextFactory<LogDbContext>
{
    public LogDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LogDbContext>();
        
        // Use a mesma path que a aplicação em runtime (padronize)
        var pastaData = Path.Combine(AppContext.BaseDirectory, "..", "DATA_PYB", "LOG");
        var dbPath = Path.Combine(pastaData, "logs.db");
        
        optionsBuilder.UseSqlite($"Data Source={dbPath}");

        return new LogDbContext(optionsBuilder.Options);
    }
}