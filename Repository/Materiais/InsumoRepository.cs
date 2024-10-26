using System.Collections.Generic;
using System.Data.SqlClient;
using Leaf.Data;
using Leaf.Models.Domain;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Leaf.Repository.Materiais
{
    public class InsumoRepository : baseSqlComandos
    {
        private readonly DbConnectionManager _dbConnectionManager;

        public InsumoRepository(DbConnectionManager dbConnectionManager)
        {
            _dbConnectionManager = dbConnectionManager;
        }

        // MÉTODO PARA MAPEAR INSUMO
        public Insumo MapearInsumo(SqlDataReader reader)
        {
            try
            {
                return new Insumo
                {
                    IdInsumo = reader["idinsumo"] != DBNull.Value ? Convert.ToInt32(reader["idinsumo"]) : 0,
                    CodBarras = reader["cod_barras"] != DBNull.Value ? reader["cod_barras"].ToString() : string.Empty,
                    Descricao = reader["descricao"] != DBNull.Value ? reader["descricao"].ToString() : string.Empty,
                    UnidadeMedida = reader["unidade_medida"] != DBNull.Value ? reader["unidade_medida"].ToString() : string.Empty,
                    Status = reader["status"] != DBNull.Value ? Convert.ToInt32(reader["status"]) : 0,
                    ValorUnitario = reader["valor_unitario"] != DBNull.Value ? Convert.ToDecimal(reader["valor_unitario"]) : 0.0m,
                    QtdeEstoque = reader["qtde_estoque"] != DBNull.Value ? Convert.ToInt32(reader["qtde_estoque"]) : 0,
                    IdPessoa = reader["id_pessoa"] != DBNull.Value ? Convert.ToInt32(reader["id_pessoa"]) : 0
                };

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
           

        }

        // MÉTODOS DE BUSCA
        public List<Insumo> GetInsumos()
        {
            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                string sql = @"SELECT * FROM insumo";
                List<Insumo> insumos = new List<Insumo>();

                try
                {
                    SqlCommand command = new SqlCommand(sql, conn);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        insumos.Add(MapearInsumo(reader));
                    }
                    return insumos;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public async Task<List<Insumo>> GetInsumosFornecedor(int idFornecedor)
        {
            List<Insumo> insumos = new List<Insumo>();

            string sql = @"select i.idinsumo, i.cod_barras, i.descricao, i.unidade_medida, i.valor_unitario, i.qtde_estoque, i.id_pessoa, i.status
                           from insumo i inner join pessoa p on i.id_pessoa = p.idpessoa
                           where 1=1";


            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                SqlCommand command = new SqlCommand(sql, conn);

                if (idFornecedor != 0)
                {
                    sql += " and p.idpessoa = @idFornecedor";
                    command.Parameters.AddWithValue("@idFornecedor", idFornecedor);
                }

                command.CommandText = sql;

                try
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        insumos.Add(MapearInsumo(reader));
                    }

                    return insumos.Any() ? insumos : new List<Insumo>();
                }
                catch (SqlException ex)
                {

                    throw new Exception("Erro ao listar insumos, erro: " + ex.Message);
                }                
            }
        }

        public List<Insumo> GetInsumosFiltro(string descricao, string cnpj, int status)
        {
            List<Insumo> insumos = new List<Insumo>();

            // Base da query SQL
            string sql = @"SELECT i.* 
                   FROM insumo i
                   JOIN pessoa p ON i.id_pessoa = p.idpessoa
                   WHERE 1 = 1";

            // Lista para armazenar os parâmetros da consulta
            List<SqlParameter> parametros = new List<SqlParameter>();

            // Condição para a descrição se fornecida
            if (!string.IsNullOrEmpty(descricao))
            {
                sql += " AND i.descricao LIKE @descricao";
                parametros.Add(new SqlParameter("@descricao", "%" + descricao + "%"));
            }

            // Condição para o CNPJ
            if (!string.IsNullOrEmpty(cnpj))
            {
                sql += " AND p.cnpj LIKE @cnpj";
                parametros.Add(new SqlParameter("@cnpj", cnpj));
            }

            // Condição para o status se fornecido
            if (status == 0 || status == 1)
            {
                sql += " AND i.status = @status";
                parametros.Add(new SqlParameter("@status", status));
            }
            else if (status == 3)
            {
                // Não filtra pelo status, retorna todos os insumos independentemente do status
            }

            // Executar a query
            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddRange(parametros.ToArray());

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    insumos.Add(MapearInsumo(reader));
                }
            }

            return insumos;
        }
        public List<Insumo> GetInsumosFiltroDescricao(string descricao)
        {
            List<Insumo> insumos = new List<Insumo>();
            string sql = @"select * from insumo where descricao like @descricao";

            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@descricao", "%" + descricao + "%");
                SqlDataReader reader = command.ExecuteReader();

                try
                {
                    while (reader.Read())
                    {
                        insumos.Add(MapearInsumo(reader));
                    }

                    return insumos;
                }
                catch (SqlException ex)
                {
                    throw new Exception("Não foi possivel listar os insumos por descrição, erro: " + ex.Message);
                }

            }

        }


        public List<Insumo> GetInsumosForPessoa(int idPessoa)
        {
            string sql = @"select * from insumo where id_pessoa = @idPessoa";
            List<Insumo> insumos = new List<Insumo>();

            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@idPessoa", idPessoa);
                SqlDataReader raeder = command.ExecuteReader();

                try
                {
                    while (raeder.Read())
                    {
                        insumos.Add(MapearInsumo(raeder));
                    }

                    return insumos;
                }
                catch (SqlException ex)
                {

                    throw new Exception("Erro ao listar insumos filtrado por pessoas, erro: " + ex.Message);
                }



            }
        }
        public List<Insumo> GetInsumosForPessoa(int idPessoa, string descicao)
        {
            string sql = @"select * from insumo where id_pessoa = @idPessoa and descricao like @descicao";

            List<Insumo> insumos = new List<Insumo>();

            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@idPessoa", idPessoa);
                command.Parameters.AddWithValue("@descicao", "%" + descicao + "%");

                SqlDataReader raeder = command.ExecuteReader();

                try
                {
                    while (raeder.Read())
                    {
                        insumos.Add(MapearInsumo(raeder));
                    }

                    return insumos;
                }
                catch (SqlException ex)
                {

                    throw new Exception("Erro ao listar insumos filtrado por pessoas, erro: " + ex.Message);
                }



            }
        }


        public Insumo GetInsumoById(int idInsumo)
        {
            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                string sql = @"SELECT * FROM insumo WHERE idinsumo = @idinsumo";

                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@idinsumo", idInsumo);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return MapearInsumo(reader);
                }
            }
            return null;
        }

        // MÉTODOS DE AÇÃO
        public void CadastrarInsumo(Insumo insumo)
        {
            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                try
                {
                    string sql = @"INSERT INTO insumo (cod_barras, descricao, unidade_medida, status, valor_unitario, qtde_estoque, id_pessoa)
                                   VALUES (@cod_barras, @descricao, @unidade_medida, @status, @valor_unitario, @qtde_estoque, @id_pessoa)";

                    SqlCommand command = new SqlCommand(sql, conn);
                    List<SqlParameter> parametros = new List<SqlParameter>
                    {
                        new SqlParameter("@cod_barras", insumo.CodBarras),
                        new SqlParameter("@descricao", insumo.Descricao),
                        new SqlParameter("@unidade_medida", insumo.UnidadeMedida),
                        new SqlParameter("@status", insumo.Status),
                        new SqlParameter("@valor_unitario", insumo.ValorUnitario),
                        new SqlParameter("@qtde_estoque", insumo.QtdeEstoque),
                        new SqlParameter("@id_pessoa", insumo.IdPessoa)
                    };

                    executaSql(sql, parametros, conn);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erro ao cadastrar insumo, erro: {ex.Message}");
                }
            }
        }

        public void AtualizarInsumo(Insumo insumo)
        {
            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                string sql = @"UPDATE insumo SET
                               cod_barras = @cod_barras,
                               descricao = @descricao,
                               unidade_medida = @unidade_medida,
                               status = @status,
                               valor_unitario = @valor_unitario,
                               qtde_estoque = @qtde_estoque,
                               id_pessoa = @id_pessoa
                               WHERE idinsumo = @idinsumo";

                SqlCommand command = new SqlCommand(sql, conn);
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    new SqlParameter("@idinsumo", insumo.IdInsumo),
                    new SqlParameter("@cod_barras", insumo.CodBarras),
                    new SqlParameter("@descricao", insumo.Descricao),
                    new SqlParameter("@unidade_medida", insumo.UnidadeMedida),
                    new SqlParameter("@status", insumo.Status),
                    new SqlParameter("@valor_unitario", insumo.ValorUnitario),
                    new SqlParameter("@qtde_estoque", insumo.QtdeEstoque),
                    new SqlParameter("@id_pessoa", insumo.IdPessoa)
                };

                try
                {
                    executaSql(sql, parametros, conn);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erro ao atualizar insumo, erro: {ex.Message}");
                }
            }
        }

        public void AtualizarStatusInsumo(int idInsumo, int status)
        {
            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                string sql = @"UPDATE insumo SET status = @status WHERE idinsumo = @idinsumo";

                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@idinsumo", idInsumo);
                command.Parameters.AddWithValue("@status", status);

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erro ao alterar o status do insumo, erro: {ex.Message}");
                }
            }
        }
    }
}
