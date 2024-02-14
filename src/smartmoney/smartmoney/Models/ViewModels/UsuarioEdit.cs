using System.ComponentModel.DataAnnotations;

namespace smartmoney.Models.ViewModels
{
    public class UsuarioEditInfo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o nome.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o email.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }

    public class UsuarioEditPassword
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Obrigatório informar a senha.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [Required(ErrorMessage = "Digite a senha novamente.")]
        [DataType(DataType.Password)]
        public string ConfirmarSenha { get; set; }
    }

    public class UsuarioEsqueciSenha
    {
        [Required(ErrorMessage = "Obrigatório informar o email.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }

    public class UsuarioRedefinirSenha
    {
        public int Id { get; set; }

        [Required]
        public string Token { get; set; }

        [Required(ErrorMessage = "Obrigatório informar a senha.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [Required(ErrorMessage = "Digite a senha novamente.")]
        [DataType(DataType.Password)]
        public string ConfirmarSenha { get; set; }
    }
}
