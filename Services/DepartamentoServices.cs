using Leaf.Data;
using Leaf.Models.Domain;
using Leaf.Repository;

namespace Leaf.Services
{
    public class DepartamentoServices
    {
        private readonly DbConnectionManager _dbConnectionManager;

        // Construtor que injeta a dependência de DbConnectionManager
        public DepartamentoServices(DbConnectionManager dbConnectionManager)
        {
            _dbConnectionManager = dbConnectionManager;
        }

        public List<Departamento> ListaDepartamenos()
        {
            DepartamentoRepository _departamentoRepository = new DepartamentoRepository(_dbConnectionManager);
            return _departamentoRepository.GetDepartamentos();
        }

        public Departamento GetDepartamento(int idUsuario)
        {
            DepartamentoRepository _departamentoRepository = new DepartamentoRepository(_dbConnectionManager);
            return _departamentoRepository.GetDepartamento(idUsuario);
        }
    }
}
