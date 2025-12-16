using Microsoft.EntityFrameworkCore;
using ProjectWebsite.Models;

namespace RiftwalkerWebsite.Data
{
    public class ApplicationDBContext : DbContext
    {
        private string DbPath;

        public DbSet<AccountModel> Accounts { get; set; }
        public DbSet<RunModel> Runs { get; set; }
        public DbSet<GameSaveModel> GameSaves { get; set; }
        public DbSet<SecurityAuditModel> SecurityAudits { get; set; }

        public ApplicationDBContext()
        {
            //DbPath = "Data/RiftwalkerWebsite.db";

            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "Riftwalker.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");
    }
}
