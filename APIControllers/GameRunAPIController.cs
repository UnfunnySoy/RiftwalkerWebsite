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

            // --- INPUT SANITIZATION (R.6.3.6): Validate Value Ranges ---
            if (runData.total_coins < 0 || runData.total_coins > 1000000)
            {
                return BadRequest(new { message = "Invalid data: Total Coins out of range (0-1,000,000)." });
            }

            if (runData.highest_round < 0 || runData.highest_round > 1000)
            {
                 return BadRequest(new { message = "Invalid data: Highest Round out of range (0-1000)." });
            }
            // -----------------------------------------------------------

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
                // --- SECURITY AUDIT (R.6.3.5): Log Failed Integrity Check ---
                using (var auditContext = new ApplicationDBContext())
                {
                    var audit = new SecurityAuditModel
                    {
                        Id = Guid.NewGuid(),
                        Timestamp = DateTime.UtcNow,
                        EventType = "INTEGRITY_FAIL",
                        IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        DeviceId = Request.Headers["X-Device-Id"].ToString() ?? "unknown",
                        Details = $"Client Hash: {clientHash}, Server Hash: {serverHash}"
                    };
                    auditContext.SecurityAudits.Add(audit);
                    auditContext.SaveChanges();
                }
                // ------------------------------------------------------------

                return BadRequest(new { message = "Integrity check failed." });
            }

            using (ApplicationDBContext dbContext = new ApplicationDBContext())
            {
                AccountModel? account = dbContext.Accounts.FirstOrDefault(x => x.Id == runData.user_id);
                if (account == null) return NotFound(new { message = "User not found" });

                // --- REPLAY PREVENTION (R.6.3.7): Check unique Run Timestamp ---
                DateTime runTime = DateTime.UnixEpoch.AddSeconds(runData.run_timestamp).ToLocalTime();
                
                // Allow a small grace period (e.g., duplicate sends within 1 second might be network retries, but strictly same ID/Time implies replay)
                // Since timestamp is second-precision, we check exact match.
                bool runExists = dbContext.Runs.Any(r => r.Account.Id == runData.user_id && r.StartTime == runTime);

                if (runExists)
                {
                    return BadRequest(new { message = "Replay attack detected: Run already submitted." });
                }
                // ---------------------------------------------------------------

                RunModel run = new RunModel();
                run.Account = account;
                run.Score = runData.total_coins;
                run.Status = runData.highest_round;
                run.StartTime = runTime; 
                run.EndTime = DateTime.Now;

                dbContext.Runs.Add(run);

                dbContext.SaveChanges();

                return Ok(new { status = "success", run_id = run.Id });
            }
        }
    }
}
