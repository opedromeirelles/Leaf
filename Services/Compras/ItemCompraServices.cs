using Leaf.Data;
using Leaf.Models;
using Leaf.Models.ItensDomain;
using Leaf.Repository.Compras;

namespace Leaf.Services.Compras
{
    public class ItemCompraServices
    {
        private readonly DbConnectionManager _dbConnectionManager;

        public ItemCompraServices(DbConnectionManager dbConnectionManager)
        {
            _dbConnectionManager = dbConnectionManager;
        }

        public List<ItemCompra> GetItemCompras(int idCompra)
        {
            ItemCompraRepository _itemCompraRepository = new ItemCompraRepository(_dbConnectionManager);

            if (idCompra != 0)
            {
                List<ItemCompra> itemCompras = _itemCompraRepository.GetItemCompra(idCompra);
                return itemCompras ?? new List<ItemCompra>();
            }

            return new List<ItemCompra>();
        }
    }
}
