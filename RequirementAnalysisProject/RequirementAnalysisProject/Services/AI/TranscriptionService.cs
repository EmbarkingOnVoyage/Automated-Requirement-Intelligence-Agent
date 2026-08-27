using System.Net.Http.Headers;
using System.Text.Json;
using Xabe.FFmpeg;

namespace RequirementAnalysisProject.Services.AI
{
    public class TranscriptionService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<TranscriptionService> _logger;
        private readonly string _tempFolder;

        public TranscriptionService(
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            ILogger<TranscriptionService> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiKey = config["Groq:ApiKey"]
                          ?? throw new Exception("Groq API key missing");
            _logger = logger;
            _tempFolder = Path.Combine(Path.GetTempPath(), "RequirementAgent");

            Directory.CreateDirectory(_tempFolder);
            FFmpeg.SetExecutablesPath(@"C:\ffmpeg\bin");
        }

        // ── Main Entry ─────────────────────────────────────────────────────
        public async Task<TranscriptResult> TranscribeAsync(
            string? videoUrl, string? videoFilePath)
        {
            string? localVideoPath = null;
            var audioChunks = new List<(string path, double startOffset)>();

            try
            {
                // Step 1: Get local video file
                if (!string.IsNullOrEmpty(videoUrl))
                {
                    if (IsYouTubeUrl(videoUrl))
                        localVideoPath = await DownloadYouTubeAsync(videoUrl);
                    else
                        localVideoPath = await DownloadDirectAsync(videoUrl);
                }
                else if (!string.IsNullOrEmpty(videoFilePath))
                {
                    if (!File.Exists(videoFilePath))
                        return Fail($"File not found: {videoFilePath}");
                    localVideoPath = videoFilePath;
                }
                else
                {
                    return Fail("No video source provided.");
                }

                // Step 2: Get video duration
                _logger.LogInformation("Getting video duration...");
                var duration = await GetVideoDurationAsync(localVideoPath);
                _logger.LogInformation("Video duration: {dur} seconds", duration);

                // Step 3: Split audio into chunks with offsets
                _logger.LogInformation("Splitting audio into chunks...");
                audioChunks = await SplitAudioIntoChunksAsync(localVideoPath, duration);
                _logger.LogInformation("Created {count} audio chunks", audioChunks.Count);

                // Step 4: Transcribe each chunk with cumulative offset
                var transcriptParts = new List<string>();
                int chunkNumber = 1;

                foreach (var (chunkPath, startOffset) in audioChunks)
                {
                    _logger.LogInformation("Transcribing chunk {n}/{total}...",
                        chunkNumber, audioChunks.Count);

                    var part = await TranscribeChunkWithRetryAsync(chunkPath, startOffset);
                    transcriptParts.Add(part);

                    _logger.LogInformation("Chunk {n} done. Words: {w}",
                        chunkNumber, part.Split(' ').Length);

                    chunkNumber++;

                    if (chunkNumber <= audioChunks.Count)
                        await Task.Delay(2000);
                }

                // Step 5: Join all parts
                var fullTranscript = string.Join("\n", transcriptParts);

                _logger.LogInformation("Full transcript ready. Total words: {w}",
                    fullTranscript.Split(' ').Length);

                return new TranscriptResult
                {
                    Success = true,
                    Transcript = fullTranscript,
                    WordCount = fullTranscript.Split(' ').Length
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Transcription failed");
                return Fail($"Transcription failed: {ex.Message}");
            }
            finally
            {
                // Cleanup chunks
                foreach (var (chunkPath, _) in audioChunks)
                    CleanupFile(chunkPath);

                // Cleanup downloaded video
                if (!string.IsNullOrEmpty(videoUrl))
                    CleanupFile(localVideoPath);
            }
        }

        // ── Get Video Duration ─────────────────────────────────────────────
        private async Task<double> GetVideoDurationAsync(string videoPath)
        {
            var mediaInfo = await FFmpeg.GetMediaInfo(videoPath);
            return mediaInfo.Duration.TotalSeconds;
        }

        // ── Split Audio into 4-minute chunks with offsets ──────────────────
        private async Task<List<(string path, double startOffset)>> SplitAudioIntoChunksAsync(
            string videoPath, double totalSeconds)
        {
            var chunks = new List<(string path, double startOffset)>();
            int chunkSecs = 240;
            int chunkNum = 1;
            double start = 0;

            while (start < totalSeconds)
            {
                var chunkPath = Path.Combine(
                    _tempFolder, $"chunk_{chunkNum}_{Guid.NewGuid()}.mp3");

                _logger.LogInformation("Extracting chunk {n}: {start}s to {end}s",
                    chunkNum, start, start + chunkSecs);

                await FFmpeg.Conversions.New()
                    .AddParameter($"-i \"{videoPath}\"")
                    .AddParameter($"-ss {start}")
                    .AddParameter($"-t {chunkSecs}")
                    .AddParameter("-vn")
                    .AddParameter("-ar 16000")
                    .AddParameter("-ac 1")
                    .AddParameter("-b:a 64k")
                    .AddParameter($"\"{chunkPath}\"")
                    .Start();

                if (File.Exists(chunkPath))
                {
                    var sizeMB = new FileInfo(chunkPath).Length / 1024.0 / 1024.0;
                    _logger.LogInformation("Chunk {n} size: {size:F2}MB", chunkNum, sizeMB);
                    chunks.Add((chunkPath, start));
                }

                start += chunkSecs;
                chunkNum++;
            }

            return chunks;
        }

        // ── Transcribe Chunk with Retry ────────────────────────────────────
        private async Task<string> TranscribeChunkWithRetryAsync(
            string audioPath, double startOffset)
        {
            int maxRetries = 5;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    return await TranscribeChunkAsync(audioPath, startOffset);
                }
                catch (Exception ex) when (ex.Message.Contains("rate_limit")
                    || ex.Message.Contains("429"))
                {
                    if (attempt == maxRetries) throw;

                    var waitSecs = ExtractWaitTime(ex.Message);
                    if (waitSecs == 0) waitSecs = 30;

                    _logger.LogWarning(
                        "Rate limited on chunk. Waiting {sec}s... (attempt {a}/{max})",
                        waitSecs, attempt, maxRetries);

                    await Task.Delay(TimeSpan.FromSeconds(waitSecs + 3));
                }
            }

