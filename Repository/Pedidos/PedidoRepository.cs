using Leaf.Data;
using System.Data.SqlClient;
using System.Data;
using Leaf.Models.Domain;

namespace Leaf.Repository.Pedidos
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
                IdPedido = reader["idpedido"] != DBNull.Value ? Convert.ToInt32(reader["idpedido"]) : 0,
                IdEntregador = reader["id_entregador"] != DBNull.Value ? Convert.ToInt32(reader["id_entregador"]) : 0,
                IdVendedor = reader["id_vendedor"] != DBNull.Value ? Convert.ToInt32(reader["id_vendedor"]) : 0,
                IdPessoa = reader["id_pessoa"] != DBNull.Value ? Convert.ToInt32(reader["id_pessoa"]) : 0,

                ValorToal = reader["valor_total"] != DBNull.Value ? Convert.ToDecimal(reader["valor_total"]) : 0m,

                EndEntrega = reader["end_entrega"] != DBNull.Value ? reader["end_entrega"].ToString() : "Não Informado",
                Cep = reader["cep"] != DBNull.Value ? reader["cep"].ToString() : "Não Informado",
                Stauts = reader["status"] != DBNull.Value ? reader["status"].ToString() : "Indefinido",

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

        public async Task<Pedido> GetPedidoAsync(int idPedido)
        {
            string sql = @"select * from pedido where idpedido = @idPedido";

            Pedido pedido = new Pedido();

            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@idPedido", idPedido);

                try
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (await reader.ReadAsync())
                    {
                        pedido = MapearPedido(reader);
                    }

                    return pedido;
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

        //Entregador - Por padrao buscar apenas pedidos entregues
        public async Task<List<Pedido>> GetListPedidosPeriodoAsync(DateTime dataInicio, DateTime dataFim, int idEntregador, int idPedido)
        {
            string sql = @"SELECT * FROM pedido
                   WHERE status = 'BX'
                   AND dta_emissao >= @dataInicio 
                   AND dta_emissao <= @dataFim";

            // Montar a query dinamicamente 
            if (idPedido != 0)
            {
                sql += " AND idpedido = @idPedido";
            }

            if (idEntregador != 0)
            {
                sql += " AND id_entregador = @idEntregador";
            }

            List<Pedido> pedidos = new List<Pedido>();

            // Use await using para garantir o descarte correto do recurso assíncrono
            await using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                SqlCommand command = new SqlCommand(sql, conn);

                // Adiciona os parâmetros
                command.Parameters.AddWithValue("@dataInicio", dataInicio);
                command.Parameters.AddWithValue("@dataFim", dataFim);

                if (idPedido != 0)
                {
                    command.Parameters.AddWithValue("@idPedido", idPedido);
                }

                if (idEntregador != 0)
                {
                    command.Parameters.AddWithValue("@idEntregador", idEntregador);
                }

                try
                {
                    // ExecuteReader de forma assíncrona
                    await using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            pedidos.Add(MapearPedido(reader));
                        }
                    }

                    return pedidos;
                }
                catch (SqlException ex)
                {
                    throw new Exception("Erro ao tentar buscar pedidos, erro: " + ex.Message);
                }
            }
        }

        //Vendedor:
        public async Task<List<Pedido>> GetListPedidosPeriodoAsync(DateTime dataInicio, DateTime dataFim, int idVendedor, int idPedido, string status)
        {
            string sql = @"SELECT * FROM pedido
                   WHERE 1=1
                   AND dta_emissao >= @dataInicio 
                   AND dta_emissao <= @dataFim";

            // Montar a query dinamicamente antes de criar o SqlCommand
            if (!string.IsNullOrEmpty(status))
            {
                sql += " AND status = @status";
            }

            if (idPedido != 0)
            {
                sql += " AND idpedido = @idPedido";
            }

            if (idVendedor != 0)
            {
                sql += " AND id_vendedor = @idVendedor";
            }


            var pedidos = new List<Pedido>();

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

                if (idPedido != 0)
                {
                    command.Parameters.AddWithValue("@idPedido", idPedido);
                }

                if (idVendedor != 0)
                {
                    command.Parameters.AddWithValue("@idVendedor", idVendedor);
                }

                try
                {
                    // ExecuteReader de forma assíncrona
                    await using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            pedidos.Add(MapearPedido(reader));
                        }
                    }

                    return pedidos;
                }
                catch (SqlException ex)
                {
                    throw new Exception("Erro ao tentar buscar pedidos, erro: " + ex.Message);
                }
            }
        }


    }
}
