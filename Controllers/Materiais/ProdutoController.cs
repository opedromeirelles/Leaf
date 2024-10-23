using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Leaf.Models.Domain;
using Leaf.Services.Materiais;

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
            List<Produto> produtos = _produtoServices.ListarProdutos();


            if (produtos == null)
            {
                TempData["MensagemErro"] = "Ops, não encontramos os dados solicitados.";
                produtos = new List<Produto>();
            }

            return View(produtos);
        }

        [HttpGet]
        public IActionResult Buscar(string descricao, int status)
        {

            try
            {
                List<Produto> produtos = _produtoServices.ListarProdutosFiltrados(descricao, status);

                if (produtos.Any()) // Verifica se há dados
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
                throw new Exception($"Não foi possível retornar os produtos solicitados: erro {ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult Detalhes(int id)
        {
            try
            {
                // Buscar a pessoa pelo ID
                Produto produto = _produtoServices.GetProduto(id);

                if (produto == null)
                {
                    TempData["MensagemErro"] = "Produto não encontrada.";
                    return RedirectToAction("Index");
                }

                // Retornar para a view de detalhes com a pessoa encontrada
                return View(produto);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao buscar os detalhes do produto: {ex.Message}";
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
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["MensagemErro"] = "Ops, algo deu errado. Erros: " + string.Join(", ", errors);
                return View("Cadastrar", produto);
            }
            else
            {

                // Conversão manual do valor decimal
                produto.ValorUnitario = decimal.Parse(produto.ValorUnitario.ToString().Replace(".", ","), new CultureInfo("pt-BR"));
                if (produto.ValorUnitario <= 0)
                {
                    TempData["MensagemErro"] = "O valor informado é inválido";
                    return View("Cadastrar", produto);
                }

                // Ativando o produto
                produto.Status = 1;

                if (_produtoServices.CadastrarProduto(produto))
                {
                    TempData["MensagemSucesso"] = "Produto cadastrado com sucesso!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["MensagemErro"] = "Ops, não foi possivel efetuar o cadastro do produto";
                    return View("Cadastrar", produto);
                }
            }



        }

        public IActionResult Editar(int id)
        {
            Produto produto = _produtoServices.GetProduto(id);
            if (produto != null)
            {
                return View(produto);
            }
            else
            {
                TempData["MensagemErro"] = "Erro ao tentar editar o produto";
                return RedirectToAction("index");
            }
        }

        [HttpPost]
        public IActionResult Atualizar(Produto produto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["MensagemErro"] = "Ops, algo deu errado. Erros: " + string.Join(", ", errors);
                return View("Editar", produto);
            }
            else
            {
                // Conversão manual do valor decimal
                produto.ValorUnitario = decimal.Parse(produto.ValorUnitario.ToString().Replace(".", ","), new CultureInfo("pt-BR"));
                if (produto.ValorUnitario <= 0)
                {
                    TempData["MensagemErro"] = "O valor informado é inválido";
                    return View("Editar", produto);
                }

                if (_produtoServices.AtualizarProduto(produto))
                {
                    TempData["MensagemSucesso"] = "Produto atualizado com sucesso!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["MensagemErro"] = "Ops, não foi possivel atualizar o produto";
                    return View("Editar", produto);
                }
            }
        }

    }
}

