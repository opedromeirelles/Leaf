using Leaf.Repository;
using Leaf.Data;
using Leaf.Models.ItensDomain;

namespace Leaf.Services
{
    public class ItemPedidoServices
    {
        private readonly DbConnectionManager _dbConnectionManager;

        public ItemPedidoServices(DbConnectionManager dbConnectionManager)
        {
            _dbConnectionManager = dbConnectionManager;
        }

        public List<ItemPedido> GetItensPedido(int idPedido)
        {
            ItemPedidoRepository _itemPedidoRepository = new ItemPedidoRepository(_dbConnectionManager);
            if (idPedido == 0)
            {
                return null; //pedido inválido.
            }
            try
            {
               List<ItemPedido> itemPedido = _itemPedidoRepository.GetItensPedido(idPedido);
                if (itemPedido != null)
                {
                    return itemPedido;
                }
                return null;
            }
            catch (Exception ex)
            {

                throw new Exception("Erro ao buscar item do pedido, erro: " + ex.Message);
            }

        }
    }
}
