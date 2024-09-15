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
            // Aqui podemos passar a lista de departamentos para o dropdown
            ViewBag.Departamentos = _departamentoService.ListaDepartamenos();
            return View();
        }

        [HttpPost]
        public IActionResult Criar(Usuario usuario, string confSenha)
        {

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["MensagemErro"] = "Ops, algo deu errado. Erros: " + string.Join(", ", errors);
                ViewBag.CssId = "eventoErro";
                

                ViewBag.Departamentos = _departamentoService.ListaDepartamenos();
                return View("NovoUsuario", usuario);
            }
            else
            {
                try
                {
                    if (usuario.Senha == confSenha)
                    {
                        if (ModelState.IsValid)
                        {
                            _usuarioService.NovoUsuario(usuario);

                            //mensagem de sucesso:
                            TempData["MensagemErro"] = "Operação realizada com sucesso!";
                            ViewBag.CssId = "eventoSucesso";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            TempData["MensagemErro"] = "Ops algo deu errado, tente novamente.";
                            ViewBag.CssId = "eventoErro";
                            ViewBag.Departamentos = _departamentoService.ListaDepartamenos();
                            return View("NovoUsuario", usuario);
                        }

                    }
                    else
                    {
                        TempData["MensagemErro"] = "Senhas diferentes.";
                        ViewBag.CssId = "eventoErro";
                        ViewBag.Departamentos = _departamentoService.ListaDepartamenos();
                        return View("NovoUsuario", usuario);
                    }

                }
                catch (Exception ex)
                {
                    ViewBag.CssId = "eventoErro";
                    TempData["MensagemErro"] = $"Ops não foi possivel realizar a operação, erro: {ex.Message}";
                    // Se houver erro, retorna à página com a mensagem de erro
                    ViewBag.Departamentos = _departamentoService.ListaDepartamenos();
                    return View("NovoUsuario", usuario);
                }
            }
            
        }

    }
}
