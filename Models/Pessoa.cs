using System.ComponentModel.DataAnnotations;

namespace Leaf.Models
{
    public class Pessoa
    {

        public int IdPessoa { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(50, ErrorMessage = "O nome deve ter no máximo 50 caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O tipo é obrigatório")]
        [StringLength(2, ErrorMessage = "O tipo deve ter no máximo 2 caracteres")]
        public string Tipo { get; set; }

        [StringLength(14, ErrorMessage = "O CPF deve ter no máximo 14 caracteres")]
        public string Cpf { get; set; }

        [StringLength(18, ErrorMessage = "O CNPJ deve ter no máximo 18 caracteres")]
        public string Cnpj { get; set; }

        [StringLength(20, ErrorMessage = "O telefone 1 deve ter no máximo 20 caracteres")]
        public string Telefone1 { get; set; }

        [StringLength(20, ErrorMessage = "O telefone 2 deve ter no máximo 20 caracteres")]
        public string Telefone2 { get; set; }

        [StringLength(50, ErrorMessage = "O email 1 deve ter no máximo 50 caracteres")]
        public string Email1 { get; set; }

        [StringLength(50, ErrorMessage = "O email 2 deve ter no máximo 50 caracteres")]
        public string Email2 { get; set; }
    }
}
