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
                    return Json(new { Response = "Falha ao emitir compra: objeto de compra está vazio ou incompleto." });
                }

                // Supondo que `idPessoa`, `idAdministrativo`, etc., sejam verificados e processados aqui
                // Log para verificar os dados recebidos
                Console.WriteLine($"idPessoa: {compra.IdPessoa}");
                Console.WriteLine($"idAdministrativo: {compra.IdAdministrativo}");
                Console.WriteLine($"valorTotal: {compra.ValorTotal}");
                Console.WriteLine($"itensCompra Count: {compra.ItensCompra.Count}");

                // Processa a compra e retorna uma resposta
                return Json(new { Response = "Compra emitida com sucesso" });
            }
            catch (Exception ex)
            {
                // Log do erro para inspeção
                Console.WriteLine("Erro ao processar compra:", ex.Message);
                return Json(new { Response = "Erro no processamento da compra", Error = ex.Message });
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
