using Microsoft.EntityFrameworkCore;
using ProjectWebsite.Controllers;
using ProjectWebsite.Models;

namespace RiftwalkerWebsite.Data
{
    public class ApplicationDBContext : DbContext
    {
        private string DbPath;

        public DbSet<AccountModel> Accounts { get; set; }
        public DbSet<RunModel> Runs { get; set; }

        public ApplicationDBContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "Riftwalker.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");
    }
}
