using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Leaf.Models.Domain;
using Leaf.Models.ViewModels;
using Leaf.Services.Agentes;

namespace Leaf.Controllers.Autenticacao
{
    public class LoginController : Controller
    {
        private readonly UsuarioServices _usuarioServices;
        public LoginController(UsuarioServices usuario)
        {
            _usuarioServices = usuario;
        }

        public IActionResult Index(LoginModelView loginModelView)
        {
            if (loginModelView != null)
            {
                return View(loginModelView);
            }

            return View(new LoginModelView());
        }

        [HttpPost]
        public async Task<IActionResult> Entrar(LoginModelView login)
        {
            if (!ModelState.IsValid)
            {
                TempData["MensagemErro"] = $"Usuário e/ou senha invalido(s). Por favor tente novamente.";
                return View(login);
            }

            try
            {
                //Percorro no banco para validar usuario
                Usuario usuario = _usuarioServices.ValidarLogin(login.Username, login.Senha);

                //Se não retornar vazio guardo as informaçoes dele
                if (usuario.Id != 0 && usuario != null)
                {

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                        new Claim(ClaimTypes.Name, usuario.Nome),
                        new Claim(ClaimTypes.Role, usuario.Departamento?.Descricao)

                    };

                    var claimsIdentity = new ClaimsIdentity(claims, "CookieAuthentication");
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true, // Persistência do cookie (login)
                    };

                    await HttpContext.SignInAsync("CookieAuthentication", new ClaimsPrincipal(claimsIdentity), authProperties);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["MensagemErro"] = $"Usuário e/ou senha invalido(s). Por favor tente novamente.";
                    return View("Index", login);
                }

            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Não foi possivel autenticar o usuário, erro: {ex.Message}";
                return View("index", login);
            }

        }

        public IActionResult ErrorLogin(LoginModelView login)
        {
            TempData["MensagemErro"] = $"Usuário e/ou senha invalido(s). Por favor tente novamente.";
            return RedirectToAction("Index", login);
        }


        public async Task<IActionResult> Sair()
        {
            await HttpContext.SignOutAsync("CookieAuthentication");
            return RedirectToAction("Index", "Login");
        }

    }


}
