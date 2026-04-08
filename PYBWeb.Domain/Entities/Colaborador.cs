using System.ComponentModel.DataAnnotations;


public class Colaborador
{
    [Key]
    public string Matricula { get; set; } = string.Empty; // PK
    public string Nome { get; set; } = string.Empty;
    public string? Setor { get; set; }
    public string? Email { get; set; }
    public string Role { get; set; } = "admin";
}