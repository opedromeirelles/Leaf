﻿using Leaf.Data;
using Leaf.Models.Domain;
using Leaf.Models.Domain.ErrorModel;
using Leaf.Repository.Agentes;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Leaf.Services.Agentes
{
    public class UsuarioServices
    {
        private readonly DbConnectionManager _dbConnectionManager;

        // Construtor que injeta a dependência de DbConnectionManager
        public UsuarioServices(DbConnectionManager dbConnectionManager)
        {
            _dbConnectionManager = dbConnectionManager;
        }

        //Validação
        public DomainErrorModel ValidarUsuario(Usuario usuario)
        {
            UsuarioRepository _usuarioRepository = new UsuarioRepository(_dbConnectionManager);

            try
            {
                List<Usuario> usuarios = _usuarioRepository.GetUsuarios();
                foreach (var userBase in usuarios)
                {

                    if (userBase.Login.Equals(usuario.Login, StringComparison.OrdinalIgnoreCase) ||
                            userBase.Nome.Equals(usuario.Nome, StringComparison.OrdinalIgnoreCase))
                    {
                        return new DomainErrorModel(false, "Usuário já cadastrado.");
                    }
                }

                return (new DomainErrorModel(true, "Usuário cadastrado com sucesso."));

            }
            catch (Exception ex)
            {
                return new DomainErrorModel(false, "Erro ao validar usuário.", "Validação", ex.Message);
            }
        }


        //Listas
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

        public List<Usuario> ListaEntregadores()
        {
            UsuarioRepository _usuarioRepository = new UsuarioRepository(_dbConnectionManager);
            try
            {
                return _usuarioRepository.GetListaEntregador();
            }
            catch (Exception ex)
            {

                throw new Exception($"Erro ao acessar a lista de usuários: {ex.Message}");
            }
        }

        public List<Usuario> ListaAdministrativo()
        {
            UsuarioRepository _usuarioRepository = new UsuarioRepository(_dbConnectionManager);
            try
            {
                return _usuarioRepository.GetListaAdministrativo();
            }
            catch (Exception ex)
            {

                throw new Exception($"Erro ao acessar a lista de usuários: {ex.Message}");
            }
        }

        public List<Usuario> ListaVendedores()
        {
            UsuarioRepository _usuarioRepository = new UsuarioRepository(_dbConnectionManager);
            try
            {
                return _usuarioRepository.GetListaVendedor();
            }
            catch (Exception ex)
            {

                throw new Exception($"Erro ao acessar a lista de usuários: {ex.Message}");
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


        // Ações:
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

        public Usuario ValidarLogin(string username, string senha)
        {
            UsuarioRepository _usuarioRepository = new UsuarioRepository(_dbConnectionManager);
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(senha))
            {
                return _usuarioRepository.ValidarUsuario(username, senha);
            }
            return new Usuario();
        }


        // GET
        public Usuario GetUsuarioId(int id)
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
    }
}
