using Leaf.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Leaf.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Entrar(LoginModelView login)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (login.Username == "teste"  && login.Senha == "teste")
                    {
                        return RedirectToAction("Index", "Home");

                    }
                    TempData["MensagemErro"] = $"Usuário e/ou senha invalido(s). Por favor tente novamente.";
                }
                return View("Index");                
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Não foi possivel realizar seu login, tente novamente, para mais informações veja: {erro.Message}";
                return RedirectToAction("Index");
            }


        }
    }

}
