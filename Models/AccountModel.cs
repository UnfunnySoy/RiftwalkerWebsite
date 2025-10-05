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

        public string Email { get; set; }

        [Required]
        public List<RunModel> Runs { get; set; }

        public AccountModel()
        {
            Id = Guid.NewGuid();
        }
    }
}
