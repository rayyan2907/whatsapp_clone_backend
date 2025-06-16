using Org.BouncyCastle.Ocsp;
using whatsapp_clone_backend.Models;

namespace whatsapp_clone_backend.Data
{
    public class Registration_DL
    {
        private readonly DbContext _db;

        public Registration_DL(DbContext db)
        {
            _db= db;
        }

        public bool userReg(object reg)
        {
            var user = new User_Model
            {
                first_name = reg.GetType().GetProperty("first_name")?.GetValue(reg)?.ToString(),
                last_name = reg.GetType().GetProperty("last_name")?.GetValue(reg)?.ToString(),
                email = reg.GetType().GetProperty("email")?.GetValue(reg)?.ToString(),
                password = reg.GetType().GetProperty("password")?.GetValue(reg)?.ToString(),
                date_of_birth = (DateTime)reg.GetType().GetProperty("date_of_birth")?.GetValue(reg),
                profile_pic_url = reg.GetType().GetProperty("profile_pic_url")?.GetValue(reg)?.ToString()
            };


            string query = "insert into users (email,password,profile_pic_url,date_of_birth,first_name,last_name) values (@email,@password,@profile_pic_url,@date_of_birth,@first_name,@last_name)";
            var parameters = new Dictionary<string, object>
            {
                { "email" , user.email },
                { "password", user.password },
                { "first_name", user.first_name },
                { "last_name", user.last_name },
                { "profile_pic_url", user.profile_pic_url},
                { "date_of_birth", user.date_of_birth }


            };
            Console.WriteLine(query);
            int rows = _db.ExecuteNonQuery(query, parameters);
            return rows > 0;
        }

        public bool checkUser(string email)
        {
            var parameters = new Dictionary<string, object>
            {
                { "email", email }
            };
            string query = "select count(*) from users where email=@email";

            var users = _db.ExecuteScalar(query, parameters);

            if (Convert.ToInt32(users) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
