
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using whatsapp_clone_backend.Models;

namespace whatsapp_clone_backend.Data

{
    public class Login_DL
    {
        private readonly DbContext _db;
        public Login_DL(DbContext db) 
        {
            _db = db;
        }

        public bool checkUser(string email)
        {
            var parameters = new Dictionary<string, object>
            {
                { "email", email }
            };
            string query = "select count(*) from users where email=@email";         
            
            var users = _db.ExecuteScalar(query,parameters);

            if (Convert.ToInt32(users)>0)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public bool checkPassword(Login_model login)
        {
            var parameters = new Dictionary<string, object>
            {
                { "email" , login.email },
                { "password", login.password }
            };
            string query = "select count(*) from users where email=@email and password=@password";

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


        public User_Model GetUser(Login_model login)
        {
            var parameters = new Dictionary<string, object>
            {
                { "email" , login.email },
                { "password", login.password }
            };
            string query = "select user_id,first_name,last_name,date_of_birth,email,profile_pic_url,password from users where email=@email and password=@password";

            var user = _db.ExecuteQuery<User_Model>(query, parameters);

            return user.FirstOrDefault();
        }


       

    }
}
