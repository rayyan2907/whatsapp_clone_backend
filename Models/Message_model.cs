namespace whatsapp_clone_backend.Models
{
    public class Message_model
    {
        public int? sender_id { get; set; }
        public int reciever_id { get; set; }
        public string time {  get; set; }
        public string type { get; set; }
        public bool is_seen { get; set; }
    }
}
