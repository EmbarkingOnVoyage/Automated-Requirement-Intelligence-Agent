//using RequirementAnalysisProject.Models;
//using RequirementAnalysisProject.Models.Entities;
//using RequirementAnalysisProject.Repositories;
//using RequirementAnalysisProject.Repositories.Interfaces;
//using RequirementAnalysisProject.Services.AI;
//using RequirementAnalysisProject.Services.AI.Prompts;
//using System.Text.Json;
//using RequirementAnalysisProject.Repositories.Interfaces;

//namespace RequirementAnalysisProject.Services
//{
//    public class AnalysisService : IAnalysisService
//    {
//        private readonly AnalysisOrchestrator _orchestrator;
//        private readonly IConversationRepository _conversationRepo;
//        private readonly IAnalysisResultRepository _analysisResultRepo;
//        private readonly IConsolidatedResultRepository _consolidatedRepo;
//        private readonly GeminiClientService _geminiClient;
//        private readonly IProjectRepository _projectRepo;
//        private readonly ILogger<AnalysisService> _logger;

//        public AnalysisService(
//            AnalysisOrchestrator orchestrator,
//            IConversationRepository conversationRepo,
//            IAnalysisResultRepository analysisResultRepo,
//            IConsolidatedResultRepository consolidatedRepo,
//            IProjectRepository projectRepo,
//            GeminiClientService geminiClient,
//            ILogger<AnalysisService> logger)
//        {
//            _orchestrator = orchestrator;
//            _conversationRepo = conversationRepo;
//            _analysisResultRepo = analysisResultRepo;
//            _consolidatedRepo = consolidatedRepo;
//            _geminiClient = geminiClient;
//            _logger = logger;
//        }

//        //// ── Analyze single conversation ────────────────────────────────────
//        //public async Task<AnalyzeResponse> AnalyzeConversation(string conversation)
//        //{
//        //    // Step 1: Save conversation
//        //    var savedConversation = await _conversationRepo.SaveAsync(new Conversation
//        //    {
//        //        Title = GenerateTitle(conversation),
//        //        Transcript = conversation,
//        //        Source = "manual",
//        //        CreatedAt = DateTime.UtcNow
//        //    });

//        //    // Step 2: Run AI analysis
//        //    var result = await _orchestrator.RunAsync(conversation);

//        //    // Step 3: Save result to DB
//        //    await _analysisResultRepo.SaveAsync(new AnalysisResult
//        //    {
//        //        ConversationId = savedConversation.Id,
//        //        ProjectTitle = result.ProjectTitle,
//        //        ProjectObjective = result.ProjectObjective,
//        //        FunctionalRequirements = JsonSerializer.Serialize(result.FunctionalRequirements),
//        //        NonFunctionalRequirements = JsonSerializer.Serialize(result.NonFunctionalRequirements),
//        //        UserStories = JsonSerializer.Serialize(result.UserStories),
//        //        BusinessRules = JsonSerializer.Serialize(result.BusinessRules),
//        //        Assumptions = JsonSerializer.Serialize(result.Assumptions),
//        //        OpenQuestions = JsonSerializer.Serialize(result.OpenQuestions),
//        //        Modules = JsonSerializer.Serialize(result.Modules),
//        //        ApiSuggestions = JsonSerializer.Serialize(result.ApiSuggestions),
//        //        DatabaseEntities = JsonSerializer.Serialize(result.DatabaseEntities),
//        //        Roles = JsonSerializer.Serialize(result.Roles),
//        //        CommunicationGaps = JsonSerializer.Serialize(result.CommunicationGaps),
//        //        RiskFlags = JsonSerializer.Serialize(result.RiskFlags),
//        //        Prioritization = JsonSerializer.Serialize(result.Prioritization),
//        //        SuggestedMilestones = JsonSerializer.Serialize(result.SuggestedMilestones),
//        //        RawJson = result.RawJson,
//        //        Status = "Completed",
//        //        CreatedAt = DateTime.UtcNow
//        //    });

//        //    result.ConversationId = savedConversation.Id;
//        //    return result;
//        //}

//        //// ── Consolidate ALL analyses from DB ───────────────────────────────
//        //public async Task<ConsolidatedReport> ConsolidateAllAnalyses()
//        //{
//        //    try
//        //    {
//        //        // Step 1: Load all analyses from DB
//        //        var allResults = await _analysisResultRepo.GetAllAsync();

