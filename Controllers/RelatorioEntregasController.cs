using Leaf.Services;
using Leaf.Services.Facede;
using Leaf.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Leaf.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Leaf.Controllers
{
    [Authorize]
    public class RelatorioEntregasController : Controller
	{
		private readonly string _pathIndex = "~/Views/Relatorios/Entregas/Index.cshtml";
		private readonly string _pathDetalhes = "~/Views/Relatorios/Entregas/Detalhes.cshtml";
        private readonly string _pathImprimir = "~/Views/Relatorios/Entregas/Imprimir.cshtml";


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


        [HttpGet]
        public async Task<IActionResult> Buscar(string numeroPedido, string entregador, string dataInicio, string dataFim)
        {
			//Pupular entregadores
            GetEntregadores();

			//Converter os parametros para o tipo certo
            var (idPedido, idEntregador, inicio, fim) = ConverterParametrosBusca(numeroPedido, entregador, dataInicio, dataFim);

            if (fim < inicio)
            {
                TempData["MensagemErro"] = "A data de fim não pode ser menor que a data de início.";
                return RedirectToAction("Index");
            }

            try
            {
				//Listar pedidos ja com filtro
                List<PedidoViewModel> pedidos = await _pedidoFacedeServices.GetRelatorioEntregas(inicio, fim, idEntregador, idPedido);

                if (pedidos.Any())
                {
					// Armazena os filtros
					ViewBag.NumeroPedido = numeroPedido;
					ViewBag.Entregador = entregador;
					ViewBag.DataInicio = dataInicio;
					ViewBag.DataFim = dataFim;

					TempData["MensagemSucesso"] = "Dados encontrados.";
                    return View(_pathIndex, pedidos);
                }
                else
                {
                    TempData["MensagemErro"] = "Não há pedidos lançados com os filtros aplicados.";
                    return View(_pathIndex, new List<PedidoViewModel>());
                }
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = "Não foi possível acessar o banco de dados, erro: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Route("relatorio-entregas/imprimir")]
        public async Task<IActionResult> Imprimir(string numeroPedido, string entregador, string dataInicio, string dataFim)
        {
            var (idPedido, idEntregador, inicio, fim) = ConverterParametrosBusca(numeroPedido, entregador, dataInicio, dataFim);

            List<PedidoViewModel> pedidoViewModel = await _pedidoFacedeServices.GetRelatorioEntregas(inicio, fim, idEntregador, idPedido);


            //Retorna os filtros:
            ViewBag.NumeroPedido = numeroPedido;
            ViewBag.Entregador = entregador;
            ViewBag.DataInicio = dataInicio;
            ViewBag.DataFim = dataFim;


            if (idEntregador != 0)
            {
                Usuario usuario = _usuarioServices.GetUsuarioId(idEntregador);
                ViewBag.NomeEntregador = usuario?.Nome ?? "Entregador não encontrado";
            }

            return View(_pathImprimir, pedidoViewModel);
        }


        [HttpGet]
		[Route("relatorio-entregas/detalhes/{id}")]
		public async Task<IActionResult> Detalhes(int id)
		{
			try
			{
				// Buscar os detalhes do pedido pelo ID
				PedidoViewModel pedidoViewModel = await _pedidoFacedeServices.GetPedidoAsync(id);

				if (pedidoViewModel == null)
				{
					TempData["MensagemErro"] = "Pedido não encontrado.";
					return RedirectToAction("Index");
				}

				return View(_pathDetalhes, pedidoViewModel);
			}
			catch (Exception ex)
			{
				TempData["MensagemErro"] = "Ocorreu um erro ao tentar buscar os detalhes do pedido: " + ex.Message;
				return RedirectToAction("Index");
			}
		}


        //Converter parametros:
        public (int idPedido, int idEntregador, DateTime inicio, DateTime fim) ConverterParametrosBusca(string numeroPedido, string entregador, string dataInicio, string dataFim)
        {
            int idEntregador = int.TryParse(entregador, out int entregadorInt) ? entregadorInt : 0;
            int idPedido = int.TryParse(numeroPedido, out int pedidoInt) ? pedidoInt : 0;

            DateTime? dtaInicio = DateTime.TryParse(dataInicio, out DateTime parsedDataInicio) ? parsedDataInicio : (DateTime?)null;
            DateTime? dtaFim = DateTime.TryParse(dataFim, out DateTime parsedDataFim) ? parsedDataFim : (DateTime?)null;

            DateTime inicio = dtaInicio ?? new DateTime(1800, 1, 1);
            DateTime fim = dtaFim ?? DateTime.Now;

            return (idPedido, idEntregador, inicio, fim);
        }


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
