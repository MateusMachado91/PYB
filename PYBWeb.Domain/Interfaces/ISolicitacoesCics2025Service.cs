using PYBWeb.Domain.Entities;

namespace PYBWeb.Domain.Interfaces;

/// <summary>
/// Interface para serviço de solicitações CICS 2025
/// </summary>
public interface ISolicitacoesCics2025Service
{
    /// <summary>
    /// Salva uma nova solicitação CICS
    /// </summary>
    Task<SolicitacaoCics2025> SalvarSolicitacaoAsync(SolicitacaoCics2025 solicitacao);
    
    /// <summary>
    /// Obtém todas as solicitações
    /// </summary>
    Task<List<SolicitacaoCics2025>> ObterTodasSolicitacoesAsync();
    
    /// <summary>
    /// Obtém solicitação por ID
    /// </summary>
    Task<SolicitacaoCics2025?> ObterSolicitacaoPorIdAsync(int id);
    
    /// <summary>
    /// Atualiza uma solicitação existente
    /// </summary>
    Task<SolicitacaoCics2025?> AtualizarSolicitacaoAsync(SolicitacaoCics2025 solicitacao);
    
    /// <summary>
    /// Exclui uma solicitação
    /// </summary>
    Task<bool> ExcluirSolicitacaoAsync(int id);
    
    /// <summary>
    /// Desconsiderar uma solicitação (soft delete)
    /// </summary>
    Task<bool> DesconsiderarSolicitacaoAsync(int id);
    
    /// <summary>
    /// Gera um número único para a solicitação
    /// </summary>
    Task<string> GerarNumeroSolicitacaoAsync();
    
    /// <summary>
    /// Obtém solicitações por status
    /// </summary>
    Task<List<SolicitacaoCics2025>> ObterSolicitacoesPorStatusAsync(string status);
    
    Task<bool> AlterarStatusSolicitacaoAsync(int id, string novoStatus, string? responsavel = null, string? observacoes = null);

    Task<int> ObterNumeroSolicitacoesPendentesAsync();

    Task<int> ObterNumeroImplementacoesPendentesAsync();

}