//        //        if (allResults.Count == 0)
//        //            return new ConsolidatedReport
//        //            {
//        //                Error = "No analyses found in database. Please analyze at least one conversation first."
//        //            };

//        //        _logger.LogInformation("Consolidating {count} analyses...", allResults.Count);

//        //        // Step 2: Build session data for AI
//        //        var sessions = new List<object>();
//        //        int dayNumber = 1;

//        //        foreach (var result in allResults.OrderBy(r => r.CreatedAt))
//        //        {
//        //            sessions.Add(new
//        //            {
//        //                sessionLabel = $"Day {dayNumber}",
//        //                analyzedAt = result.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
//        //                projectTitle = result.ProjectTitle,
//        //                projectObjective = result.ProjectObjective,
//        //                functionalRequirements = DeserializeList(result.FunctionalRequirements),
//        //                nonFunctionalRequirements = DeserializeList(result.NonFunctionalRequirements),
//        //                userStories = DeserializeList(result.UserStories),
//        //                businessRules = DeserializeList(result.BusinessRules),
//        //                assumptions = DeserializeList(result.Assumptions),
//        //                openQuestions = DeserializeList(result.OpenQuestions),
//        //                modules = DeserializeList(result.Modules),
//        //                apiSuggestions = DeserializeList(result.ApiSuggestions),
//        //                databaseEntities = DeserializeList(result.DatabaseEntities),
//        //                roles = DeserializeList(result.Roles),
//        //                communicationGaps = DeserializeList(result.CommunicationGaps),
//        //                riskFlags = DeserializeList(result.RiskFlags)
//        //            });
//        //            dayNumber++;
//        //        }

//        //        // Step 3: Build prompt and call AI
//        //        var allSessionsJson = JsonSerializer.Serialize(sessions, new JsonSerializerOptions
//        //        {
//        //            WriteIndented = true
//        //        });

//        //        var prompt = ConsolidationPrompt.Build(allSessionsJson);
//        //        var rawJson = await _geminiClient.CallGeminiAsync(prompt);

//        //        // Step 4: Parse consolidated report
//        //        var report = ParseConsolidatedReport(rawJson, allResults.Count);

//        //        // Step 5: Save to DB
//        //        await _consolidatedRepo.SaveAsync(new ConsolidatedResult
//        //        {
//        //            TotalConversations = allResults.Count,
//        //            ProjectTitle = report.ProjectTitle,
//        //            ProjectObjective = report.ProjectObjective,
//        //            ReportJson = rawJson,
//        //            CreatedAt = DateTime.UtcNow
//        //        });

//        //        return report;
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        _logger.LogError(ex, "Consolidation failed");
//        //        return new ConsolidatedReport
//        //        {
//        //            Error = $"Consolidation failed: {ex.Message}"
//        //        };
//        //    }
//        //}

//        //// ── Get history ────────────────────────────────────────────────────
//        //public async Task<List<HistoryItem>> GetAllHistoryAsync()
//        //{
//        //    var all = await _analysisResultRepo.GetAllAsync();
//        //    return all.Select(r => new HistoryItem
//        //    {
//        //        ConversationId = r.ConversationId,
//        //        Title = r.Conversation?.Title ?? "Untitled",
//        //        ProjectTitle = r.ProjectTitle ?? "Unknown",
//        //        CreatedAt = r.CreatedAt
//        //    }).ToList();
//        //}

//        // ── Analyze single conversation ────────────────────────────
//        public async Task<AnalyzeResponse> AnalyzeConversation(
//            int projectId, string conversation)
//        {
//            // Validate project exists
//            if (!await _projectRepo.ExistsAsync(projectId))
//                return new AnalyzeResponse
//                {
//                    Error = $"Project {projectId} not found."
//                };

//            // Save conversation with ProjectId
//            var savedConversation = await _conversationRepo.SaveAsync(new Conversation
//            {
//                ProjectId = projectId,           // ← KEY CHANGE
//                Title = GenerateTitle(conversation),
//                Transcript = conversation,
//                Source = "manual",
//                CreatedAt = DateTime.UtcNow
//            });

//            // Run AI + save result (same as before)
//            var result = await _orchestrator.RunAsync(conversation);

