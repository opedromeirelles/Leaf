using Microsoft.AspNetCore.Mvc;

namespace Leaf.Controllers
{
	public class RelatorioProducaoController : Controller
	{
        private readonly string _pathIndex = "~/Views/Relatorios/Producao/Index.cshtml";

        [Route("relatorio-producao")]
        public IActionResult Index()
		{
			return View(_pathIndex);
		}
	}
}
