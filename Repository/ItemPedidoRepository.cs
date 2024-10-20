using Leaf.Data;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using Leaf.Models.ItensDomain;

namespace Leaf.Repository
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
                IdItemPedido = Convert.ToInt32(reader["iditempedido"]),
                IdPedido = Convert.ToInt32(reader["id_pedido"]),
                IdProduto = Convert.ToInt32(reader["id_produto"]),
                Quantidade = Convert.ToInt32(reader["qtde"]),
                SubTotal = Convert.ToDecimal(reader["sub_total"])
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
