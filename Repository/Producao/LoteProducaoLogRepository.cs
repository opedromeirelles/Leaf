using Leaf.Data;
using System.Data.SqlClient;
using System.Data;
using Leaf.Models.DomainLog;

namespace Leaf.Repository.Producao
{
    public class LoteProducaoLogRepository
    {
        private readonly DbConnectionManager _dbConnectionManager;

        public LoteProducaoLogRepository(DbConnectionManager dbConnectionManager)
        {
            _dbConnectionManager = dbConnectionManager;

        }

        private LoteProducaoLog MapearLote(SqlDataReader reader)
        {
            return new LoteProducaoLog
            {
                IdLog = Convert.ToInt32(reader["idlog"]),
                Lote = reader["lote"] != DBNull.Value ? reader["lote"].ToString() : string.Empty,
                QtdeAntiga = reader["qtde_antiga"] != DBNull.Value ? Convert.ToInt32(reader["qtde_antiga"]) : 0,
                QtdeNova = reader["qtde_nova"] != DBNull.Value ? Convert.ToInt32(reader["qtde_nova"]) : 0,
                IdUsuario = reader["usuario"] != DBNull.Value ? Convert.ToInt32(reader["usuario"]) : 0,
				DtaAlteracao = reader["dta_alteracao"] != DBNull.Value ? Convert.ToDateTime(reader["dta_alteracao"]) : null,
			};
        }

        public List<LoteProducaoLog> GetLoteProducaoLog(string lote)
        {
            string sql = @"Select * from LOTE_PRODUCAO_LOG where lote = @lote";
            List<LoteProducaoLog> logsProducao = new List<LoteProducaoLog>();

			try
            {
				using (SqlConnection conn = _dbConnectionManager.GetConnection())
				{
					SqlCommand command = new SqlCommand(sql, conn);
                    command.Parameters.AddWithValue("@lote", lote);

					SqlDataReader reader = command.ExecuteReader();

					while (reader.Read())
					{
                        logsProducao.Add(MapearLote(reader));
					}
				}

                return logsProducao;

			}
            catch (Exception ex)
            {

                throw new Exception("Erro ao consultar log da Produção. " + ex.Message);
            }
          

		}

        
    }
}
