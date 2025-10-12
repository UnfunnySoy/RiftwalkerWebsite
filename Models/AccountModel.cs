using RiftwalkerWebsite.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace ProjectWebsite.Models
{
    public class AccountModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }

        public List<RunModel> Runs { get; set; }

        public AccountModel()
        {
            Id = Guid.NewGuid();
        }

        public AccountModel(AccountCreationViewModel viewModel) : this()
        {
            Username = viewModel.Username;
            Password = viewModel.Password;
            Email = viewModel.Email;
        }
    }
}
