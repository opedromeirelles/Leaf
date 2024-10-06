using Leaf.Data;
using Leaf.Models;
using Leaf.Repository;

namespace Leaf.Services
{
    public class CompraServices
    {
        private readonly DbConnectionManager _dbConnectionManager;

        public CompraServices(DbConnectionManager dbConnectionManager)
        {
            _dbConnectionManager = dbConnectionManager;
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
    }

}
