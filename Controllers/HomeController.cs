using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RiftwalkerWebsite.ViewModels;

namespace RiftwalkerWebsite.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}
