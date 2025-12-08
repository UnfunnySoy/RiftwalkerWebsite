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
            if (id == null) return Content("INVALID ENTRY");

            using (ApplicationDBContext dbContext = new ApplicationDBContext())
            {
                RunModel? run = dbContext.Runs.FirstOrDefault(x => x.Id == id);
                if (run == null) return Content("ENTRY DOES NOT EXIST");

                return View(run);
            }
        }

        [HttpPost]
        public IActionResult Create(RunModel? run)
        {
            if (run == null) return Content("INVALID ENTRY");

            using (ApplicationDBContext dbContext = new ApplicationDBContext())
            {
                dbContext.Runs.Add(run);
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception) { }

                return Content("SUCCESS: " + run.Id);
            }
        }

        [HttpGet]
        public IActionResult Read(Guid? id)
        {
            if (id == null) return Content("INVALID ENTRY");

            using (ApplicationDBContext dbContext = new ApplicationDBContext())
            {
                RunModel? run = dbContext.Runs.FirstOrDefault(x => x.Id == id);
                if (run == null) return Content("ENTRY DOES NOT EXIST");

                return Content("SUCCESS: " + run.Id);
            }
        }

        [HttpDelete]
        public IActionResult Delete(Guid? id)
        {
            if (id == null) return Content("INVALID ENTRY");

            using (ApplicationDBContext dbContext = new ApplicationDBContext())
            {
                RunModel? run = dbContext.Runs.FirstOrDefault(x => x.Id == id);
                if (run == null) return Content("ENTRY DOES NOT EXIST");
                
                dbContext.Runs.Remove(run);

                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception) { }

                return Content("SUCCESS: " + run.Id);
            }
        }

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

                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception)
                {
                    return Content("DATABASE FAILURE");
                }

                return Content("SUCCESS: " + run.Id);
            }
        }

        // --- NEW API HELPER CLASS ---
        public class GameRunUpload
        {
            public Guid user_id { get; set; }
            public int highest_round { get; set; }
            public int total_coins { get; set; }
            public string character_class { get; set; }
        }

        // --- NEW MEMBER ---
        private const string SECRET_SALT = "RIFTWALKER_SECRET_SALT_2025"; 

        // --- NEW API ENDPOINT FOR GODOT ---
        [HttpPost]
        [Route("api/upload-run")]
        [IgnoreAntiforgeryToken]
        public IActionResult UploadRun([FromBody] GameRunUpload runData)
        {
            if (runData == null) return BadRequest(new { message = "No run data received" });

            // 1. Verify Integrity
            if (!Request.Headers.TryGetValue("X-Integrity-Hash", out var clientHash))
            {
                 return BadRequest(new { message = "Missing integrity hash." });
            }

            // Hash = SHA256(SALT + HighestRound + TotalCoins)
            string rawData = SECRET_SALT + runData.highest_round + runData.total_coins;
            string serverHash = "";
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(rawData));
                serverHash = BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }

            if (clientHash.ToString().ToLower() != serverHash)
            {
                return BadRequest(new { message = "Integrity check failed." });
            }

            using (ApplicationDBContext dbContext = new ApplicationDBContext())
            {
                // 1. Find the user so we can link the run
                var user = dbContext.Accounts.FirstOrDefault(x => x.Id == runData.user_id);
                if (user == null) return NotFound(new { message = "User not found" });

                // 2. Create the RunModel
                RunModel newRun = new RunModel();
                newRun.User = user;
                newRun.Score = runData.total_coins; // Mapping coins to Score
                newRun.Status = runData.highest_round; // Storing round in Status
                // Ideally, you'd add 'CharacterClass' to your RunModel in the future!
                newRun.StartTime = DateTime.Now;
                newRun.EndTime = DateTime.Now;

                // 3. Save
                dbContext.Runs.Add(newRun);
                dbContext.SaveChanges();

                return Ok(new { status = "success", run_id = newRun.Id });
            }
        }
    }
}