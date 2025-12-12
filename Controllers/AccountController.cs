using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectWebsite.Models;
using RiftwalkerWebsite.Data;
using RiftwalkerWebsite.ViewModels;

namespace ProjectWebsite.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(Guid? id)
        {
            if (id == null) return BadRequest(new { message = "No id provided" });

            using (ApplicationDBContext dbContext = new ApplicationDBContext())
            {
                AccountModel? account = dbContext.Accounts.FirstOrDefault(x => x.Id == id);
                if (account == null) return NotFound(new { message = "Account not found" });

                return View(account);
            }
        }

        [HttpPost]
        public IActionResult Create(AccountCreationViewModel? viewModel)
        {
            if (viewModel == null || string.IsNullOrEmpty(viewModel.Username) || string.IsNullOrEmpty(viewModel.DeviceId))
            {
                return BadRequest(new { message = "Failure" });
            }

            using (ApplicationDBContext dbContext = new ApplicationDBContext())
            {
                AccountModel account = new AccountModel(viewModel);             //CHECK THIS CONSTRUCTOR
                
                dbContext.Accounts.Add(account);
                dbContext.SaveChanges();

                return Ok("Account created");
            }
        }

        [HttpGet]
        public IActionResult Read(Guid? id)
        {
            if (id == null) return BadRequest(new { message = "No id provided" });

            using (ApplicationDBContext dbContext = new ApplicationDBContext())
            {
                AccountModel? account = dbContext.Accounts.FirstOrDefault(x => x.Id == id);
                if (account == null) return NotFound(new { message = "Account not found" });

                return Ok(account);
            }
        }

        [HttpDelete]
        public IActionResult Delete(Guid? id)
        {
            if (id == null) return BadRequest(new { message = "No id provided" });

            using (ApplicationDBContext dbContext = new ApplicationDBContext())
            {
                AccountModel? account = dbContext.Accounts.FirstOrDefault(x => x.Id == id);
                if (account == null) return NotFound(new { message = "Account not found" });

                dbContext.Accounts.Remove(account);
                dbContext.SaveChanges();

                return Ok("Account deleted");
            }
        }

        public IActionResult TestAccount()
        {
            using (ApplicationDBContext dbContext = new ApplicationDBContext())
            {
                AccountModel account = new AccountModel
                {
                    Username = "test",
                    DeviceId = "test-device-id", // Dummy ID for test
                    Runs = null
                };

                dbContext.Accounts.Add(account);
                dbContext.SaveChanges();

                return Content("SUCCESS: " + account.Id);
            }
        }

        // --- NEW API ENDPOINT FOR GODOT ---
        [HttpPost]
        [Route("api/login")]
        [IgnoreAntiforgeryToken]
        public IActionResult Login([FromBody] AccountCreationViewModel loginData)
        {
            if (loginData == null || string.IsNullOrEmpty(loginData.DeviceId)) 
                return BadRequest(new { message = "No device ID received" });

            using (ApplicationDBContext dbContext = new ApplicationDBContext())
            {
                AccountModel? account = dbContext.Accounts.FirstOrDefault(x => x.DeviceId == loginData.DeviceId);

                if (account != null)
                {
                    // Update username if it changed
                    if (!string.IsNullOrEmpty(loginData.Username) && account.Username != loginData.Username)
                    {
                        account.Username = loginData.Username;
                        dbContext.SaveChanges();
                    }
                }
                else
                {
                    // Create new user
                    account = new AccountModel
                    {
                        DeviceId = loginData.DeviceId,
                        Username = !string.IsNullOrEmpty(loginData.Username) ? loginData.Username : "Unknown Wanderer",
                        Runs = new List<RunModel>()
                    };
                    dbContext.Accounts.Add(account);
                    dbContext.SaveChanges();
                }

                return Ok(new
                {
                    access_token = "placeholder_token_123", // You might want real JWT later
                    user_id = account.Id,
                    username = account.Username
                });
            }
        }
    }
}