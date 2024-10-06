    using Microsoft.Data;
    using Leaf.Data;
    using Leaf.Models;
    using System.Globalization;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;



    namespace Leaf.Repository
    {
        public class CompraRepository : baseSqlComandos
        {
            private readonly DbConnectionManager _dbConnectionManager;

            public CompraRepository( DbConnectionManager dbConnectionManager)
            {
                _dbConnectionManager = dbConnectionManager;
            }

            public Compra MapearCompra(SqlDataReader reader)
            {
                return new Compra
                {
                    IdOc = Convert.ToInt32(reader["idoc"]),
                    status = reader["status"].ToString(),
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
                    command.Parameters.Add(new SqlParameter("@status", compra.status));
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

        }
    }
