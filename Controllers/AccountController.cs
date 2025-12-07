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
            if (viewModel == null || string.IsNullOrEmpty(viewModel.Username) || string.IsNullOrEmpty(viewModel.Password) || string.IsNullOrEmpty(viewModel.Email))
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
                    Password = "test",
                    Email = "test",
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
        public IActionResult Login([FromBody] AccountCreationViewModel loginData)
        {
            if (loginData == null) return BadRequest(new { message = "No data received" });

            using (ApplicationDBContext dbContext = new ApplicationDBContext())
            {
                // 1. Find user by Email
                var user = dbContext.Accounts.FirstOrDefault(x => x.Email == loginData.Email);

                // 2. Check if user exists and password matches
                // Note: In a real app, use Hashing here instead of plain text comparison
                if (user == null || user.Password != loginData.Password)
                {
                    return Unauthorized(new { message = "Invalid email or password" });
                }

                // 3. Return the JSON the game needs
                return Ok(new
                {
                    access_token = "placeholder_token_123", // We can implement real JWT logic later
                    user_id = user.Id,
                    username = user.Username
                });
            }
        }
    }
}