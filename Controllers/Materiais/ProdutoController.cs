using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Leaf.Models.Domain;
using Leaf.Services.Materiais;
using Leaf.Models.Domain.ErrorModel;

namespace Leaf.Controllers.Materiais
{
    [Authorize]
    public class ProdutoController : Controller
    {
        private readonly ProdutoServices _produtoServices;

        public ProdutoController(ProdutoServices produtoServices)
        {
            _produtoServices = produtoServices;
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                List<Produto> produtos = _produtoServices.ListarProdutos();

                if (produtos == null || !produtos.Any())
                {
                    TempData["MensagemErro"] = "Ops, não encontramos os dados solicitados.";
                    produtos = new List<Produto>();
                }

                return View(produtos);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao carregar produtos: {ex.Message}";
                return View(new List<Produto>());
            }
        }

        [HttpGet]
        public IActionResult Buscar(string descricao, int status)
        {
            try
            {
                List<Produto> produtos = _produtoServices.ListarProdutosFiltrados(descricao, status);

                if (produtos.Any())
                {
                    TempData["MensagemSucesso"] = "Dados encontrados.";
                }
                else
                {
                    TempData["MensagemErro"] = "Ops, não encontramos os dados solicitados.";
                }

                return View("Index", produtos);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao buscar produtos: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public IActionResult Detalhes(int id)
        {
            try
            {
                Produto produto = _produtoServices.GetProduto(id);

                if (produto == null)
                {
                    TempData["MensagemErro"] = "Produto não encontrado.";
                    return RedirectToAction("Index");
                }

                return View(produto);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao buscar detalhes do produto: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Cadastrar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Criar(Produto produto)
        {
            try
            {
                // Tenta converter o valor para decimal
                if (!decimal.TryParse(produto.ValorUnitario.ToString().Replace(".", ","),
                                      NumberStyles.Number, new CultureInfo("pt-BR"),
                                      out decimal valorConvertido))
                {
                    // Tratamento caso a conversão falhe
                    TempData["MensagemErro"] = "O valor informado para o produto é inválido.";
                    return View("Cadastrar", produto); 
                }

                // Atribui o valor convertido.
                produto.ValorUnitario = valorConvertido;

                DomainErrorModel errorModel = _produtoServices.ValidarPrdouto(produto);

                if (errorModel.Sucesso)
                {
                    _produtoServices.CadastrarProduto(produto);
                    TempData["MensagemSucesso"] = errorModel.Mensagem;
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["MensagemErro"] = errorModel.Mensagem;
                    return View("Cadastrar", produto);
                }
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao validar produto: {ex.Message}";
                return View("Cadastrar", produto);
            }

        }

        public IActionResult Editar(int id)
        {
            try
            {
                Produto produto = _produtoServices.GetProduto(id);

                if (produto == null)
                {
                    TempData["MensagemErro"] = "Produto não encontrado para edição.";
                    return RedirectToAction("Index");
                }

                return View(produto);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao carregar produto para edição: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Atualizar(Produto produto)
        {
            try
            {

                DomainErrorModel errorModel = _produtoServices.ValidarPrdouto(produto);

                if (errorModel.Sucesso)
                {
                    _produtoServices.AtualizarProduto(produto);
                    TempData["MensagemSucesso"] = errorModel.Mensagem;
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["MensagemErro"] = errorModel.Mensagem;
                    return View("Editar", produto);
                }

            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao validar produto: {ex.Message}";
                return View("Editar", produto);
            }

        }
    }
}
