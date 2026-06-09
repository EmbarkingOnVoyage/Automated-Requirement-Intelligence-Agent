using RequirementAnalysisProject.Models;
using RequirementAnalysisProject.Services.AI.Prompts;
using System.Text.Json;

namespace RequirementAnalysisProject.Services.AI
{
    public class AnalysisOrchestrator
    {
        private readonly GeminiClientService _geminiClient;
        private readonly ILogger<AnalysisOrchestrator> _logger;

        public AnalysisOrchestrator(
            GeminiClientService geminiClient,
            ILogger<AnalysisOrchestrator> logger)
        {
            _geminiClient = geminiClient;
            _logger = logger;
        }

        //public async Task<AnalyzeResponse> RunAsync(string transcript)
        //{
        //    try
        //    {
        //        // Step 1: Clean transcript
        //        _logger.LogInformation("Step 1: Cleaning transcript...");
        //        var cleaned = CleanTranscript(transcript);

        //        // Step 2: Check token size — if too large, summarize first
        //        _logger.LogInformation("Step 2: Checking transcript size...");
        //        var wordCount = cleaned.Split(' ').Length;
        //        _logger.LogInformation("Word count: {count}", wordCount);

        //        // If transcript is too long → summarize first then analyze
        //        if (wordCount > 2000)
        //        {
        //            _logger.LogInformation("Transcript too long. Summarizing first...");
        //            cleaned = await SummarizeTranscriptAsync(cleaned);
        //            _logger.LogInformation("Summarized to {count} words",
        //                cleaned.Split(' ').Length);
        //        }

        //        // Step 3: Build prompt
        //        _logger.LogInformation("Step 3: Building prompt...");
        //        var prompt = RequirementPrompt.Build(cleaned);

        //        // Step 4: Call Groq
        //        _logger.LogInformation("Step 4: Calling Groq AI...");
        //        var rawJson = await _geminiClient.CallGeminiAsync(prompt);

        //        // Step 5: Parse response
        //        _logger.LogInformation("Step 5: Parsing AI response...");
        //        return ParseResponse(rawJson);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Orchestrator failed");
        //        return new AnalyzeResponse
        //        {
        //            Error = $"Analysis failed: {ex.Message}"
        //        };
        //    }
        //}

        //private async Task<string> SummarizeTranscriptAsync(string transcript)
        //{
        //    // Split into chunks of 1500 words each
        //    var words = transcript.Split(' ');
        //    var chunks = new List<string>();
        //    var chunkSize = 800;

        //    for (int i = 0; i < words.Length; i += chunkSize)
        //    {
        //        var chunk = string.Join(" ", words.Skip(i).Take(chunkSize));
        //        chunks.Add(chunk);
        //    }

        //    _logger.LogInformation("Split into {count} chunks", chunks.Count);

        //    // Summarize each chunk
        //    var summaries = new List<string>();
        //    int chunkNumber = 1;

        //    foreach (var chunk in chunks)
        //    {
        //        _logger.LogInformation("Summarizing chunk {n}/{total}...",
        //            chunkNumber, chunks.Count);

        //        var summaryPrompt = $@"
        //          You are a meeting transcript summarizer.
        //          Summarize the following meeting transcript segment.
        //          Keep ALL requirements, decisions, numbers, percentages, and key points.
        //          Remove only small talk and repetition.
        //          Return plain text summary only — no JSON, no markdown.

        //         TRANSCRIPT SEGMENT:
        //          {chunk}
        //          ";
        //        var summary = await _geminiClient.CallGeminiAsync(summaryPrompt);
        //        summaries.Add($"[Segment {chunkNumber}]\n{summary}");
        //        chunkNumber++;
        //    }

        //    // Combine all summaries
        //    return string.Join("\n\n", summaries);
        //}

        //public async Task<AnalyzeResponse> RunAsync(string transcript)
        //{
        //    try
        //    {
        //        //clean
        //        _logger.LogInformation("Step 1: Cleaning transcript...");
        //        var cleaned = CleanTranscript(transcript);

        //        var wordCount = cleaned.Split(' ').Length;
        //        _logger.LogInformation("Word count: {count}", wordCount);

