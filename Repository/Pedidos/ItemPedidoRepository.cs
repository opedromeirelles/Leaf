using Leaf.Data;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using Leaf.Models.ItensDomain;

namespace Leaf.Repository.Pedidos
{
    public class ItemPedidoRepository
    {
        private readonly DbConnectionManager _dbConnectionManager;
        public ItemPedidoRepository(DbConnectionManager dbConnectionManager)
        {
            _dbConnectionManager = dbConnectionManager;
        }

        public ItemPedido MapearItemPedido(SqlDataReader reader)
        {
			return new ItemPedido
			{
				IdItemPedido = reader["iditempedido"] != DBNull.Value ? Convert.ToInt32(reader["iditempedido"]) : 0,
				IdPedido = reader["id_pedido"] != DBNull.Value ? Convert.ToInt32(reader["id_pedido"]) : 0,
				IdProduto = reader["id_produto"] != DBNull.Value ? Convert.ToInt32(reader["id_produto"]) : 0,
				Quantidade = reader["qtde"] != DBNull.Value ? Convert.ToInt32(reader["qtde"]) : 0,
				SubTotal = reader["sub_total"] != DBNull.Value ? Convert.ToDecimal(reader["sub_total"]) : 0m
			};
		}

        //Listar item do pedido
        public List<ItemPedido> GetItensPedido(int idPedido)
        {
            string sql = @"select * from ITEM_PEDIDO where id_pedido = @idPedido";

            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@idPedido", idPedido);
                SqlDataReader reader = command.ExecuteReader();

                try
                {

                    List<ItemPedido> itensPedido = new List<ItemPedido>();

                    while (reader.Read())
                    {
                        itensPedido.Add(MapearItemPedido(reader));
                    }

                    return itensPedido;

                }
                catch (SqlException ex)
                {
                    throw new Exception("Não foi possível buscar os itens do pedido. Erro: " + ex.Message);
                }

            }
        }


    }
}
