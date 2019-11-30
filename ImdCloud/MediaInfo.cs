using System.Collections.Generic;
using System.Diagnostics;

namespace ImdCloud
{
    public static class MediaInfo
    {
        private const string INFORM_PARAMS = "General;%FrameCount%,%FrameRate%,%Duration%";

        public static Dictionary<string, MediaInfoResult> CachedResults = new Dictionary<string, MediaInfoResult>();

        public static MediaInfoResult GetMediaInfo(string sourceUri)
        {
            if (CachedResults.ContainsKey(sourceUri))
            {
                return CachedResults[sourceUri];
            }

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "MediaInfo.exe",
                    Arguments = @$"--Inform=""{INFORM_PARAMS}"" ""{sourceUri}""",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();

            string result = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            var mediaInfo = result.Trim().Split(',');

            CachedResults.Add(sourceUri, new MediaInfoResult()
            {
                FrameCount = int.Parse(mediaInfo[0]),
                FrameRate = float.Parse(mediaInfo[1]),
                DurationSeconds = float.Parse(mediaInfo[2]) / 1000.0f,
            });

            return CachedResults[sourceUri];
        }
            
        public class MediaInfoResult
        {
            public float DurationSeconds { get; set; }

            public float FrameRate { get; set; }

            public int FrameCount { get; set; }
        }
    }
}
