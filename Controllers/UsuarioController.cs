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
        private readonly UsuarioServices _usuarioService;
        private readonly DepartamentoServices _departamentoService;

        //injetar os serviços no meu construtor
        public UsuarioController(UsuarioServices usuarioService, DepartamentoServices departamentoServices)
        {
            _usuarioService = usuarioService;
            _departamentoService = departamentoServices;
        }


        
        [HttpGet]
        public IActionResult Index()
        {
            var usuarios = _usuarioService.ListaUsuarios();
            var departamentos = _departamentoService.ListaDepartamenos();

            ViewBag.Departamentos = departamentos;
            return View(usuarios); 
        }

        [HttpGet]
        public IActionResult Buscar(string nome, int IdDpto)
        {
            try
            {
                var usuarios = _usuarioService.ListaUsuariosFiltro(nome, IdDpto);
                var departamentos = _departamentoService.ListaDepartamenos();
                ViewBag.Departamentos = departamentos;

                if (usuarios.Any()) // Verifica se há dados
                {
                    TempData["MensagemSucesso"] = "Dados encontrados.";
                }
                else
                {
                    TempData["MensagemErro"] = "Ops, não encontramos os dados solicitados.";
                }

                return View("Index", usuarios); // Use a mesma view "Index"
            }
            catch (Exception ex)
            {
                throw new Exception($"Não foi possível retornar os usuários solicitados: erro {ex.Message}");
            }
        }

        public IActionResult NovoUsuario()
        {
            var departamentos = _departamentoService.ListaDepartamenos();
            if (departamentos == null || !departamentos.Any())
            {
                TempData["MensagemErro"] = "Não foi possível carregar os departamentos";
            }
            ViewBag.Departamentos = departamentos;
            return View();
        }

        [HttpPost]
        public IActionResult Criar(Usuario usuario, string confSenha)
        {
            // Verificar se os dados são inválidos antes de processar
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["MensagemErro"] = "Ops, algo deu errado. Erros: " + string.Join(", ", errors);

                
                ViewBag.Departamentos = _departamentoService.ListaDepartamenos();
                return View("NovoUsuario", usuario);
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
                        return View("NovoUsuario", usuario);
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
                    return View("NovoUsuario", usuario);
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

        public IActionResult Excluir(int id)
        {
            try
            {
                Usuario usuario = _usuarioService.getUsuarioId(id);
                usuario.Departamento = _departamentoService.GetDepartamento(id);

                if (usuario != null)
                {
                    return View(usuario); 
                }

                TempData["MensagemErro"] = "Usuário não encontrado.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Ops não conseguimos dar sequência na exclusão, erro: {ex.Message} - Tente novamente";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult ConfirmarExclusao(int id)
        {
            try
            {
                if (_usuarioService.ExcluirUsuario(id))
                {
                    TempData["MensagemSucesso"] = "Usuário excluido com sucesso.";
                    return RedirectToAction("Index");
                }
               

            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Ops algo deu errado. Tente novamente, erro {ex.Message}";
                return RedirectToAction("Index");
            }

            TempData["MensagemErro"] = $"Ops algo deu errado. Tente novamente";
            return RedirectToAction("Index");

        }

    }
}
