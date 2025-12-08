using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectWebsite.Models
{
    public class GameSaveModel
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public AccountModel? User { get; set; }

        public byte[] SaveData { get; set; } = Array.Empty<byte>();

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        public GameSaveModel() { 
            Id = Guid.NewGuid();
        }
    }
}
