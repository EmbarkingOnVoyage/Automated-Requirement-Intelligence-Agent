using Microsoft.AspNetCore.Mvc;
using RequirementAnalysisProject.Models;
using RequirementAnalysisProject.Services;

namespace RequirementAnalysisProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalysisController : ControllerBase
    {
        private readonly IAnalysisService _analysisService;
        private readonly ILogger<AnalysisController> _logger;

        public AnalysisController(
            IAnalysisService analysisService,
            ILogger<AnalysisController> logger)
        {
            _analysisService = analysisService;
            _logger = logger;
        }

        // POST /api/analysis/analyze
        [HttpPost("analyze")]
        public async Task<IActionResult> Analyze([FromBody] AnalyzeRequestDto request)
        {
            if (request.ProjectId <= 0)
                return BadRequest(new { error = "Valid ProjectId is required." });

            if (string.IsNullOrWhiteSpace(request.Conversation))
                return BadRequest(new { error = "Conversation text is required." });

            var result = await _analysisService.AnalyzeConversation(
                request.ProjectId, request.Conversation);

            if (!string.IsNullOrEmpty(result.Error))
                return StatusCode(500, new { error = result.Error });

            return Ok(result);
        }

        // POST /api/analysis/consolidate/{projectId}
        [HttpPost("consolidate/{projectId}")]
        public async Task<IActionResult> Consolidate(int projectId)
        {
            if (projectId <= 0)
                return BadRequest(new { error = "Valid ProjectId is required." });

            var result = await _analysisService.ConsolidateAllAnalyses(projectId);

            if (!string.IsNullOrEmpty(result.Error))
                return StatusCode(500, new { error = result.Error });

            return Ok(result);
        }

        // GET /api/analysis/history/{projectId}
        [HttpGet("history/{projectId}")]
        public async Task<IActionResult> GetHistory(int projectId)
        {
            if (projectId <= 0)
                return BadRequest(new { error = "Valid ProjectId is required." });

            var result = await _analysisService.GetAllHistoryAsync(projectId);
            return Ok(result);
        }

        // POST /api/analysis/analyze-video
        [HttpPost("analyze-video")]
        public async Task<IActionResult> AnalyzeVideo([FromBody] VideoAnalyzeRequest request)
        {
            if (request.ProjectId <= 0)
                return BadRequest(new { error = "Valid ProjectId is required." });

            if (string.IsNullOrEmpty(request.VideoUrl) &&
                string.IsNullOrEmpty(request.VideoFilePath))
                return BadRequest(new
                {
                    error = "Provide either VideoUrl or VideoFilePath."
                });

            _logger.LogInformation(
                "Video analysis request. Project: {id}, URL: {url}",
                request.ProjectId, request.VideoUrl ?? request.VideoFilePath);

            var result = await _analysisService.AnalyzeVideoAsync(request);

            if (!string.IsNullOrEmpty(result.Error))
                return StatusCode(500, new { error = result.Error });

            return Ok(result);
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _analysisService.GetDashboardStatsAsync();
            return Ok(stats);
        }
    }
}