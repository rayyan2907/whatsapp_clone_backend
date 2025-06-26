using whatsapp_clone_backend.Models;

namespace whatsapp_clone_backend.Data
{
    public class Contact_DL
    {
        private readonly DbContext _db;
        public Contact_DL(DbContext db)
        {
            _db = db;
        }

        public List<Contact_model> getContacts(int id)
        {
            var parameters = new Dictionary<string, object>
            {
                { "id", id }
            };
            string query = "select contact_id,last_msg_type,profile_pic_url as contact_dp,first_name,last_name,last_msg_time from contacts c join users u on u.user_id=c.contact_id where c.user_id = @id";

            return _db.ExecuteQuery<Contact_model>(query, parameters);
        }
    }
}
