using Leaf.Models.Domain;
using Leaf.Models.ViewModels;
using Leaf.Services.Agentes;
using Leaf.Services.Facede;
using Leaf.Services.Materiais;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Leaf.Controllers.Ordens
{
	[Authorize]
	public class PedidoController : Controller
	{
		private readonly PedidoFacedeServices _pedidoFacedeServices;
		private readonly UsuarioServices _usuarioServices;
		

		public PedidoController(PedidoFacedeServices pedidoFacedeServices, UsuarioServices usuarioServices, ProdutoServices produtoServices)
		{
			_usuarioServices = usuarioServices;
			_pedidoFacedeServices = pedidoFacedeServices;
		}

		public async Task<IActionResult> Index()
		{
			List<PedidoViewModel> pedidos = new List<PedidoViewModel>();
			try
			{
				pedidos = await _pedidoFacedeServices.GetPedidosAsync();

				if (pedidos != null && pedidos.Any())
				{
					TempData["MensagemSucesso"] = "Dados atualizados.";
				}
				else
				{
					TempData["MensagemErro"] = "Não há pedidos lançados.";
				}

				return View(pedidos);
			}
			catch (Exception ex)
			{
				TempData["MensagemErro"] = "Não foi possível estabelecer conexão, erro: " + ex.Message;
				return View(new List<PedidoViewModel>());
			}
		}

		[HttpGet]
		public async Task<IActionResult> Buscar(int numeroPedido, string status)
		{
			List<PedidoViewModel> pedidos = new List<PedidoViewModel>();
			try
			{
				pedidos = await _pedidoFacedeServices.GetPedidoFiltroAsync(numeroPedido, status);

				if (pedidos != null && pedidos.Any())
				{
					ViewBag.Status = status;
					ViewBag.NumeroPedido = numeroPedido;
					TempData["MensagemSucesso"] = "Dados atualizados.";
				}
				else
				{
					TempData["MensagemErro"] = "Não há pedidos lançados.";
				}

				return View("Index", pedidos);
			}
			catch (Exception ex)
			{
				TempData["MensagemErro"] = "Erro ao buscar registros: " + ex.Message;
				return View("Index", new List<PedidoViewModel>());
			}
		}

		[HttpGet]
		public async Task<IActionResult> Atualizar(int id)
		{
			GetEntregadores();

			try
			{
				PedidoViewModel pedidoViewModel = await _pedidoFacedeServices.GetPedidoAsync(id);
				if (pedidoViewModel != null)
				{
					return View(pedidoViewModel);
				}

				TempData["MensagemErro"] = "Não foi possível trazer os dados do pedido.";
				return RedirectToAction("Index");
			}
			catch (Exception ex)
			{
				TempData["MensagemErro"] = "Erro ao acessar o pedido: " + ex.Message;
				return RedirectToAction("Index");
			}
		}

		[HttpPost]
		public async Task<IActionResult> AtualizarStatus(PedidoViewModel pedidoViewModel, int entregadores)
		{
			GetEntregadores();

			try
			{
				pedidoViewModel = await _pedidoFacedeServices.GetPedidoAsync(pedidoViewModel.IdPedido);
                
                if (pedidoViewModel == null || entregadores == 0)
				{
					TempData["MensagemErro"] = "Erro ao atualizar o pedido, selecione um entregador novamente.";
					GetEntregadores();
					return View("Atualizar", pedidoViewModel);
				}

				List<Produto> produtosNegativos = _pedidoFacedeServices.AtualizarPedido(pedidoViewModel, entregadores);

				

				if (produtosNegativos != null && produtosNegativos.Any())
				{
					TempData["MensagemErro"] = "Erro ao atualizar o pedido, há produtos em falta de estoque.";
					List<string> produtosNegativosList = new List<string>();
					foreach (var item in produtosNegativos)
					{
						produtosNegativosList.Add($"Produto: {item.Descricao}");
					}

					// Junte todas as descrições em uma única string separada por quebras de linha
					ViewBag.Produtos = produtosNegativos;
					TempData["ProdutosNegativos"] = "Há produtos marcados sem quantidade miníma em estoque.";
					return View("Atualizar", pedidoViewModel);

                }

                TempData["MensagemSucesso"] = "Pedido atualizado com êxito. Agora em rota de entrega!";
				return RedirectToAction("Index");
			}
			catch (Exception ex)
			{
				TempData["MensagemErro"] = "Erro ao atualizar o status do pedido: " + ex.Message;
                return View("Atualizar", pedidoViewModel);

            }
        }

		private void GetEntregadores()
		{
			try
			{
				List<Usuario> entregadores = _usuarioServices.ListaEntregadores();
				if (entregadores != null && entregadores.Any())
				{
					ViewData["Entregadores"] = entregadores;
				}
				else
				{
					ViewData["Entregadores"] = new List<Usuario> { new Usuario { Id = 0, Nome = "Desconhecido" } };
				}
			}
			catch (Exception ex)
			{
				TempData["MensagemErro"] = "Erro ao carregar a lista de entregadores: " + ex.Message;
				ViewData["Entregadores"] = new List<Usuario> { new Usuario { Id = 0, Nome = "Erro ao carregar" } };
			}
		}
	}
}