        //        // ← Lowered from 2000 to 1500
        //        if (wordCount > 1500)
        //        {
        //            _logger.LogInformation("Transcript too long. Summarizing first...");
        //            cleaned = await SummarizeTranscriptAsync(cleaned);
        //            _logger.LogInformation("Summarized to {count} words",
        //                cleaned.Split(' ').Length);
        //        }

        //        _logger.LogInformation("Step 2: Building prompt...");
        //        var prompt = RequirementPrompt.Build(cleaned);

        //        _logger.LogInformation("Step 3: Calling Groq AI...");
        //        var rawJson = await _geminiClient.CallGeminiAsync(prompt);

        //        _logger.LogInformation("Step 4: Parsing response...");
        //        return ParseResponse(rawJson);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Orchestrator failed");
        //        return new AnalyzeResponse
        //        {
        //            Error = $"Analysis failed: {ex.Message}"
        //        };
        //    }
        //}

        public async Task<AnalyzeResponse> RunAsync(string transcript)
        {
            try
            {
                // Step 1: Clean
                _logger.LogInformation("Step 1: Cleaning transcript...");
                var cleaned = CleanTranscript(transcript);

                var wordCount = cleaned.Split(' ').Length;
                _logger.LogInformation("Word count: {count}", wordCount);

                if (wordCount > 2000)
                {
                    _logger.LogInformation("Transcript too long. Summarizing...");
                    cleaned = await SummarizeTranscriptAsync(cleaned);
                }

                // Step 2: Requirements Analysis
                _logger.LogInformation("Step 2: Analyzing requirements...");
                var prompt = RequirementPrompt.Build(cleaned);
                var rawJson = await _geminiClient.CallGeminiAsync(prompt);
                var result = ParseResponse(rawJson);

                // Step 3: Generate MOM
                _logger.LogInformation("Step 3: Generating MOM...");
                await Task.Delay(2000); // avoid rate limit
                var momJson = await _geminiClient.CallGeminiAsync(
                    MOMPrompt.Build(cleaned, result.ProjectTitle));
                result.MinutesOfMeeting = ParseMOM(momJson);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Orchestrator failed");
                return new AnalyzeResponse { Error = $"Analysis failed: {ex.Message}" };
            }
        }

        private string ParseMOM(string rawJson)
        {
            try
            {
                var clean = rawJson
                    .Replace("```json", "")
                    .Replace("```", "")
                    .Trim();

                clean = RepairTruncatedJson(clean);

                // Return as formatted string for easy display
                using var doc = JsonDocument.Parse(clean);
                var mom = doc.RootElement;

                if (!mom.TryGetProperty("minutesOfMeeting", out var momObj))
                    return clean;

                var sb = new System.Text.StringBuilder();

                sb.AppendLine($"# {GetString(momObj, "meetingTitle")}");
                sb.AppendLine($"**Date:** {GetString(momObj, "date")}");
                sb.AppendLine();

                // Attendees
                if (momObj.TryGetProperty("attendees", out var attendees))
                {
                    sb.AppendLine("## Attendees");
                    foreach (var a in attendees.EnumerateArray())
                        sb.AppendLine($"- {a.GetString()}");
                    sb.AppendLine();
                }

                // Agenda
                if (momObj.TryGetProperty("agenda", out var agenda))
                {
                    sb.AppendLine("## Agenda");
                    foreach (var a in agenda.EnumerateArray())
                        sb.AppendLine($"- {a.GetString()}");
                    sb.AppendLine();
                }

                // Discussion Points
                if (momObj.TryGetProperty("discussionPoints", out var points))
                {
                    sb.AppendLine("## Discussion Points");
                    int i = 1;
                    foreach (var p in points.EnumerateArray())
                    {
                        sb.AppendLine($"### {i}. {GetString(p, "topic")}");
                        sb.AppendLine($"**Discussion:** {GetString(p, "discussion")}");
                        sb.AppendLine($"**Decision:** {GetString(p, "decision")}");
                        sb.AppendLine();
                        i++;
                    }
                }

                // Action Items
                if (momObj.TryGetProperty("actionItems", out var actions))
                {
                    sb.AppendLine("## Action Items");
                    foreach (var a in actions.EnumerateArray())
                    {
                        sb.AppendLine($"- **{GetString(a, "action")}**");
                        sb.AppendLine($"  - Owner: {GetString(a, "owner")}");
                        sb.AppendLine($"  - Due: {GetString(a, "dueDate")}");
                    }
                    sb.AppendLine();
                }

                // Open Issues
                if (momObj.TryGetProperty("openIssues", out var issues))
                {
                    sb.AppendLine("## Open Issues");
                    foreach (var issue in issues.EnumerateArray())
                        sb.AppendLine($"- {issue.GetString()}");
                    sb.AppendLine();
                }

                // Next Steps
                if (momObj.TryGetProperty("nextSteps", out var steps))
                {
                    sb.AppendLine("## Next Steps");
                    foreach (var step in steps.EnumerateArray())
                        sb.AppendLine($"- {step.GetString()}");
                    sb.AppendLine();
                }

                var nextMeeting = GetString(momObj, "nextMeetingDate");
                if (!string.IsNullOrEmpty(nextMeeting))
                    sb.AppendLine($"**Next Meeting:** {nextMeeting}");

                return sb.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogWarning("MOM parse failed: {msg}", ex.Message);
                return rawJson;
            }
        }

