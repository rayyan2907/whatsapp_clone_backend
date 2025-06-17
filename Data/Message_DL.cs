using whatsapp_clone_backend.Models;
using ZstdSharp.Unsafe;

namespace whatsapp_clone_backend.Data
{
    public class Message_DL
    {
        private readonly DbContext _db;
        public Message_DL (DbContext db)
        {
            _db = db;   
        }

        public bool sendMessage(Text_msg txt)
        {
            string query = "insert into message (sender_id,receiver_id,time,type,is_seen) values (@sender_id,@receiver_id,@time,@type,@is_seen)";


            var parameters = new Dictionary<string, object>
            {
                { "sender_id" , txt.sender_id },
                { "receiver_id", txt.reciever_id },
                { "time", txt.time },
                { "type", txt.type },
                { "is_seen", txt.is_seen }

            };

            int rows = _db.ExecuteNonQuery (query, parameters);
            if (rows > 0)
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
