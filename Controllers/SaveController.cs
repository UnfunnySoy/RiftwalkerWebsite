using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectWebsite.Models;
using RiftwalkerWebsite.Data;

namespace ProjectWebsite.Controllers
{
    [Route("api/save")]
    [ApiController]
    public class SaveController : ControllerBase
    {
        // GET: api/save/{userId}
        [HttpGet("{userId}")]
        public IActionResult GetSave(Guid userId)
        {
            using (ApplicationDBContext dbContext = new ApplicationDBContext())
            {
                var save = dbContext.GameSaves.FirstOrDefault(s => s.UserId == userId);
                if (save == null || save.SaveData == null || save.SaveData.Length == 0)
                {
                    return NotFound(new { message = "No cloud save found." });
                }

                // Return as binary file
                return File(save.SaveData, "application/octet-stream", "savegame.save");
            }
        }

        // POST: api/save/upload
        // Expects multipart form data with file key "save_file" and "user_id"
        [HttpPost("upload")]
        public async Task<IActionResult> UploadSave([FromForm] Guid user_id, IFormFile save_file)
        {
            if (save_file == null || save_file.Length == 0)
                return BadRequest(new { message = "No file received." });

            using (ApplicationDBContext dbContext = new ApplicationDBContext())
            {
                // Ensure User exists
                var user = dbContext.Accounts.FirstOrDefault(x => x.Id == user_id);
                if (user == null) return NotFound(new { message = "User not found." });

                // Read file bytes
                byte[] fileBytes;
                using (var ms = new MemoryStream())
                {
                    await save_file.CopyToAsync(ms);
                    fileBytes = ms.ToArray();
                }

                // Check for existing save
                var existingSave = dbContext.GameSaves.FirstOrDefault(s => s.UserId == user_id);
                if (existingSave != null)
                {
                    existingSave.SaveData = fileBytes;
                    existingSave.LastUpdated = DateTime.UtcNow;
                }
                else
                {
                    var newSave = new GameSaveModel
                    {
                        UserId = user_id,
                        SaveData = fileBytes,
                        LastUpdated = DateTime.UtcNow
                    };
                    dbContext.GameSaves.Add(newSave);
                }

                dbContext.SaveChanges();
                return Ok(new { message = "Save uploaded successfully." });
            }
        }
    }
}
