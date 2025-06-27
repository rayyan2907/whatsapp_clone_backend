using whatsapp_clone_backend.Models;

namespace whatsapp_clone_backend.Data
{
    public class DpSet_DL
    {
        private readonly DbContext _db;
        public DpSet_DL (DbContext dbContext)
        {
            _db = dbContext;
        }

        public bool changePicture(string url,int user_id)
        {

            string query2 = "update users set profile_picture_url=@url where user_id = @user_id";
            var parameters2 = new Dictionary<string, object>
            {
                { "user_id", user_id },
                {"url", url}

            };
            int rows= _db.ExecuteNonQuery(query2, parameters2);
            return rows > 0;

        }
        public string getOldDp(int id)
        {
            string query = "select user_id,email,profile_pic_url,date_of_birth,first_name,last_name from users where user_id = @user_id";
            var parameters = new Dictionary<string, object>
            {
                { "user_id", id }
            };
            User_Model user = _db.ExecuteQuery<User_Model>(query, parameters).FirstOrDefault();
            return user.profile_pic_url;
        }
    }
}
