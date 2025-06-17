using NAudio.Wave;

namespace whatsapp_clone_backend.Services
{
    public static class AudioLengthService
    {

        public static string GetAudioDuration(IFormFile audio)
    {
        using (var stream = audio.OpenReadStream())
        using (var reader = new Mp3FileReader(stream)) // or WaveFileReader for .wav
        {
            TimeSpan duration = reader.TotalTime;
            return duration.ToString(@"hh\:mm\:ss"); // Format to string
        }
    }

}
}