//            await _analysisResultRepo.SaveAsync(new AnalysisResult
//            {
//                ConversationId = savedConversation.Id,
//                ProjectTitle = result.ProjectTitle,
//                ProjectObjective = result.ProjectObjective,
//                FunctionalRequirements = JsonSerializer.Serialize(result.FunctionalRequirements),
//                NonFunctionalRequirements = JsonSerializer.Serialize(result.NonFunctionalRequirements),
//                UserStories = JsonSerializer.Serialize(result.UserStories),
//                BusinessRules = JsonSerializer.Serialize(result.BusinessRules),
//                Assumptions = JsonSerializer.Serialize(result.Assumptions),
//                OpenQuestions = JsonSerializer.Serialize(result.OpenQuestions),
//                Modules = JsonSerializer.Serialize(result.Modules),
//                ApiSuggestions = JsonSerializer.Serialize(result.ApiSuggestions),
//                DatabaseEntities = JsonSerializer.Serialize(result.DatabaseEntities),
//                Roles = JsonSerializer.Serialize(result.Roles),
//                CommunicationGaps = JsonSerializer.Serialize(result.CommunicationGaps),
//                RiskFlags = JsonSerializer.Serialize(result.RiskFlags),
//                Prioritization = JsonSerializer.Serialize(result.Prioritization),
//                SuggestedMilestones = JsonSerializer.Serialize(result.SuggestedMilestones),
//                RawJson = result.RawJson,
//                Status = "Completed",
//                CreatedAt = DateTime.UtcNow
//            });

//            result.ConversationId = savedConversation.Id;
//            return result;
//        }

//        // ── Consolidate by ProjectId only ─────────────────────────
//        public async Task<ConsolidatedReport> ConsolidateAllAnalyses(int projectId)
//        {
//            try
//            {
//                // Only fetch analyses for THIS project
//                var allResults = await _analysisResultRepo.GetByProjectIdAsync(projectId);

//                if (allResults.Count == 0)
//                    return new ConsolidatedReport
//                    {
//                        Error = $"No analyses found for Project {projectId}."
//                    };

//                // Rest of consolidation logic same as before...
//                var sessions = new List<object>();
//                int dayNumber = 1;

//                foreach (var result in allResults.OrderBy(r => r.CreatedAt))
//                {
//                    sessions.Add(new
//                    {
//                        sessionLabel = $"Day {dayNumber}",
//                        analyzedAt = result.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
//                        projectTitle = result.ProjectTitle,
//                        projectObjective = result.ProjectObjective,
//                        functionalRequirements = DeserializeList(result.FunctionalRequirements),
//                        nonFunctionalRequirements = DeserializeList(result.NonFunctionalRequirements),
//                        userStories = DeserializeList(result.UserStories),
//                        businessRules = DeserializeList(result.BusinessRules),
//                        assumptions = DeserializeList(result.Assumptions),
//                        openQuestions = DeserializeList(result.OpenQuestions),
//                        modules = DeserializeList(result.Modules),
//                        apiSuggestions = DeserializeList(result.ApiSuggestions),
//                        databaseEntities = DeserializeList(result.DatabaseEntities),
//                        roles = DeserializeList(result.Roles),
//                        communicationGaps = DeserializeList(result.CommunicationGaps),
//                        riskFlags = DeserializeList(result.RiskFlags)
//                    });
//                    dayNumber++;
//                }

//                var allSessionsJson = JsonSerializer.Serialize(sessions, new JsonSerializerOptions
//                {
//                    WriteIndented = true
//                });

//                var prompt = ConsolidationPrompt.Build(allSessionsJson);
//                var rawJson = await _geminiClient.CallGeminiAsync(prompt);
//                var report = ParseConsolidatedReport(rawJson, allResults.Count);

//                await _consolidatedRepo.SaveAsync(new ConsolidatedResult
//                {
//                    ProjectId = projectId,        
//                    TotalConversations = allResults.Count,
//                    ProjectTitle = report.ProjectTitle,
//                    ProjectObjective = report.ProjectObjective,
//                    ReportJson = rawJson,
//                    CreatedAt = DateTime.UtcNow
//                });

//                return report;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Consolidation failed for project {id}", projectId);
//                return new ConsolidatedReport
//                {
//                    Error = $"Consolidation failed: {ex.Message}"
//                };
//            }
//        }

//        // ── History by ProjectId ───────────────────────────────────
//        public async Task<List<HistoryItem>> GetAllHistoryAsync(int projectId)
//        {
//            var all = await _analysisResultRepo.GetByProjectIdAsync(projectId);
//            return all.Select(r => new HistoryItem
//            {
//                ConversationId = r.ConversationId,
//                Title = r.Conversation?.Title ?? "Untitled",
//                ProjectTitle = r.ProjectTitle ?? "Unknown",
//                CreatedAt = r.CreatedAt
//            }).ToList();
//        }


