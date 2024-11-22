using Leaf.Models.Domain;
using Leaf.Models.ViewModels;
using Leaf.Services.Producao;
using Leaf.Services.Facede;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Leaf.Services.Materiais;
using Leaf.Models.DomainLog;

namespace Leaf.Controllers.Relatorios
{
	[Authorize]
	public class RelatorioProducaoController : Controller
	{
		private readonly string _pathIndex = "~/Views/Relatorios/Producao/Index.cshtml";
		private readonly string _pathDetalhes = "~/Views/Relatorios/Producao/Detalhes.cshtml";
		private readonly string _pathImprimir = "~/Views/Relatorios/Producao/Imprimir.cshtml";

		private readonly LoteProcucaoFacedeServices _loteProcucaoFacedeServices;
		private readonly ProdutoServices _produtoServices;
		

		public RelatorioProducaoController(LoteProcucaoFacedeServices loteProcucaoFacedeServices, ProdutoServices produtoServices)
		{
			_produtoServices = produtoServices;
			_loteProcucaoFacedeServices = loteProcucaoFacedeServices;
		}

		[Route("relatorio-producao")]
		public IActionResult Index()
		{
			try
			{
				GetProdutos();
				return View(_pathIndex);
			}
			catch (Exception ex)
			{
				TempData["MensagemErro"] = "Erro ao carregar a página de relatórios de produção: " + ex.Message;
				return RedirectToAction("Index");
			}
		}

		[HttpGet]
		public async Task<IActionResult> Buscar(string numeroLote, string estagio, string produto, string dataInicio, string dataFim)
		{
			// Popula produtos
			GetProdutos();

			// Converte os parâmetros para os tipos corretos
			var (idLote, idProduto, estagioProducao, inicio, fim) = ConverterParametrosBusca(numeroLote, estagio, produto, dataInicio, dataFim);

			if (fim < inicio)
			{
				TempData["MensagemErro"] = "A data de fim não pode ser menor que a data de início.";
				return RedirectToAction("Index");
			}

			try
			{
				// Listar lotes com base nos filtros
				List<LoteProducaoViewModel> lotes = await _loteProcucaoFacedeServices.GetLotesFiltros(inicio, fim, idProduto, idLote, estagioProducao);

				if (lotes.Any())
				{
					ViewBag.NumeroLote = numeroLote;
					ViewBag.Estagio = estagio;
					ViewBag.Produto = produto;
					ViewBag.DataInicio = dataInicio;
					ViewBag.DataFim = dataFim;

					TempData["MensagemSucesso"] = "Dados encontrados.";
					return View(_pathIndex, lotes);
				}
				else
				{
					TempData["MensagemErro"] = "Não há lotes lançados com os filtros aplicados.";
					return View(_pathIndex, new List<LoteProducaoViewModel>());
				}
			}
			catch (Exception ex)
			{
				TempData["MensagemErro"] = "Erro ao buscar lotes de produção: " + ex.Message;
				return RedirectToAction("Index");
			}
		}

		[HttpGet]
		[Route("relatorio-producao/imprimir")]
		public async Task<IActionResult> Imprimir(string numeroLote, string estagio, string produto, string dataInicio, string dataFim)
		{
			var (idLote, idProduto, estagioProducao, inicio, fim) = ConverterParametrosBusca(numeroLote, estagio, produto, dataInicio, dataFim);

			try
			{
				List<LoteProducaoViewModel> loteViewModel = await _loteProcucaoFacedeServices.GetLotesFiltros(inicio, fim, idProduto, idLote, estagioProducao);

				// Armazena os filtros
				ArmazenarFiltros(numeroLote, estagio, produto, dataInicio, dataFim);

				return View(_pathImprimir, loteViewModel);
			}
			catch (Exception ex)
			{
				TempData["MensagemErro"] = "Erro ao tentar imprimir o relatório: " + ex.Message;
				return RedirectToAction("Index");
			}
		}

