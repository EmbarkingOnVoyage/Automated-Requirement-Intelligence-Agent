using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RequirementAnalysisProject.Models.Entities
{
    public class Conversation
    {
        [Key]
        public int Id { get; set; }

        // ← ADD THIS
        [ForeignKey("Project")]
        public int ProjectId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Transcript { get; set; } = string.Empty;

        public string? Source { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Project? Project { get; set; }
        public ICollection<AnalysisResult> AnalysisResults { get; set; } = new List<AnalysisResult>();
        public string? SourceType { get; internal set; }
        //public string? SourceType { get; internal set; }
    }
}