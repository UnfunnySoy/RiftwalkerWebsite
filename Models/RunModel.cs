using System.ComponentModel.DataAnnotations;

namespace ProjectWebsite.Models
{
    public class RunModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public int Seed { get; set; }

        [Required]
        public int Status { get; set; }

        [Required]
        public int Score { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public AccountModel User { get; set; } 

        public RunModel()
        {
            Id = Guid.NewGuid();
        }
    }
}
