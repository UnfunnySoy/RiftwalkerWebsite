using Microsoft.AspNetCore.Mvc;
using ProjectWebsite.Models;
using RiftwalkerWebsite.APIModels;
using RiftwalkerWebsite.Data;

namespace RiftwalkerWebsite.APIControllers
{
    public class GameRunAPIController : ControllerBase
    {
        private readonly string SECRET_SALT = "RIFTWALKER_SECRET_SALT_2025";

        [HttpPost]
        [Route("api/upload-run")]
        [IgnoreAntiforgeryToken]
        public IActionResult UploadRun([FromBody] GameRunAPIModel runData)
        {
            if (runData == null) return BadRequest(new { message = "No run data received" });

            // 1. Verify Integrity;
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
                AccountModel? account = dbContext.Accounts.FirstOrDefault(x => x.Id == runData.user_id);
                if (account == null) return NotFound(new { message = "User not found" });

                RunModel run = new RunModel();
                run.Account = account;
                run.Score = runData.total_coins;
                run.Status = runData.highest_round;
                run.StartTime = DateTime.Now;       //This should be a recorded time from the start of a run
                run.EndTime = DateTime.Now;

                dbContext.Runs.Add(run);

                dbContext.SaveChanges();

                return Ok(new { status = "success", run_id = run.Id });
            }
        }
    }
}