//        // ── Private Helpers ────────────────────────────────────────────────
//        private List<string> DeserializeList(string? json)
//        {
//            if (string.IsNullOrEmpty(json)) return new List<string>();
//            try { return JsonSerializer.Deserialize<List<string>>(json) ?? new(); }
//            catch { return new List<string>(); }
//        }

//        private string GenerateTitle(string conversation)
//        {
//            var firstLine = conversation.Replace("\n", " ").Trim();
//            return firstLine.Length > 80
//                ? firstLine.Substring(0, 80) + "..."
//                : firstLine;
//        }

//        private ConsolidatedReport ParseConsolidatedReport(string rawJson, int totalCount)
//        {
//            try
//            {
//                var clean = rawJson
//                    .Replace("```json", "")
//                    .Replace("```", "")
//                    .Trim();

//                using var doc = JsonDocument.Parse(clean);
//                var root = doc.RootElement;

//                return new ConsolidatedReport
//                {
//                    ProjectTitle = GetString(root, "projectTitle"),
//                    ProjectObjective = GetString(root, "projectObjective"),
//                    TotalConversationsAnalyzed = totalCount,
//                    GeneratedAt = DateTime.UtcNow,
//                    FinalFunctionalRequirements = GetList(root, "finalFunctionalRequirements"),
//                    FinalNonFunctionalRequirements = GetList(root, "finalNonFunctionalRequirements"),
//                    FinalUserStories = GetList(root, "finalUserStories"),
//                    FinalBusinessRules = GetList(root, "finalBusinessRules"),
//                    FinalModules = GetList(root, "finalModules"),
//                    FinalApiSuggestions = GetList(root, "finalApiSuggestions"),
//                    FinalDatabaseEntities = GetList(root, "finalDatabaseEntities"),
//                    FinalRoles = GetList(root, "finalRoles"),
//                    EvolvedRequirements = GetList(root, "evolvedRequirements"),
//                    DroppedRequirements = GetList(root, "droppedRequirements"),
//                    NewlyAddedRequirements = GetList(root, "newlyAddedRequirements"),
//                    FinalOpenQuestions = GetList(root, "finalOpenQuestions"),
//                    FinalRiskFlags = GetList(root, "finalRiskFlags"),
//                    FinalMilestones = GetList(root, "finalMilestones"),
//                    FinalAssumptions = GetList(root, "finalAssumptions"),
//                    DetectedChanges = GetChanges(root),
//                    DetectedConflicts = GetConflicts(root),
//                    FinalPrioritization = GetFinalPrioritization(root),
//                    RawJson = clean
//                };
//            }
//            catch (Exception ex)
//            {
//                return new ConsolidatedReport
//                {
//                    Error = $"Failed to parse consolidated report: {ex.Message}",
//                    RawJson = rawJson
//                };
//            }
//        }

//        private string GetString(JsonElement root, string key)
//            => root.TryGetProperty(key, out var val) ? val.GetString() ?? "" : "";

//        private List<string> GetList(JsonElement root, string key)
//        {
//            if (!root.TryGetProperty(key, out var val)) return new();
//            return val.EnumerateArray()
//                      .Select(x => x.GetString() ?? "")
//                      .Where(x => !string.IsNullOrEmpty(x))
//                      .ToList();
//        }

//        private List<RequirementChange> GetChanges(JsonElement root)
//        {
//            if (!root.TryGetProperty("detectedChanges", out var arr)) return new();
//            return arr.EnumerateArray().Select(x => new RequirementChange
//            {
//                Day = GetString(x, "day"),
//                Type = GetString(x, "type"),
//                OldValue = GetString(x, "oldValue"),
//                NewValue = GetString(x, "newValue"),
//                Reason = GetString(x, "reason")
//            }).ToList();
//        }

//        private List<RequirementConflict> GetConflicts(JsonElement root)
//        {
//            if (!root.TryGetProperty("detectedConflicts", out var arr)) return new();
//            return arr.EnumerateArray().Select(x => new RequirementConflict
//            {
//                ConflictDescription = GetString(x, "conflictDescription"),
//                FromDay = GetString(x, "fromDay"),
//                Resolution = GetString(x, "resolution")
//            }).ToList();
//        }

