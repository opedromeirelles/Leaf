using Leaf.Data;
using Leaf.Models.Domain;
using Leaf.Repository.Pedidos;

namespace Leaf.Services.Pedidos
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

        public async Task<Pedido> GetPedido(int idPedido)
        {
            PedidoRepository _pedidoRepository = new PedidoRepository(_dbConnectionManager);
            try
            {
                //Busca o pedido
                Pedido pedido = await _pedidoRepository.GetPedidoAsync(idPedido);

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

        //Relatorios 

        //Entregas
        public async Task<List<Pedido>> GetPedidosFiltroAsync(DateTime dtInicio, DateTime dtFim, int idEntregador, int idPedido)
        {
            PedidoRepository _pedidoRepository = new PedidoRepository(_dbConnectionManager);
            try
            {
                List<Pedido> pedidos = await _pedidoRepository.GetListPedidosPeriodoAsync(dtInicio, dtFim, idEntregador, idPedido);
                return pedidos ?? new List<Pedido>();
            }
            catch (Exception ex)
            {

                throw new Exception("Erro ao consultar pedido, erro: " + ex.Message);
            }
        }

        //Vendas
        public async Task<List<Pedido>> GetPedidosFiltroAsync(DateTime dtInicio, DateTime dtFim, int idVendedor, int idPedido, string status)
        {
            PedidoRepository _pedidoRepository = new PedidoRepository(_dbConnectionManager);
            try
            {
                List<Pedido> pedidos = await _pedidoRepository.GetListPedidosPeriodoAsync(dtInicio, dtFim, idVendedor, idPedido, status);
                return pedidos ?? new List<Pedido>();
            }
            catch (Exception ex)
            {

                throw new Exception("Erro ao consultar pedido, erro: " + ex.Message);
            }
        }

    }
}
