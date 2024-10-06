using Leaf.Models;
using Leaf.Repository;
using Leaf.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;

namespace Leaf.Controllers
{
    public class UsuarioController : Controller
    {
        //injetar os serviços no meu construtor

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
            try
            {
                List<Usuario> usuarios = _usuarioService.ListaUsuarios();
                List<Departamento> departamentos = _departamentoService.ListaDepartamenos();

                if (usuarios == null)
                {
                    TempData["MensagemErro"] = "Ops, não encontramos os dados solicitados.";
                    usuarios = new List<Usuario>();
                }
                if (departamentos == null)
                {
                    TempData["MensagemErro"] = "Ops, não encontramos os departamentos solicitados.";

                    departamentos = new List<Departamento>();
                }

                ViewBag.Departamentos = departamentos;
                return View(usuarios);

            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = "Não foi possivel estabelecer conexão, erro: " + ex.Message;
                List<Usuario> usuarioNull = new List<Usuario>();
                List<Departamento> departamentoNull = new List<Departamento>();
                ViewBag.Departamentos = departamentoNull;
                return View(usuarioNull);

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

                if (usuarios.Any()) // Verifica se há dados
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
                throw new Exception($"Não foi possível retornar os usuários solicitados: erro {ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult Detalhes(int id)
        {
            try
            {
                // Buscar a pessoa pelo ID
                Usuario usuario = _usuarioService.getUsuarioId(id);

                if (usuario == null)
                {
                    TempData["MensagemErro"] = "Usuário não encontrada.";
                    return RedirectToAction("Index");
                }

                // Retornar para a view de detalhes com a pessoa encontrada
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
            List<Departamento> departamentos = _departamentoService.ListaDepartamenos();
            if (departamentos == null || !departamentos.Any())
            {
                TempData["MensagemErro"] = "Não foi possível carregar os departamentos";
            }
            ViewBag.Departamentos = departamentos;
            return View();
        }

        [HttpPost]
        public IActionResult Cadastrar(Usuario usuario, string confSenha)
        {
            // Verificar se os dados são inválidos antes de processar
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["MensagemErro"] = "Ops, algo deu errado. Erros: " + string.Join(", ", errors);

                
                ViewBag.Departamentos = _departamentoService.ListaDepartamenos();
                return View("Criar", usuario);
            }
            else
            {
                try
                {
                    // Verificar se as senhas coincidem
                    if (usuario.Senha != confSenha)
                    {
                        TempData["MensagemErro"] = "Senhas diferentes.";
                        ViewBag.Departamentos = _departamentoService.ListaDepartamenos();
                        return View("Criar", usuario);
                    }
                    else
                    {
                        usuario.Status = 1;
                        _usuarioService.NovoUsuario(usuario);

                        // Mensagem de sucesso e redirecionamento
                        TempData["MensagemSucesso"] = "Operação realizada com sucesso!";
                        return RedirectToAction("Index");
                    }
                    
                }
                catch (Exception ex)
                {
                    TempData["MensagemErro"] = $"Ops não foi possível realizar a operação, erro: {ex.Message}";
                    ViewBag.Departamentos = _departamentoService.ListaDepartamenos();
                    return View("Criar", usuario);
                }

            }
            
        }

        public IActionResult Editar(int id)
        {
            // Chama o serviço ou repositório para buscar o usuário pelo id
            Usuario usuario = _usuarioService.getUsuarioId(id);

            if (usuario == null)
            {
                ViewData["MenssagemErro"] = "Erro ao editar, tente novamente mais tarde";
                return View("Index");
            }

            // Passa o usuário para a view
            ViewBag.Departamentos = _departamentoService.ListaDepartamenos();

            return View(usuario);
        }

        [HttpPost]
        public IActionResult Atualizar(Usuario usuario, string confSenha)
    {
            
            // Verificar se os dados são inválidos antes de processar
            if (!ModelState.IsValid || usuario.Id == 0)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["MensagemErro"] = "Ops, algo deu errado. Erros: " + string.Join(", ", errors);
                ViewBag.Departamentos = _departamentoService.ListaDepartamenos();
                return View("EditarUsuario", usuario);
            }
            else
            {
                try
                {
                    // Verificar se as senhas coincidem
                    if (usuario.Senha != confSenha)
                    {
                        TempData["MensagemErro"] = "Senhas diferentes.";
                        ViewBag.Departamentos = _departamentoService.ListaDepartamenos();
                        return View("EditarUsuario", usuario);
                    }
                    else
                    {
                        //verifica se houve mudança no status
                        if (usuario.Status == 0)
                        {
                            if (_usuarioService.AtualizaStatusUsuario(usuario.Id))
                            {
                                TempData["MensagemSucesso"] = "Status do usuario alterado. Demais mudanças foram descartadas!";
                                return RedirectToAction("Index");
                            }

                            TempData["MensagemErro"] = "Erro ao alterar status do usuário, tente novamente.";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            if (_usuarioService.AtualizarUsuario(usuario))
                            {
                                // Mensagem de sucesso e redirecionamento
                                TempData["MensagemSucesso"] = "Operação realizada com sucesso!";
                                return RedirectToAction("Index");
                            }
                            // Mensagem de erro e redirecionamento
                            TempData["MensagemErro"] = "Erro ao tentar atualizar usuario. Tente novamente";
                            return RedirectToAction("Index");
                        }

                    }

                }
                catch (Exception ex)
                {
                    TempData["MensagemErro"] = $"Ops não foi possível realizar a operação, erro: {ex.Message}";
                    ViewBag.Departamentos = _departamentoService.ListaDepartamenos();
                    return View("NovoUsuario", usuario);
                }

            }
        }

    }
}
