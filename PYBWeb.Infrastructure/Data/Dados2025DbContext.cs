using Microsoft.EntityFrameworkCore;
using PYBWeb.Domain.Entities;

namespace PYBWeb.Infrastructure.Data;

/// <summary>
/// Contexto para o banco de dados dados2025.db
/// </summary>
public class Dados2025DbContext : DbContext
{
    private readonly string _connectionString;

    public Dados2025DbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DbSet<SolicitacaoCics2025> SolicitacoesCics { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(_connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurações da entidade SolicitacaoCics2025
        modelBuilder.Entity<SolicitacaoCics2025>(entity =>
        {
            entity.ToTable("SolicitacoesCics");
            
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.NumeroSolicitacao)
                .IsRequired()
                .HasMaxLength(50);
                
            entity.Property(e => e.Appli)
                .IsRequired()
                .HasMaxLength(50);
                
            entity.Property(e => e.Usuario)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.Operacao)
                .IsRequired()
                .HasMaxLength(20);
                
            entity.Property(e => e.Css)
                .IsRequired()
                .HasMaxLength(3);
                
            entity.Property(e => e.TipoTabela)
                .IsRequired()
                .HasMaxLength(10);
                
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pendente");
                
            entity.Property(e => e.DataSolicitacao)
                .HasDefaultValueSql("datetime('now', 'localtime')");
                
            // Índices para melhor performance
            entity.HasIndex(e => e.NumeroSolicitacao).IsUnique();
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.DataSolicitacao);
            entity.HasIndex(e => e.Usuario);
            entity.HasIndex(e => e.Operacao);
            entity.HasIndex(e => e.TipoTabela);
        });
    }
}