using Microsoft.AspNetCore.Mvc;
using RequirementAnalysisProject.Models;
using RequirementAnalysisProject.Models.Entities;
using RequirementAnalysisProject.Repositories.Interfaces;

namespace RequirementAnalysisProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectRepository _projectRepo;
        private readonly ILogger<ProjectController> _logger;

        public ProjectController(
            IProjectRepository projectRepo,
            ILogger<ProjectController> logger)
        {
            _projectRepo = projectRepo;
            _logger = logger;
        }

        // POST /api/project — Create new project
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(new { error = "Project name is required." });

            var project = await _projectRepo.CreateAsync(new Project
            {
                Name = dto.Name,
                Description = dto.Description,
                Domain = dto.Domain,
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            });

            return Ok(new ProjectResponseDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Domain = project.Domain,
                Status = project.Status,
                CreatedAt = project.CreatedAt,
                TotalConversations = 0
            });
        }

        // GET /api/project — Get all projects
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var projects = await _projectRepo.GetAllAsync();
            var result = projects.Select(p => new ProjectResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Domain = p.Domain,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                TotalConversations = p.Conversations?.Count ?? 0
            });
            return Ok(result);
        }

        // GET /api/project/{id} — Get project by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var project = await _projectRepo.GetByIdAsync(id);
            if (project == null)
                return NotFound(new { error = $"Project {id} not found." });

            return Ok(new ProjectResponseDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Domain = project.Domain,
                Status = project.Status,
                CreatedAt = project.CreatedAt,
                TotalConversations = project.Conversations?.Count ?? 0
            });
        }
    }
}