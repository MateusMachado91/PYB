namespace PYBWeb.Domain.Enums;

/// <summary>
/// Tipos de tabelas CICS disponíveis no sistema
/// </summary>
public enum TipoTabela
{
    /// <summary>
    /// Destination Control Table - Controle de destinos CICS
    /// </summary>
    DCT = 1,
    
    /// <summary>
    /// File Control Table - Controle de arquivos CICS
    /// </summary>
    FCT = 2,
    
    /// <summary>
    /// Program Control Table - Controle de programas CICS
    /// </summary>
    PCT = 3,
    
    /// <summary>
    /// Processing Program Table - Programas de processamento CICS
    /// </summary>
    PPT = 4
}