using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace smartmoney.Models
{
    [Table("Carteiras")]
    public class Carteira
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o título.")]
        [Display(Name = "Título")]
        public string Titulo { get; set; }

        public decimal? Saldo { get; set; }

        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set;}

        public ICollection<Transacao>? Transacoes { get; set; }

    }
}
