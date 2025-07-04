using Microsoft.AspNetCore.Mvc;
using whatsapp_clone_backend.Data;
using whatsapp_clone_backend.Models;

namespace whatsapp_clone_backend.Services
{
    public class MessageService
    {

        private readonly Message_DL _msg_dl;
        private Azure_services _azure = new Azure_services();

        public MessageService(Message_DL msg_dl)
        {
            _msg_dl = msg_dl;
        }
        public bool sendtxtMessage(Text_msg txt)
        {
            Console.WriteLine("function called");
           

            Console.WriteLine("sender id is " + txt.sender_id.ToString() + "receiver id is " + txt.reciever_id.ToString());
            bool isSend = _msg_dl.sendTxtMessage(txt);
            if (isSend)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

      
       

       
     
        public async Task<string> sendvideoMessage( Video_msg _video)
        {

            if (_video.video == null || _video.video.Length == 0)
            {
                return "";
            }


            var allowedVideoTypes = new[] { "video/mp4", "video/x-msvideo", "video/x-matroska", "video/webm", "video/quicktime" };

            if (!allowedVideoTypes.Contains(_video.video.ContentType.ToLower()))
                return "";

            _video.duration = LengthService.GetVideoDuration(_video.video);

            Console.WriteLine(_video.duration);
            _video.video_url = await _azure.sendVideo(_video.video);

            if (_video.video_url == null)
            {
                return "";
            }

            bool isSend = _msg_dl.sendvideoMessage(_video);
            if (isSend)
            {
                return _video.video_url;
            }
            else
            {
                return "";
            }
        }

        //audio conversion service
        public static IFormFile? Base64ToFormFile(string base64String, string fileName)
        {
            try
            {
                var bytes = Convert.FromBase64String(base64String);
                var stream = new MemoryStream(bytes);
                return new FormFile(stream, 0, bytes.Length, "voice", fileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "audio/aac"
                };
            }
            catch
            {
                return null;
            }
        }
        //image conversion method
        public static IFormFile? Base64ToImageFormFile(string base64String, string fileName, string contentType = "image/jpeg")
        {
            try
            {
                var bytes = Convert.FromBase64String(base64String);
                var stream = new MemoryStream(bytes);
                return new FormFile(stream, 0, bytes.Length, "image", fileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = contentType
                };
            }
            catch
            {
                return null;
            }
        }


    }
}
