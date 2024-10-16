using Microsoft.Data;
using Leaf.Data;
using System.Globalization;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Leaf.Models;

namespace Leaf.Repository
{
    public class CompraRepository : baseSqlComandos
	{
		private readonly DbConnectionManager _dbConnectionManager;

		public CompraRepository(DbConnectionManager dbConnectionManager)
		{
			_dbConnectionManager = dbConnectionManager;
		}

		public Compra MapearCompra(SqlDataReader reader)
		{
			return new Compra
			{
				IdOc = Convert.ToInt32(reader["idoc"]),
				Status = reader["status"].ToString(),
				IdPessoa = Convert.ToInt32(reader["id_pessoa"]),
				DtaEmissao = Convert.ToDateTime(reader["dta_emissao"], new CultureInfo("pt-BR")),
				DtaBaixa = Convert.ToDateTime(reader["dta_baixa"], new CultureInfo("pt-BR")),
				DtaCancelamento = Convert.ToDateTime(reader["dta_cancelamento"], new CultureInfo("pt-BR")),
				IdUsuario = Convert.ToInt32(reader["id_usuario"])
			};
		}

		public int NovaCompra(Compra compra)
		{
			string sql = @"INSERT INTO ORDEM_COMPRA
                       (status, id_pessoa, dta_emissao, dta_baixa, dta_cancelamento, id_usuario)
                       VALUES (@status, @id_pessoa, @dta_emissao, @dta_baixa, @dta_cancelamento, @id_usuario);
                       SELECT SCOPE_IDENTITY();";

			using (SqlConnection conn = _dbConnectionManager.GetConnection())
			{
				SqlCommand command = new SqlCommand(sql, conn);

				// Adicionar parâmetros
				command.Parameters.Add(new SqlParameter("@status", compra.Status));
				command.Parameters.Add(new SqlParameter("@id_pessoa", compra.IdPessoa));
				command.Parameters.Add(new SqlParameter("@dta_emissao", compra.DtaEmissao));
				command.Parameters.Add(new SqlParameter("@dta_baixa", compra.DtaBaixa.HasValue ? compra.DtaBaixa.Value : DBNull.Value));
				command.Parameters.Add(new SqlParameter("@dta_cancelamento", compra.DtaCancelamento.HasValue ? compra.DtaCancelamento.Value : DBNull.Value));
				command.Parameters.Add(new SqlParameter("@id_usuario", compra.IdUsuario));

				try
				{
					command.ExecuteNonQuery();
					return compra.IdOc = Convert.ToInt32(command.ExecuteScalar());

				}
				catch (SqlException ex)
				{
					throw new Exception($"Erro ao criar ordem de compra, erro: {ex.Message}");
				}
			}
		}


		// Relatorios de Compras
		public List<Compra> GetListComprasPeriodo(DateTime dataInicio, DateTime dataFim, int idFornecedor, string status)
		{
			// Base da query SQL
			string sql = @"SELECT * FROM ORDEM_COMPRA
                       WHERE 1=1
                       AND dta_emissao >= @dataInicio 
                       AND dta_emissao <= @dataFim";

			List<Compra> compras = new List<Compra>();

			using (SqlConnection conn = _dbConnectionManager.GetConnection())
			{
				SqlCommand command = new SqlCommand(sql, conn);

				// Obrigatoriamente haverá duas datas, por isso os parâmetros são adicionados diretamente
				command.Parameters.AddWithValue("@dataInicio", dataInicio);
				command.Parameters.AddWithValue("@dataFim", dataFim);

				// Filtrar pelo status, se fornecido
				if (!string.IsNullOrEmpty(status))
				{
					sql += " AND status = @status";
					command.Parameters.AddWithValue("@status", status);
				}

				// Filtrar por fornecedor, se fornecido
				if (idFornecedor != 0)
				{
					sql += " AND id_pessoa = @idFornecedor";
					command.Parameters.AddWithValue("@idFornecedor", idFornecedor);
				}

				try
				{
					// Atualizar o texto da consulta, já que ele é modificado dinamicamente
					command.CommandText = sql;

					SqlDataReader reader = command.ExecuteReader();

					while (reader.Read())
					{
						compras.Add(MapearCompra(reader));
					}

					// Se houver compras, retornar a lista; caso contrário, retornar uma lista vazia
					return compras.Any() ? compras : new List<Compra>();
				}
				catch (SqlException ex)
				{
					// Tratar erro e lançar exceção, se necessário
					throw new Exception("Erro ao tentar buscar compras, erro: " + ex.Message);
				}
			}
		}
	}
}
