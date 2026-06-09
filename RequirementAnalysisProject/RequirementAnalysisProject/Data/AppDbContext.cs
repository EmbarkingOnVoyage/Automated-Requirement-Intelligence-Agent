using Microsoft.EntityFrameworkCore;
using RequirementAnalysisProject.Models.Entities;

namespace RequirementAnalysisProject.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<AnalysisResult> AnalysisResults { get; set; }
        public DbSet<ConsolidatedResult> ConsolidatedResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Project
            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Status).HasMaxLength(50).HasDefaultValue("Active");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
            });

            // Conversation → Project
            modelBuilder.Entity<Conversation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Project)
                      .WithMany(p => p.Conversations)
                      .HasForeignKey(e => e.ProjectId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Transcript).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
            });

            // AnalysisResult → Conversation
            modelBuilder.Entity<AnalysisResult>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Conversation)
                      .WithMany(c => c.AnalysisResults)
                      .HasForeignKey(e => e.ConversationId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.Status).HasMaxLength(50).HasDefaultValue("Completed");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
            });

            // ConsolidatedResult → Project
            modelBuilder.Entity<ConsolidatedResult>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Project)
                      .WithMany(p => p.ConsolidatedResults)
                      .HasForeignKey(e => e.ProjectId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
            });
        }
    }
}