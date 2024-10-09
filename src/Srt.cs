using System;
using System.IO;
using System.Text;
using Vosk;

namespace VideoTranscription
{
    internal class Srt
    {
        string modelPath;
        int maxLetters; // Este será el límite de caracteres, ahora establecido en 15
        string ffmpegPath;

        internal Srt(string modelPath, string ffmpegPath, int maxLetters)
        {
            this.modelPath = modelPath;
            this.maxLetters = maxLetters; // Ahora se refiere a caracteres máximos por línea, ajustar a 15 en la creación
            this.ffmpegPath = ffmpegPath;
        }

        internal string TranscribeToSrt(string audioFilePath)
        {
            StringBuilder srtContent = new StringBuilder();
            Vosk.Vosk.SetLogLevel(0);
            Model model = new Model(this.modelPath);
            using (var rec = new VoskRecognizer(model, 16000))
            {
                rec.SetMaxAlternatives(0);
                rec.SetWords(true);
                using (var stream = File.OpenRead(audioFilePath))
                {
                    byte[] buffer = new byte[4096];
                    int subtitleIndex = 1;
                    StringBuilder currentBlock = new StringBuilder();
                    float lastStart = 0;
                    float lastEnd = 0;
                    float silenceThreshold = 0.1f; // 100 milisegundos
                    VoskResult voskResult = new VoskResult();

                    while (stream.Read(buffer, 0, buffer.Length) > 0)
                    {
                        if (rec.AcceptWaveform(buffer, buffer.Length))
                        {
                            var result = rec.Result();
                            ProcessResults(voskResult, result,  ref lastStart, ref lastEnd, ref subtitleIndex, ref currentBlock, ref srtContent, silenceThreshold);
                        }
                        else
                        {
                            Console.WriteLine("Partial Result: " + rec.PartialResult());
                        }
                    }
                    ProcessResults(voskResult, rec.FinalResult(),  ref lastStart, ref lastEnd, ref subtitleIndex, ref currentBlock, ref srtContent, silenceThreshold);

                    if (currentBlock.Length > 0)
                    {
                        srtContent.Append(FormatSrtBlock(subtitleIndex, currentBlock.ToString().Trim(), lastStart, new Audio(this.ffmpegPath).GetAudioDurationInSeconds(audioFilePath)));
                    }
                }
            }

            return srtContent.ToString();
        }

        private void ProcessResults(VoskResult voskResult, string results,  ref float lastStart, ref float lastEnd, ref int subtitleIndex, ref StringBuilder currentBlock, ref StringBuilder srtContent, float silenceThreshold)
        {
            var words = voskResult.VoskResultToWords(results);
            foreach (var word in words)
            {
                // Comprobamos silencio y longitud de línea
                if (word.Start - lastEnd > silenceThreshold || currentBlock.Length + word.Text.Length + 1 > this.maxLetters)
                {
                    // Iniciar un nuevo bloque de subtítulo si hay silencio suficiente o si se excede el límite de caracteres
                    if (currentBlock.Length > 0)
                    {
                        srtContent.Append(FormatSrtBlock(subtitleIndex++, currentBlock.ToString(), lastStart, lastEnd));
                        currentBlock.Clear();
                    }
                    lastStart = word.Start;
                }
                currentBlock.Append($"{word.Text} ");
                lastEnd = word.End;
            }
        }

        internal string FormatSrtBlock(int index, string text, float start, float end)
        {
            return $"{index}\n{TimeSpan.FromSeconds(start):hh\\:mm\\:ss\\,fff} --> {TimeSpan.FromSeconds(end):hh\\:mm\\:ss\\,fff}\n{text}\n\n";
        }
    }
}
