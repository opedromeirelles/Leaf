using Leaf.Models.Domain;
using Leaf.Models.ViewModels.Json;
using Leaf.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        public IActionResult Index()
        {
           
            return View();
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
