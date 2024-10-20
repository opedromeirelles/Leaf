using Leaf.Services;
using Leaf.Services.Facede;
using Leaf.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Leaf.Models.ViewModels;

namespace Leaf.Controllers
{
    public class RelatorioComprasController : Controller
    {
        private readonly string _pathIndex = "~/Views/Relatorios/Compras/Index.cshtml";
		private readonly string _pathDetalhes = "~/Views/Relatorios/Compras/Detalhes.cshtml";

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
			GetAdministrativos();
			GetFornecedores();

			return View(_pathIndex);
        }

        [HttpGet]
        public async Task<IActionResult> Buscar(string numeroCompra, string status, string dataInicio, string dataFim, string fornecedor, string solicitante)
        {
			GetAdministrativos();
			GetFornecedores();

            //Tratamente de nuancias:
			int idFornecedor = int.TryParse(fornecedor, out int fornecedorInt) ? fornecedorInt : 0;
			int idUsuario = int.TryParse(solicitante, out int solicitanteInt) ? solicitanteInt : 0;

			DateTime? dtaInicio = DateTime.TryParse(dataInicio, out DateTime parsedDataInicio) ? parsedDataInicio : (DateTime?)null;
            DateTime? dtaFim = DateTime.TryParse(dataFim, out DateTime parsedDataFim) ? parsedDataFim : (DateTime?)null;

			DateTime inicio = dtaInicio ?? new DateTime(1800, 1, 1);
			DateTime fim = dtaFim ?? DateTime.Now;

            //Tratamento de valores:
            if (fim < inicio)
            {
                TempData["MensagemErro"] = "A data de fim não pode ser menor que a data de início.";
                return RedirectToAction("Index");
            }


            //Executa a busca:
            try
            {
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

		[HttpGet]
		[Route("relatorio-compras/imprimir")]
		public async Task<IActionResult> Imprimir(string numeroCompra, string status, string dataInicio, string dataFim, string fornecedor, string solicitante)
		{
			// Obter os filtros e fazer a busca
			int idFornecedor = int.TryParse(fornecedor, out int fornecedorInt) ? fornecedorInt : 0;
			int idUsuario = int.TryParse(solicitante, out int solicitanteInt) ? solicitanteInt : 0;


			DateTime? dtaInicio = DateTime.TryParse(dataInicio, out DateTime parsedDataInicio) ? parsedDataInicio : (DateTime?)null;
			DateTime? dtaFim = DateTime.TryParse(dataFim, out DateTime parsedDataFim) ? parsedDataFim : (DateTime?)null;

			DateTime inicio = dtaInicio ?? new DateTime(1800, 1, 1);
			DateTime fim = dtaFim ?? DateTime.Now;

			// Realizar a busca com base nos filtros
			List<CompraViewModel> compraViewModels = await _compraFacedeServices.GetComprasFiltrosAsync(inicio, fim, idFornecedor, status, numeroCompra, idUsuario);

			// Armazenar os filtros no ViewBag para passar para a view de impressão
			ViewBag.NumeroCompra = numeroCompra;
			ViewBag.Status = status;
			ViewBag.DataInicio = dataInicio;
			ViewBag.DataFim = dataFim;
			ViewBag.Fornecedor = fornecedor;
			ViewBag.Solicitante = solicitante;

			if (idFornecedor != 0)
			{
                Pessoa pessoa = _pessoaServices.GetPessoa(idFornecedor);
                ViewBag.NomeFornecedor = pessoa.Nome;
            }
			

            if (idUsuario != 0)
			{
				Usuario usuario = _usuarioServices.GetUsuarioId(idUsuario);
				ViewBag.NomeSolicitante = usuario.Nome;

            }

			// Retornar a view de impressão com os dados e filtros
			return View("~/Views/Relatorios/Compras/Imprimir.cshtml", compraViewModels);
		}



		// POPULAR VIEW BAGS
		public void GetFornecedores()
		{
			List<Pessoa> fornecedores = _pessoaServices.GetFornecedores();
			if (fornecedores == null || !fornecedores.Any())
			{
				fornecedores = new List<Pessoa> { new Pessoa { IdPessoa = 0, Nome = "SEM FORNECEDOR" } };
			}

			ViewBag.Fornecedores = fornecedores;
		}

		public void GetAdministrativos()
		{
			List<Usuario> usuarios = _usuarioServices.ListaAdministrativo();
			if (usuarios == null || !usuarios.Any())
			{
				usuarios = new List<Usuario> { new Usuario { Id = 0, Nome = "SEM ADMINISTRATIVO" } };
			}

			ViewBag.Administrativo = usuarios;
		}


	}
}
