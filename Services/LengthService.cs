using MediaToolkit;
using MediaToolkit.Model;


using System.IO;

namespace whatsapp_clone_backend.Services
{
    public static class LengthService
    {
      
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
