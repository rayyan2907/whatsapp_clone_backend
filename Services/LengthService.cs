using MediaToolkit;
using MediaToolkit.Model;
using Xabe.FFmpeg;
using System.IO;

namespace whatsapp_clone_backend.Services
{
    public static class LengthService
    {
        public static async Task<string> GetAudioDuration(IFormFile audio)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "ffmpeg");
            FFmpeg.SetExecutablesPath(path);

            var tempFilePath = Path.Combine(Path.GetTempPath(), audio.FileName);
            try
            {
                using (var stream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await audio.CopyToAsync(stream);
                }

                var mediaInfo = await FFmpeg.GetMediaInfo(tempFilePath);
                var audioStream = mediaInfo.AudioStreams.FirstOrDefault();
                if (audioStream != null)
                {
                    var duration = audioStream.Duration;
                    return duration.ToString(@"hh\:mm\:ss");
                }
            }
            finally
            {
                // Cleanup
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }

            return "00:00:00";
        }


        public static string GetVideoDuration(IFormFile video)
        {
            // Save to a temporary file first
            var tempFilePath = Path.GetTempFileName();
            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                video.CopyTo(stream);
            }

            var inputFile = new MediaFile { Filename = tempFilePath };

            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile);
                TimeSpan duration = inputFile.Metadata.Duration;

                // Clean up
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }

                return duration.ToString(@"hh\:mm\:ss");
            }
        }

    }
}
