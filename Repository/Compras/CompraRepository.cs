using Microsoft.Data;
using Leaf.Data;
using System.Globalization;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Leaf.Models.Domain;

namespace Leaf.Repository.Compras
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
                IdOc = reader["idoc"] != DBNull.Value ? Convert.ToInt32(reader["idoc"]) : 0,
                Status = reader["status"] != DBNull.Value ? reader["status"].ToString() : string.Empty,
                IdPessoa = reader["id_pessoa"] != DBNull.Value ? Convert.ToInt32(reader["id_pessoa"]) : 0,
                DtaEmissao = reader["dta_emissao"] != DBNull.Value ? Convert.ToDateTime(reader["dta_emissao"], new CultureInfo("pt-BR")) : DateTime.MinValue,
                DtaBaixa = reader["dta_baixa"] != DBNull.Value ? Convert.ToDateTime(reader["dta_baixa"], new CultureInfo("pt-BR")) : null,
                DtaCancelamento = reader["dta_cancelamento"] != DBNull.Value ? Convert.ToDateTime(reader["dta_cancelamento"], new CultureInfo("pt-BR")) : null,
                IdUsuario = reader["id_usuario"] != DBNull.Value ? Convert.ToInt32(reader["id_usuario"]) : 0
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
                command.Parameters.Add(new SqlParameter("@status", "EM"));
                command.Parameters.Add(new SqlParameter("@id_pessoa", compra.IdPessoa));
                command.Parameters.Add(new SqlParameter("@dta_emissao", DateTime.Now));
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



        public async Task<Compra> GetCompra(int idCompra)
        {
            // Base da query SQL
            string sql = @"SELECT * FROM ORDEM_COMPRA
                       WHERE idoc = @idoc";

            Compra compra = new Compra();

            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                SqlCommand command = new SqlCommand(sql, conn);

                command.Parameters.AddWithValue("@idoc", idCompra);

                try
                {

                    // ExecuteReader de forma assíncrona
                    await using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            compra = MapearCompra(reader);
                        }
                    }

                    return compra ?? new Compra();
                }
                catch (SqlException ex)
                {
                    // Tratar erro e lançar exceção, se necessário
                    throw new Exception("Erro ao tentar buscar compras, erro: " + ex.Message);
                }
            }
        }



        // Relatorios de Compras
        public async Task<List<Compra>> GetListComprasPeriodoAsync(DateTime dataInicio, DateTime dataFim, int idFornecedor, string status, string numeroCompra, int idSolicitante)
        {
            string sql = @"SELECT * FROM ORDEM_COMPRA
                   WHERE 1=1
                   AND dta_emissao >= @dataInicio 
                   AND dta_emissao <= @dataFim";

            // Montar a query dinamicamente antes de criar o SqlCommand
            if (!string.IsNullOrEmpty(status))
            {
                sql += " AND status = @status";
            }

            if (idFornecedor != 0)
            {
                sql += " AND id_pessoa = @idFornecedor";
            }

            if (!string.IsNullOrEmpty(numeroCompra))
            {
                sql += " AND idoc = @numeroCompra";
            }

            if (idSolicitante != 0)
            {
                sql += " AND id_usuario = @idSolicitante";
            }

            var compras = new List<Compra>();

            // Use await using para garantir o descarte correto do recurso assíncrono
            await using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                SqlCommand command = new SqlCommand(sql, conn);

                // Adiciona os parâmetros
                command.Parameters.AddWithValue("@dataInicio", dataInicio);
                command.Parameters.AddWithValue("@dataFim", dataFim);

                if (!string.IsNullOrEmpty(status))
                {
                    command.Parameters.AddWithValue("@status", status);
                }

                if (idFornecedor != 0)
                {
                    command.Parameters.AddWithValue("@idFornecedor", idFornecedor);
                }

                if (!string.IsNullOrEmpty(numeroCompra))
                {
                    command.Parameters.AddWithValue("@numeroCompra", numeroCompra);
                }

                if (idSolicitante != 0)
                {
                    command.Parameters.AddWithValue("@idSolicitante", idSolicitante);
                }

                try
                {
                    // ExecuteReader de forma assíncrona
                    await using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            compras.Add(MapearCompra(reader));
                        }
                    }

                    return compras;
                }
                catch (SqlException ex)
                {
                    throw new Exception("Erro ao tentar buscar compras, erro: " + ex.Message);
                }
            }
        }



    }
}
