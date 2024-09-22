using Leaf.Data;
using Leaf.Models;
using Leaf.Repository;
using System.Diagnostics.CodeAnalysis;

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
            try
            {
                return _usuarioRepository.GetUsuarios();

            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar os usuários: {ex.Message}");
                
            }
        }

        public List<Usuario> ListaUsuariosFiltro(string nome, int idDpto)
        {
            UsuarioRepository _usuarioRepository = new UsuarioRepository(_dbConnectionManager);
            try
            {
                return _usuarioRepository.GetUsuariosFiltro(nome, idDpto);
            }
            catch (Exception ex)
            {

                throw new Exception($"Erro ao acessar a lista de usuários: {ex.Message}");
            }
        }
        public bool NovoUsuario(Usuario usuario)
        {
            UsuarioRepository _usuarioRepository = new UsuarioRepository(_dbConnectionManager);
            try
            {
                _usuarioRepository.NovoUsuario(usuario);
                return true;

            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cadastrar o usuário: {ex.Message}");

            }
        }

        public bool AtualizarUsuario(Usuario usuario)
        {
            try
            {
                UsuarioRepository _usuarioRepository = new UsuarioRepository(_dbConnectionManager);
                _usuarioRepository.AtualizarUsuario(usuario);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar o usuário: {ex.Message}");
            }

        }

        public Usuario getUsuarioId(int id)
        {
            try
            {
                UsuarioRepository _usuarioRepository = new UsuarioRepository(_dbConnectionManager);
                return _usuarioRepository.GetUsuarioById(id);
                
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao localizar o usuário: {ex.Message}");

            }

        }

        public bool ExcluirUsuario(int id)
        {
            try
            {
                UsuarioRepository _usuarioRepository = new UsuarioRepository(_dbConnectionManager);
                _usuarioRepository.DeletarUsuario(id);
                return true;

            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao excluir o usuário: {ex.Message}");
            }
        }

        public bool AtualizaStatusUsuario(int id)
        {
            UsuarioRepository _usuarioRepository = new UsuarioRepository(_dbConnectionManager);

            try
            {
                if (_usuarioRepository.AtualizaStatus(id))
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar status do usuário: {ex.Message}");
            }
        }

    }
}
