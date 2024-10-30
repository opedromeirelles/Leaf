using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Leaf.Models.Domain;
using Leaf.Services.Agentes;
using Leaf.Models.Domain.ErrorModel;

namespace Leaf.Controllers.Agentes
{
    [Authorize]
    public class PessoaController : Controller
    {
        private readonly PessoaServices _pessoaServices;

        // Injeção de dependência para o serviço de Pessoa
        public PessoaController(PessoaServices pessoaServices)
        {
            _pessoaServices = pessoaServices;
        }

        // Exibir lista de pessoas
        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                List<Pessoa> pessoas = _pessoaServices.ListarPessoas();
                if (pessoas == null)
                {
                    TempData["MensagemErro"] = "Ops, não encontramos os dados solicitados.";
                    pessoas = new List<Pessoa>();
                }
                return View(pessoas);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao carregar a lista de pessoas: {ex.Message}";
                return View(new List<Pessoa>());
            }
        }

        // Buscar pessoas com filtros
        [HttpGet]
        public IActionResult Buscar(string nome, string tipo)
        {
            try
            {
                List<Pessoa> pessoas = _pessoaServices.ListarPessoasFiltradas(nome, tipo);

                if (pessoas.Any())
                {
                    TempData["MensagemSucesso"] = "Dados encontrados.";
                }
                else
                {
                    TempData["MensagemErro"] = "Ops, não encontramos os dados solicitados.";
                }

                return View("Index", pessoas);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao buscar pessoas: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public IActionResult Detalhes(int id)
        {
            try
            {
                // Buscar a pessoa pelo ID
                Pessoa pessoa = _pessoaServices.GetPessoa(id);

                if (pessoa == null)
                {
                    TempData["MensagemErro"] = "Pessoa não encontrada.";
                    return RedirectToAction("Index");
                }

                return View(pessoa);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao buscar os detalhes da pessoa: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // Exibir a tela de cadastro de nova pessoa
        public IActionResult Cadastrar()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao carregar a página de cadastro: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // Criar nova pessoa
        [HttpPost]
        public IActionResult Criar(Pessoa pessoa)
        {
            try
            {
                DomainErrorModel errorModel = _pessoaServices.ValidarPessoa(pessoa);
                if (errorModel.Sucesso)
                {
                    _pessoaServices.CadastrarPessoa(pessoa);
                    TempData["MensagemSucesso"] = errorModel.Mensagem;
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["MensagemErro"] = errorModel.Mensagem;
                    return View("Cadastrar", pessoa);
                }
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao tentar cadastrar a pessoa: {ex.Message}";
                return View("Cadastrar", pessoa);
            }
        }

        // Exibir a tela de edição de pessoa
        public IActionResult Editar(int id)
        {
            try
            {
                Pessoa pessoa = _pessoaServices.GetPessoa(id);
                if (pessoa != null)
                {
                    return View(pessoa);
                }
                else
                {
                    TempData["MensagemErro"] = "Erro ao tentar editar a pessoa.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao carregar a página de edição: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // Atualizar uma pessoa existente
        [HttpPost]
        public IActionResult Atualizar(Pessoa pessoa)
        {
            try
            {
                if (_pessoaServices.AtualizarPessoa(pessoa))
                {
                    TempData["MensagemSucesso"] = "Pessoa atualizada com sucesso!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["MensagemErro"] = "Ops, não foi possível atualizar a pessoa.";
                    return View("Editar", pessoa);
                }
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao tentar atualizar a pessoa: {ex.Message}";
                return View("Editar", pessoa);
            }
        }

        [HttpGet]
        public JsonResult ValidarPessoa(string cnpj)
        {
            try
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
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erro ao validar pessoa: {ex.Message}" });
            }
        }
    }
}
