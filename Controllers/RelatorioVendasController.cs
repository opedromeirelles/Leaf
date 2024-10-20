using Microsoft.AspNetCore.Mvc;

namespace Leaf.Controllers
{
	public class RelatorioVendasController : Controller
	{
        private readonly string _pathIndex = "~/Views/Relatorios/Vendas/Index.cshtml";

        [Route("relatorio-vendas")]
        public IActionResult Index()
		{
			return View(_pathIndex);
		}
	}
}
