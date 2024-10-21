using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Leaf.Controllers
{
    [Authorize]
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
