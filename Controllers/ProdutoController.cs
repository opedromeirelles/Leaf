using Leaf.Services;
using Leaf.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Globalization;

namespace Leaf.Controllers
{
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
            var produto = _produtoServices.ListarProdutos();

            if (produto == null)
            {
                TempData["MensagemErro"] = "Ops, não encontramos os dados solicitados.";
                produto = new List<Produto>();
            }

            //ViewBag.StatusProduto = _produtoServices.getStatus();

            return View(produto);
        }
        
        [HttpGet]
        public IActionResult Buscar(string descricao, int status)
        {
            try
            {
                var produtos = _produtoServices.ListarProdutosFiltrados(descricao, status);

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
                throw new Exception($"Não foi possível retornar os usuários solicitados: erro {ex.Message}");
            }
        }
        
        public  IActionResult Cadastrar()
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

        public IActionResult Excluir(int id)
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
        public IActionResult Deletar(Produto produto)
        {
            try
            {
                _produtoServices.ExcluirProduto(produto.IdProduto);
                TempData["MensagemSucesso"] = "Produto excluido com sucesso!";
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Não foi possivel concluir a ação, erro: {ex.Message}";
                return View("Excluir", produto);
            }

        }

    }
}

