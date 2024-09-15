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


        public IActionResult Index()
        {
            List<Usuario> usuarios = _usuarioService.ListaUsuarios();
            return View(usuarios);
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

       
        // Exibir formulário de edição
        public IActionResult Editar(int id) // renomeado para Editar
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
            if (!ModelState.IsValid)
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
