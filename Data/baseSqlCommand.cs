using System.Data.SqlClient;
using System.Data;


namespace Leaf.Data
{
    public abstract class baseSqlComandos
    {
        public void executaSql(string stringSql, List<SqlParameter> sqlParameters, SqlConnection conexaoBanco)
        {
            // rotina de injeção de comandos no sql
            SqlCommand comando = new SqlCommand(stringSql, conexaoBanco);
            comando.Parameters.AddRange(sqlParameters.ToArray());
            comando.ExecuteNonQuery();
        }
    }
}
