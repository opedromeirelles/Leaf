using Leaf.Data;
using Leaf.Models.Domain;
using Leaf.Repository.Compras;

namespace Leaf.Services.Compras
{
    public class CompraServices
    {
        private readonly DbConnectionManager _dbConnectionManager;

        public CompraServices(DbConnectionManager dbConnectionManager)
        {
            _dbConnectionManager = dbConnectionManager;
        }

        public List<Compra> GetCompras(string status, int idOc)
        {
            List<Compra> compras = new List<Compra>();

            try
            {
                CompraRepository _compraRepository = new CompraRepository(_dbConnectionManager);

                compras = _compraRepository.GetCompras(status, idOc);
                return compras.Any() ? compras : new List<Compra>();

            }
            catch (Exception ex)
            {

                throw new Exception("Erro ao listar compras, erro: " + ex.Message);
            }
        }

        public int NovaCompra(Compra compra)
        {
            try
            {
                CompraRepository _compraRepository = new CompraRepository(_dbConnectionManager);
                return _compraRepository.NovaCompra(compra);
            }
            catch (Exception ex)
            {
                throw new Exception("Não foi possivel efetuar nova compra, erro: " + ex.Message);
            }

        }

        public async Task<Compra> GetCompra(int idCompra)
        {
            if (idCompra != 0)
            {
                try
                {
                    CompraRepository compraRepository = new CompraRepository(_dbConnectionManager);
                    Compra compra = await compraRepository.GetCompra(idCompra);
                    return compra ?? new Compra();
                }
                catch (Exception ex)
                {

                    throw new Exception("Erro ao acessar compra, erro: " + ex.Message);
                }
            }
            return new Compra();
        }

        public async Task<List<Compra>> GetListComprasPeriodo(DateTime dataInicio, DateTime dataFim, int idFornecedor, string status, string numeroCompra, int idSolicitante)
        {
            CompraRepository compraRepository = new CompraRepository(_dbConnectionManager);

            List<Compra> compras = await compraRepository.GetListComprasPeriodoAsync(dataInicio, dataFim, idFornecedor, status, numeroCompra, idSolicitante);

            return compras ?? new List<Compra>();
        }
    }

}
