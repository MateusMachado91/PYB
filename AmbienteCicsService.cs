using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PYBWeb.Domain.Entities;
using PYBWeb.Domain.Interfaces;
using PYBWeb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace PYBWeb.Infrastructure.Services;

/// <summary>
/// Serviço para gerenciar ambientes CICS
/// </summary>
public class AmbienteCicsService : IAmbienteCicsService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AmbienteCicsService> _logger;

    public AmbienteCicsService(IConfiguration configuration, ILogger<AmbienteCicsService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<IEnumerable<AmbienteCics>> ObterAmbientesAtivosAsync()
    {
        try
        {
            var connectionString = ObterConnectionString();
            using var context = new AmbienteDbContext(connectionString);
            
            // Verificar se há dados, se não há, inicializar com dados de exemplo
            var count = await context.Ambientes.CountAsync();
            
            return await context.Ambientes
                .Where(a => a.Ativo)
                .OrderBy(a => a.Nome)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter ambientes ativos");
            return new List<AmbienteCics>();
        }
    }

    public async Task<IEnumerable<AmbienteCics>> ObterTodosAmbientesAsync()
    {
        try
        {
            var connectionString = ObterConnectionString();
            using var context = new AmbienteDbContext(connectionString);
            
            return await context.Ambientes
                .OrderBy(a => a.Nome)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter todos os ambientes");
            return new List<AmbienteCics>();
        }
    }


    public async Task<AmbienteCics?> ObterAmbientePorIdAsync(int id)
    {
        try
        {
            var connectionString = ObterConnectionString();
            using var context = new AmbienteDbContext(connectionString);
            
            return await context.Ambientes
                .FirstOrDefaultAsync(a => a.Id == id && a.Ativo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter ambiente por ID: {Id}", id);
            return null;
        }
    }

    public async Task<AmbienteCics?> ObterAmbientePorNomeAsync(string nome)
    {
        try
        {
            var connectionString = ObterConnectionString();
            using var context = new AmbienteDbContext(connectionString);
            
            return await context.Ambientes
                .FirstOrDefaultAsync(a => a.Nome.ToLower() == nome.ToLower() && a.Ativo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter ambiente por nome: {Nome}", nome);
            return null;
        }
    }

    public async Task<AmbienteCics? > ObterPorNomeAsync(string idChave)
{
    try
    {
        var connectionString = ObterConnectionString();
        using var context = new AmbienteDbContext(connectionString);
        
        // Busca por IdChave (case-insensitive)
        return await context.Ambientes
            .FirstOrDefaultAsync(a => a.IdChave.ToLower() == idChave.ToLower());
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Erro ao obter ambiente por IdChave: {IdChave}", idChave);
        return null;
    }
}

    public async Task<List<AmbienteCics>> ObterAmbientesPxuPxsAsync()
    {
        try
        {
            using var context = new AmbienteDbContext(ObterConnectionString());
            
            // Lista dos ambientes que fazem parte do "TODOS" para PXU/PXS
            var ambientesPxuPxs = new[]
            {
                "ACICSE", "ACICSF", "ACICSG", "ACICSH", "ACICSI", "ACICSJ",
                "ACICSM", "ACICSN", "ACICSO", "ACICS", "ACICS2", "ACICS8",
                "ACICS9", "BCICS5", "CCICS3", "CCICS4", "ACICS6", "ACICS7"
            };
            
            return await context.Ambientes
                .Where(a => a.Ativo && ambientesPxuPxs.Contains(a.IdChave))
                .OrderBy(a => a.IdChave)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter ambientes PXU/PXS");
            return new List<AmbienteCics>();
        }
    }

    public async Task<bool> CriarAmbienteAsync(AmbienteCics novoAmbiente)
    {
        try
        {
            using var context = new AmbienteDbContext(ObterConnectionString());
            context.Ambientes.Add(novoAmbiente);
            await context.SaveChangesAsync();
            _logger.LogInformation("Novo ambiente criado com sucesso: {Nome}", novoAmbiente.Nome);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar ambiente: {Nome}", novoAmbiente.Nome);
            return false;
        }
    }

    public async Task<bool> ExcluirAmbienteAsync(int id)
    {
        try
        {
            using var context = new AmbienteDbContext(ObterConnectionString());
            var ambiente = await context.Ambientes.FindAsync(id);
            if (ambiente == null)
                return false;

            context.Ambientes.Remove(ambiente);
            await context.SaveChangesAsync();
            return true;
        }
        catch
        {
            // logar erro se quiser
            return false;
        }
    }


    public async Task<bool> AtualizarAmbienteAsync(AmbienteCics ambiente)
    {
        try
        {
            using var context = new AmbienteDbContext(ObterConnectionString());
            
            var ambienteExistente = await context.Ambientes.FindAsync(ambiente.Id);
            if (ambienteExistente == null)
            {
                _logger.LogWarning("Ambiente não encontrado para atualização: {Id}", ambiente.Id);
                return false;
            }

            // Atualizar campos
            ambienteExistente.Nome = ambiente.Nome;
            ambienteExistente.Descricao = ambiente.Descricao;
            ambienteExistente.Ambiente = ambiente.Ambiente;
            ambienteExistente.Maquina = ambiente.Maquina;
            ambienteExistente.Sufixo = ambiente.Sufixo;
            ambienteExistente.Isc = ambiente.Isc;
            ambienteExistente.SteplibCsd = ambiente.SteplibCsd;
            ambienteExistente.DsnameDfhcsd = ambiente.DsnameDfhcsd;
            ambienteExistente.Servidor = ambiente.Servidor;
            ambienteExistente.Porta = ambiente.Porta;
            ambienteExistente.Ativo = ambiente.Ativo;

            await context.SaveChangesAsync();
            _logger.LogInformation("Ambiente atualizado com sucesso: {Id} - {Nome}", ambiente.Id, ambiente.Nome);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar ambiente: {Id}", ambiente.Id);
            return false;
        }
    }

    public async Task<bool> AlterarStatusAmbienteAsync(int id, bool ativo)
    {
        try
        {
            using var context = new AmbienteDbContext(ObterConnectionString());
            
            var ambiente = await context.Ambientes.FindAsync(id);
            if (ambiente == null)
            {
                _logger.LogWarning("Ambiente não encontrado para alteração de status: {Id}", id);
                return false;
            }

            ambiente.Ativo = ativo;
            await context.SaveChangesAsync();
            
            _logger.LogInformation("Status do ambiente alterado: {Id} - {Nome} -> {Ativo}", id, ambiente.Nome, ativo);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alterar status do ambiente: {Id}", id);
            return false;
        }
    }

    private string ObterConnectionString()
    {
        var pastaData = _configuration.GetValue<string>("PastaData") ?? ".\\DATA_PYB\\";
        var caminhoCompleto = Path.GetFullPath(Path.Combine(pastaData, "ambiente.db"));
        return $"Data Source={caminhoCompleto}";
    }
}