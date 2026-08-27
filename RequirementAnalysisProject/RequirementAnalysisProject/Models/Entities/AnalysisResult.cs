using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RequirementAnalysisProject.Models.Entities
{
    public class AnalysisResult
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Conversation")]
        public int ConversationId { get; set; }

        public string? ProjectTitle { get; set; }
        public string? ProjectObjective { get; set; }
        public string? MinutesOfMeeting { get; set; } // ← MOM
        public string? FunctionalRequirements { get; set; }    // stored as JSON string
        public string? NonFunctionalRequirements { get; set; } // stored as JSON string
        public string? UserStories { get; set; }               // stored as JSON string
        public string? BusinessRules { get; set; }             // stored as JSON string
        public string? Assumptions { get; set; }               // stored as JSON string
        public string? OpenQuestions { get; set; }             // stored as JSON string
        public string? Modules { get; set; }                   // stored as JSON string
        public string? ApiSuggestions { get; set; }            // stored as JSON string
        public string? DatabaseEntities { get; set; }          // stored as JSON string
        public string? Roles { get; set; }                     // stored as JSON string
        public string? CommunicationGaps { get; set; }         // stored as JSON string
        public string? RiskFlags { get; set; }                 // stored as JSON string
        public string? Prioritization { get; set; }            // stored as JSON string
        public string? SuggestedMilestones { get; set; }       // stored as JSON string
        public string? RawJson { get; set; }

        public string Status { get; set; } = "Completed";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Conversation? Conversation { get; set; }
    }
}