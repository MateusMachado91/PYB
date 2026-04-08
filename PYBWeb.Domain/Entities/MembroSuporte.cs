using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PYBWeb.Domain.Entities
{
    [Table("Suporte")]
    public class MembroSuporte
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; } = "";

        [Required]
        [MaxLength(150)]
        public string Email { get; set; } = "";

        [Required]
        [MaxLength(10)]
        public string Ramal { get; set; } = "";

        [Required]
        [MaxLength(50)]
        public string Categoria { get; set; } = "Equipe CICS"; // Coordenador, Lider Técnico CICS, Equipe CICS, Desenvolvimento

        public DateTime DataCriacao { get; set; } = DateTime.Now;
        
        public DateTime?  DataAtualizacao { get; set; }
    }
}