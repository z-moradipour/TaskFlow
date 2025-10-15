using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Api.Models
{
    public class ActivityLog
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Foreign key to know which board this activity belongs to
        public int BoardId { get; set; }

        // Foreign key to know who performed the action
        [Required]
        public string UserId { get; set; }
        public User User { get; set; }
    }
}