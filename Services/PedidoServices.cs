using Leaf.Repository;
using Leaf.Models;
using Leaf.Data;

namespace Leaf.Services
{
    public class PedidoServices
    {
        private readonly DbConnectionManager _dbConnectionManager;

        public PedidoServices(DbConnectionManager dbConnectionManager)
        {
            _dbConnectionManager = dbConnectionManager;
        }

        public List<Pedido> GetPedidos()
        {
            PedidoRepository _pedidoRepository = new PedidoRepository(_dbConnectionManager);
            try
            {
                List<Pedido> pedidos = _pedidoRepository.GetPedidos();
                if (pedidos != null && pedidos.Any())
                {
                    return pedidos;
                }
                return null;
            }
            catch (Exception ex)
            {

                throw new Exception("Erro ao consultar pedido, erro: " + ex.Message);
            }
        }

        public Pedido GetPedido(int idPedido)
        {
            PedidoRepository _pedidoRepository = new PedidoRepository(_dbConnectionManager);
            try
            {
                //Busca o pedido
                Pedido pedido = _pedidoRepository.GetPedido(idPedido);

                //Validações sobre o pedido
                if (pedido != null)
                {
                    return pedido;
                }

                return null;

            }
            catch (Exception ex)
            {

                throw new Exception("Erro ao consultar pedido, erro: " + ex.Message);
            }
        }

        public bool AtualizaStatusPedido(int idPedido, int idEntregador)
        {

            PedidoRepository _pedidoRepository = new PedidoRepository(_dbConnectionManager);
            if (idPedido == 0 && idEntregador == 0)
            {
                return false;
            }
            else if (_pedidoRepository.AlterarStatusPedido(idPedido, idEntregador))
            {
                return true;
            }

            return false;
        }

        public List<Pedido> GetPedidosFiltro(int idPedido, string status)
        {
            PedidoRepository _pedidoRepository = new PedidoRepository(_dbConnectionManager);
            try
            {
                //Busca o pedido
                List<Pedido> pedidos = _pedidoRepository.BuscarPedido(idPedido, status);

                //Validações sobre o pedido
                if (pedidos != null && pedidos.Any())
                {
                    return pedidos;
                }

                return null;

            }
            catch (Exception ex)
            {

                throw new Exception("Erro ao consultar pedido, erro: " + ex.Message);
            }

        }

    }
}
