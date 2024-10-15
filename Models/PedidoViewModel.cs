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
            this.Pedido = pedido;
            this.IdPedido = pedido.IdPedido;
            this.ItensPedido = itensPedido;
        }

        public PedidoViewModel()
        {
            
        }


    }
}
