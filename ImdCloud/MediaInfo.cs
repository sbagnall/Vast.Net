using System.Diagnostics;

namespace ImdCloud
{
    public class MediaInfo
    {
        private const string INFORM_PARAMS = "General;%FrameCount%,%FrameRate%,%Duration%";

        private static MediaInfoResult cachedResult = null;

        private string sourceUri;

        public MediaInfo(string sourceUri)
        {
            this.sourceUri = sourceUri;
        }

        public MediaInfoResult GetMediaInfo()
        {
            if (cachedResult != null)
            {
                return cachedResult;
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

            cachedResult = new MediaInfoResult()
            {
                FrameCount = int.Parse(mediaInfo[0]),
                FrameRate = float.Parse(mediaInfo[1]),
                DurationSeconds = float.Parse(mediaInfo[2]) / 1000.0f,
            };

            return cachedResult;
        }
            
        public class MediaInfoResult
        {
            public float DurationSeconds { get; set; }

            public float FrameRate { get; set; }

            public int FrameCount { get; set; }
        }
    }
}
