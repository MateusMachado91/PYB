using Microsoft.EntityFrameworkCore;
using PYBWeb.Domain.Entities;

namespace PYBWeb.Infrastructure.Data;

/// <summary>
/// Contexto para o banco ambiente.db
/// </summary>
public class AmbienteDbContext : DbContext
{
    private readonly string _connectionString;

    public AmbienteDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DbSet<AmbienteCics> Ambientes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(_connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Mapeia para a tabela existente (assumindo que existe uma tabela 'ambiente')
        modelBuilder.Entity<AmbienteCics>(entity =>
        {
            entity.ToTable("ambiente"); // Mapeia para a tabela 'ambiente'
            entity.HasKey(e => e.Id);
            entity.Property(e => e.IdChave).HasColumnName("id_chave").HasMaxLength(50);
            entity.Property(e => e.Nome).HasMaxLength(100);
            entity.Property(e => e.Descricao).HasMaxLength(200);
            entity.Property(e => e.Ambiente).HasMaxLength(50);
            entity.Property(e => e.Maquina).HasMaxLength(10);
            entity.Property(e => e.Sufixo).HasMaxLength(10);
            entity.Property(e => e.Isc).HasMaxLength(50);
            entity.Property(e => e.SteplibCsd).HasMaxLength(200);
            entity.Property(e => e.DsnameDfhcsd).HasMaxLength(200);
            entity.Property(e => e.Servidor).HasMaxLength(100);
            entity.Property(e => e.Pxu).HasMaxLength(10);
        });
    }
}