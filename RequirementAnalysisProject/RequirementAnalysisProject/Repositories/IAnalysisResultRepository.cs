using RequirementAnalysisProject.Models.Entities;

namespace RequirementAnalysisProject.Repositories.Interfaces
{
    public interface IAnalysisResultRepository
    {
        Task<AnalysisResult> SaveAsync(AnalysisResult result);
        Task<AnalysisResult?> GetByIdAsync(int id);
        Task<AnalysisResult?> GetByConversationIdAsync(int conversationId);
        Task<List<AnalysisResult>> GetAllAsync();
        Task<List<AnalysisResult>> GetByProjectIdAsync(int projectId); // ← NEW
        Task<List<AnalysisResult>> GetByDateRangeAsync(DateTime from, DateTime to);
    }
}