using Leaf.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        public IActionResult NovaCompra()
        {

            return View();
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
