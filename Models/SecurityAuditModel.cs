using System.ComponentModel.DataAnnotations;

namespace ProjectWebsite.Models
{
    public class SecurityAuditModel
    {
        [Key]
        public Guid Id { get; set; }

        public string DeviceId { get; set; } = string.Empty;

        public string IPAddress { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; }

        public string EventType { get; set; } = string.Empty;

        public string Details { get; set; } = string.Empty;
    }
}
