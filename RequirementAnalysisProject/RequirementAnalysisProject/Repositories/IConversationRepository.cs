using RequirementAnalysisProject.Models.Entities;

namespace RequirementAnalysisProject.Repositories
{
    public interface IConversationRepository
    {
        Task<Conversation> SaveAsync(Conversation conversation);
        Task<Conversation?> GetByIdAsync(int id);
        Task<List<Conversation>> GetAllAsync();
    }
}
