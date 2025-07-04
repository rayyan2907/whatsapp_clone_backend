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

      
       

       
        public async Task<string> sendvoiceMessage(Audio_msg _audio)
        {
            Console.Write("audio service called");

            if (!string.IsNullOrEmpty(_audio.voice_byte))
            {
                Console.WriteLine("conversion called");
                _audio.voice = Base64ToFormFile(_audio.voice_byte, _audio.file_name ?? $"voice_{Guid.NewGuid()}.aac");
            }
            else
            {
                Console.WriteLine("bytes not recieved");
            }

            if (_audio.voice == null || _audio.voice.Length == 0)
            {
                Console.WriteLine("no audio here");
                return "";
            }


            var allowedTypes = new[] { "audio/mpeg", "audio/wav", "audio/ogg", "audio/aac" };
            if (!allowedTypes.Contains(_audio.voice.ContentType.ToLower()))
            {
                Console.WriteLine("format issue");
                return "";

            }

            _audio.duration = await LengthService.GetAudioDuration(_audio.voice);
            Console.WriteLine(_audio.duration);
            _audio.voice_url = await _azure.sendVoice(_audio.voice);

            if (_audio.voice_url == null)
            {
                Console.WriteLine("error in uploading");
                return "";
            }

            bool isSend = _msg_dl.sendvoiceMessage(_audio);
            if (isSend)
            {
                return _audio.voice_url;
            }
            else
            {
                Console.WriteLine("error in data layer");
                return "";
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
