namespace PYBWeb.Domain.Entities;

/// <summary>
/// Entidade para ambientes CICS do banco ambiente.db
/// </summary>
public class AmbienteCics
{
    public int Id { get; set; }
    public string IdChave { get; set; } = "";
    public string Nome { get; set; } = "";
    public string Descricao { get; set; } = "";
    public string Ambiente { get; set; } = "";
    public string Maquina { get; set; } = "";
    public string Sufixo { get; set; } = "";
    public string Isc { get; set; } = "";
    public string SteplibCsd { get; set; } = "";
    public string DsnameDfhcsd { get; set; } = "";
    public string Servidor { get; set; } = "";
    public int Pxu { get; set; }
    public bool Ativo { get; set; } = true;
    
}

