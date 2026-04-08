using Microsoft.EntityFrameworkCore;
using PYBWeb.Domain.Entities;
using PYBWeb.Infrastructure.Data;

var pastaData = Path.Combine(AppContext.BaseDirectory, "..", "DATA_PYB");
var connectionString = $"Data Source={Path.Combine(pastaData, "ambiente.db")}";
using var context = new AmbienteDbContext(connectionString);

// Verificar se já existem dados
var existentes = await context.AmbientesCics.CountAsync();
Console.WriteLine($"Ambientes existentes: {existentes}");

// Listar todos os ambientes
var ambientes = await context.AmbientesCics.Where(a => a.Ativo).ToListAsync();
Console.WriteLine($"\nAmbientes ativos ({ambientes.Count}):");
foreach (var ambiente in ambientes)
{
    Console.WriteLine($"- {ambiente.Nome}: {ambiente.Descricao} ({ambiente.Servidor}:{ambiente.Pxu})");
}