//        private Prioritization GetFinalPrioritization(JsonElement root)
//        {
//            if (!root.TryGetProperty("finalPrioritization", out var p)) return new();
//            return new Prioritization
//            {
//                MustHave = GetList(p, "mustHave"),
//                ShouldHave = GetList(p, "shouldHave"),
//                NiceToHave = GetList(p, "niceToHave")
//            };
//        }
//    }
//}

using RequirementAnalysisProject.Models;
using RequirementAnalysisProject.Models.Entities;
using RequirementAnalysisProject.Repositories;
using RequirementAnalysisProject.Repositories.Interfaces;
using RequirementAnalysisProject.Services.AI;
using RequirementAnalysisProject.Services.AI.Prompts;
using System.Text.Json;

namespace RequirementAnalysisProject.Services
{
    public class AnalysisService : IAnalysisService
    {
        private readonly AnalysisOrchestrator _orchestrator;
        private readonly IConversationRepository _conversationRepo;
        private readonly IAnalysisResultRepository _analysisResultRepo;
        private readonly IConsolidatedResultRepository _consolidatedRepo;
        private readonly IProjectRepository _projectRepo;
        private readonly GeminiClientService _geminiClient;
        private readonly ILogger<AnalysisService> _logger;
        private readonly TranscriptionService _transcriptionService;

        public AnalysisService(
            AnalysisOrchestrator orchestrator,
            IConversationRepository conversationRepo,
            IAnalysisResultRepository analysisResultRepo,
            IConsolidatedResultRepository consolidatedRepo,
            IProjectRepository projectRepo,
            GeminiClientService geminiClient,
            ILogger<AnalysisService> logger,
             TranscriptionService transcriptionService)
        {
            _orchestrator = orchestrator;
            _conversationRepo = conversationRepo;
            _analysisResultRepo = analysisResultRepo;
            _consolidatedRepo = consolidatedRepo;
            _projectRepo = projectRepo;
            _geminiClient = geminiClient;
            _logger = logger;
            _transcriptionService = transcriptionService;
        }

        public async Task<AnalyzeResponse> AnalyzeConversation(
            int projectId, string conversation)
        {
            // Validate project exists
            if (!await _projectRepo.ExistsAsync(projectId))
                return new AnalyzeResponse
                {
                    Error = $"Project {projectId} not found."
                };

            // Save conversation
            var savedConversation = await _conversationRepo.SaveAsync(new Conversation
            {
                ProjectId = projectId,
                Title = GenerateTitle(conversation),
                Transcript = conversation,
                Source = "manual",
                CreatedAt = DateTime.UtcNow
            });

            // Run AI analysis
            var result = await _orchestrator.RunAsync(conversation);

            // Save result to DB
            await _analysisResultRepo.SaveAsync(new AnalysisResult
            {
                ConversationId = savedConversation.Id,
                ProjectTitle = result.ProjectTitle,
                ProjectObjective = result.ProjectObjective,
                MinutesOfMeeting = result.MinutesOfMeeting,
                FunctionalRequirements = JsonSerializer.Serialize(result.FunctionalRequirements),
                NonFunctionalRequirements = JsonSerializer.Serialize(result.NonFunctionalRequirements),
                UserStories = JsonSerializer.Serialize(result.UserStories),
                BusinessRules = JsonSerializer.Serialize(result.BusinessRules),
                Assumptions = JsonSerializer.Serialize(result.Assumptions),
                OpenQuestions = JsonSerializer.Serialize(result.OpenQuestions),
                Modules = JsonSerializer.Serialize(result.Modules),
                ApiSuggestions = JsonSerializer.Serialize(result.ApiSuggestions),
                DatabaseEntities = JsonSerializer.Serialize(result.DatabaseEntities),
                Roles = JsonSerializer.Serialize(result.Roles),
                CommunicationGaps = JsonSerializer.Serialize(result.CommunicationGaps),
                RiskFlags = JsonSerializer.Serialize(result.RiskFlags),
                Prioritization = JsonSerializer.Serialize(result.Prioritization),
                SuggestedMilestones = JsonSerializer.Serialize(result.SuggestedMilestones),
                RawJson = result.RawJson,
                Status = "Completed",
                CreatedAt = DateTime.UtcNow
            });

            result.ConversationId = savedConversation.Id;
            return result;
        }



        //public async Task<ConsolidatedReport> ConsolidateAllAnalyses(int projectId)
        //{
        //    try
        //    {
        //        var allResults = await _analysisResultRepo.GetByProjectIdAsync(projectId);

        //        if (allResults.Count == 0)
        //            return new ConsolidatedReport
        //            {
        //                Error = $"No analyses found for Project {projectId}."
        //            };