        private async Task<string> SummarizeTranscriptAsync(string transcript)
        {
            var words = transcript.Split(' ');
            var chunks = new List<string>();
            var chunkSize = 800; // ← reduced from 1500

            for (int i = 0; i < words.Length; i += chunkSize)
            {
                var chunk = string.Join(" ", words.Skip(i).Take(chunkSize));
                chunks.Add(chunk);
            }

            _logger.LogInformation("Split into {count} chunks", chunks.Count);

            var summaries = new List<string>();
            int chunkNumber = 1;

            foreach (var chunk in chunks)
            {
                _logger.LogInformation("Summarizing chunk {n}/{total}...",
                    chunkNumber, chunks.Count);

                var summaryPrompt = $"Extract key requirements, decisions, numbers, rules from this text. Be very brief:\n\n{chunk}";

                var summary = await _geminiClient.CallGeminiAsync(summaryPrompt);
                summaries.Add(summary);
                chunkNumber++;

                if (chunkNumber <= chunks.Count)
                    await Task.Delay(3000); // ← wait between chunks
            }

            return string.Join("\n\n", summaries);
        }

        // ─── Private Helpers ────────────────────────────────────────────────

        private string CleanTranscript(string transcript)
        {
            return transcript
                .Replace("\r\n", " ")
                .Replace("\r", " ")
                .Replace("\n", " ")
                .Trim();
        }

        //private AnalyzeResponse ParseResponse(string rawJson)
        //{
        //    try
        //    {
        //        // Strip markdown code fences if Gemini adds them
        //        var clean = rawJson
        //            .Replace("```json", "")
        //            .Replace("```", "")
        //            .Trim();

        //        using var doc = JsonDocument.Parse(clean);
        //        var root = doc.RootElement;

        //        return new AnalyzeResponse
        //        {
        //            ProjectTitle = GetString(root, "projectTitle"),
        //            ProjectObjective = GetString(root, "projectObjective"),
        //            FunctionalRequirements = GetList(root, "functionalRequirements"),
        //            NonFunctionalRequirements = GetList(root, "nonFunctionalRequirements"),
        //            UserStories = GetList(root, "userStories"),
        //            BusinessRules = GetList(root, "businessRules"),
        //            Assumptions = GetList(root, "assumptions"),
        //            OpenQuestions = GetList(root, "openQuestions"),
        //            Modules = GetList(root, "modules"),
        //            ApiSuggestions = GetList(root, "apiSuggestions"),
        //            DatabaseEntities = GetList(root, "databaseEntities"),
        //            RawJson = clean
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new AnalyzeResponse
        //        {
        //            Error = $"Failed to parse AI response: {ex.Message}",
        //            RawJson = rawJson
        //        };
        //    }
        //}

        //private string GetString(JsonElement root, string key)
        //{
        //    return root.TryGetProperty(key, out var val)
        //        ? val.GetString() ?? string.Empty
        //        : string.Empty;
        //}

        //private List<string> GetList(JsonElement root, string key)
        //{
        //    if (!root.TryGetProperty(key, out var val))
        //        return new List<string>();

