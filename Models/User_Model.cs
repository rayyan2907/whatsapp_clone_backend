namespace whatsapp_clone_backend.Models
{
    public class User_Model
    {
        public int user_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string profile_pic_url { get; set; }
        public DateTime date_of_birth { get; set; }

    }
}
