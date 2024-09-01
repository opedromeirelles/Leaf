using Microsoft.AspNetCore.Mvc;

namespace Leaf.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
