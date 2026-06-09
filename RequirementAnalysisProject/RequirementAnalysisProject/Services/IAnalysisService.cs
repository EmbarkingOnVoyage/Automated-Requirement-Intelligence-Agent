using RequirementAnalysisProject.Models;

namespace RequirementAnalysisProject.Services
{
    public interface IAnalysisService
    {
        Task<AnalyzeResponse> AnalyzeConversation(int projectId, string conversation);
        Task<VideoAnalyzeResponse> AnalyzeVideoAsync(VideoAnalyzeRequest request);
        Task<ConsolidatedReport> ConsolidateAllAnalyses(int projectId);
        Task<List<HistoryItem>> GetAllHistoryAsync(int projectId);
        Task<DashboardStats> GetDashboardStatsAsync();
    }

    public class HistoryItem
    {
        public int ConversationId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ProjectTitle { get; set; } = string.Empty;
        public string SourceType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}