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
            Console.WriteLine("query called");
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
            Console.WriteLine(query2);
            Console.WriteLine(query);
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

        public bool sendvoiceMessage(Audio_msg _msg)
        {

            var transactions = new List<(string, Dictionary<string, Object>)>();
            string query = "insert into message (sender_id,receiver_id,time,type,is_seen) values (@sender_id,@receiver_id,@time,'voice',@is_seen)";


            var parameters = new Dictionary<string, object>
            {
                { "sender_id" , _msg.sender_id },
                { "receiver_id", _msg.reciever_id },
                { "time", _msg.time },
                { "is_seen", _msg.is_seen }

            };
            transactions.Add((query, parameters));
            string query2 = "insert into voice_msg (msg_id,voice_url,duration) values (last_insert_id(),@voice_url,@duration)";
            var paramters2 = new Dictionary<string, object>
            {
                {"voice_url",_msg.voice_url },
                {"duration",_msg.duration}
            };
            transactions.Add((query2, paramters2));
            return _db.ExecuteTransaction(transactions);
        }

        public bool sendvideoMessage(Video_msg _msg)
        {

            var transactions = new List<(string, Dictionary<string, Object>)>();
            string query = "insert into message (sender_id,receiver_id,time,type,is_seen) values (@sender_id,@receiver_id,@time,'video',@is_seen)";


            var parameters = new Dictionary<string, object>
            {
                { "sender_id" , _msg.sender_id },
                { "receiver_id", _msg.reciever_id },
                { "time", _msg.time },
                { "is_seen", _msg.is_seen }

            };
            transactions.Add((query, parameters));
            string query2 = "insert into video_msg (msg_id,video_url,duration) values (last_insert_id(),@video_url,@duration)";
            var paramters2 = new Dictionary<string, object>
            {
                {"video_url",_msg.video_url },
                {"duration",_msg.duration}
            };
            transactions.Add((query2, paramters2));
            return _db.ExecuteTransaction(transactions);
        }


        public List<Message_recieve_model> getMessages(int senderid, int receiverid, int ofsset)
        {
            string query = "call getMessages(@senderid,@receiverid,@ofsset)";
            var parameters = new Dictionary<string, object>
            {
                { "senderid", senderid },
                { "receiverid", receiverid },
                { "ofsset", ofsset }
            };
            return _db.ExecuteQuery<Message_recieve_model>(query, parameters);
        }
    }
}
