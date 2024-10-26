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

        public bool NovaCompraItens(int idOc, ItemCompra itemCompra)
        {
            try
            {
                if (idOc == 0 && itemCompra == null)
                {
                    return false;
                }

                ItemCompraRepository _itemCompraRepository = new ItemCompraRepository(_dbConnectionManager);
                return _itemCompraRepository.NovoItemCompra(itemCompra);

            }
            catch (Exception ex)
            {

                throw new Exception($"Erro ao inserir novos itens a compra: {idOc}, erro: {ex.Message}");
            }
            
        }
    }
}
