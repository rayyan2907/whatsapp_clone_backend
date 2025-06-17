using whatsapp_clone_backend.Models;
using ZstdSharp.Unsafe;
using static System.Net.Mime.MediaTypeNames;

namespace whatsapp_clone_backend.Data
{
    public class Message_DL
    {
        private readonly DbContext _db;
        public Message_DL(DbContext db)
        {
            _db = db;
        }

        public bool sendTxtMessage(Text_msg txt)
        {

            var transactions = new List<(string, Dictionary<string, Object>)>();
            string query = "insert into message (sender_id,receiver_id,time,type,is_seen) values (@sender_id,@receiver_id,@time,'msg',@is_seen)";


            var parameters = new Dictionary<string, object>
            {
                { "sender_id" , txt.sender_id },
                { "receiver_id", txt.reciever_id },
                { "time", txt.time },
                { "is_seen", txt.is_seen }

            };
            transactions.Add((query, parameters));
            string query2 = "insert into text_msg (msg_id,text_msg) values (last_insert_id(),@text_msg)";
            var paramters2 = new Dictionary<string, object>
            {
                {"text_msg",txt.text_msg }
            };
            transactions.Add((query2, paramters2));
            return _db.ExecuteTransaction(transactions);


        }

        public bool sendimgMessage(Image_msg img)
        {

            var transactions = new List<(string, Dictionary<string, Object>)>();
            string query = "insert into message (sender_id,receiver_id,time,type,is_seen) values (@sender_id,@receiver_id,@time,'img',@is_seen)";


            var parameters = new Dictionary<string, object>
            {
                { "sender_id" , img.sender_id },
                { "receiver_id", img.reciever_id },
                { "time", img.time },
                { "is_seen", img.is_seen }

            };
            transactions.Add((query, parameters));
            string query2 = "insert into img_msg (msg_id,img_url,caption) values (last_insert_id(),@img_url,@caption)";
            var paramters2 = new Dictionary<string, object>
            {
                {"img_url",img.img_url },
                {"caption",img.caption }
            };
            transactions.Add((query2, paramters2));
            return _db.ExecuteTransaction(transactions);
        }
    }
}
