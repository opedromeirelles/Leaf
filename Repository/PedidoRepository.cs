using Leaf.Data;
using System.Data.SqlClient;
using System.Data;
using Leaf.Models.Domain;

namespace Leaf.Repository
{
    public class PedidoRepository
    {

        private readonly DbConnectionManager _dbConnectionManager;
        public PedidoRepository(DbConnectionManager dbConnectionManager)
        {
            _dbConnectionManager = dbConnectionManager;
        }

        //Mapear Pedido
        public Pedido MapearPedido(SqlDataReader reader)
        {
            return new Pedido
            {
                IdPedido = Convert.ToInt32(reader["idpedido"]),
               IdEntregador = reader["id_entregador"] != DBNull.Value ? Convert.ToInt32(reader["id_entregador"]) : (int)0,
                IdVendedor = Convert.ToInt32(reader["id_vendedor"]),
                IdPessoa = Convert.ToInt32(reader["id_pessoa"]),

                ValorToal = Convert.ToDecimal(reader["valor_total"]),

                EndEntrega = reader["end_entrega"] != DBNull.Value ? (reader["end_entrega"].ToString()) : "Não Informado",
                Cep = reader["cep"].ToString(),
                Stauts = reader["status"].ToString(),


                DtaEmissao = reader["dta_emissao"] != DBNull.Value ? Convert.ToDateTime(reader["dta_emissao"]) : (DateTime?)null,
                DtaSaida = reader["dta_saida"] != DBNull.Value ? Convert.ToDateTime(reader["dta_saida"]) : (DateTime?)null,
                DtaEntrega = reader["dta_entrega"] != DBNull.Value ? Convert.ToDateTime(reader["dta_entrega"]) : (DateTime?)null,
                DtaCancelamento = reader["dta_cancelamento"] != DBNull.Value ? Convert.ToDateTime(reader["dta_cancelamento"]) : (DateTime?)null
            };
        }

        //Ler Pedidos
        public List<Pedido> GetPedidos()
        {
            string sql = @"select * from pedido where status = 'EM' ";

            List<Pedido> pedidos = new List<Pedido>();


            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                SqlCommand command = new SqlCommand(sql, conn);
                SqlDataReader reader = command.ExecuteReader();

                try
                {
                    while (reader.Read())
                    {
                        pedidos.Add(MapearPedido(reader));
                    }
                    if (pedidos != null && pedidos.Any())
                    {
                        return pedidos;
                    }

                    return null;
                    
                }
                catch (SqlException ex)
                {

                    throw new Exception("Erro ao listar pedido, erro: " + ex.Message);
                }
            }
        }

