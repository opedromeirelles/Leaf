using Leaf.Models.Domain;
using Leaf.Models.ViewModels.Json;
using Leaf.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Leaf.Services.Facede;
using Leaf.Services.Agentes;

namespace Leaf.Controllers.Ordens
{
    [Authorize]
    public class CompraController : Controller
    {
        private readonly CompraFacedeServices _compraFacedeServices;

        public CompraController(CompraFacedeServices compraFacedeServices)
        {
            _compraFacedeServices = compraFacedeServices;
        }

        public async Task<IActionResult> Index(List<CompraViewModel> compraViewModels)
        {

             compraViewModels = await _compraFacedeServices.GetCompras();

            if (compraViewModels != null)
            {
                TempData["MensagemSucesso"] = "Compras encontradas.";
            }
            else
            {
                TempData["MensagemErro"] = "Não há compras lançadas";
            }
           
            return View("Index", compraViewModels.Any() ? compraViewModels : new List<CompraViewModel>());
        }

        [HttpGet]
        public async Task<IActionResult> Buscar(string numeroConta, string status)
        {
            int idOc = Convert.ToInt32(numeroConta);

            List<CompraViewModel> compraViewModels = new List<CompraViewModel>();
            try
            {
                compraViewModels = await _compraFacedeServices.GetCompras(status, idOc);
                if (compraViewModels.Any() && compraViewModels != null)
                {
                    TempData["MensagemSucesso"] = "Dados filtrados encontrados.";
                    return View("Index", compraViewModels);
                }
                else
                {
                    TempData["MensagemErro"] = "Não há compras lançadas com os filtros atuais.";
                    return RedirectToAction("Index", compraViewModels);
                }

            }
            catch (Exception ex)
            {

                throw new Exception("Erro ao buscar compras, erro: " + ex.Message);
            }

        }


        [HttpGet]
        public async Task<IActionResult> Atualizar(int id)
        {

            try
            {
                CompraViewModel compra = await _compraFacedeServices.MapearCompraAsync(id);

                if (compra != null)
                {
                    return View(compra);
                }

                TempData["MensagemErro"] = "Não foi possivel trazer os dados da compra";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Não foi possivel acessar a compra, erro: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Imprimir(int id)
        {

            try
            {
                CompraViewModel compra = await _compraFacedeServices.MapearCompraAsync(id);

                if (compra != null && compra.IdCompra != 0)
                {
                    TempData["MensagemSucesso"] = "Dados encontrados";
                    return View(compra);
                }
                else
                {
                    TempData["MensagemErro"] = "Não foi possivel encontrar os dados da compra";
                    return RedirectToAction("Index");
                }

            }
            catch (Exception ex)
            {

                throw new Exception("Erro ao solicitar impressao, erro: " + ex.Message);
            }
        }


        [HttpGet]
        public IActionResult Emitir()
        {
            return View();
        }



        [HttpPost]
        public JsonResult NovaCompra([FromBody] CompraJsonView compra)
        {
            try
            {
                if (compra == null || compra.ItensCompra == null || !compra.ItensCompra.Any())
                {
                    return Json(new { Response = false, Message = "Dados da compra estão incompletos." });
                }

                ProcessarCompraResult resultado = _compraFacedeServices.ProcessarCompra(compra);

                if (resultado.Sucesso)
                {
                    return Json(new { Response = true, Message = resultado.Mensagem ?? "Compra emitida com sucesso." });
                }
                else
                {
                    return Json(new { Response = false, Message = resultado.Mensagem ?? "Erro desconhecido ao emitir compra." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao processar compra:", ex.Message);
                return Json(new { Response = false, Message = "Erro no processamento da compra.", Error = ex.Message });
            }
        }


        [HttpGet]
        public async Task<JsonResult> BuscarPessoa(int id)
        {
            List<Pessoa> pessoas = await _compraFacedeServices.FornecedoresInsumos(id);
            return Json(pessoas);
        }

        [HttpGet]
        public async Task<JsonResult> BuscarInsumo(int id)
        {
            List<Insumo> insumos = await _compraFacedeServices.InsumosFornecedores(id);
            return Json(insumos);
        }



    }
}
