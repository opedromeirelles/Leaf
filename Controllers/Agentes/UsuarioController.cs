using Leaf.Models.Domain;
using Leaf.Models.Domain.ErrorModel;
using Leaf.Repository;
using Leaf.Services.Agentes;
using Leaf.Services.Departamentos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Leaf.Controllers.Agentes
{
    [Authorize]
    public class UsuarioController : Controller
    {
        private readonly UsuarioServices _usuarioService;
        private readonly DepartamentoServices _departamentoService;

        public UsuarioController(UsuarioServices usuarioService, DepartamentoServices departamentoServices)
        {
            _usuarioService = usuarioService;
            _departamentoService = departamentoServices;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<Departamento> departamentos = new List<Departamento> { new Departamento { IdDpto = 0, Descricao = "INDISPONIVEL" } };
            List<Usuario> usuarios = new List<Usuario>();

            try
            {
                usuarios = _usuarioService.ListaUsuarios();
                departamentos = _departamentoService.ListaDepartamenos();

                if (usuarios == null || !usuarios.Any())
                {
                    TempData["MensagemErro"] = "Ops, não encontramos os dados solicitados.";
                }

                if (departamentos == null || !departamentos.Any())
                {
                    TempData["MensagemErro"] = "Ops, não encontramos os departamentos solicitados.";
                }

                ViewBag.Departamentos = departamentos;
                return View(usuarios);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Não foi possível estabelecer conexão, erro: {ex.Message}";
                ViewBag.Departamentos = departamentos;
                return View(usuarios);
            }
        }

        [HttpGet]
        public IActionResult Buscar(string nome, int IdDpto)
        {
            try
            {
                List<Usuario> usuarios = _usuarioService.ListaUsuariosFiltro(nome, IdDpto);
                List<Departamento> departamentos = _departamentoService.ListaDepartamenos();
                ViewBag.Departamentos = departamentos;

                if (usuarios.Any())
                {
                    TempData["MensagemSucesso"] = "Dados encontrados.";
                }
                else
                {
                    TempData["MensagemErro"] = "Ops, não encontramos os dados solicitados.";
                }

                return View("Index", usuarios);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Não foi possível retornar os usuários solicitados: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public IActionResult Detalhes(int id)
        {
            try
            {
                Usuario usuario = _usuarioService.GetUsuarioId(id);

                if (usuario == null)
                {
                    TempData["MensagemErro"] = "Usuário não encontrado.";
                    return RedirectToAction("Index");
                }

                return View(usuario);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao buscar os detalhes do usuário: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Criar()
        {
            List<Departamento> departamentos = new List<Departamento>();

            try
            {
                departamentos = _departamentoService.ListaDepartamenos();

                if (departamentos == null || !departamentos.Any())
                {
                    TempData["MensagemErro"] = "Não foi possível carregar os departamentos.";
                }

                ViewBag.Departamentos = departamentos;
                return View();
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Não foi possível estabelecer os departamentos, erro: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Cadastrar(Usuario usuario, string confSenha)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["MensagemErro"] = "Ops, algo deu errado. Erros: " + string.Join(", ", errors);

                ViewBag.Departamentos = _departamentoService.ListaDepartamenos();
                return View("Criar", usuario);
            }

            try
            {
                if (usuario.Senha != confSenha)
                {
                    TempData["MensagemErro"] = "Senhas diferentes.";
                    ViewBag.Departamentos = _departamentoService.ListaDepartamenos();
                    return View("Criar", usuario);
                }

                usuario.Status = 1;
                DomainErrorModel domainErrorModel = _usuarioService.ValidarUsuario(usuario);

                if (domainErrorModel.Sucesso)
                {
                    _usuarioService.NovoUsuario(usuario);
                    TempData["MensagemSucesso"] = domainErrorModel.Mensagem;
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["MensagemErro"] = domainErrorModel.Mensagem;
                    ViewBag.Departamentos = _departamentoService.ListaDepartamenos();
                    return View("Criar", usuario);
                }
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Ops, não foi possível realizar a operação, erro: {ex.Message}";
                ViewBag.Departamentos = _departamentoService.ListaDepartamenos();
                return View("Criar", usuario);
            }
        }

        public IActionResult Editar(int id)
        {
            try
            {
                Usuario usuario = _usuarioService.GetUsuarioId(id);

                if (usuario == null)
                {
                    TempData["MensagemErro"] = "Erro ao editar, usuário não encontrado.";
                    return RedirectToAction("Index");
                }

                ViewBag.Departamentos = _departamentoService.ListaDepartamenos();
                return View(usuario);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao tentar carregar o usuário para edição: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Atualizar(Usuario usuario, string confSenha)
        {
            if (!ModelState.IsValid || usuario.Id == 0)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["MensagemErro"] = "Ops, algo deu errado. Erros: " + string.Join(", ", errors);


                ViewBag.Departamentos = _departamentoService.ListaDepartamenos();
                return View("EditarUsuario", usuario);
            }

            try
            {
                if (usuario.Senha != confSenha)
                {
                    TempData["MensagemErro"] = "Senhas diferentes.";
                    ViewBag.Departamentos = _departamentoService.ListaDepartamenos();
                    return View("EditarUsuario", usuario);
                }

                if (usuario.Status == 0)
                {
                    if (_usuarioService.AtualizaStatusUsuario(usuario.Id))
                    {
                        TempData["MensagemSucesso"] = "Status do usuário alterado. Demais mudanças foram descartadas!";
                        return RedirectToAction("Index");
                    }

                    TempData["MensagemErro"] = "Erro ao alterar status do usuário, tente novamente.";
                    return RedirectToAction("Index");
                }
                else
                {
                    if (_usuarioService.AtualizarUsuario(usuario))
                    {
                        TempData["MensagemSucesso"] = "Operação realizada com sucesso!";
                        return RedirectToAction("Index");
                    }

                    TempData["MensagemErro"] = "Erro ao tentar atualizar o usuário. Tente novamente.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Ops, não foi possível realizar a operação, erro: {ex.Message}";
                ViewBag.Departamentos = _departamentoService.ListaDepartamenos();
                return View("EditarUsuario", usuario);
            }
        }
    }
}
