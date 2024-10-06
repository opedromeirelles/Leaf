using System.Data.SqlClient;
using Microsoft.Data;
using Leaf.Data;
using Leaf.Models;
using System.Globalization;
using System.Xml.Serialization;
using Microsoft.IdentityModel.Protocols;

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
                IdItemOc = Convert.ToInt32(reader["iditemoc"]),
                IdInsumo = Convert.ToInt32(reader["id_insumo"]),
                Quantidade = Convert.ToInt32(reader["quantidade"]),
                SubTotal = Convert.ToDecimal(reader["subtotal"]),
                IdOc = Convert.ToInt32(reader["id_oc"])
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


    }
}
