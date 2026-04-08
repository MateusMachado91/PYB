namespace PYBWeb.Domain.Enums;

/// <summary>
/// Status possíveis para uma solicitação CICS
/// </summary>
public enum StatusSolicitacao
{
    /// <summary>
    /// Solicitação criada, aguardando análise
    /// </summary>
    Pendente = 1,
    
    /// <summary>
    /// Solicitação em análise
    /// </summary>
    EmAnalise = 2,
    
    /// <summary>
    /// Solicitação aprovada
    /// </summary>
    Aprovada = 3,
    
    /// <summary>
    /// Solicitação rejeitada
    /// </summary>
    Rejeitada = 4,
    
    /// <summary>
    /// Solicitação implementada
    /// </summary>
    Implementada = 5,
    
    /// <summary>
    /// Solicitação cancelada
    /// </summary>
    Cancelada = 6
}