using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoTranscription
{
    internal class Audio
    {
        string ffmpegPath;
        public Audio(string ffmpegPath) { 
        this.ffmpegPath = ffmpegPath;
        }
        internal void ExtractAudio( string videoPath, string audioOutputPath)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = this.ffmpegPath,
                Arguments = $"-i \"{videoPath}\" -vn -acodec pcm_s16le -ar 16000 -ac 1 \"{audioOutputPath}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(startInfo))
            {
                process.WaitForExit();
            }
        }
        internal float GetAudioDurationInSeconds(string audioFilePath)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = this.ffmpegPath,
                Arguments = $"-i \"{audioFilePath}\" -hide_banner",
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process ffmpeg = new Process { StartInfo = startInfo })
            {
                ffmpeg.Start();
                string output = ffmpeg.StandardError.ReadToEnd();
                ffmpeg.WaitForExit();

                string durationLine = output.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
                    .FirstOrDefault(line => line.Contains("Duration"));

                if (durationLine != null)
                {
                    string duration = durationLine.Split(',')[0].Split(new[] { "Duration: " }, StringSplitOptions.None)[1].Trim();
                    TimeSpan durationTime = TimeSpan.Parse(duration);
                    return (float)durationTime.TotalSeconds;
                }
            }

            return 0.0f; // Devuelve 0 si no se encuentra la duración
        }
    }
}
