using System.ComponentModel.DataAnnotations;

namespace Leaf.Models
{
    public class ItemCompra
    {
        public int IdItemOc { get; set; }
        public int IdInsumo { get; set; }

        [Required(ErrorMessage = "A quantidade é obrigatório")]
        public int Quantidade { get; set; }
        public Decimal SubTotal { get; set; }
        public int IdOc { get; set; }
        public List<Insumo> itemCompras { get; set; }
    }
}
