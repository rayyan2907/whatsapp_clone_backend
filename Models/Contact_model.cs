namespace whatsapp_clone_backend.Models
{
    public class Contact_model
    {
        public int user_id {  get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string profile_pic_url { get; set; }
        public string last_msg_time { get; set; }
        public string last_msg_type { get; set; }

    }
}
