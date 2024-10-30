using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Leaf.Models.Domain;
using Leaf.Services.Agentes;
using Leaf.Services.Materiais;

namespace Leaf.Controllers.Materiais
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
            try
            {
                GetFornecedoresViewBag();
                List<Insumo> insumos = _insumoServices.ListarInsumos();
                return View(insumos.Any() ? insumos : new List<Insumo>());
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = "Erro ao carregar a lista de insumos: " + ex.Message;
                return View(new List<Insumo>());
            }
        }

        [HttpGet]
        public IActionResult Buscar(string descricao, int fornecedor, int status)
        {
            try
            {
                GetFornecedoresViewBag();
                List<Insumo> insumos = _insumoServices.BuscarInsumosFiltro(descricao, fornecedor, status);

                if (insumos != null && insumos.Any())
                {
                    TempData["MensagemSucesso"] = "Dados encontrados.";
                    return View("Index", insumos);
                }
                else
                {
                    TempData["MensagemErro"] = "Não há insumos com os filtros atuais.";
                    return View("Index", insumos);
                }
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = "Não foi possível listar os insumos, erro: " + ex.Message;
                return View("Index", new List<Insumo>());
            }
        }

        [HttpGet]
        public IActionResult Detalhes(int id)
        {
            try
            {
                Insumo insumo = _insumoServices.GetInsumo(id);
                if (insumo == null)
                {
                    TempData["MensagemErro"] = "Insumo não encontrado.";
                    return RedirectToAction("Index");
                }

                return View(insumo);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = "Erro ao carregar detalhes do insumo, erro: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        public IActionResult Cadastrar()
        {
            try
            {
                return View(new Insumo());
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = "Erro ao abrir a página de cadastro: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Criar(Insumo insumo)
        {
            if (insumo.ValorUnitario <= 0)
            {
                TempData["MensagemErro"] = "O valor unitário inválido.";
                return View("Cadastrar", insumo);
            }
            try
            {
                
                if (_insumoServices.CadastrarInsumo(insumo))
                {
                    TempData["MensagemSucesso"] = "Insumo cadastrado com sucesso!";
                    return RedirectToAction("Index");
                }

                TempData["MensagemErro"] = "Erro ao tentar cadastrar o insumo.";
                return View("Cadastrar", insumo);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = "Erro ao tentar cadastrar o insumo, erro: " + ex.Message;
                return View("Cadastrar", insumo);
            }
        }

        public IActionResult Editar(int id)
        {
            try
            {
                Insumo insumo = _insumoServices.GetInsumo(id);
                if (insumo == null)
                {
                    TempData["MensagemErro"] = "Insumo não encontrado.";
                    return RedirectToAction("Index");
                }

                // Carrega o nome da pessoa associado ao insumo para exibição
                ViewBag.CodBarras = insumo.CodBarras;

                return View(insumo);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = "Erro ao carregar a página de edição do insumo, erro: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Atualizar(Insumo insumo)
        {
            if (insumo.ValorUnitario <= 0)
            {
                TempData["MensagemErro"] = "O valor unitário inválido.";

                // Mantém o nome da pessoa e o código de barras em caso de erro de validação
                ViewBag.CodBarras = insumo.CodBarras;

                return View("Editar", insumo);
            }

            try
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

                // Mantém os valores da ViewBag para que não sumam na exibição
                ViewBag.CodBarras = insumo.CodBarras;

                return View("Editar", insumo);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = "Erro ao atualizar o insumo, erro: " + ex.Message;

                // Mantém os valores da ViewBag em caso de exceção
                ViewBag.CodBarras = insumo.CodBarras;

                return View("Editar", insumo);
            }
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

        // Popula a ViewBag com fornecedores
        public void GetFornecedoresViewBag()
        {
            try
            {
                List<Pessoa> fornecedores = _pessoaServices.GetFornecedores();
                ViewBag.Fornecedores = fornecedores.Any() ? fornecedores : new List<Pessoa> { new Pessoa { IdPessoa = 0, Nome = "FORNECEDOR INDISPONÍVEL" } };
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = "Erro ao carregar fornecedores: " + ex.Message;
                ViewBag.Fornecedores = new List<Pessoa> { new Pessoa { IdPessoa = 0, Nome = "FORNECEDOR INDISPONÍVEL" } };
            }
        }
    }
}
