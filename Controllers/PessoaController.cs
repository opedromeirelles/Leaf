using Leaf.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Leaf.Models.Domain;

namespace Leaf.Controllers
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
            List<Pessoa> pessoas = _pessoaServices.ListarPessoas();

            if (pessoas == null)
            {
                TempData["MensagemErro"] = "Ops, não encontramos os dados solicitados.";
                pessoas = new List<Pessoa>();
            }

            return View(pessoas);
        }

        // Buscar pessoas com filtros
        [HttpGet]
        public IActionResult Buscar(string nome, string tipo)
        {
            try
            {
                List<Pessoa> pessoas = _pessoaServices.ListarPessoasFiltradas(nome, tipo);

                if (pessoas.Any()) // Verifica se há dados
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
                throw new Exception($"Não foi possível retornar as pessoas solicitadas: erro {ex.Message}");
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

                // Retornar para a view de detalhes com a pessoa encontrada
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
            return View();
        }

        // Criar nova pessoa
        [HttpPost]
        public IActionResult Criar(Pessoa pessoa)
        {
            if (_pessoaServices.CadastrarPessoa(pessoa))
            {
                TempData["MensagemSucesso"] = "Pessoa cadastrada com sucesso!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["MensagemErro"] = "Ops, não foi possível efetuar o cadastro da pessoa";
                return View("Cadastrar", pessoa);
            }
        }

        // Exibir a tela de edição de pessoa
        public IActionResult Editar(int id)
        {
            Pessoa pessoa = _pessoaServices.GetPessoa(id);
            if (pessoa != null)
            {
                return View(pessoa);
            }
            else
            {
                TempData["MensagemErro"] = "Erro ao tentar editar a pessoa";
                return RedirectToAction("Index");
            }
        }

        // Atualizar uma pessoa existente
        [HttpPost]
        public IActionResult Atualizar(Pessoa pessoa)
        {
            if (_pessoaServices.AtualizarPessoa(pessoa))
            {
                TempData["MensagemSucesso"] = "Pessoa atualizada com sucesso!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["MensagemErro"] = "Ops, não foi possível atualizar a pessoa";
                return View("Editar", pessoa);
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
    }
}
