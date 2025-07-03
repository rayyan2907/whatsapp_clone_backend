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

      
        public async Task<bool> sendimgMessage( Image_msg _img)
        {
          

          
            if (_img.image == null || _img.image.Length == 0)
            {
                return false;
            }

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
            if (!allowedTypes.Contains(_img.image.ContentType.ToLower()))
                return false;

            _img.img_url = await _azure.sendImg(_img.image);

            if (_img.img_url == null)
            {
                return false;
            }

            bool isSend = _msg_dl.sendimgMessage(_img);
            if (isSend)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


       
        public async Task<bool> sendvoiceMessage(Audio_msg _audio)
        {
           

            if (_audio.voice == null || _audio.voice.Length == 0)
            {
                return false;
            }


            var allowedTypes = new[] { "audio/mpeg", "audio/wav", "audio/ogg", "audio/mp4" };
            if (!allowedTypes.Contains(_audio.voice.ContentType.ToLower()))
                return false;

            _audio.duration = LengthService.GetAudioDuration(_audio.voice);
            Console.WriteLine(_audio.duration);
            _audio.voice_url = await _azure.sendVoice(_audio.voice);

            if (_audio.voice_url == null)
            {
                return false;
            }

            bool isSend = _msg_dl.sendvoiceMessage(_audio);
            if (isSend)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> sendvideoMessage( Video_msg _video)
        {

            if (_video.video == null || _video.video.Length == 0)
            {
                return false;
            }


            var allowedVideoTypes = new[] { "video/mp4", "video/x-msvideo", "video/x-matroska", "video/webm", "video/quicktime" };

            if (!allowedVideoTypes.Contains(_video.video.ContentType.ToLower()))
                return false;

            _video.duration = LengthService.GetVideoDuration(_video.video);

            Console.WriteLine(_video.duration);
            _video.video_url = await _azure.sendVideo(_video.video);

            if (_video.video_url == null)
            {
                return false;
            }

            bool isSend = _msg_dl.sendvideoMessage(_video);
            if (isSend)
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
