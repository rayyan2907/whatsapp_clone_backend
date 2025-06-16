
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
            Console.WriteLine("Rows returned: " + users);

            if (Convert.ToInt32(users)>0)
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
