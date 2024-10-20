using System.ComponentModel.DataAnnotations;

namespace Leaf.Models.ViewModels
{
    public class LoginModelView
    {
        [Required(ErrorMessage = "Digite o seu login")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Digite a senha")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

    }
}