        //        _logger.LogInformation("Consolidating {count} analyses for project {id}...",
        //            allResults.Count, projectId);

        //        var sessions = new List<object>();
        //        int dayNumber = 1;

        //        foreach (var result in allResults.OrderBy(r => r.CreatedAt))
        //        {
        //            sessions.Add(new
        //            {
        //                sessionLabel = $"Day {dayNumber}",
        //                analyzedAt = result.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
        //                projectTitle = result.ProjectTitle,
        //                projectObjective = result.ProjectObjective,
        //                functionalRequirements = DeserializeList(result.FunctionalRequirements),
        //                nonFunctionalRequirements = DeserializeList(result.NonFunctionalRequirements),
        //                userStories = DeserializeList(result.UserStories),
        //                businessRules = DeserializeList(result.BusinessRules),
        //                assumptions = DeserializeList(result.Assumptions),
        //                openQuestions = DeserializeList(result.OpenQuestions),
        //                modules = DeserializeList(result.Modules),
        //                apiSuggestions = DeserializeList(result.ApiSuggestions),
        //                databaseEntities = DeserializeList(result.DatabaseEntities),
        //                roles = DeserializeList(result.Roles),
        //                communicationGaps = DeserializeList(result.CommunicationGaps),
        //                riskFlags = DeserializeList(result.RiskFlags)
        //            });
        //            dayNumber++;
        //        }

        //        var allSessionsJson = JsonSerializer.Serialize(sessions,
        //            new JsonSerializerOptions { WriteIndented = true });

        //        var prompt = ConsolidationPrompt.Build(allSessionsJson);
        //        var rawJson = await _geminiClient.CallGeminiAsync(prompt);
        //        var report = ParseConsolidatedReport(rawJson, allResults.Count);

        //        await _consolidatedRepo.SaveAsync(new ConsolidatedResult
        //        {
        //            ProjectId = projectId,
        //            TotalConversations = allResults.Count,
        //            ProjectTitle = report.ProjectTitle,
        //            ProjectObjective = report.ProjectObjective,
        //            ReportJson = rawJson,
        //            CreatedAt = DateTime.UtcNow
        //        });

        //        return report;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Consolidation failed for project {id}", projectId);
        //        return new ConsolidatedReport
        //        {
        //            Error = $"Consolidation failed: {ex.Message}"
        //        };
        //    }
        //}


        public async Task<ConsolidatedReport> ConsolidateAllAnalyses(int projectId)
        {
            try
            {
                var allResults = await _analysisResultRepo.GetByProjectIdAsync(projectId);

                if (allResults.Count == 0)
                    return new ConsolidatedReport
                    {
                        Error = $"No analyses found for Project {projectId}."
                    };

                _logger.LogInformation("Consolidating {count} analyses...", allResults.Count);

                var sessions = new List<object>();
                int dayNumber = 1;

                foreach (var result in allResults.OrderBy(r => r.CreatedAt))
                {
                    // ← Trim each list to max 5 items to save tokens
                    sessions.Add(new
                    {
                        sessionLabel = $"Day {dayNumber}",
                        analyzedAt = result.CreatedAt.ToString("yyyy-MM-dd"),
                        projectTitle = result.ProjectTitle,
                        functionalRequirements = DeserializeList(result.FunctionalRequirements).Take(5),
                        nonFunctionalRequirements = DeserializeList(result.NonFunctionalRequirements).Take(3),
                        userStories = DeserializeList(result.UserStories).Take(3),
                        businessRules = DeserializeList(result.BusinessRules).Take(5),
                        modules = DeserializeList(result.Modules).Take(5),
                        openQuestions = DeserializeList(result.OpenQuestions).Take(3),
                        roles = DeserializeList(result.Roles).Take(3),
                        communicationGaps = DeserializeList(result.CommunicationGaps).Take(3),
                        riskFlags = DeserializeList(result.RiskFlags).Take(3),
                    });
                    dayNumber++;
                }

                var allSessionsJson = JsonSerializer.Serialize(sessions, new JsonSerializerOptions
                {
                    WriteIndented = false  // ← compact JSON saves tokens
                });

                _logger.LogInformation("Session JSON size: {len} chars", allSessionsJson.Length);

                var prompt = ConsolidationPrompt.Build(allSessionsJson);
                var rawJson = await _geminiClient.CallGeminiAsync(prompt);
                var report = ParseConsolidatedReport(rawJson, allResults.Count);

                await _consolidatedRepo.SaveAsync(new ConsolidatedResult
                {
                    ProjectId = projectId,
                    TotalConversations = allResults.Count,
                    ProjectTitle = report.ProjectTitle,
                    ProjectObjective = report.ProjectObjective,
                    ReportJson = rawJson,
                    CreatedAt = DateTime.UtcNow
                });

                return report;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Consolidation failed for project {id}", projectId);
                return new ConsolidatedReport
                {
                    Error = $"Consolidation failed: {ex.Message}"
                };
            }
        }
        public async Task<List<HistoryItem>> GetAllHistoryAsync(int projectId)
        {
            var all = await _analysisResultRepo.GetByProjectIdAsync(projectId);
            return all.Select(r => new HistoryItem
            {
                ConversationId = r.ConversationId,
                Title = r.Conversation?.Title ?? "Untitled",
                ProjectTitle = r.ProjectTitle ?? "Unknown",
                CreatedAt = r.CreatedAt
            }).ToList();
        }

