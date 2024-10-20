using System.Collections.Generic;
using System.Data.SqlClient;
using Leaf.Data;
using Leaf.Models.Domain;

namespace Leaf.Repository
{
    public class DepartamentoRepository
    {
        private readonly DbConnectionManager _dbConnectionManager;

        public DepartamentoRepository(DbConnectionManager dbConnectionManager)
        {
            _dbConnectionManager = dbConnectionManager;
        }

        // Método para listar os departamentos
        public List<Departamento> GetDepartamentos()
        {
            List<Departamento> departamentos = new List<Departamento>();

            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                string sql = "SELECT IdDpto, Descricao FROM Departamento";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Departamento departamento = new Departamento
                    {
                        IdDpto = Convert.ToInt32(reader["IdDpto"]),
                        Descricao = reader["Descricao"].ToString()
                    };

                    departamentos.Add(departamento);
                }
            }

            return departamentos;
        }

        public Departamento? GetDepartamento(int idUsuario)
        {
            using (SqlConnection conn = _dbConnectionManager.GetConnection())
            {
                //Pego o departamento especifico do meu usuario
                string sql = @"Select d.iddpto, d.descricao from departamento d
                               inner join usuario u on d.iddpto = u.id_dpto 
                               where u.idusuario = @idUsuario";

                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@idUsuario", idUsuario);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    Departamento departamento = new Departamento
                    {
                        IdDpto = Convert.ToInt32(reader["iddpto"]),
                        Descricao = reader["descricao"].ToString()
                    };

                    if (departamento != null)
                    {
                        return departamento;
                    }
                }

                return null;

            }

        }
    }
}
