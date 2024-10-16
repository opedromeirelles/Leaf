using System.ComponentModel.DataAnnotations;

namespace Leaf.Models
{
    public class ItemLote
    {

        public int IdItemLote { get; set; }

        [Required(ErrorMessage = "O código do insumo é obrigatório")]
        public int IdInsumo { get; set; }
        public Insumo? Insumo { get; set; }
        public int? Qtde { get; set; }

        [Required(ErrorMessage = "O código do lote é obrigatório")]
        public string IdLote { get; set; }
        public LoteProducao? LoteProducao { get; set; }

    }
}
