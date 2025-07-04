namespace whatsapp_clone_backend.Models
{
    public class _Message
    {
        public int sender_id { get; set; }
        public int reciever_id { get; set; }
        public string type { get; set; }
        public bool is_seen { get; set; }
        public string? text_msg { get; set; }
        public IFormFile? img { get; set; }
        public IFormFile? video { get; set; }
        public IFormFile? voice { get; set; }
        public string? voice_byte { get; set; }
        public string? file_name { get; set; }
        public string? img_url { get; set; }
        public string? video_url { get; set; }
        public string? voice_url { get; set; }
        public string? caption { get; set; }
        public string? duration { get; set; }
        public string time { get;  set; }
        public bool is_sent { get; set; }
    }
}
