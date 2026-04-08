using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using PYBWeb.Domain.Entities;
using PYBWeb.Domain.Interfaces;
using PYBWeb.Infrastructure.Data;

namespace PYBWeb.Infrastructure.Services;

/// <summary>
/// Serviço para gerenciar solicitações CICS 2025
/// </summary>





public class SolicitacoesCics2025Service : ISolicitacoesCics2025Service
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SolicitacoesCics2025Service> _logger;

    public async Task<int> ObterNumeroSolicitacoesPendentesAsync()
    {
    var todas = await ObterTodasSolicitacoesAsync();
    return todas.Count(x => x.Status == "Pendente");
    }

    public async Task<int> ObterNumeroImplementacoesPendentesAsync()
    {
    var todas = await ObterTodasSolicitacoesAsync();
    return todas.Count(x => x.Status == "Pendente");
    }

    public SolicitacoesCics2025Service(IConfiguration configuration, ILogger<SolicitacoesCics2025Service> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<SolicitacaoCics2025> SalvarSolicitacaoAsync(SolicitacaoCics2025 solicitacao)
    {
        try
        {
            using var context = new Dados2025DbContext(ObterConnectionString());
            
            // Garante que o banco existe (sem deletar dados existentes)
            await context.Database.EnsureCreatedAsync();
            
            // Gera número da solicitação se não informado
            if (string.IsNullOrEmpty(solicitacao.NumeroSolicitacao))
            {
                solicitacao.NumeroSolicitacao = await GerarNumeroSolicitacaoInternoAsync(context);
            }
            
            // Define campos de auditoria
            solicitacao.DataCriacao = DateTime.Now;
            solicitacao.UsuarioCriacao = solicitacao.Usuario;
            solicitacao.Ativo = true;
            
            _logger.LogInformation("Adicionando solicitação ao contexto: {NumeroSolicitacao}", solicitacao.NumeroSolicitacao);
            
            context.SolicitacoesCics.Add(solicitacao);
            await context.SaveChangesAsync();
            
            _logger.LogInformation("Solicitação CICS salva com sucesso: {NumeroSolicitacao}", solicitacao.NumeroSolicitacao);
            return solicitacao;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao salvar solicitação CICS");
            throw;
        }
    }

    public async Task<List<SolicitacaoCics2025>> ObterTodasSolicitacoesAsync()
    {
        try
        {
            _logger.LogInformation("🔍 ObterTodasSolicitacoesAsync INICIADO");
            
            using var context = new Dados2025DbContext(ObterConnectionString());
            
            // Garante que o banco existe antes de tentar ler
            var created = await context.Database.EnsureCreatedAsync();
            _logger.LogInformation($"📁 Banco de dados criado: {created}");
            
            var connectionString = ObterConnectionString();
            _logger.LogInformation($"📋 Connection String: {connectionString}");
            
            var solicitacoes = await context.SolicitacoesCics
                .Where(s => s.Ativo)
                .OrderByDescending(s => s.DataCriacao)
                .ToListAsync();
                
            _logger.LogInformation($"📊 Encontradas {solicitacoes.Count} solicitações no SQLite");
            
            foreach (var sol in solicitacoes)
            {
                _logger.LogInformation($"  - {sol.NumeroSolicitacao}: {sol.Status}");
            }
            
            return solicitacoes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao obter todas as solicitações");
            return new List<SolicitacaoCics2025>();
        }
    }

    public async Task<SolicitacaoCics2025?> ObterSolicitacaoPorIdAsync(int id)
    {
        try
        {
            using var context = new Dados2025DbContext(ObterConnectionString());
            
            return await context.SolicitacoesCics
                .FirstOrDefaultAsync(s => s.Id == id && s.Ativo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter solicitação por ID: {Id}", id);
            return null;
        }
    }

    public async Task<SolicitacaoCics2025?> AtualizarSolicitacaoAsync(SolicitacaoCics2025 solicitacao)
    {
        try
        {
            using var context = new Dados2025DbContext(ObterConnectionString());
            
            var solicitacaoExistente = await context.SolicitacoesCics
                .FirstOrDefaultAsync(s => s.Id == solicitacao.Id && s.Ativo);
                
            if (solicitacaoExistente == null)
                return null;
                
            // Atualiza campos modificáveis
            solicitacaoExistente.DataAtualizacao = DateTime.Now;
            solicitacaoExistente.UsuarioAtualizacao = solicitacao.UsuarioAtualizacao ?? solicitacao.Usuario;
            
            // Copia os campos do formulário
            CopiarCamposFormulario(solicitacao, solicitacaoExistente);
            
            await context.SaveChangesAsync();
            
            _logger.LogInformation("Solicitação atualizada com sucesso: {Id}", solicitacao.Id);
            return solicitacaoExistente;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar solicitação: {Id}", solicitacao.Id);
            throw;
        }
    }

    public async Task<bool> ExcluirSolicitacaoAsync(int id)
    {
        try
        {
            using var context = new Dados2025DbContext(ObterConnectionString());
            
            var solicitacao = await context.SolicitacoesCics
                .FirstOrDefaultAsync(s => s.Id == id && s.Ativo);
                
            if (solicitacao == null)
                return false;
                
            context.SolicitacoesCics.Remove(solicitacao);
            await context.SaveChangesAsync();
            
            _logger.LogInformation("Solicitação excluída com sucesso: {Id}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir solicitação: {Id}", id);
            return false;
        }
    }

    public async Task<bool> AlterarStatusSolicitacaoAsync(int id, string novoStatus, string? responsavel = null, string? observacoes = null)
    {
    try
    {
        using var context = new Dados2025DbContext(ObterConnectionString());

        var solicitacao = await context.SolicitacoesCics.FirstOrDefaultAsync(s => s.Id == id && s.Ativo);
        if (solicitacao == null)
            return false;

        solicitacao.Status = novoStatus;
        solicitacao.DataAtualizacao = DateTime.Now;

        // Se o status for Rejeitada ou Aprovada, registra DataEfetivacao e Responsável
        if (novoStatus == "Rejeitada" || novoStatus == "Aprovada")
        {
            solicitacao.DataEfetivacao = DateTime.Now;
            solicitacao.ResponsavelEfetivacao = responsavel;
        }

        // Se houver observações, salva em ObservacoesAdmin
        if (!string.IsNullOrWhiteSpace(observacoes))
        {
            solicitacao.ObservacoesAdmin = observacoes;
        }

        await context.SaveChangesAsync();
        _logger.LogInformation("Status da solicitação alterado com sucesso: {Id} -> {NovoStatus}", id, novoStatus);
        return true;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Erro ao alterar status da solicitação: {Id} -> {NovoStatus}", id, novoStatus);
        return false;
    }
    }

    public async Task<bool> DesconsiderarSolicitacaoAsync(int id)
    {
        try
        {
            using var context = new Dados2025DbContext(ObterConnectionString());
            
            var solicitacao = await context.SolicitacoesCics
                .FirstOrDefaultAsync(s => s.Id == id && s.Ativo);
                
            if (solicitacao == null)
                return false;
                
            solicitacao.Ativo = false;
            solicitacao.Status = "Desconsiderada";
            solicitacao.DataAtualizacao = DateTime.Now;
            
            await context.SaveChangesAsync();
            
            _logger.LogInformation("Solicitação desconsiderada com sucesso: {Id}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao desconsiderar solicitação: {Id}", id);
            return false;
        }
    }

    public async Task<string> GerarNumeroSolicitacaoAsync()
    {
        try
        {
            using var context = new Dados2025DbContext(ObterConnectionString());
            
            await context.Database.EnsureCreatedAsync();
            
            return await GerarNumeroSolicitacaoInternoAsync(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar número da solicitação");
            return $"CICS{DateTime.Now.Year}{DateTime.Now.Ticks % 10000:D4}";
        }
    }

    private async Task<string> GerarNumeroSolicitacaoInternoAsync(Dados2025DbContext context)
    {
        try
        {
            var ano = DateTime.Now.Year;
            var prefixo = $"CICS{ano}";
            
            var ultimoNumero = await context.SolicitacoesCics
                .Where(s => s.NumeroSolicitacao.StartsWith(prefixo))
                .Select(s => s.NumeroSolicitacao)
                .OrderByDescending(n => n)
                .FirstOrDefaultAsync();
                
            int proximoSequencial = 1;
            
            if (!string.IsNullOrEmpty(ultimoNumero))
            {
                var parteSequencial = ultimoNumero.Substring(prefixo.Length);
                if (int.TryParse(parteSequencial, out int numeroAtual))
                {
                    proximoSequencial = numeroAtual + 1;
                }
            }
            
            return $"{prefixo}{proximoSequencial:D4}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar número da solicitação interno");
            return $"CICS{DateTime.Now.Year}{DateTime.Now.Ticks % 10000:D4}";
        }
    }

    public async Task<List<SolicitacaoCics2025>> ObterSolicitacoesPorStatusAsync(string status)
    {
        try
        {
            using var context = new Dados2025DbContext(ObterConnectionString());
            
            return await context.SolicitacoesCics
                .Where(s => s.Ativo && s.Status == status)
                .OrderByDescending(s => s.DataCriacao)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter solicitações por status: {Status}", status);
            return new List<SolicitacaoCics2025>();
        }
    }

    private void CopiarCamposFormulario(SolicitacaoCics2025 origem, SolicitacaoCics2025 destino)
    {
        // Campos gerais
        destino.AmbienteId = origem.AmbienteId;
        destino.Appli = origem.Appli;
        destino.Usuario = origem.Usuario;
        destino.Css = origem.Css;
        destino.TipoTabela = origem.TipoTabela;
        destino.Status = origem.Status;
        destino.DataEfetivacao = origem.DataEfetivacao;
        destino.ResponsavelEfetivacao = origem.ResponsavelEfetivacao;
        destino.Operacao = origem.Operacao;
        
        // Campos FCT
        destino.NameArq = origem.NameArq;
        destino.Type = origem.Type;
        destino.DsnameArq = origem.DsnameArq;
        destino.EstInit = origem.EstInit;
        destino.Service = origem.Service;
        destino.NumStrng = origem.NumStrng;
        
        // Campos DCT
        destino.FileName = origem.FileName;

        destino.Dsname = origem.Dsname;

        destino.FormReg = origem.FormReg;
        destino.FormReg2 = origem.FormReg2;
        
        // Campos PCT
        destino.NameTrans = origem.NameTrans;
        destino.ActiveSoft = origem.ActiveSoft;
        destino.TwaSize = origem.TwaSize;
        destino.Coment = origem.Coment;
        destino.Prev = origem.Prev;
        destino.DataAllocation = origem.DataAllocation;
        
        // Campos PPT
        destino.NameSoft = origem.NameSoft;
        destino.LinkName = origem.LinkName;
        destino.Language = origem.Language;
        destino.AutoAlt = origem.AutoAlt;
        
        // Observações
        destino.ObservacoesAdmin = origem.ObservacoesAdmin;
    }

    private string ObterConnectionString()
        {
            var pastaData = _configuration.GetValue<string>("PastaData") ?? ".\\DATA_PYB\\";
            var caminhoCompleto = Path.GetFullPath(Path.Combine(pastaData, "dados2025.db"));
            return $"Data Source={caminhoCompleto}";
        }


}