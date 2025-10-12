using Microsoft.AspNetCore.Mvc;
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

        [HttpPost]
        public IActionResult Create(AccountCreationViewModel? viewModel)
        {
            if (viewModel == null || viewModel.Username == null || viewModel.Password == null || viewModel.Email == null)
            {
                return RedirectToAction("Index");
            }

            ApplicationDBContext dBContext = new ApplicationDBContext();

            AccountModel account = new AccountModel(viewModel);
            dBContext.Accounts.Add(account);

            try
            {
                dBContext.SaveChanges();
            }
            catch (Exception) { }

            return Content("SUCCESS: " + account.Id);
        }

        [HttpGet]
        public IActionResult Read(Guid? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            ApplicationDBContext dbContext = new ApplicationDBContext();

            AccountModel account = dbContext.Accounts.FirstOrDefault(x => x.Id == id);
            if (account == null)
            {
                return RedirectToAction("Index");
            }

            return Content("SUCCESS: " + account.Id);
        }

        [HttpDelete]
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            ApplicationDBContext dBContext = new ApplicationDBContext();

            AccountModel? account = dBContext.Accounts.FirstOrDefault(x => x.Id == id);
            if (account == null)
            {
                return RedirectToAction("Index");
            }
            dBContext.Accounts.Remove(account);

            try
            {
                dBContext.SaveChanges();
            } catch (Exception) { }

            return Content("SUCCESS: " + account.Id);
        }

    }
}
