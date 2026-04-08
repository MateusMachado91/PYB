using System.ComponentModel.DataAnnotations;

namespace PYBWeb.Domain.Entities;

/// <summary>
/// Entidade para armazenar solicitações CICS de 2025
/// </summary>
public class SolicitacaoCics2025 : BaseEntity
{

    public string? JustificativaBelow { get; set; }

    // Campos identificadores
    [Required]
    [MaxLength(50)]
    public string NumeroSolicitacao { get; set; } = "";
    
    // Informações gerais
    [Required]
    public int AmbienteId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Appli { get; set; } = "";
    
    [Required]
    [MaxLength(20)]
    public string Usuario { get; set; } = "";
    
    [Required]
    [MaxLength(3)]
    public string Css { get; set; } = "";
    
    // Tipo de tabela
    [Required]
    [MaxLength(10)]
    public string TipoTabela { get; set; } = "";
    
    // Status da solicitação
    [MaxLength(20)]
    public string Status { get; set; } = "Pendente";
    
    public DateTime DataSolicitacao { get; set; } = DateTime.Now;
    
    public DateTime? DataEfetivacao { get; set; }
    
    [MaxLength(20)]
    public string? ResponsavelEfetivacao { get; set; }
    
    public string? Operacao { get; set; }
    
    
    // Campos específicos para FCT
    [MaxLength(100)]
    public string? NameArq { get; set; }
    
    [MaxLength(50)]
    public string? Type { get; set; }
    
    [MaxLength(200)]
    public string? DsnameArq { get; set; }
    
    [MaxLength(50)]
    public string? EstInit { get; set; }
    
    [MaxLength(50)]
    public string? Service { get; set; }
    
    [MaxLength(20)]
    public string? NumStrng { get; set; }
    
    // Campos específicos para DCT
    [MaxLength(100)]
    public string? FileName { get; set; }
    
    
    [MaxLength(200)]
    public string? Dsname { get; set; }
    
    
    [MaxLength(50)]
    public string? FormReg { get; set; }
    
    [MaxLength(50)]
    public string? FormReg2 { get; set; }
        
    
    
    // Campos específicos para PCT
    [MaxLength(100)]
    public string? NameTrans { get; set; }
    
    [MaxLength(100)]
    public string? ActiveSoft { get; set; }
    
    [MaxLength(20)]
    public string? TwaSize { get; set; }
    
    [MaxLength(500)]
    public string? Coment { get; set; }
    
    [MaxLength(50)]
    public string? Prev { get; set; }
    
    [MaxLength(100)]
    public string? DataAllocation { get; set; }
    
    // Campos específicos para PPT
    [MaxLength(100)]
    public string? NameSoft { get; set; }
    
    [MaxLength(100)]
    public string? LinkName { get; set; }
    
    [MaxLength(50)]
    public string? Language { get; set; }
    
    
    [MaxLength(50)]
    public string? AutoAlt { get; set; }
    
    [MaxLength(500)]
    public string? JustificativaAutoAlt { get; set; }
        
    // Campo para observações administrativas
    [MaxLength(1000)]
    public string? ObservacoesAdmin { get; set; }
}