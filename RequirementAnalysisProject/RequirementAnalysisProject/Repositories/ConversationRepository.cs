using Microsoft.EntityFrameworkCore;
using RequirementAnalysisProject.Data;
using RequirementAnalysisProject.Models.Entities;


namespace RequirementAnalysisProject.Repositories
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly AppDbContext _context;

        public ConversationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Conversation> SaveAsync(Conversation conversation)
        {
            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();
            return conversation;
        }

        public async Task<Conversation?> GetByIdAsync(int id)
        {
            return await _context.Conversations
                .Include(c => c.AnalysisResults)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        //public async Task<List<Conversation>> GetAllAsync()
        //{
        //    return await _context.Conversations
        //        .Include(c => c.AnalysisResults)
        //        .OrderByDescending(c => c.CreatedAt)
        //        .ToListAsync();
        //}

        public async Task<List<Conversation>> GetAllAsync()
        {
            return await _context.Conversations
                .Include(c => c.Project)        // ← ADD THIS
                .Include(c => c.AnalysisResults)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }
    }
}