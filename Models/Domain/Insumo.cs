using System.ComponentModel.DataAnnotations;

namespace Leaf.Models.Domain
{
    public class Insumo
    {
        public int IdInsumo { get; set; }

        [Required(ErrorMessage = "O código de barras é obrigatório")]
        [StringLength(15, ErrorMessage = "O código de barras deve ter no máximo 15 caracteres")]
        public string CodBarras { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória")]
        [StringLength(100, ErrorMessage = "A descrição deve ter no máximo 100 caracteres")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "A unidade de medida é obrigatória")]
        [StringLength(3, ErrorMessage = "A unidade de medida deve ter no máximo 3 caracteres")]
        public string UnidadeMedida { get; set; }

        [Required(ErrorMessage = "O status é obrigatório")]
        public int Status { get; set; }

        [Required(ErrorMessage = "O valor unitário é obrigatório")]
        public decimal ValorUnitario { get; set; }

        [Required(ErrorMessage = "A quantidade em estoque é obrigatória")]
        public int QtdeEstoque { get; set; }

        [Required(ErrorMessage = "O id da pessoa é obrigatório")]
        public int IdPessoa { get; set; }
        public Pessoa? Pessoa { get; set; }
    }
}
