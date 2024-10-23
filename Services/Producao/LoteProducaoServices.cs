using Leaf.Data;
using Leaf.Models;
using Leaf.Models.Domain;
using Leaf.Repository;
using Leaf.Repository.Producao;

namespace Leaf.Services.Producao
{
    public class LoteProducaoServices
    {
        private readonly DbConnectionManager _dbConnectionManager;

        public LoteProducaoServices(DbConnectionManager dbConnectionManager)
        {
            _dbConnectionManager = dbConnectionManager;
        }

        //GET LOTE
        public async Task<LoteProducao> GetLoteProducao(string idLote)
        {
            LoteProducaoRepository _loteProducaoRepository = new LoteProducaoRepository(_dbConnectionManager);
            LoteProducao loteProducao = new LoteProducao();

            try
            {
				if (!string.IsNullOrEmpty(idLote))
				{
					loteProducao = await _loteProducaoRepository.GetLoteProducao(idLote);

				}

            return loteProducao ?? new LoteProducao();

			}
            catch (Exception ex)
            {

                throw new Exception("Não foi possivel consultar o lote de produção, erro: " + ex.Message);
            }
			

        }

        public async Task<List<LoteProducao>> GetLoteProducaoPeriodo(DateTime dataInicio, DateTime dataFim, int idProduto, string idLote, int estagio)
        {
			LoteProducaoRepository _loteProducaoRepository = new LoteProducaoRepository(_dbConnectionManager);
			List<LoteProducao> lotesProducao = new List<LoteProducao>();

			try
            {
                lotesProducao = await _loteProducaoRepository.GetListLotesPeriodo(dataInicio, dataFim, idProduto, idLote, estagio);

                return lotesProducao.Any() ? lotesProducao : new List<LoteProducao>();
            }
            catch (Exception ex)
            {

                throw new Exception("Erro ao consultar lote com filtros, erro: " + ex.Message);
            }

        }

    }
}
