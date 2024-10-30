using Leaf.Services.Facede;
using Leaf.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Leaf.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Leaf.Services.Agentes;

namespace Leaf.Controllers.Relatorios
{
	[Authorize]
	public class RelatorioComprasController : Controller
	{
		private readonly string _pathIndex = "~/Views/Relatorios/Compras/Index.cshtml";
		private readonly string _pathDetalhes = "~/Views/Relatorios/Compras/Detalhes.cshtml";
		private readonly string _pathImprimir = "~/Views/Relatorios/Compras/Imprimir.cshtml";

		private readonly PessoaServices _pessoaServices;
		private readonly UsuarioServices _usuarioServices;
		private readonly CompraFacedeServices _compraFacedeServices;

		public RelatorioComprasController(PessoaServices pessoaServices, CompraFacedeServices compraFacede, UsuarioServices usuarioServices)
		{
			_pessoaServices = pessoaServices;
			_compraFacedeServices = compraFacede;
			_usuarioServices = usuarioServices;
		}

		[Route("relatorio-compras")]
		public IActionResult Index()
		{
			try
			{
				GetAdministrativos();
				GetFornecedores();
				return View(_pathIndex);
			}
			catch (Exception ex)
			{
				TempData["MensagemErro"] = "Erro ao carregar a página de relatórios de compras: " + ex.Message;
				return RedirectToAction("Index");
			}
		}

		[HttpGet]
		public async Task<IActionResult> Buscar(string numeroCompra, string status, string dataInicio, string dataFim, string fornecedor, string solicitante)
		{
			// Popula administrativos e fornecedores
			GetAdministrativos();
			GetFornecedores();

			// Trata o número da compra: se estiver vazio ou inválido, define como 0
			int idCompra = string.IsNullOrWhiteSpace(numeroCompra) ? 0 : (int.TryParse(numeroCompra, out int compraInt) ? compraInt : 0);
			if (idCompra == 0)
			{
				TempData["MensagemErro"] = "Número da conta incorreto.";
				return RedirectToAction("Index");
			}

			// Tratamento de parâmetros
			var (idFornecedor, idUsuario, inicio, fim) = ConverterParametrosBuscaFornecedorUsuario(fornecedor, solicitante, dataInicio, dataFim);

			if (fim < inicio)
			{
				TempData["MensagemErro"] = "A data de fim não pode ser menor que a data de início.";
				return RedirectToAction("Index");
			}

			try
			{
				// Listar compras com filtro
				List<CompraViewModel> compraViewModels = await _compraFacedeServices.GetComprasFiltrosAsync(inicio, fim, idFornecedor, status, numeroCompra, idUsuario);

				if (compraViewModels.Any())
				{
					TempData["MensagemSucesso"] = "Dados encontrados.";
					return View(_pathIndex, compraViewModels);
				}
				else
				{
					TempData["MensagemErro"] = "Não há compras lançadas com os filtros aplicados.";
					return View(_pathIndex, new List<CompraViewModel>());
				}
			}
			catch (Exception ex)
			{
				TempData["MensagemErro"] = "Não foi possível acessar o banco de dados, erro: " + ex.Message;
				return RedirectToAction("Index");
			}
		}

