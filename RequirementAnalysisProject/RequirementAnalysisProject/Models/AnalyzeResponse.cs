namespace RequirementAnalysisProject.Models
{
    public class AnalyzeResponse
    {
        public int ConversationId { get; set; }
        public string ProjectTitle { get; set; } = string.Empty;
        public string ProjectObjective { get; set; } = string.Empty;
        public string MinutesOfMeeting { get; set; } = string.Empty;
        public List<string> FunctionalRequirements { get; set; } = new();
        public List<string> NonFunctionalRequirements { get; set; } = new();
        public List<string> UserStories { get; set; } = new();
        public List<string> BusinessRules { get; set; } = new();
        public List<string> Assumptions { get; set; } = new();
        public List<string> OpenQuestions { get; set; } = new();
        public List<string> Modules { get; set; } = new();
        public List<string> ApiSuggestions { get; set; } = new();
        public List<string> DatabaseEntities { get; set; } = new();
        public List<string> Roles { get; set; } = new();
        public List<string> CommunicationGaps { get; set; } = new();
        public List<string> RiskFlags { get; set; } = new();
        public Prioritization Prioritization { get; set; } = new();
        public List<string> SuggestedMilestones { get; set; } = new();
        public string RawJson { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }

    public class Prioritization
    {
        public List<string> MustHave { get; set; } = new();
        public List<string> ShouldHave { get; set; } = new();
        public List<string> NiceToHave { get; set; } = new();
    }
}