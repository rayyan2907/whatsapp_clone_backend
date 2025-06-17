using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using whatsapp_clone_backend.Data;
using whatsapp_clone_backend.Models;
using whatsapp_clone_backend.Services;

namespace whatsapp_clone_backend.Controllers
{
    [ApiController]
    //[Authorize]
    public class MessageController : ControllerBase
    {
        private readonly Message_DL _msg_dl;
        private Azure_services _azure = new Azure_services();

        public MessageController (Message_DL msg_dl)
        {
            _msg_dl = msg_dl;
        }


        [HttpPost]
        [Route("sendtext")]
        public IActionResult sendtxtMessage(Text_msg txt)
        {
            bool isSend=_msg_dl.sendTxtMessage(txt);
            if (isSend)
            {
                return Ok(isSend);
            }
            else
            {
                return BadRequest(isSend);
            }
        }

        [HttpPost]
        [Route("sendimage")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> sendimgMessage([FromForm] Image_msg _img)
        {
            
            if (_img.image== null || _img.image.Length == 0)
            {
                return BadRequest(new { message = "no image recieved" });
            }

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
            if (!allowedTypes.Contains(_img.image.ContentType.ToLower()))
                return BadRequest(new { message = "Only image files are allowed (JPEG, PNG, etc.)" });

            _img.img_url = await _azure.sendImg(_img.image);

            if(_img.img_url == null)
            {
                return BadRequest(new { message = "error in uploading image" });
            }

            bool isSend = _msg_dl.sendimgMessage(_img);
            if (isSend)
            {
                return Ok(isSend);
            }
            else
            {
                return BadRequest(isSend);
            }
        }


        [HttpPost]
        [Route("sendvoice")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> sendvoiceMessage([FromForm] Audio_msg _audio)
        {
           

            if (_audio.voice == null || _audio.voice.Length == 0)
            {
                return BadRequest(new { message = "no audio recieved" });
            }


            var allowedTypes = new[] { "audio/mpeg", "audio/wav", "audio/ogg", "audio/mp4" };
            if (!allowedTypes.Contains(_audio.voice.ContentType.ToLower()))
                return BadRequest(new { message = "Only audio files are allowed (MP3, WAV, etc.)" });

            _audio.duration=AudioLengthService.GetAudioDuration(_audio.voice);
            Console.WriteLine(_audio.duration);
            _audio.voice_url = await _azure.sendVoice(_audio.voice);

            if (_audio.voice_url == null)
            {
                return BadRequest(new { message = "error in sending audio" });
            }

            bool isSend = _msg_dl.sendvoiceMessage(_audio);
            if (isSend)
            {
                return Ok(isSend);
            }
            else
            {
                return BadRequest(isSend);
            }
        }





    }
}
