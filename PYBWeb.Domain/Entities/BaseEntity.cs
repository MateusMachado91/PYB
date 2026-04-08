namespace PYBWeb.Domain.Entities;

/// <summary>
/// Classe base para todas as entidades do domínio
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Identificador único da entidade
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Data de criação do registro
    /// </summary>
    public DateTime DataCriacao { get; set; }
    
    /// <summary>
    /// Data da última atualização do registro
    /// </summary>
    public DateTime? DataAtualizacao { get; set; }
    
    /// <summary>
    /// Usuário que criou o registro
    /// </summary>
    public string UsuarioCriacao { get; set; } = string.Empty;
    
    /// <summary>
    /// Usuário que fez a última atualização
    /// </summary>
    public string? UsuarioAtualizacao { get; set; }
    
    /// <summary>
    /// Indica se o registro está ativo
    /// </summary>
    public bool Ativo { get; set; } = true;
}