using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RequirementAnalysisProject.Models.Entities
{
    public class ConsolidatedResult
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Project")]
        public int ProjectId { get; set; }

        public int TotalConversations { get; set; }
        public string? ProjectTitle { get; set; }
        public string? ProjectObjective { get; set; }
        public string? ReportJson { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Project? Project { get; set; }
    }
}