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
        // GET: api/save/{id}
        [HttpGet("{id}")]
        public IActionResult GetSave(Guid id)
        {
            using (ApplicationDBContext dbContext = new ApplicationDBContext())
            {
                GameSaveModel? save = dbContext.GameSaves.FirstOrDefault(s => s.Account.Id == id);
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

                // --- SECURITY CHECK (R.6.3.4): Validate Magic Numbers ---
                if (fileBytes.Length >= 2)
                {
                    // Check for EXE Header (MZ = 0x4D 0x5A)
                    if (fileBytes[0] == 0x4D && fileBytes[1] == 0x5A)
                    {
                        return BadRequest(new { message = "Invalid file format: Executables are not allowed." });
                    }

                    // Check for JSON ({ = 0x7B)
                    // Note: Godot saves are JSON, so we expect '{' as the first byte.
                    if (fileBytes[0] != 0x7B) 
                    {
                         return BadRequest(new { message = "Invalid file format: Not a valid save file." });
                    }
                }
                // --------------------------------------------------------

                // Check for existing save
                var existingSave = dbContext.GameSaves.FirstOrDefault(s => s.Account.Id == user_id);
                if (existingSave != null)
                {
                    existingSave.SaveData = fileBytes;
                    existingSave.LastUpdated = DateTime.UtcNow;
                }
                else
                {
                    var newSave = new GameSaveModel
                    {
                        Account = user,
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
