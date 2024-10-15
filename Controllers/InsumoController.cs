using Leaf.Services;
using Leaf.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Authorization;

namespace Leaf.Controllers
{
    [Authorize]
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
            List<Insumo> insumos = GetInsumosPessoa(_insumoServices.ListarInsumos());
            if (insumos.Any())
            {
                return View(insumos);

            }
            else
            {
                List<Insumo> insumosNull = new List<Insumo>();
                return View(insumosNull);
            }

        }

        [HttpGet]
        public IActionResult Buscar(string descricao, string cnpj, int status)
        {
            List<Insumo> insumos = GetInsumosPessoa(_insumoServices.ListarInsumosFiltrados(descricao, cnpj, status));

            try
            {
                if (insumos.Any()) // Verifica se há dados
                {
                    TempData["MensagemSucesso"] = "Dados encontrados.";
                }
                else
                {
                    TempData["MensagemErro"] = "Ops, não encontramos os dados solicitados.";
                }

                return View("Index", insumos);
            }
            catch (Exception ex)
            {
                throw new Exception($"Não foi possível retornar as pessoas solicitadas: erro {ex.Message}");
            }
        }


        [HttpGet]
        public IActionResult Detalhes(int id)
        {
            Insumo insumo = _insumoServices.GetInsumo(id);
            insumo.Pessoa = _pessoaServices.GetPessoa(insumo.IdPessoa);

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
            insumo.Pessoa = _pessoaServices.GetPessoa(insumo.IdPessoa);
            return insumo == null ? RedirectToAction("Index") : View(insumo);
        }

        [HttpPost]
        public IActionResult Atualizar(Insumo insumo)
        {
            try
            {
                // Verificar se os dados enviados estão válidos
                if (ModelState.IsValid)
                {

                    if (_insumoServices.AtualizarInsumo(insumo))
                    {
                        TempData["MensagemSucesso"] = "Insumo atualizado com sucesso!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["MensagemErro"] = "Erro ao tentar atualizar o insumo no banco de dados.";
                    }
                }
                else
                {
                    TempData["MensagemErro"] = "Erro na validação dos dados do insumo. Verifique os campos e tente novamente.";
                }

                return View("Editar", insumo);
            }
            catch (Exception ex)
            {

                TempData["MensagemErro"] = "Erro na validação dos dados do insumo. erro: " + ex.Message;
                return View("Editar", insumo);
            }
           
        }

        public List<Insumo> GetInsumosPessoa(List<Insumo> insumo)
        {
            foreach (var item in insumo)
            {
                item.Pessoa = _pessoaServices.GetPessoa(item.IdPessoa);
            }

            return insumo;
        }

    }
}
