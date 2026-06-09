using Microsoft.EntityFrameworkCore;
using RequirementAnalysisProject.Data;
using RequirementAnalysisProject.Models.Entities;
using RequirementAnalysisProject.Repositories.Interfaces;


namespace RequirementAnalysisProject.Repositories
{
    public class AnalysisResultRepository : IAnalysisResultRepository
    {
        private readonly AppDbContext _context;

        public AnalysisResultRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AnalysisResult> SaveAsync(AnalysisResult result)
        {
            _context.AnalysisResults.Add(result);
            await _context.SaveChangesAsync();
            return result;
        }

        public async Task<AnalysisResult?> GetByIdAsync(int id)
        {
            return await _context.AnalysisResults
                .Include(r => r.Conversation)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<AnalysisResult?> GetByConversationIdAsync(int conversationId)
        {
            return await _context.AnalysisResults
                .FirstOrDefaultAsync(r => r.ConversationId == conversationId);
        }

        //public async Task<List<AnalysisResult>> GetAllAsync()
        //{
        //    return await _context.AnalysisResults
        //        .Include(r => r.Conversation)
        //        .OrderByDescending(r => r.CreatedAt)
        //        .ToListAsync();
        //}

        public async Task<List<AnalysisResult>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            return await _context.AnalysisResults
                .Include(r => r.Conversation)
                .Where(r => r.CreatedAt >= from && r.CreatedAt <= to)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }
        public async Task<List<AnalysisResult>> GetByProjectIdAsync(int projectId)
        {
            return await _context.AnalysisResults
                .Include(r => r.Conversation)
                .Where(r => r.Conversation!.ProjectId == projectId)
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<AnalysisResult>> GetAllAsync()
        {
            return await _context.AnalysisResults
                .Include(r => r.Conversation)
                    .ThenInclude(c => c.Project)  // ← ADD THIS
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }
    }
}