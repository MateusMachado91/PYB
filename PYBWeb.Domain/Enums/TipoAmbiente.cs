namespace PYBWeb.Domain.Enums;

/// <summary>
/// Ambientes disponíveis para as solicitações CICS
/// </summary>
public enum TipoAmbiente
{
    /// <summary>
    /// Ambiente de desenvolvimento
    /// </summary>
    Desenvolvimento = 1,
    
    /// <summary>
    /// Ambiente de teste
    /// </summary>
    Teste = 2,
    
    /// <summary>
    /// Ambiente de homologação
    /// </summary>
    Homologacao = 3,
    
    /// <summary>
    /// Ambiente de produção
    /// </summary>
    Producao = 4
}