        //    return val.EnumerateArray()
        //              .Select(x => x.GetString() ?? string.Empty)
        //              .Where(x => !string.IsNullOrEmpty(x))
        //              .ToList();
        //}

        //private AnalyzeResponse ParseResponse(string rawJson)
        //{
        //    try
        //    {
        //        var clean = rawJson
        //            .Replace("```json", "")
        //            .Replace("```", "")
        //            .Trim();

        //        using var doc = JsonDocument.Parse(clean);
        //        var root = doc.RootElement;

        //        return new AnalyzeResponse
        //        {
        //            ProjectTitle = GetString(root, "projectTitle"),
        //            ProjectObjective = GetString(root, "projectObjective"),
        //            FunctionalRequirements = GetListSafe(root, "functionalRequirements"),
        //            NonFunctionalRequirements = GetListSafe(root, "nonFunctionalRequirements"),
        //            UserStories = GetListSafe(root, "userStories"),
        //            BusinessRules = GetListSafe(root, "businessRules"),
        //            Assumptions = GetListSafe(root, "assumptions"),
        //            OpenQuestions = GetListSafe(root, "openQuestions"),
        //            Modules = GetListSafe(root, "modules"),
        //            ApiSuggestions = GetListSafe(root, "apiSuggestions"),
        //            DatabaseEntities = GetListSafe(root, "databaseEntities"),
        //            Roles = GetListSafe(root, "roles"),
        //            CommunicationGaps = GetListSafe(root, "communicationGaps"),
        //            RiskFlags = GetListSafe(root, "riskFlags"),
        //            SuggestedMilestones = GetListSafe(root, "suggestedMilestones"),
        //            Prioritization = GetPrioritizationSafe(root),
        //            RawJson = clean
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new AnalyzeResponse
        //        {
        //            Error = $"Failed to parse AI response: {ex.Message}",
        //            RawJson = rawJson
        //        };
        //    }
        //}

        private AnalyzeResponse ParseResponse(string rawJson)
        {
            try
            {
                var clean = rawJson
                    .Replace("```json", "")
                    .Replace("```", "")
                    .Trim();

                // ── Auto-repair truncated JSON ─────────────────────────────
                clean = RepairTruncatedJson(clean);

                using var doc = JsonDocument.Parse(clean);
                var root = doc.RootElement;

                return new AnalyzeResponse
                {
                    ProjectTitle = GetString(root, "projectTitle"),
                    ProjectObjective = GetString(root, "projectObjective"),
                    FunctionalRequirements = GetListSafe(root, "functionalRequirements"),
                    NonFunctionalRequirements = GetListSafe(root, "nonFunctionalRequirements"),
                    UserStories = GetListSafe(root, "userStories"),
                    BusinessRules = GetListSafe(root, "businessRules"),
                    Assumptions = GetListSafe(root, "assumptions"),
                    OpenQuestions = GetListSafe(root, "openQuestions"),
                    Modules = GetListSafe(root, "modules"),
                    ApiSuggestions = GetListSafe(root, "apiSuggestions"),
                    DatabaseEntities = GetListSafe(root, "databaseEntities"),
                    Roles = GetListSafe(root, "roles"),
                    CommunicationGaps = GetListSafe(root, "communicationGaps"),
                    RiskFlags = GetListSafe(root, "riskFlags"),
                    SuggestedMilestones = GetListSafe(root, "suggestedMilestones"),
                    Prioritization = GetPrioritizationSafe(root),
                    RawJson = clean
                };
            }
            catch (Exception ex)
            {
                return new AnalyzeResponse
                {
                    Error = $"Failed to parse AI response: {ex.Message}",
                    RawJson = rawJson
                };
            }
        }

