using Leaf.Models.Domain;
using Leaf.Models.ItensDomain;
using Leaf.Models.ViewModels;

namespace Leaf.Services.Facede
{
	public class PedidoFacedeServices
	{
		//Serviços
		private readonly PedidoServices _pedidoService;
		private readonly ItemPedidoServices _itemPedidoService;
		private readonly UsuarioServices _usuarioService;
		private readonly PessoaServices _pessoaService;
		private readonly ProdutoServices _produtoService;

		//Injection de services
		public PedidoFacedeServices(PedidoServices pedidoServices, ItemPedidoServices itemPedidoServices, UsuarioServices usuarioServices, PessoaServices pessoaServices, ProdutoServices produtoServices)
		{
			_pedidoService = pedidoServices;
			_itemPedidoService = itemPedidoServices;
			_usuarioService = usuarioServices;
			_pessoaService = pessoaServices;
			_produtoService = produtoServices;
		}

		//Mapear Pedido
		private async Task<PedidoViewModel> MapearPedidoAsync(int idPedido)
		{
			if (idPedido <= 0)
			{
				return new PedidoViewModel();
			}

			//Busco o pedido no banco
			Pedido pedido = await _pedidoService.GetPedido(idPedido);
			if (pedido == null)
			{
				return new PedidoViewModel();
			}

			//Alimento meu objeto com as informações estabelecidas no pedido
			PedidoViewModel pedidoViewModel = new PedidoViewModel
			{
				IdPedido = pedido.IdPedido,
				Pedido = pedido,
				Enregador = _usuarioService.GetUsuarioId(pedido.IdEntregador),
				Vendedor = _usuarioService.GetUsuarioId(pedido.IdVendedor),
				Cliente = _pessoaService.GetPessoa(pedido.IdPessoa),
				ItensPedido = _itemPedidoService.GetItensPedido(pedido.IdPedido)
			};

			//Alimento os itens do meu pedido
			foreach (var itemPedido in pedidoViewModel.ItensPedido)
			{

				itemPedido.Produto = _produtoService.GetProduto(itemPedido.IdProduto);

				if (itemPedido.Produtos == null)
				{
					itemPedido.Produtos = new List<Produto>();
				}
				itemPedido.Produtos.Add(itemPedido.Produto);
			}

			//Mapear Tempo médio:
			pedidoViewModel = TempoMedioPedido(pedidoViewModel);

			return pedidoViewModel;
		}

		private async Task<List<PedidoViewModel>> MapearListaPedidoAsync(List<Pedido> pedidos)
		{
			List<PedidoViewModel> pedidoViewModels = new List<PedidoViewModel>();

			if (pedidos != null && pedidos.Any())
			{
				foreach (var pedido in pedidos)
				{
					PedidoViewModel pedidoView = await MapearPedidoAsync(pedido.IdPedido);
					pedidoViewModels.Add(pedidoView);
				}
			}

			return pedidoViewModels ?? new List<PedidoViewModel>();
		}

		//Calular tempo medio de Pedido
		private PedidoViewModel TempoMedioPedido(PedidoViewModel pedidosView)
		{
			if (pedidosView != null)
			{
				if (pedidosView.Pedido.DtaEntrega.HasValue && pedidosView.Pedido.DtaSaida.HasValue)
				{
					TimeSpan diferenca = pedidosView.Pedido.DtaEntrega.Value - pedidosView.Pedido.DtaSaida.Value;

					if (diferenca.TotalHours >= 1)
					{
						pedidosView.TempoMedioEntrega = diferenca.TotalHours;
						pedidosView.TempoDescricao = "horas";
					}
					else
					{
						pedidosView.TempoMedioEntrega = diferenca.TotalMinutes;
						pedidosView.TempoDescricao = "minutos";
					}
				}

				return pedidosView;
			}
			return pedidosView;



		}


