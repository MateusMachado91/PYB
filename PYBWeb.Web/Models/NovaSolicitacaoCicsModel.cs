using System.ComponentModel.DataAnnotations;
using PYBWeb.Domain.Enums;

namespace PYBWeb.Web.Models;

/// <summary>
/// Modelo para criação de nova solicitação CICS 2025
/// 🗄️ Especificamente para SQLite na pasta DATA
/// </summary>
public class NovaSolicitacaoCicsModel
{
    // =====================================================================
    // 📋 CAMPOS BÁSICOS OBRIGATÓRIOS
    // =====================================================================
    
    [Required(ErrorMessage = "A sigla do sistema é obrigatória")]
    [StringLength(3, ErrorMessage = "A sigla deve ter exatamente 3 caracteres")]
    public string Css { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "O ambiente é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "Selecione um ambiente válido")]
    public int AmbienteId { get; set; }
    
    [Required(ErrorMessage = "O usuário solicitante é obrigatório")]
    [StringLength(20, ErrorMessage = "O usuário deve ter no máximo 20 caracteres")]
    public string Usuario { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "O tipo de tabela é obrigatório")]
    public string TipoTabela { get; set; } = string.Empty;
    
    // Campo calculado automaticamente
    public string Appli { get; set; } = string.Empty;

    public string Operacao { get; set; } = string.Empty;
    // =====================================================================
    // 🔧 CAMPOS ESPECÍFICOS FCT (File Control Table)
    // =====================================================================
    
    [StringLength(100)]
    public string? NameArq { get; set; }
    
    [StringLength(50)]
    public string? Type { get; set; }
    
    [StringLength(200)]
    public string? DsnameArq { get; set; }
    
    [StringLength(50)]
    public string? EstInit { get; set; }
    
    public List<string> Services { get; set; } = new();
    
    [StringLength(20)]
    public string? NumStrng { get; set; }
    
    // =====================================================================
    // 🌐 CAMPOS ESPECÍFICOS FCT REMOTE (File Control Table Remote)
    // =====================================================================
    
    [Display(Name = "Ambiente Remoto")]
    public int? AmbienteRemotoId { get; set; }
    
    [StringLength(100)]
    [Display(Name = "Nome do Ambiente Remoto")]
    public string? AmbienteRemotoNome { get; set; }
    
    // =====================================================================
    // 🏦 CAMPOS ESPECÍFICOS FCT PADRÃO BNO
    // =====================================================================
    
    [StringLength(4)]
    [Display(Name = "Código da Agência")]
    public string? CodigoAgencia { get; set; }
    
    [Display(Name = "Arquivos Padrão BNO")]
    public List<string> ArquivosPadraoBno { get; set; } = new();
    
    // =====================================================================
    // 📊 CAMPOS ESPECÍFICOS DCT (Destination Control Table)
    // =====================================================================
    
    [StringLength(100)]
    public string? FileName { get; set; }
    
    [StringLength(50)]
    public string? QueueType { get; set; }
    
    [StringLength(50)]
    public string? Ddname { get; set; }
    
    [StringLength(200)]
    public string? Dsname { get; set; }
    
    [StringLength(50)]
    public string? EstFile { get; set; }
    
    [StringLength(50)]
    public string? FormReg { get; set; }
    
    [StringLength(50)]
    public string? FormReg2 { get; set; }
    
    [StringLength(50)]
    public string? FormReg3 { get; set; }
    
    [StringLength(50)]
    public string? FileType { get; set; }
    
    [StringLength(20)]
    public string? RegSize { get; set; }
    
    [StringLength(20)]
    public string? BlockSize { get; set; }
    
    // Campos DCT INTRA específicos
    [StringLength(50)]
    public string? Transacao { get; set; }
    
    public int? TrigLevel { get; set; }
    
    [StringLength(50)]
    public string? Destino { get; set; }
    
    [StringLength(50)]
    public string? Terminal { get; set; }
    
    // =====================================================================
    // 💻 CAMPOS ESPECÍFICOS PCT (Program Control Table)
    // =====================================================================
    
    [StringLength(100)]
    public string? NameTrans { get; set; }
    
    [StringLength(100)]
    public string? ActiveSoft { get; set; }
    
    [StringLength(20)]
    public string? TwaSize { get; set; } = "0";
    
    [StringLength(500)]
    public string? Coment { get; set; }
    
    [StringLength(50)]
    public string? Prev { get; set; }
    
    [StringLength(100)]
    public string? DataAllocation { get; set; } = "ANY";
    
    [StringLength(500)]
    public string? JustificativaBelow { get; set; }
    
    // =====================================================================
    // 🔗 CAMPOS ESPECÍFICOS PPT (Processing Program Table)
    // =====================================================================
    
    [StringLength(100)]
    public string? NameSoft { get; set; }
    
    [StringLength(100)]
    public string? LinkName { get; set; }
    
    [StringLength(50)]
    public string? Language { get; set; }
    
    
    [StringLength(50)]
    public string? AutoAlt { get; set; }
    
    [StringLength(500)]
    public string? JustificativaAutoAlt { get; set; }
        
    // =====================================================================
    // 📝 CAMPOS ADMINISTRATIVOS
    // =====================================================================
    
    [StringLength(1000)]
    public string? ObservacoesAdmin { get; set; }

}