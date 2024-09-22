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
                Departamento = new Departamento
                {
                    IdDpto = Convert.ToInt32(reader["id_dpto"]),
                    Descricao = reader["descricao"].ToString()
                }
            };
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






        /*
        public List<Usuario> GetUsuariosFiltro(string nome, int idDpto)
        {
            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                string sql;
                try
                {
                    //caso tenha dois valores
                    if (!string.IsNullOrEmpty(nome) && idDpto != 0)
                    {
                        sql = @"select u.idusuario, u.nome, u.login, u.senha, u.id_dpto, u.status, d.descricao
                            from usuario u inner join departamento d
                            on u.id_dpto = d.iddpto
                            where id_dpto = @iddpto and u.nome like @nome";


                        SqlCommand command = new SqlCommand(sql, conn);
                        List<SqlParameter> parametros = new List<SqlParameter>
                    {
                        new SqlParameter("@nome", "%"+ nome +"%"),
                        new SqlParameter("@iddpto", idDpto)
                    };

                        SqlDataReader reader = command.ExecuteReader();

                        List<Usuario> usuariosFiltrados = new List<Usuario>();


                        while (reader.Read())
                        {


                            Usuario usuario = new Usuario
                            {
                                Id = Convert.ToInt32(reader["idusuario"]),
                                Nome = reader["nome"].ToString(),
                                Login = reader["login"].ToString(),
                                Senha = reader["senha"].ToString(),
                                Status = Convert.ToInt32(reader["status"]),
                                Departamento = new Departamento
                                {
                                    IdDpto = Convert.ToInt32(reader["iddpto"]),
                                    Descricao = reader["descriacao"].ToString()
                                }
                            };

                            usuariosFiltrados.Add(usuario);

                        };

                        return usuariosFiltrados;
                    }
                    /// caso venha somente departamento
                    else if (string.IsNullOrEmpty(nome) && idDpto != 0)
                    {
                        sql = @"select u.idusuario, u.nome, u.login, u.senha, u.id_dpto, u.status, d.descricao
                            from usuario u inner join departamento d
                            on u.id_dpto = d.iddpto
                            where id_dpto = @iddpto";


                        SqlCommand command = new SqlCommand(sql, conn);
                        command.Parameters.AddWithValue("@iddpto", idDpto);


                        SqlDataReader reader = command.ExecuteReader();

                        List<Usuario> usuariosFiltrados = new List<Usuario>();


                        while (reader.Read())
                        {


                            Usuario usuario = new Usuario
                            {
                                Id = Convert.ToInt32(reader["idusuario"]),
                                Nome = reader["nome"].ToString(),
                                Login = reader["login"].ToString(),
                                Senha = reader["senha"].ToString(),
                                Status = Convert.ToInt32(reader["status"]),
                                Departamento = new Departamento
                                {
                                    IdDpto = Convert.ToInt32(reader["iddpto"]),
                                    Descricao = reader["descriacao"].ToString()
                                }
                            };

                            usuariosFiltrados.Add(usuario);

                        };

                        return usuariosFiltrados;

                    }
                    //caso so passe o nome
                    else if (!string.IsNullOrEmpty(nome) && idDpto == 0)
                    {
                        sql = @"select u.idusuario, u.nome, u.login, u.senha, u.id_dpto, u.status, d.descricao
                            from usuario u inner join departamento d
                            on u.id_dpto = d.iddpto
                            where u.nome like @nome";


                        SqlCommand command = new SqlCommand(sql, conn);
                        command.Parameters.AddWithValue("@nome", "%" + nome + "%");


                        SqlDataReader reader = command.ExecuteReader();

                        List<Usuario> usuariosFiltrados = new List<Usuario>();


                        while (reader.Read())
                        {


                            Usuario usuario = new Usuario
                            {
                                Id = Convert.ToInt32(reader["idusuario"]),
                                Nome = reader["nome"].ToString(),
                                Login = reader["login"].ToString(),
                                Senha = reader["senha"].ToString(),
                                Status = Convert.ToInt32(reader["status"]),
                                Departamento = new Departamento
                                {
                                    IdDpto = Convert.ToInt32(reader["iddpto"]),
                                    Descricao = reader["descricao"].ToString()
                                }
                            };

                            usuariosFiltrados.Add(usuario);

                        };

                        return usuariosFiltrados;

                    }

                    return null;

                }
                catch (Exception ex)
                {
                    throw new Exception($"Impossivel trazer os filtros solicitados, erro: {ex.Message}");
                }

            }
        }
        */

        /*
        public List<Usuario> GetUsuariosFiltro(string nome, int idDpto)
        {
            List<Usuario> usuarios = new List<Usuario>();
            string sql = "";


            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                 sql = "SELECT * FROM usuario u WHERE 1=1"; // base da query

                if (!string.IsNullOrEmpty(nome))
                {
                    sql += " AND u.nome LIKE @nome"; // Adiciona filtro por nome
                }

                if (idDpto != 0)
                {
                    sql += " AND u.id_dpto = @idDpto"; // Adiciona filtro por departamento
                }

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    // Define os parâmetros de forma segura
                    if (!string.IsNullOrEmpty(nome))
                    {
                        cmd.Parameters.AddWithValue("@nome", "%" + nome + "%");
                    }

                    if (idDpto != 0)
                    {
                        cmd.Parameters.AddWithValue("@idDpto", idDpto);
                    }

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
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
                                
                            };

                            usuarios.Add(usuario);
                        }
                    }
                }
            }

            return usuarios;
        }
        */


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

        public void DeletarUsuario(int id)
        {
            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                string sql = @"Delete from usuario where idusuario = @id";

                SqlCommand comando = new SqlCommand(sql, conn);
                comando.Parameters.AddWithValue("@id", id);
                
                try
                {
                    comando.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception($"Erro ao excluir o usuário: {ex.Message}");
                }

            }
        }



    }
}