		//GETS PEDIDOS:
		public async Task<PedidoViewModel> GetPedidoAsync(int idPedido)
		{
			PedidoViewModel pedidoViewModel = new PedidoViewModel();

			if (idPedido != 0)
			{
				pedidoViewModel = await MapearPedidoAsync(idPedido);
			}

			return pedidoViewModel ?? new PedidoViewModel();
		}

		public async Task<List<PedidoViewModel>> GetPedidosAsync()
		{
			List<PedidoViewModel> pedidoViewModels = new List<PedidoViewModel>();
			List<Pedido> pedidos = _pedidoService.GetPedidos();

			if (pedidos != null && pedidos.Any())
			{
				pedidoViewModels = await MapearListaPedidoAsync(pedidos);
			}

			return pedidoViewModels ?? new List<PedidoViewModel>();


		}



		//Filtro de Numero do pedido e Status - View Index de Pedido
		public async Task<List<PedidoViewModel>> GetPedidoFiltroAsync(int idPedido, string status)
		{
			List<PedidoViewModel> pedidoViewModels = new List<PedidoViewModel>();
			List<Pedido> pedidos = _pedidoService.GetPedidosFiltro(idPedido, status);
			if (pedidos != null && pedidos.Any())
			{
				pedidoViewModels = await MapearListaPedidoAsync(pedidos);
			}

			return pedidoViewModels ?? new List<PedidoViewModel>();
		}


		//Relatório de pedidos:

		//Entregas
		public async Task<List<PedidoViewModel>> GetRelatorioEntregas(DateTime dataInicio, DateTime DataFim, int idEntregador, int idPedido)
		{
			
			List<PedidoViewModel> pedidoViewModels = new List<PedidoViewModel>();
			List<Pedido> pedidos = await _pedidoService.GetPedidosFiltroAsync(dataInicio, DataFim, idEntregador, idPedido);
			if (pedidos != null && pedidos.Any())
			{
				pedidoViewModels = await MapearListaPedidoAsync(pedidos);
			}

			return pedidoViewModels ?? new List<PedidoViewModel>();
		}

		//Vendas
		public async Task<List<PedidoViewModel>> GetRelatorioVendas(DateTime dataInicio, DateTime DataFim, int idVendedor, int idPedido, string status)
		{

			List<PedidoViewModel> pedidoViewModels = new List<PedidoViewModel>();
			List<Pedido> pedidos = await _pedidoService.GetPedidosFiltroAsync(dataInicio, DataFim, idVendedor, idPedido, status);
			if (pedidos != null && pedidos.Any())
			{
				pedidoViewModels = await MapearListaPedidoAsync(pedidos);
			}

			return pedidoViewModels ?? new List<PedidoViewModel>();
		}




		//Atualizar Pedido tratativas de regra de negocio
		public List<Produto> AtualizarPedido(PedidoViewModel pedido, int idEntregador)
		{

			//Para cada item do meu pedido, preciso verificar se há a quantidade em estoque para fazer a liberação:


			List<Produto> produtosNegativos = new List<Produto>(); //listar os produtos negativos em minha View

			foreach (var item in pedido.ItensPedido)
			{
				//Capto a quantidade daquele produto em estoque
				int quantidadeEstoque = _produtoService.GetQuantidadeEstoque(item.IdProduto);

				//Verifico se a quantidade solicidada é maior do que tenho em estoque
				if (item.Quantidade > quantidadeEstoque)
				{
					// se verdaderio adiciona o produto em minha lista
					produtosNegativos.Add(_produtoService.GetProduto(item.IdProduto));
				}
			}

			//Após o laço de repetição verificando produto a produto, verifico se há algo em minha lista
			if (produtosNegativos != null && produtosNegativos.Any())
			{
				return produtosNegativos; // Passo para minha view os produtos negativos
			}
			else if (_pedidoService.AtualizaStatusPedido(pedido.IdPedido, idEntregador)) // Vejo se foi possivel atualizar
			{
				return new List<Produto>(); // Retorno lista vazia
			}

			return produtosNegativos; // Passo para minha view os produtos negativos
		}


	}
}
