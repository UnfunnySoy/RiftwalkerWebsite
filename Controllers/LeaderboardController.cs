using Microsoft.AspNetCore.Mvc;
using ProjectWebsite.Models;
using RiftwalkerWebsite.Data;

namespace ProjectWebsite.Controllers
{
    [Route("api/leaderboard")]
    [ApiController]
    public class LeaderboardController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetLeaderboard()
        {
            using (ApplicationDBContext dbContext = new ApplicationDBContext())
            {
                // We want to find the highest round (Status) for each unique user
                var leaderboard = dbContext.Runs
                    .Where(r => r.User != null)
                    // Group by the User entity (or ID/Username if simpler for SQL translation)
                    .GroupBy(r => r.User.Username)
                    .Select(g => new
                    {
                        username = g.Key,
                        highest_round = g.Max(r => r.Status),
                        lifetime_coins = g.Max(r => r.Score)
                    })
                    .OrderByDescending(x => x.highest_round)
                    .ThenByDescending(x => x.lifetime_coins) // Tie-breaker
                    .Take(10)
                    .ToList();

                return Ok(leaderboard);
            }
        }
    }
}
