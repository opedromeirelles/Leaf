using Leaf.Services;
using Leaf.Models;
using Microsoft.AspNetCore.Mvc;

namespace Leaf.Controllers
{
    public class InsumoController : Controller
    {
        private readonly PessoaServices _pessoaServices;
        private readonly InsumoServices _insumoServices;

        public InsumoController(InsumoServices insumoServices, PessoaServices pessoaServices)
        {
            _insumoServices = insumoServices;
            _pessoaServices = pessoaServices;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<Insumo> insumos = _insumoServices.ListarInsumos() ?? new List<Insumo>();
            return View(insumos);
        }

        [HttpGet]
        public IActionResult Detalhes(int id)
        {
            Insumo insumo = _insumoServices.GetInsumo(id);
            if (insumo == null)
            {
                TempData["MensagemErro"] = "Insumo não encontrado.";
                return RedirectToAction("Index");
            }
            return View(insumo);
        }

        public IActionResult Cadastrar()
        {
            return View(new Insumo());
        }

        [HttpGet]
        public JsonResult ValidarPessoa(string cnpj)
        {
            if (!string.IsNullOrEmpty(cnpj) && cnpj.Length == 18)
            {
                Pessoa pessoa = _pessoaServices.PessoaComCnpj(cnpj);
                if (pessoa != null)
                {
                    return Json(new
                    {
                        idpessoa = pessoa.IdPessoa,
                        nome = pessoa.Nome,
                        cnpj = pessoa.Cnpj
                    });
                }
            }
            return Json(null);
        }

        [HttpPost]
        public IActionResult Criar(Insumo insumo)
        {
            if (ModelState.IsValid && insumo.IdPessoa != 0)
            {
                if (_insumoServices.CadastrarInsumo(insumo))
                {
                    TempData["MensagemSucesso"] = "Insumo cadastrado com sucesso!";
                    return RedirectToAction("Index");
                }
            }

            TempData["MensagemErro"] = "Erro ao tentar cadastrar o insumo.";
            return View("Cadastrar", insumo);
        }

        public IActionResult Editar(int id)
        {
            Insumo insumo = _insumoServices.GetInsumo(id);
            return insumo == null ? RedirectToAction("Index") : View(insumo);
        }

        [HttpPost]
        public IActionResult Atualizar(Insumo insumo)
        {
            if (ModelState.IsValid && _insumoServices.AtualizarInsumo(insumo))
            {
                TempData["MensagemSucesso"] = "Insumo atualizado com sucesso!";
                return RedirectToAction("Index");
            }

            TempData["MensagemErro"] = "Erro ao atualizar o insumo.";
            return View("Editar", insumo);
        }
    }
}
