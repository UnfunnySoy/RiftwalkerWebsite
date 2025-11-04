using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
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

        public IActionResult Details()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(AccountCreationViewModel? viewModel)
        {
            if (viewModel == null || viewModel.Username == null || viewModel.Password == null || viewModel.Email == null)
            {
                return Content("INVALID ENTRY");
            }

            ApplicationDBContext dbContext = new ApplicationDBContext();

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

        [HttpGet]
        public IActionResult Read(Guid? id)
        {
            if (id == null)
            {
                return Content("INVALID ENTRY");
            }

            ApplicationDBContext dbContext = new ApplicationDBContext();

            AccountModel? account = dbContext.Accounts.FirstOrDefault(x => x.Id == id);
            if (account == null)
            {
                return Content("ENTRY DOES NOT EXIST");
            }

            return Content("SUCCESS: " + account.Id);
        }

        [HttpDelete]
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return Content("INVALID ENTRY");
            }

            ApplicationDBContext dbContext = new ApplicationDBContext();

            AccountModel? account = dbContext.Accounts.FirstOrDefault(x => x.Id == id);
            if (account == null)
            {
                return Content("ENTRY DOES NOT EXIST");
            }
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

        [HttpPost]
        public IActionResult TestAccount()
        {
            AccountModel account = new AccountModel();
            account.Username = "test";
            account.Password = "test";
            account.Email = "test";
            account.Runs = null;

            ApplicationDBContext dbContext = new ApplicationDBContext();
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
}
