using System.ComponentModel.DataAnnotations;

namespace Leaf.Models
{
    public class LoteProducao
    {
        [Required(ErrorMessage = "O código do lote é obrigatório")]
        public string IdLote { get; set; }

        public int? Estagio { get; set; }

        [Required(ErrorMessage = "O código do produto é obrigatório")]
        public int IdProduto { get; set; }
        public Produto? Produto { get; set; }
        public int? Qtde { get; set; }
        public DateTime? DtaSemeadura { get; set; }
        public DateTime? DtaCrescimento { get; set; }
        public DateTime? DtaPlantio { get; set; }
        public DateTime? DtaColheita { get; set; }

        [Required(ErrorMessage = "O código do Usuário responsável é obrigatório")]
        public int IdUsuario { get; set; }
        public Usuario? Usuario { get; set; }


    }
}
