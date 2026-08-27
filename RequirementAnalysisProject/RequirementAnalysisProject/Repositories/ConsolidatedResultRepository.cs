using Microsoft.EntityFrameworkCore;
using RequirementAnalysisProject.Data;
using RequirementAnalysisProject.Models.Entities;
using RequirementAnalysisProject.Repositories.Interfaces;

namespace RequirementAnalysisProject.Repositories
{
    public class ConsolidatedResultRepository : IConsolidatedResultRepository
    {
        private readonly AppDbContext _context;

        public ConsolidatedResultRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ConsolidatedResult> SaveAsync(ConsolidatedResult result)
        {
            _context.ConsolidatedResults.Add(result);
            await _context.SaveChangesAsync();
            return result;
        }

        public async Task<ConsolidatedResult?> GetLatestAsync()
        {
            return await _context.ConsolidatedResults
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<List<ConsolidatedResult>> GetAllAsync()
        {
            return await _context.ConsolidatedResults
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }
    }
}