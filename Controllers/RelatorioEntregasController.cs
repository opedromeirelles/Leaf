using Leaf.Services;
using Leaf.Services.Facede;
using Leaf.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Leaf.Models.ViewModels;

namespace Leaf.Controllers
{
	public class RelatorioEntregasController : Controller
	{
		private readonly string _pathIndex = "~/Views/Relatorios/Entregas/Index.cshtml";
		private readonly string _pathDetalhes = "~/Views/Relatorios/Entregas/Detalhes.cshtml";

		private readonly UsuarioServices _usuarioServices;
		private readonly PedidoFacedeServices _pedidoFacedeServices;

		public RelatorioEntregasController(UsuarioServices usuarioServices, PedidoFacedeServices pedidoFacedeServices)
		{
			_usuarioServices = usuarioServices;
			_pedidoFacedeServices = pedidoFacedeServices;
		}

		[Route("relatorio-entregas")]
		public IActionResult Index()
		{
			GetEntregadores();

			return View(_pathIndex);
		}

		/*
		[HttpGet]
		public async Task<IActionResult> Buscar(string numeroPedido, string entregador, string dataInicio, string dataFim)
		{
			GetEntregadores();

			// Tratamento de nuâncias:
			int idEntregador = int.TryParse(entregador, out int entregadorInt) ? entregadorInt : 0;

			DateTime? dtaInicio = DateTime.TryParse(dataInicio, out DateTime parsedDataInicio) ? parsedDataInicio : (DateTime?)null;
			DateTime? dtaFim = DateTime.TryParse(dataFim, out DateTime parsedDataFim) ? parsedDataFim : (DateTime?)null;

			DateTime inicio = dtaInicio ?? new DateTime(1800, 1, 1);
			DateTime fim = dtaFim ?? DateTime.Now;

			// Tratamento de valores:
			if (fim < inicio)
			{
				TempData["MensagemErro"] = "A data de fim não pode ser menor que a data de início.";
				return RedirectToAction("Index");
			}

			// Executa a busca:
			try
			{
				List<Pedido> pedidos = await _pedidoFacedeServices.GetPedidosFiltrosAsync(inicio, fim, idEntregador, numeroPedido);

				if (pedidos.Any())
				{
					TempData["MensagemSucesso"] = "Dados encontrados.";
					return View(_pathIndex, pedidos);
				}
				else
				{
					TempData["MensagemErro"] = "Não há pedidos lançados com os filtros aplicados.";
					return View(_pathIndex, new List<Pedido>());
				}
			}
			catch (Exception ex)
			{
				TempData["MensagemErro"] = "Não foi possível acessar o banco de dados, erro: " + ex.Message;
				return RedirectToAction("Index");
			}
		}

		[HttpGet]
		[Route("relatorio-entregas/detalhes/{id}")]
		public async Task<IActionResult> Detalhes(int id)
		{
			try
			{
				// Buscar os detalhes do pedido pelo ID
				Pedido pedido = await _pedidoFacedeServices.MapearPedidoAsync(id);

				if (pedido == null)
				{
					TempData["MensagemErro"] = "Pedido não encontrado.";
					return RedirectToAction("Index");
				}

				return View(_pathDetalhes, pedido);
			}
			catch (Exception ex)
			{
				TempData["MensagemErro"] = "Ocorreu um erro ao tentar buscar os detalhes do pedido: " + ex.Message;
				return RedirectToAction("Index");
			}
		}

		[HttpGet]
		[Route("relatorio-entregas/imprimir")]
		public async Task<IActionResult> Imprimir(string numeroPedido, string entregador, string dataInicio, string dataFim)
		{
			// Obter os filtros e fazer a busca
			int idEntregador = int.TryParse(entregador, out int entregadorInt) ? entregadorInt : 0;

			DateTime? dtaInicio = DateTime.TryParse(dataInicio, out DateTime parsedDataInicio) ? parsedDataInicio : (DateTime?)null;
			DateTime? dtaFim = DateTime.TryParse(dataFim, out DateTime parsedDataFim) ? parsedDataFim : (DateTime?)null;

			DateTime inicio = dtaInicio ?? new DateTime(1800, 1, 1);
			DateTime fim = dtaFim ?? DateTime.Now;

			// Realizar a busca com base nos filtros
			List<Pedido> pedidos = await _pedidoFacedeServices.GetPedidosFiltrosAsync(inicio, fim, idEntregador, numeroPedido);

			// Armazenar os filtros no ViewBag para passar para a view de impressão
			ViewBag.NumeroPedido = numeroPedido;
			ViewBag.Entregador = entregador;
			ViewBag.DataInicio = dataInicio;
			ViewBag.DataFim = dataFim;

			if (idEntregador != 0)
			{
				Usuario usuario = _usuarioServices.GetUsuarioId(idEntregador);
				ViewBag.NomeEntregador = usuario?.Nome ?? "Entregador não encontrado";
			}

			// Retornar a view de impressão com os dados e filtros
			return View("~/Views/Relatorios/Entregas/Imprimir.cshtml", pedidos);
		}

		*/

		// POPULAR VIEW BAGS
		public void GetEntregadores()
		{
			List<Usuario> entregadores = _usuarioServices.ListaEntregadores();
			if (entregadores == null || !entregadores.Any())
			{
				entregadores = new List<Usuario> { new Usuario { Id = 0, Nome = "SEM ENTREGADOR" } };
			}

			ViewBag.Entregadores = entregadores;
		}
	}
}
