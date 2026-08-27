namespace RequirementAnalysisProject.Services.AI.Prompts
{
    public static class MOMPrompt
    {
        public static string Build(string transcript, string projectTitle)
        {
            var words = transcript.Split(' ');
            if (words.Length > 2000)
                transcript = string.Join(" ", words.Take(2000));

            return $@"You are an expert Meeting Secretary.
Generate professional Minutes of Meeting (MOM) from the transcript below.
Return ONLY valid JSON. No markdown. No explanation.

OUTPUT FORMAT:
{{
  ""minutesOfMeeting"": {{
    ""meetingTitle"": ""{projectTitle} — Meeting"",
    ""date"": ""As per transcript"",
    ""attendees"": [],
    ""agenda"": [],
    ""discussionPoints"": [
      {{
        ""topic"": ""topic title"",
        ""discussion"": ""what was discussed"",
        ""decision"": ""decision made or pending""
      }}
    ],
    ""actionItems"": [
      {{
        ""action"": ""what needs to be done"",
        ""owner"": ""who is responsible"",
        ""dueDate"": ""timeline if mentioned""
      }}
    ],
    ""openIssues"": [],
    ""nextSteps"": [],
    ""nextMeetingDate"": """"
  }}
}}

RULES:
- Extract real names of attendees from transcript
- Each discussion point must have topic + discussion + decision
- Action items must have owner if mentioned
- Open issues = things raised but not resolved
- Next steps = agreed follow-up actions

TRANSCRIPT:
{transcript}";
        }
    }
}
