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
                string sql = @"SELECT u.idusuario, u.nome, u.login, u.senha, u.status, u.id_dpto, d.descricao
                               FROM Usuario u
                               INNER JOIN Departamento d ON u.id_dpto = d.idDpto";

                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Usuario usuario = new Usuario
                    {
                        Id = Convert.ToInt32(reader["idusuario"]),
                        Nome = reader["nome"].ToString(),
                        Login = reader["login"].ToString(),
                        Senha = reader["senha"].ToString(),
                        Status = Convert.ToInt32(reader["status"]),
                        IdDpto = Convert.ToInt32(reader["id_dpto"]),
                        Departamento = new Departamento
                        {
                            IdDpto = Convert.ToInt32(reader["id_dpto"]),
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
                string sql = @"INSERT INTO Usuario (nome, login, senha, status, id_dpto)
                               VALUES (@nome, @login, @senha, @status, @id_dpto)";

                List<SqlParameter> sqlParametros = new List<SqlParameter>
                {
                    new SqlParameter("@nome", usuario.Nome),
                    new SqlParameter("@login", usuario.Login),  
                    new SqlParameter("@senha", usuario.Senha),  
                    new SqlParameter("@status", usuario.Status),
                    new SqlParameter("@id_dpto", usuario.IdDpto)
                };

                try
                {
                    executaSql(sql, sqlParametros, conn);
                }
                catch (SqlException ex)
                {
                    
                    throw new Exception($"Erro ao inserir o usuário: {ex.Message}");
                }
            }
        }

        public void AtualizarUsuario(Usuario usuario)
        {
            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                string sql = @"UPDATE Usuario SET nome = @nome, login = @login, senha = @senha, status = @status, id_dpto = @idDpto 
                       WHERE id = @id";

                List<SqlParameter> sqlParametros = new List<SqlParameter>
                {
                    new SqlParameter("@id", usuario.Id),
                    new SqlParameter("@nome", usuario.Nome),
                    new SqlParameter("@login", usuario.Login),
                    new SqlParameter("@senha", usuario.Senha),
                    new SqlParameter("@status", usuario.Status),
                    new SqlParameter("@idDpto", usuario.IdDpto)
                };

                try
                {
                    executaSql(sql, sqlParametros, conn);
                }
                catch (SqlException ex)
                {
                    throw new Exception($"Erro ao inserir o usuário: {ex.Message}");
                }
            }
        }

        public Usuario GetUsuarioById(int id)
        {
            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                string sql = @"select * from usuario where idusuario = @id";

                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@id", id);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    // Cria o objeto Usuario e preenche com os dados do banco
                    Usuario usuario = new Usuario
                    {
                        Id = Convert.ToInt32(reader["idusuario"]),
                        Nome = reader["nome"].ToString(),
                        Login = reader["login"].ToString(),
                        Senha = reader["senha"].ToString(),
                        Status = Convert.ToInt32(reader["status"]),
                        IdDpto = Convert.ToInt32(reader["id_dpto"]) 
                    };

                    return usuario;
                }


            }
                return null;
        }

    }
}
