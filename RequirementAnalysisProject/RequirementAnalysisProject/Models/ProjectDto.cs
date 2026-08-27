namespace RequirementAnalysisProject.Models
{
    public class CreateProjectDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Domain { get; set; }
    }

    public class ProjectResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Domain { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int TotalConversations { get; set; }
    }

    // Updated analyze request — now includes ProjectId
    public class AnalyzeRequestDto
    {
        public int ProjectId { get; set; }
        public string Conversation { get; set; } = string.Empty;
    }
}