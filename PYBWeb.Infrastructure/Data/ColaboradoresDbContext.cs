using PYBWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace PYBWeb.Infrastructure.Data
{
    public class ColaboradoresDbContext : DbContext
    {
        public ColaboradoresDbContext(DbContextOptions<ColaboradoresDbContext> options)
            : base(options)
        {
        }

        public DbSet<Colaborador> Colaboradores { get; set; }
        public DbSet<LogModificacao> LogsModificacoes { get; set; }
        public DbSet<MembroSuporte> Suporte { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // REMOVER ou COMENTAR estas linhas que usam EhResponsavel
            // modelBuilder.Entity<MembroSuporte>()
            //     .HasIndex(m => m.EhResponsavel);

            // Adicionar índice para Categoria (opcional, mas recomendado)
            modelBuilder.Entity<MembroSuporte>()
                .HasIndex(m => m.Categoria);
        }
    }
}