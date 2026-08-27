namespace RequirementAnalysisProject.Models
{
    public class DashboardStats
    {
        public int TotalProjects { get; set; }
        public int TotalAnalyses { get; set; }
        public int TotalVideoAnalyses { get; set; }
        public int TotalConsolidations { get; set; }
        public List<RecentActivity> RecentActivities { get; set; } = new();
    }

    public class RecentActivity
    {
        public int ConversationId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string ProjectTitle { get; set; } = string.Empty;
        public string SourceType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}