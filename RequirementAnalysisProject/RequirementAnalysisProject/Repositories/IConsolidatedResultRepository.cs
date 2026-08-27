using RequirementAnalysisProject.Models.Entities;

namespace RequirementAnalysisProject.Repositories.Interfaces
{
    public interface IConsolidatedResultRepository
    {
        Task<ConsolidatedResult> SaveAsync(ConsolidatedResult result);
        Task<ConsolidatedResult?> GetLatestAsync();
        Task<List<ConsolidatedResult>> GetAllAsync();
    }
}