            throw new Exception("Max retries exceeded for chunk transcription.");
        }

        // ── Transcribe Single Chunk ────────────────────────────────────────
        private async Task<string> TranscribeChunkAsync(
            string audioPath, double startOffset)
        {
            await using var fs = File.OpenRead(audioPath);
            using var form = new MultipartFormDataContent();
            var fileContent = new StreamContent(fs);
            fileContent.Headers.ContentType =
                new MediaTypeHeaderValue("audio/mpeg");

            form.Add(fileContent, "file", Path.GetFileName(audioPath));
            //form.Add(new StringContent("whisper-large-v3"), "model");
            form.Add(new StringContent("whisper-large-v3-turbo"), "model");
            form.Add(new StringContent("en"), "language");
            form.Add(new StringContent("verbose_json"), "response_format");
            form.Add(new StringContent("segment"), "timestamp_granularities[]");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await _httpClient.PostAsync(
                "https://api.groq.com/openai/v1/audio/transcriptions", form);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Groq Whisper Error: {error}");
            }

            var result = await response.Content.ReadAsStringAsync();
            return FormatTranscriptWithSpeakers(result, startOffset);
        }

        // ── Format transcript with speaker labels + timestamps ─────────────
        private string FormatTranscriptWithSpeakers(
            string rawResponse, double startOffset = 0)
        {
            try
            {
                using var doc = JsonDocument.Parse(rawResponse);
                var root = doc.RootElement;

                if (!root.TryGetProperty("segments", out var segments))
                {
                    var plainText = root.GetProperty("text").GetString() ?? string.Empty;
                    return $"User A [{FormatTimestamp(startOffset)}]: {plainText}\n";
                }

                var lines = new List<string>();
                var speakerMap = new Dictionary<int, string>();
                int speakerIndex = 0;
                double lastEnd = 0;
                int currentSpeaker = 0;
                var currentText = new System.Text.StringBuilder();
                double blockStart = startOffset;

                speakerMap[0] = "User A";

                foreach (var segment in segments.EnumerateArray())
                {
                    var text = segment.GetProperty("text").GetString()?.Trim() ?? "";
                    var start = (segment.TryGetProperty("start", out var s)
                        ? s.GetDouble() : 0) + startOffset;
                    var end = (segment.TryGetProperty("end", out var e)
                        ? e.GetDouble() : 0) + startOffset;

                    bool newTurn = (start - lastEnd) > 2.0 && currentText.Length > 0;

                    if (newTurn)
                    {
                        lines.Add($"{speakerMap[currentSpeaker]} [{FormatTimestamp(blockStart)}]: {currentText.ToString().Trim()}");
                        lines.Add(string.Empty);
                        currentText.Clear();

                        speakerIndex++;
                        currentSpeaker = speakerIndex;
                        if (!speakerMap.ContainsKey(currentSpeaker))
                        {
                            char letter = (char)('A' + (speakerIndex % 26));
                            speakerMap[currentSpeaker] = $"User {letter}";
                        }
                        blockStart = start;
                    }
                    else if (currentText.Length == 0)
                    {
                        blockStart = start;
                    }

                    currentText.Append(" " + text);
                    lastEnd = end;
                }

                // Last block
                if (currentText.Length > 0)
                {
                    lines.Add($"{speakerMap[currentSpeaker]} [{FormatTimestamp(blockStart)}]: {currentText.ToString().Trim()}");
                    lines.Add(string.Empty);
                }

                return string.Join("\n", lines);
            }
            catch
            {
                try
                {
                    using var doc = JsonDocument.Parse(rawResponse);
                    var text = doc.RootElement.GetProperty("text").GetString() ?? string.Empty;
                    return $"User A [{FormatTimestamp(startOffset)}]: {text}\n";
                }
                catch { return rawResponse; }
            }
        }

        // ── Format seconds → M:SS ──────────────────────────────────────────
        private string FormatTimestamp(double seconds)
        {
            var ts = TimeSpan.FromSeconds(seconds);
            int mins = (int)ts.TotalMinutes;
            int secs = ts.Seconds;
            return $"{mins}:{secs:D2}";
        }

        // ── Download Direct URL ────────────────────────────────────────────
        private async Task<string> DownloadDirectAsync(string url)
        {
            url = ConvertToDirectUrl(url);

            var filePath = Path.Combine(_tempFolder, $"video_{Guid.NewGuid()}.mp4");

            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(15);
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Download failed: {response.StatusCode}");

            await using var fs = File.Create(filePath);
            await response.Content.CopyToAsync(fs);

            _logger.LogInformation("Downloaded: {size}MB",
                new FileInfo(filePath).Length / 1024 / 1024);

            return filePath;
        }

        // ── Download YouTube ───────────────────────────────────────────────
        private async Task<string> DownloadYouTubeAsync(string url)
        {
            var outputPath = Path.Combine(_tempFolder, $"yt_{Guid.NewGuid()}.mp4");

            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "yt-dlp",
                    Arguments = $"-f \"best[ext=mp4][height<=480]\" -o \"{outputPath}\" \"{url}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            await process.WaitForExitAsync();

            if (!File.Exists(outputPath))
                throw new Exception("yt-dlp download failed.");

            return outputPath;
        }

        // ── Convert share links to direct download ─────────────────────────
        private string ConvertToDirectUrl(string url)
        {
            if (url.Contains("1drv.ms") || url.Contains("onedrive.live.com"))
                return url.Contains("?") ? url + "&download=1" : url + "?download=1";

            if (url.Contains("drive.google.com/file/d/"))
            {
                var fileId = url.Split("/d/")[1].Split("/")[0];
                return $"https://drive.google.com/uc?export=download&id={fileId}";
            }

            return url;
        }

        private bool IsYouTubeUrl(string url) =>
            url.Contains("youtube.com") || url.Contains("youtu.be");

        private int ExtractWaitTime(string errorMessage)
        {
            try
            {
                // Matches "1m35s" format
                var minMatch = System.Text.RegularExpressions.Regex.Match(
                    errorMessage, @"(\d+)m(\d+)");
                if (minMatch.Success)
                {
                    int mins = int.Parse(minMatch.Groups[1].Value);
                    int secs = int.Parse(minMatch.Groups[2].Value);
                    return (mins * 60) + secs + 5;
                }

                // Matches plain "in 43s"
                var secMatch = System.Text.RegularExpressions.Regex.Match(
                    errorMessage, @"in (\d+)s");
                if (secMatch.Success)
                    return int.Parse(secMatch.Groups[1].Value) + 5;
            }
            catch { }
            return 60;
        }

        private static TranscriptResult Fail(string error) =>
            new() { Success = false, Error = error };

        private void CleanupFile(string? path)
        {
            try
            {
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                    File.Delete(path);
            }
            catch { }
        }
    }

    public class TranscriptResult
    {
        public bool Success { get; set; }
        public string Transcript { get; set; } = string.Empty;
        public int WordCount { get; set; }
        public string Error { get; set; } = string.Empty;
    }
}