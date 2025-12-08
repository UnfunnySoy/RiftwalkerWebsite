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
        public string DeviceId { get; set; }

        public List<RunModel>? Runs { get; set; }

        public AccountModel()
        {
            Id = Guid.NewGuid();
        }

        public AccountModel(AccountCreationViewModel viewModel) : this()
        {
            Username = viewModel.Username;
            DeviceId = viewModel.DeviceId;
        }
    }
}
