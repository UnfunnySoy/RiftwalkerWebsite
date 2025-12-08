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
            if (id == null) return Content("INVALID ENTRY");

            using (ApplicationDBContext dbContext = new ApplicationDBContext())
            {
                AccountModel? account = dbContext.Accounts.FirstOrDefault(x => x.Id == id);
                if (account == null) return Content("ENTRY DOES NOT EXIST");
                
                return View(account);
            }
        }

        [HttpPost]
        public IActionResult Create(AccountCreationViewModel? viewModel)
        {
            if (viewModel == null || string.IsNullOrEmpty(viewModel.Username) || string.IsNullOrEmpty(viewModel.DeviceId))
            {
                return Content("INVALID ENTRY");
            }

            using (ApplicationDBContext dbContext = new ApplicationDBContext())
            {
                AccountModel account = new AccountModel(viewModel);
                dbContext.Accounts.Add(account);

                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception)
                {
                    return Content("DATABASE FAILURE");
                }

                return Content("SUCCESS: " + account.Id);
            }
        }

        [HttpGet]
        public IActionResult Read(Guid? id)
        {
            if (id == null) return Content("INVALID ENTRY");

            using (ApplicationDBContext dbContext = new ApplicationDBContext())
            {
                AccountModel? account = dbContext.Accounts.FirstOrDefault(x => x.Id == id);
                if (account == null) return Content("ENTRY DOES NOT EXIST");

                return Content("SUCCESS: " + account.Id);
            }
        }

        [HttpDelete]
        public IActionResult Delete(Guid? id)
        {
            if (id == null) return Content("INVALID ENTRY");

            using (ApplicationDBContext dbContext = new ApplicationDBContext())
            {
                AccountModel? account = dbContext.Accounts.FirstOrDefault(x => x.Id == id);
                if (account == null) return Content("ENTRY DOES NOT EXIST");

                dbContext.Accounts.Remove(account);

                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception)
                {
                    return Content("DATABASE FAILURE");
                }

                return Content("SUCCESS: " + account.Id);
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

                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception)
                {
                    return Content("DATABASE FAILURE");
                }

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
                // Find user by DeviceId
                var user = dbContext.Accounts.FirstOrDefault(x => x.DeviceId == loginData.DeviceId);

                if (user != null)
                {
                    // Update username if it changed
                    if (!string.IsNullOrEmpty(loginData.Username) && user.Username != loginData.Username)
                    {
                        user.Username = loginData.Username;
                        dbContext.SaveChanges();
                    }
                }
                else
                {
                    // Create new user
                    user = new AccountModel
                    {
                        DeviceId = loginData.DeviceId,
                        Username = !string.IsNullOrEmpty(loginData.Username) ? loginData.Username : "Unknown Wanderer",
                        Runs = new List<RunModel>()
                    };
                    dbContext.Accounts.Add(user);
                    dbContext.SaveChanges();
                }

                return Ok(new
                {
                    access_token = "placeholder_token_123", // You might want real JWT later
                    user_id = user.Id,
                    username = user.Username
                });
            }
        }
    }
}