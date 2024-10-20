using Leaf.Models.Domain;
using Leaf.Models.ItensDomain;
using System.Reflection.Metadata.Ecma335;

namespace Leaf.Models.ViewModels
{
	public class CompraViewModel
	{
        public int IdCompra { get; set; }
        public Compra Compra { get; set; }
        public Usuario Vendedor { get; set; }
        public Pessoa Fornecedor { get; set; }

        public List<ItemCompra> ItensCompra { get; set; }
    }
}
