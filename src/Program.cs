using VideoTranscription;

string ffmpegPath = @"ffmpeg/bin/ffmpeg.exe";
// Generar una marca de tiempo
string timestamp = DateTime.UtcNow.ToString("yyyy_MM_dd_HHmmss");
string voskModelPath = @"vosk-model-small-es-0.42";
Console.WriteLine("Ruta del video:");
string videoPath = Console.ReadLine();
string rutaficheros = Path.Combine(Path.GetDirectoryName(videoPath),
                      $"{Path.GetFileNameWithoutExtension(videoPath)}_{timestamp}");
string audioPath = $"{rutaficheros}.wav";

// Crear el srtPath con la marca de tiempo antes de la extensión
string srtPath = $"{rutaficheros}.srt";
int maxLeters = 15;
Console.WriteLine("Maximo de letras:");
string leters=Console.ReadLine();
if(!string.IsNullOrEmpty(leters))
 int.TryParse(leters, out maxLeters);

var transcriber = new VideoTranscriber(ffmpegPath, voskModelPath,videoPath,audioPath,srtPath, maxLeters);
transcriber.ExtractAudio();
transcriber.TranscribeAudioToSRT();

Console.WriteLine("Transcription completed.");