        // ── Repair truncated JSON by closing open brackets ─────────────────────
        private string RepairTruncatedJson(string json)
        {
            if (string.IsNullOrEmpty(json)) return "{}";

            try
            {
                // Test if already valid
                JsonDocument.Parse(json);
                return json;
            }
            catch
            {
                // Count open brackets and close them
                int openCurly = json.Count(c => c == '{') - json.Count(c => c == '}');
                int openSquare = json.Count(c => c == '[') - json.Count(c => c == ']');

                var repaired = new System.Text.StringBuilder(json.TrimEnd());

                // Remove trailing comma if present
                if (repaired.Length > 0 && repaired[^1] == ',')
                {
                    repaired.Remove(repaired.Length - 1, 1);
                }

                // Close any open string
                int quoteCount = json.Count(c => c == '"');
                if (quoteCount % 2 != 0)
                    repaired.Append('"');

                // Close open arrays
                for (int i = 0; i < openSquare; i++)
                    repaired.Append(']');

                // Close open objects
                for (int i = 0; i < openCurly; i++)
                    repaired.Append('}');

                var repairedStr = repaired.ToString();

                // Verify repair worked
                try
                {
                    JsonDocument.Parse(repairedStr);
                    _logger.LogWarning("JSON was truncated — repaired successfully");
                    return repairedStr;
                }
                catch
                {
                    // If repair failed — return empty valid response
                    _logger.LogError("JSON repair failed — returning empty response");
                    return @"{
                ""projectTitle"": ""Analysis incomplete"",
                ""projectObjective"": ""Response was truncated. Please try again with shorter transcript."",
                ""functionalRequirements"": [],
                ""nonFunctionalRequirements"": [],
                ""userStories"": [],
                ""businessRules"": [],
                ""assumptions"": [],
                ""openQuestions"": [],
                ""modules"": [],
                ""apiSuggestions"": [],
                ""databaseEntities"": [],
                ""roles"": [],
                ""communicationGaps"": [],
                ""riskFlags"": [],
                ""prioritization"": { ""mustHave"": [], ""shouldHave"": [], ""niceToHave"": [] },
                ""suggestedMilestones"": []
            }";
                }
            }
        }

        // ── Safe string getter ─────────────────────────────────────────────────
        private string GetString(JsonElement root, string key)
        {
            try
            {
                if (!root.TryGetProperty(key, out var val)) return string.Empty;
                return val.ValueKind == JsonValueKind.String
                    ? val.GetString() ?? string.Empty
                    : val.ToString();
            }
            catch { return string.Empty; }
        }

        // ── Safe list getter — handles string, object, array of anything ───────
        private List<string> GetListSafe(JsonElement root, string key)
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
                        // ✅ Normal string item
                        case JsonValueKind.String:
                            var str = item.GetString();
                            if (!string.IsNullOrEmpty(str))
                                result.Add(str);
                            break;

                        // ✅ Object item → flatten to key: value string
                        case JsonValueKind.Object:
                            var parts = new List<string>();
                            foreach (var prop in item.EnumerateObject())
                            {
                                var propVal = prop.Value.ValueKind == JsonValueKind.String
                                    ? prop.Value.GetString() ?? ""
                                    : prop.Value.ToString();
                                parts.Add($"{prop.Name}: {propVal}");
                            }
                            if (parts.Count > 0)
                                result.Add(string.Join(" | ", parts));
                            break;

                        // ✅ Number or bool → convert to string
                        case JsonValueKind.Number:
                        case JsonValueKind.True:
                        case JsonValueKind.False:
                            result.Add(item.ToString());
                            break;

                        // ✅ Nested array → flatten
                        case JsonValueKind.Array:
                            foreach (var nested in item.EnumerateArray())
                            {
                                var nestedStr = nested.ValueKind == JsonValueKind.String
                                    ? nested.GetString() ?? ""
                                    : nested.ToString();
                                if (!string.IsNullOrEmpty(nestedStr))
                                    result.Add(nestedStr);
                            }
                            break;

                        default:
                            break;
                    }
                }

                return result;
            }
            catch { return new(); }
        }

        // ── Safe prioritization getter ─────────────────────────────────────────
        private Prioritization GetPrioritizationSafe(JsonElement root)
        {
            try
            {
                if (!root.TryGetProperty("prioritization", out var p))
                    return new Prioritization();

                return new Prioritization
                {
                    MustHave = GetListSafe(p, "mustHave"),
                    ShouldHave = GetListSafe(p, "shouldHave"),
                    NiceToHave = GetListSafe(p, "niceToHave")
                };
            }
            catch { return new Prioritization(); }
        }
    }
}