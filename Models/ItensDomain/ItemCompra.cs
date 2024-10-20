using System.ComponentModel.DataAnnotations;
using Leaf.Models.Domain;

namespace Leaf.Models.ItensDomain
{
    public class ItemCompra
    {
        public int IdItemOc { get; set; }
        public int IdInsumo { get; set; }
        public Insumo insumo { get; set; }

		[Required(ErrorMessage = "A quantidade é obrigatório")]
        public int Quantidade { get; set; }
        public decimal SubTotal { get; set; }
        public int IdOc { get; set; }
        public List<Insumo> insumos { get; set; }
    }
}
