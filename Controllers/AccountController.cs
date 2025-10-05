using Microsoft.AspNetCore.Mvc;

namespace ProjectWebsite.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