		[HttpGet]
		[Route("relatorio-producao/detalhes/{id}")]
		public async Task<IActionResult> Detalhes(string id)
		{
			try
			{
				// Buscar os detalhes do lote pelo ID
				LoteProducaoViewModel loteProducaoViewModel = await _loteProcucaoFacedeServices.GetLote(id);

				if (loteProducaoViewModel == null)
				{
					TempData["MensagemErro"] = "Lote não encontrado.";
					return RedirectToAction("Index");
				}

				return View(_pathDetalhes, loteProducaoViewModel);
			}
			catch (Exception ex)
			{
				TempData["MensagemErro"] = "Ocorreu um erro ao tentar buscar os detalhes do lote: " + ex.Message;
				return RedirectToAction("Index");
			}
		}

        [HttpGet]
        [Route("relatorio-producao/detalhesLog")]
        public JsonResult LogProducao(string lote)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(lote))
                {
                    return Json(new { Response = false, Message = "O identificador do lote não foi informado." });
                }

                var logs = _loteProcucaoFacedeServices.GetProducaoLog(lote);

                if (logs == null || !logs.Any())
                {
                    return Json(new { Response = true, Logs = new List<object>(), Message = "Nenhum registro encontrado para o lote informado." });
                }

                // Retorna os logs formatados para o JSON
                var response = logs.Select(log => new
                {
                    log.IdLog,
                    log.Lote,
                    QtdeAntiga = log.QtdeAntiga,
                    QtdeNova = log.QtdeNova,
                    Usuario = new
                    {
                        Nome = log.Usuario?.Nome ?? "Não informado"
                    },
                    DtaAtualizacao = log.DtaAlteracao?.ToString("yyyy-MM-ddTHH:mm:ss")
                });

                return Json(response);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = "Erro ao buscar registros deste lote: " + ex.Message;
                return Json(new { Response = false, Message = "Erro ao buscar log.", Error = ex.Message });
            }
        }


        // Método para converter os parâmetros da busca
        public (string idLote, int idProduto, int estagioProducao, DateTime inicio, DateTime fim) ConverterParametrosBusca(string numeroLote, string estagio, string produto, string dataInicio, string dataFim)
		{
			string idLote = numeroLote;
			int idProduto = int.TryParse(produto, out int produtoInt) ? produtoInt : 0;
			int estagioProducao = int.TryParse(estagio, out int estagioInt) ? estagioInt : 0;

			DateTime? dtaInicio = DateTime.TryParse(dataInicio, out DateTime parsedDataInicio) ? parsedDataInicio : null;
			DateTime? dtaFim = DateTime.TryParse(dataFim, out DateTime parsedDataFim) ? parsedDataFim : null;

			DateTime inicio = dtaInicio ?? new DateTime(1800, 1, 1);
			DateTime fim = dtaFim ?? DateTime.Now;

			return (idLote, idProduto, estagioProducao, inicio, fim);
		}

		// Método para armazenar os filtros no ViewBag para passar para a view de impressão
		private void ArmazenarFiltros(string numeroLote, string estagio, string produto, string dataInicio, string dataFim)
		{
			ViewBag.NumeroLote = numeroLote;
			ViewBag.Estagio = estagio;
			ViewBag.Produto = produto;
			ViewBag.DataInicio = dataInicio;
			ViewBag.DataFim = dataFim;
		}

		// Método para popular os produtos na ViewBag
		private void GetProdutos()
		{
			try
			{
				List<Produto> produtos = _produtoServices.ListarProdutos();
				if (produtos == null || !produtos.Any())
				{
					produtos = new List<Produto> { new Produto { IdProduto = 0, Descricao = "SEM PRODUTO" } };
				}

				ViewBag.Produtos = produtos;
			}
			catch (Exception ex)
			{
				TempData["MensagemErro"] = "Erro ao carregar a lista de produtos: " + ex.Message;
			}
		}
	}
}
