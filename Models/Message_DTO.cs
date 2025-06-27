namespace whatsapp_clone_backend.Models
{
    public class Message_DTO
    {
        public int message_id {  get; set; }
        public bool is_sent { get; set; }
        public int sender_id { get; set; }
        public int reciever_id { get; set; }
        public string type { get; set; }
        public bool is_seen { get; set; }
        public string? text_msg { get; set; }
        public string? img_url { get; set; }
        public string? video_url { get; set; }
        public string? voice_url { get; set; }
        public string? caption { get; set; }
        public string? duration { get; set; }
        public string time { get;  set; }

    }
}
