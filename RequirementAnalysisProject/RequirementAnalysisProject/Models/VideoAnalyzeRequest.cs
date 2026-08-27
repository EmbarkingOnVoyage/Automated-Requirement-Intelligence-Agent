namespace RequirementAnalysisProject.Models
{
    public class VideoAnalyzeRequest
    {
        public int ProjectId { get; set; }
        public string? VideoUrl { get; set; }
        public string? VideoFilePath { get; set; }
    }

    public class VideoAnalyzeResponse
    {
        public int ProjectId { get; set; }
        public int ConversationId { get; set; }
        public string Transcript { get; set; } = string.Empty;
        public int WordCount { get; set; }
        public AnalyzeResponse? Analysis { get; set; }
        public string Error { get; set; } = string.Empty;
    }
}