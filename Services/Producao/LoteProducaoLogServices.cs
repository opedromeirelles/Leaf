using Leaf.Data;
using Leaf.Models.DomainLog;
using Leaf.Repository;
using Leaf.Repository.Producao;

namespace Leaf.Services.Producao
{
    public class LoteProducaoLogServices
    {
        private readonly DbConnectionManager _dbConnectionManager;

        public LoteProducaoLogServices(DbConnectionManager dbConnectionManager)
        {
            _dbConnectionManager = dbConnectionManager;
        }

        public List<LoteProducaoLog> GetLoteProducaoLog(string lote)
        {
            List<LoteProducaoLog> logsProducao = new List<LoteProducaoLog>();

            if (string.IsNullOrEmpty(lote))
            {
                return logsProducao;

			}
            else
            {
                try
                {
					LoteProducaoLogRepository _loteProducaoLogRepository = new LoteProducaoLogRepository(_dbConnectionManager);
                    logsProducao = _loteProducaoLogRepository.GetLoteProducaoLog(lote);
                    return logsProducao;
				}
                catch (Exception ex)
                {

                    throw new Exception("Erro ao consultar log do lote informado. " + ex.Message);
                }
			}
        }

    }
}
