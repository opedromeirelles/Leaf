using Leaf.Models.Domain;
using Leaf.Models.ItensDomain;

namespace Leaf.Models.ViewModels
{
    public class PedidoViewModel
    {
        public int IdPedido { get; set; }

        public Pedido Pedido { get; set; }
        public List<ItemPedido> ItensPedido { get; set; }
        public Pessoa Cliente { get; set; }
        public Usuario Enregador { get; set; }
        public Usuario Vendedor { get; set; }

        public double? TempoMedioEntrega { get; set; }
        public string? TempoDescricao { get; set; }


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
