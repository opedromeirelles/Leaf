using System.Collections.Generic;
using System.Data.SqlClient;
using Leaf.Data;
using Leaf.Models;

namespace Leaf.Repository
{
    public class UsuarioRepository : baseSqlComandos
    {
        private readonly DbConnectionManager _dbConnectionManager;

        public UsuarioRepository(DbConnectionManager dbConnectionManager)
        {
            _dbConnectionManager = dbConnectionManager;
        }

        public List<Usuario> GetUsuarios()
        {
            List<Usuario> usuarios = new List<Usuario>();

            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                // Fazer o join com a tabela Departamento
                string sql = @" select u.id, u.nome, u.loginusuario, u.senha, u.statusUsuario, u.idDepartamento, d.descricao
                                from Usuario u
                                inner join Departamento d on u.idDepartamento = d.idDpto";
;

                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Usuario usuario = new Usuario
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Nome = reader["nome"].ToString(),
                        Login = reader["loginUsuario"].ToString(),
                        Senha = reader["senha"].ToString(),
                        Status = Convert.ToInt32(reader["statusUsuario"]),
                        IdDpto = Convert.ToInt32(reader["idDepartamento"]), 
                        Departamento = new Departamento
                        {
                            IdDpto = Convert.ToInt32(reader["idDepartamento"]),
                            Descricao = reader["descricao"].ToString()
                        }
                    };

                    usuarios.Add(usuario);
                }
            }

            return usuarios;
        }

        public void NovoUsuario(Usuario usuario)
        {
            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {

                string sql = @"INSERT INTO usuario (nome, loginUsuario, senha, statusUsuario, idDepartamento)
                           VALUES (@nome, @senha, @loginUsuario, @senha, @statusUsuario, @idDepartamento)";

                List<SqlParameter> sqlParametros = new List<SqlParameter>
            {
                new SqlParameter("@nome", usuario.Nome),
                new SqlParameter("@loginUsuario", usuario.Nome),
                new SqlParameter("@senha", usuario.Nome),
                new SqlParameter("@statusUsuarios", usuario.Status),
                new SqlParameter("@idDepartamento", usuario.IdDpto)
            };

                try
                {
                    executaSql(sql, sqlParametros, conn);
                }
                catch (SqlException)
                {
                    
                }
            }
               

        }


    }
}
