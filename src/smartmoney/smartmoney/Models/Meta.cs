using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace smartmoney.Models
{
    [Table("Metas")]
    public class Meta
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o título.")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "Obrigatório informar a data inicial.")]
        [Display(Name = "Data inicial")]
        public DateTime DataInicial { get; set; }

        [Required(ErrorMessage = "Obrigatório informar a data final.")]
        [Display(Name = "Data Final")]
        public DateTime DataFinal { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o valor da Meta.")]
        [Display(Name = "Valor da Meta")]
        public decimal ValorMeta { get; set; }

        public decimal? Valor { get; set; }

        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }
    }
}
