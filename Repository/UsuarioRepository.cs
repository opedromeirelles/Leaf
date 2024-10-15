using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Security.Cryptography.Pkcs;
using Leaf.Data;
using Leaf.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.IdentityModel.Tokens;

namespace Leaf.Repository
{
    public class UsuarioRepository : baseSqlComandos
    {
        private readonly DbConnectionManager _dbConnectionManager;

        public UsuarioRepository(DbConnectionManager dbConnectionManager)
        {
            _dbConnectionManager = dbConnectionManager;
        }

        // Método auxiliar para mapear o resultado do SqlDataReader para um objeto Usuario
        private Usuario MapearUsuario(SqlDataReader reader)
        {
            return new Usuario
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
        }

        // VALIDAR USUARIO
        public Usuario ValidarUsuario(string username, string senha)
        {
            
            string sql = @"SELECT u.idusuario, u.nome, u.login, u.senha, u.status, u.id_dpto, d.descricao
                               FROM Usuario u
                               INNER JOIN Departamento d ON u.id_dpto = d.idDpto
                               WHERE login = @username and senha = @senha";

            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@senha", senha);

                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        Usuario usuario = MapearUsuario(reader);
                        return usuario;
                    }

                    return new Usuario();
                }
                catch (SqlException ex)
                {

                    throw new Exception("Erro ao consultar usuario, erro: " + ex.Message);
                }

            }
        }


        // MÉTODOS DE BUSCA
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
                    usuarios.Add(MapearUsuario(reader));
                }
            }

            return usuarios;
        }

        public List<Usuario> GetUsuariosFiltro(string nome, int idDpto)
        {
            List<Usuario> usuariosFiltrados = new List<Usuario>();

            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
            
                // Base da query
                string sql = @"
                select u.idusuario, u.nome, u.login, u.senha, u.id_dpto, u.status, d.descricao
                from usuario u
                inner join departamento d on u.id_dpto = d.iddpto
                where 1=1";

                // Lista de parâmetros
                List<SqlParameter> parametros = new List<SqlParameter>();

                // Condições dinâmicas
                if (!string.IsNullOrEmpty(nome))
                {
                    sql += " and u.nome like @nome";
                    parametros.Add(new SqlParameter("@nome", "%" + nome + "%"));
                }

                if (idDpto != 0)
                {
                    sql += " and u.id_dpto = @iddpto";
                    parametros.Add(new SqlParameter("@iddpto", idDpto));
                }

                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    // Adicionar os parâmetros ao comando
                    command.Parameters.AddRange(parametros.ToArray());

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Ler e mapear os dados
                        while (reader.Read())
                        {
                            usuariosFiltrados.Add(MapearUsuario(reader));
                        }
                    }
                }
            }

            return usuariosFiltrados;
        }

        public Usuario? GetUsuarioById(int id)
        {
            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                // Fazer o join com a tabela Departamento
                string sql = @"SELECT u.idusuario, u.nome, u.login, u.senha, u.status, u.id_dpto, d.descricao
                               FROM Usuario u
                               INNER JOIN Departamento d ON u.id_dpto = d.idDpto
                               WHERE u.idusuario = @id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return MapearUsuario(reader);
                }
                else
                {
                    return null;
                }

            }
        }

        public List<Usuario> GetListaEntregador()
        {
            List<Usuario> usuarios = new List<Usuario>();

            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                // Fazer o join com a tabela Departamento
                string sql = @"SELECT u.idusuario, u.nome, u.login, u.senha, u.status, u.id_dpto, d.descricao
                               FROM Usuario u
                               INNER JOIN Departamento d ON u.id_dpto = d.idDpto
                               where u.id_dpto = 6";

                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    usuarios.Add(MapearUsuario(reader));
                }
            }

            return usuarios;
        }


        // MÉTODOS DE AÇÃO
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
                string sql = @"UPDATE Usuario SET 
                            nome = @nome, 
                            login = @login, 
                            senha = @senha, 
                            status = @status,
                            id_dpto = @id_dpto 
                            WHERE idusuario = @idusuario";

                List<SqlParameter> sqlParametros = new List<SqlParameter>
                {
                    new SqlParameter("@idusuario", usuario.Id),
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
                    throw new Exception($"Erro ao atualizar usuário: {ex.Message}");
                }
            }
        }           

        public bool AtualizaStatus(int id)
        {
            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                string sql = @"UPDATE usuario
                               SET status = 0
                               WHERE idusuario = @idUsuario";

                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@idUsuario", id);

                try
                {
                    if (command.ExecuteNonQuery() == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                catch (Exception ex)
                {

                    throw new Exception($"Erro ao alterar status do usuário: {ex.Message}");

                }

            }
        }

    }
}
