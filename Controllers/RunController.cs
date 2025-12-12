using Microsoft.AspNetCore.Mvc;
using ProjectWebsite.Models;
using RiftwalkerWebsite.Data;
using RiftwalkerWebsite.ViewModels;

namespace ProjectWebsite.Controllers
{
    public class RunController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            using (ApplicationDBContext dbContext = new ApplicationDBContext())
            {
                List<RunModel> runModels = dbContext.Runs.OrderByDescending(x => x.Score).ThenByDescending(x => x.Id).ToList();
                List<RunViewModel> runViewModels = new List<RunViewModel>();
                foreach (RunModel run in runModels)
                {
                    RunViewModel runView = new RunViewModel(run);
                    runViewModels.Add(runView);
                }
                return View(runViewModels);
            }
        }

        [HttpGet]
        public IActionResult Details(Guid? id)
        {
            if (id == null) return BadRequest(new { message = "No id provided" });

            using (ApplicationDBContext dbContext = new ApplicationDBContext())
            {
                RunModel? run = dbContext.Runs.FirstOrDefault(x => x.Id == id);
                if (run == null) return NotFound(new { message = "Run not found" });

                return View(run);
            }
        }

        /*      //Unused. Handled by Godot API classes
        [HttpPost]
        public IActionResult Create(RunModel? run)
        {
            if (run == null) return BadRequest(new { message = "Failure" });

            using (ApplicationDBContext dbContext = new ApplicationDBContext())
            {
                dbContext.Runs.Add(run);
                dbContext.SaveChanges();

                return Ok("Run created");
            }
        }
        */

        [HttpGet]
        public IActionResult Read(Guid? id)
        {
            if (id == null) return BadRequest(new { message = "No id provided" });

            using (ApplicationDBContext dbContext = new ApplicationDBContext())
            {
                RunModel? run = dbContext.Runs.FirstOrDefault(x => x.Id == id);
                if (run == null) return NotFound(new { message = "Run not found" });

                return Ok(run);
            }
        }

        [HttpDelete]
        public IActionResult Delete(Guid? id)
        {
            if (id == null) return BadRequest(new { message = "No id provided" });

            using (ApplicationDBContext dbContext = new ApplicationDBContext())
            {
                RunModel? run = dbContext.Runs.FirstOrDefault(x => x.Id == id);
                if (run == null) return NotFound(new { message = "Run not found" });

                dbContext.Runs.Remove(run);
                dbContext.SaveChanges();

                return Ok("Run deleted");
            }
        }

        /*      //Unused. Testing endpoint for making runs
        [HttpPost]
        public IActionResult TestRun()
        {
            using (ApplicationDBContext dbContext = new ApplicationDBContext())
            {
                RunModel run = new RunModel
                {
                    Seed = 0,
                    Status = 0,
                    Score = 0,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now,
                    User = null
                };

                dbContext.Runs.Add(run);
                dbContext.SaveChanges();

                return Content("SUCCESS: " + run.Id);
            }
        }
        */
    }
}