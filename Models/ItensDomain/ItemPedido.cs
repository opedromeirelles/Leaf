using Leaf.Models.Domain;

namespace Leaf.Models.ItensDomain
{
    public class ItemPedido
    {
        public int IdItemPedido { get; set; }
        public int IdProduto { get; set; }
        public int Quantidade { get; set; }
        public decimal SubTotal { get; set; }
        public int IdPedido { get; set; }

        public Produto Produto { get; set; }
        public List<Produto> Produtos { get; set; }
    }
}