        public Pedido GetPedido(int idPedido)
        {
            string sql = @"select * from pedido where idpedido = @idPedido";


            Pedido pedido = new Pedido();


            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@idPedido", idPedido);
                SqlDataReader reader = command.ExecuteReader();

                try
                {
                    if (reader.Read())
                    {
                        pedido = MapearPedido(reader);
                    }
                    if (pedido != null)
                    {
                        return pedido;
                    }

                    return null;

                }
                catch (SqlException ex)
                {

                    throw new Exception("Erro ao listar pedido, erro: " + ex.Message);
                }
            }
        }

        public List<Pedido> BuscarPedido(int idPedido, string status)
        {
            List<Pedido> pedidos = new List<Pedido>();

            // Base da query SQL
            string sql = @"SELECT * FROM pedido WHERE 1 = 1";

            // Parâmetros para a consulta
            List<SqlParameter> parametros = new List<SqlParameter>();

            // Condição para idPedido se foi fornecido
            if (idPedido != 0)
            {
                sql += " AND idpedido = @idpedido"; // Use = para o id numérico
                parametros.Add(new SqlParameter("@idpedido", idPedido));
            }

            // Condição para status se foi fornecido
            if (!string.IsNullOrEmpty(status))
            {
                sql += " AND status = @status";
                parametros.Add(new SqlParameter("@status", "EM"));
            }

            // Executar a query
            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddRange(parametros.ToArray());

                try
                {
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        pedidos.Add(MapearPedido(reader));
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Erro ao tentar buscar pedido, erro: " + ex.Message);
                }
            }

            return pedidos;
        }


        // Alterar pedido
        public bool AlterarStatusPedido(int idPedido, int idEntregador)
        {
            string sql = @"update pedido
                           set status = @status,
                           id_entregador = @idEntregador,
                           dta_saida = @dtaSaida
                           where idpedido = @idPedido";

            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@idPedido", idPedido);
                command.Parameters.AddWithValue("@idEntregador", idEntregador);
                command.Parameters.AddWithValue("@status", "RT");
                command.Parameters.AddWithValue("@dtaSaida", DateTime.Now);


                try
                {
                    int linhasafetadas = command.ExecuteNonQuery();
                    if (linhasafetadas > 0)
                    {
                        return true;
                    }

                    return false;

                }
                catch (SqlException ex)
                {
                    throw new Exception("Erro ao atualizar status do pedido, erro: " + ex.Message);
                }
                
            }

        }


        // Relatorios de pedido
        public List<Pedido> GetListPedidosPeriodo(DateTime dataInicio, DateTime dataFim, int idVendedor, string status)
        {
            string sql = @"SELECT * FROM pedido
                           WHERE 1=1
                           AND dta_emissao >= @dataInicio 
                           AND dta_emissao <= @dataFim";

			List<Pedido> pedidos = new List<Pedido>();

            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {

                SqlCommand command = new SqlCommand(sql, conn);

                //Obrigatóriamente havera duas duas datas
                command.Parameters.AddWithValue("@dataInicio", dataInicio);
                command.Parameters.AddWithValue("@dataFim", dataFim);

                //Pega o status
                if (status != "" && !string.IsNullOrEmpty(status))
                {
                    sql += " AND status = @status";
                    command.Parameters.AddWithValue("@status", status);
                }

                //Se tiver vendedor filtre por vendedor
                if (idVendedor != 0)
                {
                    sql += " AND id_vendedor = @idVendedor";
					command.Parameters.AddWithValue("@idVendedor", idVendedor);
				}

				// Atualizar o texto da consulta, já que ele é modificado dinamicamente
				command.CommandText = sql;

				try
                {
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        pedidos.Add(MapearPedido(reader));
                    }

                    //verifico se esta preenchido, se não, retorno a lista vazia
                    return pedidos.Any() ? pedidos : new List<Pedido>();
				}
                catch (SqlException ex)
                {
                    // Tratar erro e lançar exceção, se necessário
                    throw new Exception("Erro ao tentar buscar pedidos, erro: " + ex.Message);
                }
            } 				
        }
        
        public List<Pedido> GetListPedidosEntregador(DateTime? dataInicio, DateTime? dataFim, int idEntregador)
        {
            string sql = @"SELECT * FROM pedido
                           WHERE 1=1
                           AND id_entregador = @idEntregador ";

			List<Pedido> pedidos = new List<Pedido>();

			using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@idEntregador", idEntregador);

                if (dataInicio != null && dataFim != null)
                {
                    sql += "AND dta_emissao >= @dataInicio AND dta_emissao <= @dataFim";
					command.Parameters.AddWithValue("@dataInicio", dataInicio);
					command.Parameters.AddWithValue("@dataFim", dataFim);
				}

				try
				{
					SqlDataReader reader = command.ExecuteReader();

					while (reader.Read())
					{
						pedidos.Add(MapearPedido(reader));
					}

					if (pedidos.Any() && pedidos != null)
					{
						return pedidos;
					}

					return new List<Pedido>();
				}
				catch (SqlException ex)
				{
					// Tratar erro e lançar exceção, se necessário
					throw new Exception("Erro ao tentar buscar pedidos, erro: " + ex.Message);
				}
			}
        }


    }
}
