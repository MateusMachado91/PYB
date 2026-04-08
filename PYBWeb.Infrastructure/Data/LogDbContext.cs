using Microsoft.EntityFrameworkCore;
using PYBWeb.Domain.Entities;

namespace PYBWeb.Infrastructure.Data;

/// <summary>
/// Contexto de banco de dados para logs de modificações
/// </summary>
public class LogDbContext : DbContext
{
    public DbSet<LogModificacao> Logs { get; set; }

    public LogDbContext(DbContextOptions<LogDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LogModificacao>(entity =>
        {
            entity.ToTable("Logs");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.DataHora)
                .IsRequired();
            
            entity.Property(e => e.Usuario)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.Acao)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.Property(e => e.Tabela)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.Property(e => e.RegistroIdentificador)
                .HasMaxLength(100);
            
            entity.Property(e => e.StatusAnterior)
                .HasMaxLength(50);
            
            entity.Property(e => e.StatusNovo)
                .HasMaxLength(50);
            
            // Índices para melhorar performance de consultas
            entity.HasIndex(e => e.DataHora);
            entity.HasIndex(e => e.Usuario);
            entity.HasIndex(e => e.Acao);
            entity.HasIndex(e => e.Tabela);
        });
    }
}
