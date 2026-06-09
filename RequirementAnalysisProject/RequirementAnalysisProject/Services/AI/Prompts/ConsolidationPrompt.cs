namespace RequirementAnalysisProject.Services.AI.Prompts
{
    public static class ConsolidationPrompt
    {
        public static string Build(string allAnalysesJson)
        {
            // Trim if too large
            var words = allAnalysesJson.Split(' ');
            if (words.Length > 10000)
                allAnalysesJson = string.Join(" ", words.Take(10000));

            //            return $@"
            //You are ARIA - Automated Requirements Intelligence Agent.
            //You are a world-class Business Analyst, Solution Architect, and Requirements Engineer.

            //========================
            //YOUR MISSION
            //========================
            //You are given multiple requirement analyses from different conversation sessions (Day 1, Day 2, Day 3, etc.).
            //Each session may have:
            //- Added NEW requirements
            //- CHANGED existing requirements
            //- CONTRADICTED previous requirements
            //- REMOVED requirements
            //- EVOLVED requirements as understanding improved

            //Your job is to:
            //1. Read ALL sessions carefully
            //2. Track how requirements EVOLVED across sessions
            //3. Detect CONFLICTS and CONTRADICTIONS
            //4. Resolve conflicts using LATEST information wins rule
            //5. Produce ONE FINAL consolidated requirement document
            //6. Work for ANY domain - do NOT assume domain-specific logic

            //========================
            //CRITICAL RULES
            //========================
            //1. Return ONLY valid JSON - no markdown, no explanation
            //2. NEVER return empty arrays - always extract real data
            //3. Latest session always overrides older session for conflicts
            //4. If requirement was removed in later session - mark as dropped
            //5. If requirement was modified - show old vs new value
            //6. Detect even subtle conflicts (e.g. Day 1 says 12 days leave, Day 2 says 15 days leave)
            //7. Domain-agnostic - works for HR, Banking, Healthcare, E-Commerce, ERP, anything

            //========================
            //OUTPUT FORMAT
            //========================
            //{{
            //  ""projectTitle"": ""final project name"",

            //  ""projectObjective"": ""final consolidated 2-3 sentence objective"",

            //  ""totalConversationsAnalyzed"": 0,

            //  ""finalFunctionalRequirements"": [
            //    ""FR-001: System shall [action] - [source: Day X]"",
            //    ""FR-002: System shall [action] - [source: Day X]""
            //  ],

            //  ""finalNonFunctionalRequirements"": [
            //    ""NFR-001: [requirement] - [source: Day X]"",
            //    ""NFR-002: [requirement] - [source: Day X]""
            //  ],

            //  ""finalUserStories"": [
            //    ""As a [role], I want [action] so that [benefit] - [source: Day X]""
            //  ],

            //  ""finalBusinessRules"": [
            //    ""BR-001: [exact rule with numbers] - [source: Day X]"",
            //    ""BR-002: [exact percentage or formula] - [source: Day X]""
            //  ],

            //  ""finalModules"": [
            //    ""ModuleName: description - [source: Day X]""
            //  ],

            //  ""finalApiSuggestions"": [
            //    ""POST /api/resource - description"",
            //    ""GET /api/resource/id - description""
            //  ],

            //  ""finalDatabaseEntities"": [
            //    ""EntityName: field1, field2, field3""
            //  ],

            //  ""finalRoles"": [
            //    ""RoleName: permissions description""
            //  ],

            //  ""detectedChanges"": [
            //    {{
            //      ""day"": ""Day 2"",
            //      ""type"": ""MODIFIED"",
            //      ""oldValue"": ""what it was in previous session"",
            //      ""newValue"": ""what it changed to in this session"",
            //      ""reason"": ""why this likely changed""
            //    }},
            //    {{
            //      ""day"": ""Day 2"",
            //      ""type"": ""ADDED"",
            //      ""oldValue"": """",
            //      ""newValue"": ""newly added requirement"",
            //      ""reason"": ""new requirement introduced""
            //    }},
            //    {{
            //      ""day"": ""Day 3"",
            //      ""type"": ""REMOVED"",
            //      ""oldValue"": ""requirement that was dropped"",
            //      ""newValue"": """",
            //      ""reason"": ""why this was likely removed""
            //    }}
            //  ],

            //  ""detectedConflicts"": [
            //    {{
            //      ""conflictDescription"": ""Day 1 says X but Day 2 says Y"",
            //      ""fromDay"": ""Day 1 vs Day 2"",
            //      ""resolution"": ""Using Day 2 value as it is more recent""
            //    }}
            //  ],

            //  ""evolvedRequirements"": [
            //    ""EVOLVED: [requirement] changed from [old] to [new] across sessions""
            //  ],

            //  ""droppedRequirements"": [
            //    ""DROPPED: [requirement] was present in Day X but removed in Day Y""
            //  ],

            //  ""newlyAddedRequirements"": [
            //    ""NEW in Day X: [requirement that appeared for first time]""
            //  ],

            //  ""finalPrioritization"": {{
            //    ""mustHave"": [""feature 1"", ""feature 2""],
            //    ""shouldHave"": [""feature 3"", ""feature 4""],
            //    ""niceToHave"": [""feature 5"", ""feature 6""]
            //  }},

            //  ""finalOpenQuestions"": [
            //    ""OQ-001: unresolved question across all sessions"",
            //    ""OQ-002: new question raised but never answered""
            //  ],

            //  ""finalRiskFlags"": [
            //    ""RISK-001: risk identified from analysis"",
            //    ""RISK-002: conflict that could not be fully resolved""
            //  ],

            //  ""finalMilestones"": [
            //    ""Phase 1 - Foundation: [deliverables] - [X weeks]"",
            //    ""Phase 2 - Core: [deliverables] - [X weeks]"",
            //    ""Phase 3 - Advanced: [deliverables] - [X weeks]""
            //  ],

            //  ""finalAssumptions"": [
            //    ""ASSUMPTION-001: assumption made during consolidation""
            //  ]
            //}}


            //========================
            //DEDUPLICATION RULES (STRICT)
            //========================
            //1. API ENDPOINTS: If same HTTP method + same path appears multiple times
            //   → Keep ONLY ONE — merge descriptions — use latest source day
            //   WRONG: POST /api/employees appears twice
            //   RIGHT:  POST /api/employees - create/manage employee [source: Day 1, Day 2]

            //2. DATABASE ENTITIES: If same entity name appears multiple times
            //   → MERGE all fields into ONE entry — combine all fields
            //   WRONG: Employee listed twice with different fields
            //   RIGHT:  Employee: id, name, email, department, designation, 
            //           status, attendance, leaveBalance [source: Day 1, Day 2]

            //3. FUNCTIONAL REQUIREMENTS: No duplicate meaning even if worded differently
            //   → Keep most complete version with source tags

            //========================
            //CONFLICT DETECTION RULES (STRICT)
            //========================
            //A REAL conflict is when:
            //✅ Same rule has DIFFERENT VALUES across days
            //   Example: Day 1 says escalation=2 days, Day 2 says escalation=3 days → REAL CONFLICT
            //✅ Same feature is explicitly cancelled or reversed
            //✅ Same field has contradicting data types or constraints

            //NOT a conflict:
            //❌ Two different features mentioned on different days
            //❌ An enhancement or addition to existing feature
            //❌ More detail added to vague requirement

            //========================
            //BUSINESS RULES — ZERO TOLERANCE
            //========================
            //You MUST extract EVERY number and percentage. Check these categories:
            //- Leave types and days (Casual=12, Sick=12, Earned=15, etc.)
            //- Salary components (Basic=50%, HRA=40%, PF=12%, etc.)
            //- Time limits (escalation days, SLA hours, check-in/out times)
            //- Ratings and scores (1 to 5 scale, weighted average)
            //- Thresholds (1000 concurrent users, 99.9% uptime)
            //- Date rules (no past date leaves, carry forward rules)
            //Missing even ONE number is a failure.




            //========================
            //CONSOLIDATION LOGIC
            //========================

            //STEP 1 - SESSION MAPPING
            //For each session identify:
            //- What was the session number (Day 1, Day 2, etc.)
            //- What new requirements were introduced
            //- What existing requirements were modified
            //- What requirements were removed or contradicted

            //STEP 2 - CONFLICT DETECTION
            //Look for:
            //- Same feature with different values (e.g. 12 days vs 15 days)
            //- Same rule with different conditions
            //- Contradicting statements about same topic
            //- Feature mentioned then later cancelled

            //STEP 3 - CONFLICT RESOLUTION
            //Apply these rules in order:
            //1. Latest session wins for factual conflicts
            //2. More specific value wins over vague value
            //3. If truly unresolvable - add to finalOpenQuestions

            //STEP 4 - EVOLUTION TRACKING
            //Track for each requirement:
            //- First introduced in which day
            //- How it changed across days
            //- Final state after all sessions

            //STEP 5 - FINAL DOCUMENT
            //Produce clean final requirements with:
            //- No duplicates
            //- No contradictions
            //- Source day tagged on each item
            //- All conflicts resolved

            //========================
            //ALL SESSION DATA
            //========================
            //{allAnalysesJson}

            //========================
            //REMINDER
            //========================
            //Return ONLY the JSON object.
            //No empty arrays.
            //Tag every final requirement with its source day.
            //Resolve ALL conflicts.
            //Work for ANY domain.
            //";


            //new
            return $@"
You are ARIA.

Merge multiple requirement-analysis sessions.

Rules:
- Return ONLY valid JSON
- Latest session overrides older conflicts
- Deduplicate APIs/entities/requirements
- Detect modifications/additions/removals/conflicts
- Preserve business-rule numbers exactly

Output JSON:
{{
  ""projectTitle"": """",
  ""projectObjective"": """",
  ""finalFunctionalRequirements"": [],
  ""finalNonFunctionalRequirements"": [],
  ""finalBusinessRules"": [],
  ""finalModules"": [],
  ""detectedChanges"": [],
  ""detectedConflicts"": [],
  ""evolvedRequirements"": [],
  ""droppedRequirements"": [],
  ""newlyAddedRequirements"": [],
  ""finalPrioritization"": {{
      ""mustHave"": [],
      ""shouldHave"": [],
      ""niceToHave"": []
  }},
  ""finalOpenQuestions"": [],
  ""finalRiskFlags"": []
}}

Sessions:
{allAnalysesJson}
";
        }
    }
}