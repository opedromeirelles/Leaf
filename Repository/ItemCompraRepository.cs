using System.Data.SqlClient;
using System.Data;
using Leaf.Data;
using Leaf.Models.ItensDomain;

namespace Leaf.Repository
{
    public class ItemCompraRepository : baseSqlComandos
    {
        private readonly DbConnectionManager _dbConnectionManager;

        public ItemCompraRepository( DbConnectionManager dbConnectionManager)
        {
            _dbConnectionManager = dbConnectionManager;
        }

        public ItemCompra MapearItemCompra(SqlDataReader reader)
        {
            return new ItemCompra
            {
                IdItemOc = reader["iditemoc"] != DBNull.Value ? Convert.ToInt32(reader["iditemoc"]) : 0,
                IdInsumo = reader["id_insumo"] != DBNull.Value ? Convert.ToInt32(reader["id_insumo"]) : 0,
                Quantidade = reader["qtde"] != DBNull.Value ? Convert.ToInt32(reader["qtde"]) : 0,
                SubTotal = reader["sub_total"] != DBNull.Value ? Convert.ToDecimal(reader["sub_total"]) : 0.0m,
                IdOc = reader["id_oc"] != DBNull.Value ? Convert.ToInt32(reader["id_oc"]) : 0
            };

        }

        public bool NovoItemCompra(int idOc, ItemCompra itemCompra)
        {
            if (itemCompra == null || idOc == 0)
            {
                return false;
            }

            string sql = @"insert into ITEM_OC
                           values (@id_insumo, @quantidade, @subtotal, @id_oc)";
            

            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                SqlCommand command = new SqlCommand(sql, conn);
                List<SqlParameter> parametros = new List<SqlParameter>
                {
                    new SqlParameter("id_insumo", itemCompra.IdInsumo),
                    new SqlParameter("quantidade", itemCompra.Quantidade),
                    new SqlParameter("subtotal", itemCompra.SubTotal),
                    new SqlParameter("@id_oc", itemCompra.IdOc),
                };
                try
                {
                    executaSql(sql, parametros, conn);
                    return true;
                }
                catch (SqlException ex)
                {

                    throw new Exception($"Erro ao cadastrar item compra, erro: {ex.Message}");

                }

            }
        }


        public List<ItemCompra> GetItemCompra(int idCompra)
        {
            List<ItemCompra> itemCompras = new List<ItemCompra>();
            string sql = @"select * from ITEM_OC where id_oc = @idCompra";

            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@idCompra", idCompra);
                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        itemCompras.Add(MapearItemCompra(reader));
                    }

                    return itemCompras ?? new List<ItemCompra>();
                }
                catch (Exception ex)
                {

                    throw new Exception("Erro ao consultar itens da compra, erro: " + ex.Message);
                }
            }


        }


    }
}
