using System.ComponentModel.DataAnnotations;

namespace Leaf.Models.Domain
{
    public class Produto
    {
        public int IdProduto { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatório")]
        public string Descricao { get; set; }

        
        [Required(ErrorMessage = "A valor unitário é obrigatório")]
        public decimal ValorUnitario { get; set; }

        [Required(ErrorMessage = "A quantidade é obrigatório")]
        public int QtdeEstoque { get; set; }

        public int Status { get; set; }
    }
}
