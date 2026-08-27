using System.Net.Http.Headers;
using System.Text.Json;

namespace RequirementAnalysisProject.Services.AI
{
    public class GeminiClientService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<GeminiClientService> _logger;

        public GeminiClientService(
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            ILogger<GeminiClientService> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiKey = config["Groq:ApiKey"]
                          ?? throw new Exception("Groq API key missing");
            _logger = logger;
        }

        //public async Task<string> CallGeminiAsync(string prompt)
        //{
        //    // Trim prompt to safe token size
        //    var words = prompt.Split(' ');
        //    if (words.Length > 3000)
        //    {
        //        prompt = string.Join(" ", words.Take(3000));
        //        _logger.LogWarning("Prompt trimmed to 4000 words");
        //    }

        //    var requestBody = new
        //    {
        //        model = "llama-3.1-8b-instant",
        //        messages = new[]
        //        {
        //            new
        //            {
        //                role    = "system",
        //                content = "You are ARIA, a Business Analyst AI. Return complete JSON only."
        //            },
        //            new
        //            {
        //                role    = "user",
        //                content = prompt
        //            }
        //        },
        //        temperature = 0.3,
        //        max_tokens = 4000
        //    };

        //    // Retry up to 5 times with wait
        //    int maxRetries = 5;
        //    int retryDelay = 45; // seconds

        //    for (int attempt = 1; attempt <= maxRetries; attempt++)
        //    {
        //        try
        //        {
        //            _logger.LogInformation(
        //                "Groq API call attempt {attempt}/{max}...",
        //                attempt, maxRetries);

        //            _httpClient.DefaultRequestHeaders.Clear();
        //            _httpClient.DefaultRequestHeaders.Add(
        //                "Authorization", $"Bearer {_apiKey}");

        //            var response = await _httpClient.PostAsJsonAsync(
        //                "https://api.groq.com/openai/v1/chat/completions",
        //                requestBody);

        //            // ── Rate limited → wait and retry ─────────────────────
        //            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        //            {
        //                var errorBody = await response.Content.ReadAsStringAsync();

        //                // Try to extract wait time from error message
        //                var waitSeconds = ExtractWaitTime(errorBody);
        //                if (waitSeconds == 0) waitSeconds = retryDelay;

        //                _logger.LogWarning(
        //                    "Rate limited. Waiting {sec}s before retry {attempt}/{max}...",
        //                    waitSeconds, attempt, maxRetries);

        //                await Task.Delay(TimeSpan.FromSeconds(waitSeconds + 5));
        //                continue; 
        //            }

        //            // ── Other error ────────────────────────────────────────
        //            if (!response.IsSuccessStatusCode)
        //            {
        //                var error = await response.Content.ReadAsStringAsync();
        //                throw new Exception($"Groq API Error: {error}");
        //            }

        //            // ── Success ────────────────────────────────────────────
        //            var result = await response.Content.ReadAsStringAsync();

        //            using var doc = JsonDocument.Parse(result);
        //            var text = doc.RootElement
        //                .GetProperty("choices")[0]
        //                .GetProperty("message")
        //                .GetProperty("content")
        //                .GetString();

        //            _logger.LogInformation("Groq API call successful on attempt {attempt}",
        //                attempt);

        //            return text ?? throw new Exception("Groq returned empty response");
        //        }
        //        catch (Exception ex) when (attempt < maxRetries
        //            && ex.Message.Contains("rate_limit"))
        //        {
        //            _logger.LogWarning(
        //                "Rate limit exception. Waiting {sec}s...", retryDelay);
        //            await Task.Delay(TimeSpan.FromSeconds(retryDelay));
        //        }
        //    }

        //    throw new Exception(
        //        "Groq API rate limit exceeded after 5 retries. " +
        //        "Please wait a few minutes and try again, " +
        //        "or upgrade at https://console.groq.com/settings/billing");
        //}

        //// ── Extract wait time from Groq error message ──────────────────────
        //private int ExtractWaitTime(string errorBody)
        //{
        //    try
        //    {
        //        // Groq error says "Please try again in 43.26s"
        //        var match = System.Text.RegularExpressions.Regex.Match(
        //            errorBody, @"try again in (\d+)");

        //        if (match.Success)
        //            return int.Parse(match.Groups[1].Value);
        //    }
        //    catch { }
        //    return 0;
        //}

