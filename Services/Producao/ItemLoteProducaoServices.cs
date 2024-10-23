using Leaf.Data;
using Leaf.Models;
using Leaf.Models.ItensDomain;
using Leaf.Repository;
using Leaf.Repository.Producao;

namespace Leaf.Services.Producao
{
    public class ItemLoteProducaoServices
    {
        private readonly DbConnectionManager _dbConnectionManager;

        public ItemLoteProducaoServices(DbConnectionManager dbConnectionManager)
        {
            _dbConnectionManager = dbConnectionManager;
        }

        public List<ItemLoteProducao> GetItensLote(string idLote)
        {
            try
            {
                if (!string.IsNullOrEmpty(idLote))
                {
                    ItemLoteProducaoRepository _itemLoteProducaoRepository = new ItemLoteProducaoRepository(_dbConnectionManager);
                    List<ItemLoteProducao> itensLote = _itemLoteProducaoRepository.GetItensLote(idLote);
                    return itensLote ?? new List<ItemLoteProducao>();
                }
                return new List<ItemLoteProducao>();
            }
            catch (Exception ex)
            {

                throw new Exception("Erro ao consultar itens do lote: " + idLote + "Erro: " + ex.Message);
            }
        }
    }
}