        // ── Private Helpers ────────────────────────────────────────────────

        private string GenerateTitle(string conversation)
        {
            var firstLine = conversation.Replace("\n", " ").Trim();
            return firstLine.Length > 80
                ? firstLine.Substring(0, 80) + "..."
                : firstLine;
        }

        private List<string> DeserializeList(string? json)
        {
            if (string.IsNullOrEmpty(json)) return new List<string>();
            try { return JsonSerializer.Deserialize<List<string>>(json) ?? new(); }
            catch { return new List<string>(); }
        }

        private ConsolidatedReport ParseConsolidatedReport(string rawJson, int totalCount)
        {
            try
            {
                var clean = rawJson
                    .Replace("```json", "")
                    .Replace("```", "")
                    .Trim();

                using var doc = JsonDocument.Parse(clean);
                var root = doc.RootElement;

                return new ConsolidatedReport
                {
                    ProjectTitle = GetString(root, "projectTitle"),
                    ProjectObjective = GetString(root, "projectObjective"),
                    TotalConversationsAnalyzed = totalCount,
                    GeneratedAt = DateTime.UtcNow,
                    FinalFunctionalRequirements = GetList(root, "finalFunctionalRequirements"),
                    FinalNonFunctionalRequirements = GetList(root, "finalNonFunctionalRequirements"),
                    FinalUserStories = GetList(root, "finalUserStories"),
                    FinalBusinessRules = GetList(root, "finalBusinessRules"),
                    FinalModules = GetList(root, "finalModules"),
                    FinalApiSuggestions = GetList(root, "finalApiSuggestions"),
                    FinalDatabaseEntities = GetList(root, "finalDatabaseEntities"),
                    FinalRoles = GetList(root, "finalRoles"),
                    EvolvedRequirements = GetList(root, "evolvedRequirements"),
                    DroppedRequirements = GetList(root, "droppedRequirements"),
                    NewlyAddedRequirements = GetList(root, "newlyAddedRequirements"),
                    FinalOpenQuestions = GetList(root, "finalOpenQuestions"),
                    FinalRiskFlags = GetList(root, "finalRiskFlags"),
                    FinalMilestones = GetList(root, "finalMilestones"),
                    FinalAssumptions = GetList(root, "finalAssumptions"),
                    DetectedChanges = GetChanges(root),
                    DetectedConflicts = GetConflicts(root),
                    FinalPrioritization = GetFinalPrioritization(root),
                    RawJson = clean
                };
            }
            catch (Exception ex)
            {
                return new ConsolidatedReport
                {
                    Error = $"Failed to parse consolidated report: {ex.Message}",
                    RawJson = rawJson
                };
            }
        }

        private string GetString(JsonElement root, string key)
            => root.TryGetProperty(key, out var val) ? val.GetString() ?? "" : "";

        //private List<string> GetList(JsonElement root, string key)
        //{
        //    if (!root.TryGetProperty(key, out var val)) return new();
        //    return val.EnumerateArray()
        //              .Select(x => x.GetString() ?? "")
        //              .Where(x => !string.IsNullOrEmpty(x))
        //              .ToList();
        //}

