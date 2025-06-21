using Google.Protobuf.Collections;
using whatsapp_clone_backend.Models;

namespace whatsapp_clone_backend.Data
{
    public class SearchUser_DL
    {
        private readonly DbContext _db;

        public SearchUser_DL (DbContext db)
        {
            _db = db;
        }

        public List<User_Model> seacrchByEmail(string prefix)
        {
            string query = "select user_id,email,profile_pic_url,date_of_birth,first_name,last_name from users where email like @prefix";
            var parameters = new Dictionary<string, object>
            {
                { "prefix", "%" + prefix + "%" }
            };
            return _db.ExecuteQuery<User_Model>(query, parameters); 
        }
    }
}
