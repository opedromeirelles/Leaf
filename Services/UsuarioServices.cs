using Leaf.Data;
using Leaf.Models;
using Leaf.Repository;

namespace Leaf.Services
{
    public class UsuarioServices
    {
        private readonly DbConnectionManager _dbConnectionManager;

        // Construtor que injeta a dependência de DbConnectionManager
        public UsuarioServices(DbConnectionManager dbConnectionManager)
        {
            _dbConnectionManager = dbConnectionManager;
        }

        public List<Usuario> ListaUsuarios()
        {
            UsuarioRepository _usuarioRepository = new UsuarioRepository(_dbConnectionManager);
            return _usuarioRepository.GetUsuarios();
        }

        public bool NovoUsuario(Usuario usuario)
        {
            UsuarioRepository _usuarioRepository = new UsuarioRepository(_dbConnectionManager);
            try
            {
                _usuarioRepository.NovoUsuario(usuario);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
