using Leaf.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using Leaf.Models;

namespace Leaf.Repository
{
    public class ItemLoteProducaoRepository
	{
		private readonly DbConnectionManager _dbConnectionManager;

		public ItemLoteProducaoRepository(DbConnectionManager dbConnectionManager)
		{
			_dbConnectionManager = dbConnectionManager;
		}

		// Método para mapear um ItemLote a partir do SqlDataReader
		public ItemLote MapearItemLote(SqlDataReader reader)
		{
			return new ItemLote
			{
				IdItemLote = reader["iditemlote"] != DBNull.Value ? Convert.ToInt32(reader["iditemlote"]) : 0,
				IdInsumo = reader["id_insumo"] != DBNull.Value ? Convert.ToInt32(reader["id_insumo"]) : 0,
				Qtde = reader["qtde"] != DBNull.Value ? Convert.ToInt32(reader["qtde"]) : 0,
				IdLote = reader["id_lote"]?.ToString() ?? string.Empty,
				Insumo = null, // Pode ser carregado posteriormente se necessário
				LoteProducao = null    // Pode ser carregado posteriormente se necessário
			};
		}

		// Método para listar os itens de um lote específico
		public List<ItemLote> GetItensLote(string idLote)
		{
			string sql = @"SELECT * FROM ITEM_LOTE WHERE id_lote = @idLote";

			using (SqlConnection conn = _dbConnectionManager.GetConnection())
			{
				SqlCommand command = new SqlCommand(sql, conn);
				command.Parameters.AddWithValue("@idLote", idLote);

				try
				{
					SqlDataReader reader = command.ExecuteReader();
					List<ItemLote> itensLote = new List<ItemLote>();

					while (reader.Read())
					{
						itensLote.Add(MapearItemLote(reader));
					}

					return itensLote;
				}
				catch (SqlException ex)
				{
					throw new Exception("Não foi possível buscar os itens do lote. Erro: " + ex.Message);
				}
			}
		}
	}
}
