namespace Leaf.Models
{
    public class PedidoViewModel
    {
        public int IdPedido { get; set; }
        public int IdItemPedido { get; set; }
        public Pedido Pedido { get; set; }
        public ItemPedido ItemPedido { get; set; }
        public List<ItemPedido> ItensPedido { get; set; }


        public PedidoViewModel(Pedido pedido, List<ItemPedido> itensPedido)
        {
            Pedido = pedido;
            IdPedido = pedido.IdPedido;
            ItensPedido = itensPedido;
        }

        public PedidoViewModel()
        {

        }


    }
}
