using RequirementAnalysisProject.Models.Entities;

namespace RequirementAnalysisProject.Repositories.Interfaces
{
    public interface IProjectRepository
    {
        Task<Project> CreateAsync(Project project);
        Task<Project?> GetByIdAsync(int id);
        Task<List<Project>> GetAllAsync();
        Task<bool> ExistsAsync(int id);
    }
}