		[HttpGet]
		[Route("relatorio-compras/imprimir")]
		public async Task<IActionResult> Imprimir(string numeroCompra, string status, string dataInicio, string dataFim, string fornecedor, string solicitante)
		{
			// Trata o número da compra: se estiver vazio ou inválido, define como 0
			int idCompra = string.IsNullOrWhiteSpace(numeroCompra) ? 0 : (int.TryParse(numeroCompra, out int compraInt) ? compraInt : 0);
			if (idCompra == 0)
			{
				TempData["MensagemErro"] = "Número da conta incorreto.";
				return RedirectToAction("Index");
			}


			// Obter os filtros e realizar a busca
			var (idFornecedor, idUsuario, inicio, fim) = ConverterParametrosBuscaFornecedorUsuario(fornecedor, solicitante, dataInicio, dataFim);

			try
			{
				List<CompraViewModel> compraViewModels = await _compraFacedeServices.GetComprasFiltrosAsync(inicio, fim, idFornecedor, status, numeroCompra, idUsuario);

				// Armazenar os filtros no ViewBag
				ViewBag.NumeroCompra = numeroCompra;
				ViewBag.Status = status;
				ViewBag.DataInicio = dataInicio;
				ViewBag.DataFim = dataFim;
				ViewBag.Fornecedor = fornecedor;
				ViewBag.Solicitante = solicitante;

				if (idFornecedor != 0)
				{
					Pessoa pessoa = _pessoaServices.GetPessoa(idFornecedor);
					ViewBag.NomeFornecedor = pessoa?.Nome ?? "Fornecedor não encontrado";
				}

				if (idUsuario != 0)
				{
					Usuario usuario = _usuarioServices.GetUsuarioId(idUsuario);
					ViewBag.NomeSolicitante = usuario?.Nome ?? "Solicitante não encontrado";
				}

				return View(_pathImprimir, compraViewModels);
			}
			catch (Exception ex)
			{
				TempData["MensagemErro"] = "Erro ao tentar imprimir o relatório: " + ex.Message;
				return RedirectToAction("Index");
			}
		}

		[HttpGet]
		[Route("relatorio-compras/detalhes/{id}")]
		public async Task<IActionResult> Detalhes(int id)
		{
			try
			{
				// Buscar os detalhes da compra pelo ID
				CompraViewModel compraViewModel = await _compraFacedeServices.MapearCompraAsync(id);

				if (compraViewModel == null)
				{
					TempData["MensagemErro"] = "Compra não encontrada.";
					return RedirectToAction("Index");
				}

				return View(_pathDetalhes, compraViewModel);
			}
			catch (Exception ex)
			{
				TempData["MensagemErro"] = "Ocorreu um erro ao tentar buscar os detalhes da compra: " + ex.Message;
				return RedirectToAction("Index");
			}
		}

		// Popula ViewBags
		public void GetFornecedores()
		{
			try
			{
				List<Pessoa> fornecedores = _pessoaServices.GetFornecedores();
				if (fornecedores == null || !fornecedores.Any())
				{
					fornecedores = new List<Pessoa> { new Pessoa { IdPessoa = 0, Nome = "SEM FORNECEDOR" } };
				}

				ViewBag.Fornecedores = fornecedores;
			}
			catch (Exception ex)
			{
				TempData["MensagemErro"] = "Erro ao carregar a lista de fornecedores: " + ex.Message;
			}
		}

		public void GetAdministrativos()
		{
			try
			{
				List<Usuario> usuarios = _usuarioServices.ListaAdministrativo();
				if (usuarios == null || !usuarios.Any())
				{
					usuarios = new List<Usuario> { new Usuario { Id = 0, Nome = "SEM ADMINISTRATIVO" } };
				}

				ViewBag.Administrativo = usuarios;
			}
			catch (Exception ex)
			{
				TempData["MensagemErro"] = "Erro ao carregar a lista de administrativos: " + ex.Message;
			}
		}

		// Converte parâmetros
		public (int idFornecedor, int idUsuario, DateTime inicio, DateTime fim) ConverterParametrosBuscaFornecedorUsuario(string fornecedor, string solicitante, string dataInicio, string dataFim)
		{
			int idFornecedor = int.TryParse(fornecedor, out int fornecedorInt) ? fornecedorInt : 0;
			int idUsuario = int.TryParse(solicitante, out int solicitanteInt) ? solicitanteInt : 0;

			DateTime? dtaInicio = DateTime.TryParse(dataInicio, out DateTime parsedDataInicio) ? parsedDataInicio : null;
			DateTime? dtaFim = DateTime.TryParse(dataFim, out DateTime parsedDataFim) ? parsedDataFim : null;

			DateTime inicio = dtaInicio ?? new DateTime(1800, 1, 1);
			DateTime fim = dtaFim ?? DateTime.Now;

			return (idFornecedor, idUsuario, inicio, fim);
		}
	}
}
