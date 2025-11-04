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

        [HttpGet]
        public IActionResult Details(Guid? id)
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

            return View(run);
        }

        [HttpPost]
        public IActionResult Create(RunModel? run)
        {
            if (run == null)
            {
                return RedirectToAction("Index");
            }

            ApplicationDBContext dbContext = new ApplicationDBContext();

            dbContext.Runs.Add(run);

            try
            {
                dbContext.SaveChanges();
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

        public IActionResult TestRun(Guid? id)
        {
            RunModel run = new RunModel();
            run.Seed = 0;
            run.Status = 0;
            run.Score = 0;
            run.StartTime = DateTime.Now;
            run.EndTime = DateTime.Now;
            run.User = null;

            ApplicationDBContext dbContext = new ApplicationDBContext();
            dbContext.Runs.Add(run);

            dbContext.SaveChanges();
            /*
            try
            {
                dbContext.SaveChanges();
            }
            catch (Exception)
            {
                return Content("DATABASE FAILURE");
            }
            */

            return Content("SUCCESS: " + run.Id);
        }
    }
}
