using Microsoft.AspNetCore.Mvc;
using ProjectWebsite.Models;
using RiftwalkerWebsite.Data;
using System.Security.Principal;

namespace ProjectWebsite.Controllers
{
    public class RunController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(RunModel? run)
        {
            if (run == null)
            {
                return RedirectToAction("Index");
            }

            ApplicationDBContext dBContext = new ApplicationDBContext();

            dBContext.Runs.Add(run);

            try
            {
                dBContext.SaveChanges();
            }
            catch (Exception) { }

            return Content("SUCCESS: " + run.Id);
        }

        [HttpGet]
        public IActionResult Read(Guid? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            ApplicationDBContext dbContext = new ApplicationDBContext();

            RunModel? run = dbContext.Runs.FirstOrDefault(x => x.Id == id);
            if (run == null)
            {
                return RedirectToAction("Index");
            }

            return Content("SUCCESS: " + run.Id);
        }

        [HttpDelete]
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            ApplicationDBContext dbContext = new ApplicationDBContext();

            RunModel? run = dbContext.Runs.FirstOrDefault(x => x.Id == id);
            if (run == null)
            {
                return RedirectToAction("Index");
            }
            dbContext.Runs.Remove(run);

            try
            {
                dbContext.SaveChanges();
            }
            catch (Exception) { }

            return Content("SUCCESS: " + run.Id);
        }
    }
}
