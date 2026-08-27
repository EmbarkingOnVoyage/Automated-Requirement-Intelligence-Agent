namespace RequirementAnalysisProject.Models
{
    public class ConsolidatedReport
    {
        public string ProjectTitle { get; set; } = string.Empty;
        public string ProjectObjective { get; set; } = string.Empty;
        public int TotalConversationsAnalyzed { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        // Final merged requirements
        public List<string> FinalFunctionalRequirements { get; set; } = new();
        public List<string> FinalNonFunctionalRequirements { get; set; } = new();
        public List<string> FinalUserStories { get; set; } = new();
        public List<string> FinalBusinessRules { get; set; } = new();
        public List<string> FinalModules { get; set; } = new();
        public List<string> FinalApiSuggestions { get; set; } = new();
        public List<string> FinalDatabaseEntities { get; set; } = new();
        public List<string> FinalRoles { get; set; } = new();

        // Evolution tracking
        public List<RequirementChange> DetectedChanges { get; set; } = new();
        public List<RequirementConflict> DetectedConflicts { get; set; } = new();
        public List<string> EvolvedRequirements { get; set; } = new();
        public List<string> DroppedRequirements { get; set; } = new();
        public List<string> NewlyAddedRequirements { get; set; } = new();

        // Final outputs
        public Prioritization FinalPrioritization { get; set; } = new();
        public List<string> FinalOpenQuestions { get; set; } = new();
        public List<string> FinalRiskFlags { get; set; } = new();
        public List<string> FinalMilestones { get; set; } = new();
        public List<string> FinalAssumptions { get; set; } = new();

        public string RawJson { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }

    public class RequirementChange
    {
        public string Day { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;   // ADDED, MODIFIED, REMOVED
        public string OldValue { get; set; } = string.Empty;
        public string NewValue { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }

    public class RequirementConflict
    {
        public string ConflictDescription { get; set; } = string.Empty;
        public string FromDay { get; set; } = string.Empty;
        public string Resolution { get; set; } = string.Empty;
    }
}