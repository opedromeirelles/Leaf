    using Leaf.Data;
using System.Data.SqlClient;
using System.Data;
using Leaf.Models.Domain;

namespace Leaf.Repository.Producao
{
    public class LoteProducaoRepository
    {
        private readonly DbConnectionManager _dbConnectionManager;

        public LoteProducaoRepository(DbConnectionManager dbConnectionManager)
        {
            _dbConnectionManager = dbConnectionManager;

        }

        //Mapear LOTE
        private LoteProducao MapearLoteProducao(SqlDataReader reader)
        {
            return new LoteProducao
            {
                IdLote = reader["idlote"].ToString(),
                Estagio = reader["estagio"] != DBNull.Value ? Convert.ToByte(reader["estagio"]) : (byte?)null,
                IdProduto = reader["id_produto"] != DBNull.Value ? Convert.ToInt32(reader["id_produto"]) : 0,
                Produto = null, // Aqui poderá ser popular se necessário depois
                Qtde = reader["qtde"] != DBNull.Value ? Convert.ToInt32(reader["qtde"]) : 0,
                DtaSemeadura = reader["dta_semeadura"] != DBNull.Value ? Convert.ToDateTime(reader["dta_semeadura"]) : null,
                DtaCrescimento = reader["dta_crescimento"] != DBNull.Value ? Convert.ToDateTime(reader["dta_crescimento"]) : null,
                DtaPlantio = reader["dta_plantio"] != DBNull.Value ? Convert.ToDateTime(reader["dta_plantio"]) : null,
                DtaColheita = reader["dta_colheita"] != DBNull.Value ? Convert.ToDateTime(reader["dta_colheita"]) : null,
                IdUsuario = reader["id_usuario"] != DBNull.Value ? Convert.ToInt32(reader["id_usuario"]) : 0,
                Usuario = null // Pode ser popular depois, caso seja necessário

            };
        }

		//Get Lote
		public async Task<LoteProducao> GetLoteProducao(string idLote)
		{
            LoteProducao loteProducao = new LoteProducao();

			// Base da query SQL
			string sql = @"SELECT * FROM LOTE_PRODUCAO WHERE idlote = @idLote";

			// Executa a query e faz o mapeamento dos lotes
			using (SqlConnection conn = _dbConnectionManager.GetConnection())
			{
				SqlCommand command = new SqlCommand(sql, conn);
				command.Parameters.AddWithValue("@idLote", idLote);

				try
				{
					SqlDataReader reader = await command.ExecuteReaderAsync();

					if (await reader.ReadAsync())
					{
						loteProducao = MapearLoteProducao(reader);
					}
				}
				catch (SqlException ex)
				{
					// Tratar erro e lançar exceção, se necessário
					throw new Exception("Erro ao tentar buscar lotes de produção, erro: " + ex.Message);
				}
			}

			return loteProducao ?? new LoteProducao();
		}

		//Relatório Lote
		public async Task<List<LoteProducao>> GetListLotesPeriodo(DateTime dataInicio, DateTime dataFim, int idProduto, string idLote, int estagio)
        {
            List<LoteProducao> lotes = new List<LoteProducao>();

            // Base da query SQL
            string sql = @"SELECT * FROM LOTE_PRODUCAO WHERE 1 = 1
                   AND dta_semeadura >= @dataInicio 
                   AND dta_semeadura <= @dataFim";

            // Lista de parâmetros para adicionar à consulta
            List<SqlParameter> parametros = new List<SqlParameter>();

            // Adiciona os parâmetros obrigatórios de datas
            parametros.Add(new SqlParameter("@dataInicio", dataInicio));
            parametros.Add(new SqlParameter("@dataFim", dataFim));

            // Condição para filtrar pelo idProduto se ele for fornecido
            if (idProduto != 0)
            {
                sql += " AND id_produto = @idProduto";
                parametros.Add(new SqlParameter("@idProduto", idProduto));
            }
			if (!string.IsNullOrEmpty(idLote))
			{
				sql += " AND idlote = @idLote";
				parametros.Add(new SqlParameter("@idLote", idLote));
			}
			if (estagio != 0)
			{
				sql += " AND estagio = @estagio";
				parametros.Add(new SqlParameter("@estagio", estagio));
            }

			// Executa a query e faz o mapeamento dos lotes
			using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddRange(parametros.ToArray());

                try
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        lotes.Add(MapearLoteProducao(reader));
                    }
                }
                catch (SqlException ex)
                {
                    // Tratar erro e lançar exceção, se necessário
                    throw new Exception("Erro ao tentar buscar lotes de produção, erro: " + ex.Message);
                }
            }

            return lotes.Any() ? lotes : new List<LoteProducao>();
        }



    }
}
