using Microsoft.AspNetCore.Mvc;

namespace ProjectWebsite.Controllers
{
    public class RunInfoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
