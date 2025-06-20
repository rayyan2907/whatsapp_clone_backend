namespace whatsapp_clone_backend.Models
{
    public class Video_msg : Message_model
    {
        public IFormFile video { get; set; }   
        public string? video_url { get; set; }
        public string? caption { get; set; }
        public string? duration { get; set; }

    }
}
