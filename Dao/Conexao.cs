using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace Leaf.Dao
{
    public class Conexao
    {
        SqlConnection sqlCon = new SqlConnection();
        private string _stringConexao = "Data Source=localhost;Initial Catalog=CadUsuarios;Integrated Security=True";
        
        public string StringConexao
        {
            get
            {
                return _stringConexao;
            }

        }

        // Método para configurar e retornar a conexão
        public SqlConnection conexaoSql()
        {
            // Atribui a string de conexão à propriedade ConnectionString
            sqlCon.ConnectionString = _stringConexao;

            // Retorna o objeto SqlConnection configurado
            return sqlCon;
        }




    }
}
