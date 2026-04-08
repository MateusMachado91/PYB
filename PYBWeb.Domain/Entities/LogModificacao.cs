namespace PYBWeb.Domain.Entities;

/// <summary>
/// Entidade para registro de logs de modificações
/// </summary>
public class LogModificacao
{
    public int Id { get; set; }
    
    /// <summary>
    /// Data e hora da modificação
    /// </summary>
    public DateTime DataHora { get; set; }
    
    /// <summary>
    /// Usuário que realizou a modificação (ex: CORP\E38235)
    /// </summary>
    public string Usuario { get; set; } = string.Empty;
    
    /// <summary>
    /// Tipo de ação realizada
    /// </summary>
    public string Acao { get; set; } = string.Empty;
    
    /// <summary>
    /// Tabela/Entidade afetada
    /// </summary>
    public string Tabela { get; set; } = string.Empty;
    
    /// <summary>
    /// ID do registro afetado (nullable)
    /// </summary>
    public int? RegistroId { get; set; }
    
    /// <summary>
    /// Identificador do registro (ex: número da solicitação, nome do ambiente)
    /// </summary>
    public string? RegistroIdentificador { get; set; }
    
    /// <summary>
    /// Detalhes da modificação (JSON ou texto livre)
    /// </summary>
    public string? Detalhes { get; set; }
    
    /// <summary>
    /// Status anterior (para mudanças de status)
    /// </summary>
    public string? StatusAnterior { get; set; }
    
    /// <summary>
    /// Status novo (para mudanças de status)
    /// </summary>
    public string? StatusNovo { get; set; }
}
