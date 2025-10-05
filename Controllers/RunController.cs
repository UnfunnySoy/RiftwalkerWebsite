using Microsoft.AspNetCore.Mvc;

namespace ProjectWebsite.Controllers
{
    public class RunController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