        private List<string> GetList(JsonElement root, string key)
        {
            try
            {
                if (!root.TryGetProperty(key, out var val)) return new();
                if (val.ValueKind != JsonValueKind.Array) return new();

                var result = new List<string>();
                foreach (var item in val.EnumerateArray())
                {
                    switch (item.ValueKind)
                    {
                        case JsonValueKind.String:
                            var s = item.GetString();
                            if (!string.IsNullOrEmpty(s)) result.Add(s);
                            break;

                        case JsonValueKind.Object:
                            var parts = new List<string>();
                            foreach (var prop in item.EnumerateObject())
                                parts.Add($"{prop.Name}: {prop.Value}");
                            if (parts.Count > 0)
                                result.Add(string.Join(" | ", parts));
                            break;

                        default:
                            result.Add(item.ToString());
                            break;
                    }
                }
                return result;
            }
            catch { return new(); }
        }

        private List<RequirementChange> GetChanges(JsonElement root)
        {
            if (!root.TryGetProperty("detectedChanges", out var arr)) return new();
            return arr.EnumerateArray().Select(x => new RequirementChange
            {
                Day = GetString(x, "day"),
                Type = GetString(x, "type"),
                OldValue = GetString(x, "oldValue"),
                NewValue = GetString(x, "newValue"),
                Reason = GetString(x, "reason")
            }).ToList();
        }

        private List<RequirementConflict> GetConflicts(JsonElement root)
        {
            if (!root.TryGetProperty("detectedConflicts", out var arr)) return new();
            return arr.EnumerateArray().Select(x => new RequirementConflict
            {
                ConflictDescription = GetString(x, "conflictDescription"),
                FromDay = GetString(x, "fromDay"),
                Resolution = GetString(x, "resolution")
            }).ToList();
        }

        private Prioritization GetFinalPrioritization(JsonElement root)
        {
            if (!root.TryGetProperty("finalPrioritization", out var p)) return new();
            return new Prioritization
            {
                MustHave = GetList(p, "mustHave"),
                ShouldHave = GetList(p, "shouldHave"),
                NiceToHave = GetList(p, "niceToHave")
            };
        }


        public async Task<VideoAnalyzeResponse> AnalyzeVideoAsync(VideoAnalyzeRequest request)
        {
            try
            {
                // Step 1: Validate project
                if (!await _projectRepo.ExistsAsync(request.ProjectId))
                    return new VideoAnalyzeResponse
                    {
                        Error = $"Project {request.ProjectId} not found."
                    };

                // Step 2: Transcribe video → text
                _logger.LogInformation("Transcribing video for project {id}...",
                    request.ProjectId);

                var transcriptResult = await _transcriptionService.TranscribeAsync(
                    request.VideoUrl,
                    request.VideoFilePath);

                if (!transcriptResult.Success)
                    return new VideoAnalyzeResponse
                    {
                        Error = transcriptResult.Error
                    };

                _logger.LogInformation(
                    "Transcript ready. Words: {count}. Sending to Groq for analysis...",
                    transcriptResult.WordCount);

                // Step 3: Feed transcript into existing analysis pipeline
                var analysis = await AnalyzeConversation(
                    request.ProjectId,
                    transcriptResult.Transcript);

                return new VideoAnalyzeResponse
                {
                    ProjectId = request.ProjectId,
                    ConversationId = analysis.ConversationId,
                    Transcript = transcriptResult.Transcript,
                    WordCount = transcriptResult.WordCount,
                    Analysis = analysis,
                    Error = analysis.Error
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Video analysis failed");
                return new VideoAnalyzeResponse
                {
                    Error = $"Video analysis failed: {ex.Message}"
                };
            }
        }

        public async Task<DashboardStats> GetDashboardStatsAsync()
        {
            try
            {
                var allResults = await _analysisResultRepo.GetAllAsync();
                var allProjects = await _projectRepo.GetAllAsync();
                var allConsolidated = await _consolidatedRepo.GetAllAsync();

                var recentActivities = allResults
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(5)
                    .Select(r => new RecentActivity
                    {
                        ConversationId = r.ConversationId,
                        ProjectName = r.Conversation?.Project?.Name ?? "Unknown",
                        ProjectTitle = r.ProjectTitle ?? "Untitled",
                        SourceType = r.Conversation?.SourceType ?? "text",
                        CreatedAt = r.CreatedAt
                    }).ToList();

                return new DashboardStats
                {
                    TotalProjects = allProjects.Count,
                    TotalAnalyses = allResults.Count,
                    TotalVideoAnalyses = allResults.Count(r =>
                        r.Conversation?.SourceType == "video"),
                    TotalConsolidations = allConsolidated.Count,
                    RecentActivities = recentActivities
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get dashboard stats");
                return new DashboardStats();
            }
        }

    }
}