        public async Task<string> CallGeminiAsync(string prompt)
        {
            // ── Smart token trimmer ────────────────────────────────────────
            prompt = TrimPromptToTokenLimit(prompt, maxWords: 3500);

            //    var requestBody = new
            //    {
            //        model = "llama-3.1-8b-instant",
            //        messages = new[]
            //        {
            //    new
            //    {
            //        role    = "system",
            //        content = "You are ARIA, a Business Analyst AI. Return complete JSON only. No markdown."
            //    },
            //    new
            //    {
            //        role    = "user",
            //        content = prompt
            //    }
            //},
            //        temperature = 0.3,
            //        max_tokens = 2000  // ← reduced from 4000
            //    };

            var requestBody = new
            {
                //model = "llama-3.1-8b-instant",
                //messages = new[]

                model = "openai/gpt-oss-20b",  // ← NEW model name
                messages = new[]
    {
        new
        {
            role    = "system",
            content = "You are ARIA, a Business Analyst AI. Return complete valid JSON only. Never truncate."
        },
        new { role = "user", content = prompt }
    },
                temperature = 0.3,
                max_tokens = 3000  // ← increased
            };


            int maxRetries = 5;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    _logger.LogInformation("Groq API call attempt {attempt}/{max}...",
                        attempt, maxRetries);

                    _httpClient.DefaultRequestHeaders.Clear();
                    _httpClient.DefaultRequestHeaders.Add(
                        "Authorization", $"Bearer {_apiKey}");

                    var response = await _httpClient.PostAsJsonAsync(
                        "https://api.groq.com/openai/v1/chat/completions",
                        requestBody);

                    if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        var errorBody = await response.Content.ReadAsStringAsync();
                        var waitSecs = ExtractWaitTime(errorBody);
                        if (waitSecs == 0) waitSecs = 45;

                        _logger.LogWarning("Rate limited. Waiting {sec}s...", waitSecs);
                        await Task.Delay(TimeSpan.FromSeconds(waitSecs + 5));
                        continue;
                    }

                    if (!response.IsSuccessStatusCode)
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        throw new Exception($"Groq API Error: {error}");
                    }

                    var result = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(result);

                    var text = doc.RootElement
                        .GetProperty("choices")[0]
                        .GetProperty("message")
                        .GetProperty("content")
                        .GetString();

                    _logger.LogInformation("Groq API call successful on attempt {attempt}",
                        attempt);

                    return text ?? throw new Exception("Groq returned empty response");
                }
                catch (Exception ex) when (attempt < maxRetries
                    && (ex.Message.Contains("rate_limit") || ex.Message.Contains("429")))
                {
                    _logger.LogWarning("Rate limit exception. Waiting 45s...");
                    await Task.Delay(TimeSpan.FromSeconds(45));
                }
            }

            throw new Exception("Groq API rate limit exceeded after 5 retries.");
        }

        // ── Trim prompt to stay under token limit ──────────────────────────────
        private string TrimPromptToTokenLimit(string prompt, int maxWords)
        {
            var words = prompt.Split(' ');
            if (words.Length <= maxWords) return prompt;

            _logger.LogWarning("Prompt too large ({count} words). Trimming to {max}...",
                words.Length, maxWords);

            // Keep the instruction part (first 800 words) + trimmed transcript
            var instructionWords = words.Take(800).ToArray();
            var remainingBudget = maxWords - 800;
            var transcriptWords = words.Skip(800).Take(remainingBudget).ToArray();

            return string.Join(" ", instructionWords) + " " +
                   string.Join(" ", transcriptWords);
        }

        private int ExtractWaitTime(string errorBody)
        {
            try
            {
                var minMatch = System.Text.RegularExpressions.Regex.Match(
                    errorBody, @"(\d+)m(\d+)");
                if (minMatch.Success)
                {
                    int mins = int.Parse(minMatch.Groups[1].Value);
                    int secs = int.Parse(minMatch.Groups[2].Value);
                    return (mins * 60) + secs + 5;
                }

                var secMatch = System.Text.RegularExpressions.Regex.Match(
                    errorBody, @"in (\d+)s");
                if (secMatch.Success)
                    return int.Parse(secMatch.Groups[1].Value) + 5;
            }
            catch { }
            return 0;
        }
    }
}