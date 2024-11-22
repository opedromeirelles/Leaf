using Leaf.Services.Materiais;
using Leaf.Services.Agentes;
using Leaf.Models.Domain;
using Leaf.Services.Producao;
using Leaf.Models.ViewModels;
using Leaf.Models.ItensDomain;
using Microsoft.Identity.Client;
using Leaf.Models.DomainLog;

namespace Leaf.Services.Facede
{
	public class LoteProcucaoFacedeServices
	{
		private readonly ProdutoServices _produtoServices;
		private readonly InsumoServices _insumoServices;

		private readonly UsuarioServices _usuarioServices;

		private readonly ItemLoteProducaoServices _itemLoteProducaoServices;
		private readonly LoteProducaoServices _loteProducaoServices;

		private readonly LoteProducaoLogServices _loteProducaoLogServices;


		public LoteProcucaoFacedeServices(InsumoServices insumoServices, ProdutoServices produtoServices, UsuarioServices usuarioServices, LoteProducaoServices loteProducaoServices, ItemLoteProducaoServices itemLoteProducaoServices, LoteProducaoLogServices loteProducaoLogServices)
        {
			_itemLoteProducaoServices = itemLoteProducaoServices;
			_insumoServices = insumoServices;
			_loteProducaoServices = loteProducaoServices;
			_usuarioServices = usuarioServices;
			_produtoServices = produtoServices;
			_loteProducaoLogServices = loteProducaoLogServices;

		}

		private async Task<LoteProducaoViewModel> MapearLote(string idLote)
		{
			// Buscar lote:
			if (string.IsNullOrEmpty(idLote))
			{
				return new LoteProducaoViewModel();
			}

			// Populando o Lote
			LoteProducao loteProducao = await _loteProducaoServices.GetLoteProducao(idLote);
			loteProducao.Usuario = _usuarioServices.GetUsuarioId(loteProducao.IdUsuario);
			loteProducao.Produto = _produtoServices.GetProduto(loteProducao.IdProduto);

			// Populando os itens do lote
			List<ItemLoteProducao> itensLoteProducao = _itemLoteProducaoServices.GetItensLote(idLote);

			// Criando o ViewModel e populando com os dados mapeados
			LoteProducaoViewModel loteView = new LoteProducaoViewModel
			{
				LoteProducao = loteProducao,
				ItemLote = itensLoteProducao,
				InsumosLote = new List<Insumo>()
			};

			// Adicionando insumos ao view model
			foreach (var insumoLote in loteView.ItemLote)
			{
				Insumo insumo = _insumoServices.GetInsumo(insumoLote.IdInsumo);
				if (insumo != null)
				{
					loteView.InsumosLote.Add(insumo);
				}
				else
				{
					loteView.InsumosLote.Add(new Insumo { IdInsumo = 0, Descricao = "INSUMO NAO ENCONTRADO" });
				}
			}

			return loteView ?? new LoteProducaoViewModel();
		}

		public async Task<LoteProducaoViewModel> GetLote(string idLote)
		{
			if (string.IsNullOrEmpty(idLote))
			{
				return new LoteProducaoViewModel();
			}

			LoteProducaoViewModel loteProducaoViewModel = await MapearLote(idLote);

			return loteProducaoViewModel ?? new LoteProducaoViewModel();

		}

		


		//Relatorio de lotes
		public async Task<List<LoteProducaoViewModel>> GetLotesFiltros(DateTime dataInicio, DateTime dataFim, int idProduto, string idLote, int estagio)
		{
			try
			{
				List<LoteProducaoViewModel> lotesView = new List<LoteProducaoViewModel>();

				List<LoteProducao> loteProducaoFiltrado = await _loteProducaoServices.GetLoteProducaoPeriodo(dataInicio, dataFim, idProduto, idLote, estagio);

				foreach (var lote in loteProducaoFiltrado)
				{
					LoteProducaoViewModel loteProducaoViewModel = await MapearLote(lote.IdLote);
					lotesView.Add(loteProducaoViewModel);
				}

				return lotesView.Any() ? lotesView : new List<LoteProducaoViewModel>();
				
			}
			catch (Exception ex)
			{

				throw new Exception("Não foi possivel filtrar os dados, erro: " + ex.Message);
			}
			
		}

		// Capturando log de produção
		public List<LoteProducaoLog> GetProducaoLog(string lote)
		{
            List<LoteProducaoLog> loteProducaoLogs = new List<LoteProducaoLog>();
			try
			{
				loteProducaoLogs = _loteProducaoLogServices.GetLoteProducaoLog(lote);
                foreach (var log in loteProducaoLogs)
                {
					log.Usuario = _usuarioServices.GetUsuarioId(log.IdUsuario);
                }
            }
			catch (Exception ex)
			{

				throw new Exception("Não foi possivel retornar o log do lote de produção. " + ex.Message);
			}

			return loteProducaoLogs;
		}

	}
}
