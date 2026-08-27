using System.ComponentModel.DataAnnotations;

namespace RequirementAnalysisProject.Models.Entities
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Domain { get; set; }  // HR, E-Commerce, Banking, etc.

        public string Status { get; set; } = "Active"; // Active, Completed, OnHold

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();
        public ICollection<ConsolidatedResult> ConsolidatedResults { get; set; } = new List<ConsolidatedResult>();
    }
}