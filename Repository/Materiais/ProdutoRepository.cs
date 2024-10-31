using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Leaf.Data;
using Leaf.Models.Domain;
using Leaf.Services;

namespace Leaf.Repository.Materiais
{
    public class ProdutoRepository : baseSqlComandos
    {
        private readonly DbConnectionManager _dbConnectionManager;

        public ProdutoRepository(DbConnectionManager dbConnectionManager)
        {
            _dbConnectionManager = dbConnectionManager;
        }

        //MÉTODO PARA MAPEAR PRODUTO
        public Produto MapearProduto(SqlDataReader reader)
        {
            return new Produto
            {
                IdProduto = Convert.ToInt32(reader["idproduto"]),
                Descricao = reader["descricao"].ToString(),
                ValorUnitario = Convert.ToDecimal(reader["valor_unitario"]),
                QtdeEstoque = Convert.ToInt32(reader["qtde_estoque"]),
                Status = Convert.ToInt32(reader["status"])
            };
        }

        //MÉTODOS DE BUSCA
        public List<Produto> GetProdutos()
        {
            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                string sql = @"SELECT * FROM produto";
                List<Produto> produtos = new List<Produto>();

                try
                {
                    SqlCommand command = new SqlCommand(sql, conn);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        produtos.Add(MapearProduto(reader));
                    }
                    return produtos;

                }
                catch (Exception)
                {
                    return null;
                }


            }
        }

        public List<Produto> GetProdutosFiltro(string descricao, int status)
        {
            List<Produto> produtos = new List<Produto>();

            // Base da query SQL
            string sql = @"SELECT idproduto, descricao, valor_unitario, qtde_estoque, status 
                           FROM produto WHERE 1 = 1";

            // Parâmetros para a consulta
            List<SqlParameter> parametros = new List<SqlParameter>();

            // Condição para descrição se foi fornecida
            if (!string.IsNullOrEmpty(descricao))
            {
                sql += " AND descricao LIKE @descricao";
                parametros.Add(new SqlParameter("@descricao", "%" + descricao + "%"));
            }

            // Condição para status se foi fornecido
            if (status.Equals(1) || status.Equals(0))
            {
                sql += " AND status = @status";
                parametros.Add(new SqlParameter("@status", status));
            }
            else if (status.Equals(3))
            {
                // Não filtrar por status, retorna todos os produtos independentemente do status
                // Aqui, não fazemos nada, pois não queremos adicionar a condição de status
            }


            // Executar a query
            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddRange(parametros.ToArray());

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    produtos.Add(MapearProduto(reader));
                }
            }

            return produtos;
        }

        public Produto GetProdutoById(int idProduto)
        {
            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                string sql = @"Select * FROM produto Where idproduto = @idproduto";

                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@idproduto", idProduto);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return MapearProduto(reader);
                }

            }
            return null;
        }

        //Consultar quantidade estoque
        public int ConsultaEstoqueProduto(int idProduto)
        {
            string sql = @"select p.qtde_estoque from produto p where idproduto = @idProduto";
            int qtdeEstoque = 0;

            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@idProduto", idProduto);
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    if (reader.Read())
                    {
                        qtdeEstoque = Convert.ToInt32(reader["qtde_estoque"]);
                        return qtdeEstoque;
                    }

                    return qtdeEstoque;

                }
                catch (SqlException ex)
                {

                    throw new Exception("Erro ao consultar estoque, erro: " + ex.Message);
                }

            }
        }


        //MÉTODOS DE AÇÃO

        public void CadastrarProduto(Produto produto)
        {
            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                try
                {
                    string sql = @"INSERT INTO produto
                                (descricao, valor_unitario)
                                VALUES (@descricao, @valor_unitario)";

                    SqlCommand command = new SqlCommand(sql, conn);
                    List<SqlParameter> parametros = new List<SqlParameter>
                    {
                        new SqlParameter("@descricao", produto.Descricao),
                        new SqlParameter("@valor_unitario", produto.ValorUnitario)
                    };

                    executaSql(sql, parametros, conn);


                }

                catch (Exception ex)
                {

                    throw new Exception($"Erro ao cadastrar produto, erro: {ex.Message}");
                }


            }
        }

        public void AtualizarProduto(Produto produto)
        {
            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                string sql = @"UPDATE produto SET
                                descricao = @descricao,
                                valor_unitario = @valor_unitario,
                                qtde_estoque = @qtde_estoque,
                                status = @status
                                WHERE idproduto = @idproduto";

                SqlCommand command = new SqlCommand(sql, conn);
                List<SqlParameter> paramentros = new List<SqlParameter>
               {
                    new SqlParameter("@idproduto", produto.IdProduto),
                    new SqlParameter("@descricao", produto.Descricao),
                    new SqlParameter("@valor_unitario", produto.ValorUnitario),
                    new SqlParameter("@qtde_estoque", produto.QtdeEstoque),
                    new SqlParameter("@status", produto.Status)
               };

                try
                {
                    executaSql(sql, paramentros, conn);
                }
                catch (Exception ex)
                {

                    throw new Exception($"Erro ao deletar protudo, erro: {ex.Message}");
                }


            }
        }

        public void AtualizarSatusProduto(int idProduto)
        {
            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                string sql = @"UPDATE  SET produto
                               status = @status
                               WHERE idproduto = @idproduto";

                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@idproduto", idProduto);

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                    throw new Exception($"Erro alterar o status do protudo, erro: {ex.Message}");
                }


            }
        }
    }
}
