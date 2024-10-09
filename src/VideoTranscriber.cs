using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoTranscription
{
    internal class VideoTranscriber
    {
        private readonly int maxLeters;
        private string ffmpegPath;
        private string voskModelPath;
        private string videoPath;
        private string audioPath;
        private string srtPath;
        internal VideoTranscriber(string ffmpegExecutablePath, string voskModelDirectory, string videoPath, string audioPath, string srtPath, int maxLeters = 15)
        {
            this.maxLeters = maxLeters;
            this.ffmpegPath = ffmpegExecutablePath;
            this.voskModelPath = voskModelDirectory;
            this.videoPath = videoPath;
            this.audioPath = audioPath;
            this.srtPath = srtPath;
        }
        internal void ExtractAudio() {
            Audio audio = new Audio(this.ffmpegPath);
            audio.ExtractAudio(this.videoPath, this.audioPath);
        }
        internal void TranscribeAudioToSRT() {
            Srt srt = new Srt(this.voskModelPath, this.ffmpegPath, this.maxLeters);
          string result=  srt.TranscribeToSrt(this.audioPath);
            using (StreamWriter writer = new StreamWriter(this.srtPath, false, Encoding.UTF8))
            {
                writer.Write(result);
            }
        }
    }
}
