using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using whatsapp_clone_backend.Data;
using whatsapp_clone_backend.Models;
using whatsapp_clone_backend.Services;
using static System.Net.Mime.MediaTypeNames;

namespace whatsapp_clone_backend.Controllers
{
    [ApiController]
    [Route("message")]
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
            Console.WriteLine("function called");
            var userIdClaim = User.FindFirst("user_id"); // custom claim name from token

            if (userIdClaim == null)
                return Unauthorized("You have been Logged Out.");

            txt.sender_id = int.Parse(userIdClaim.Value);
            if (txt.sender_id == null)
                return Unauthorized("You have been Logged Out.");


            Console.WriteLine("sender id is "+txt.sender_id.ToString()+"receiver id is "+ txt.reciever_id.ToString());
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
            var userIdClaim = User.FindFirst("user_id"); // custom claim name from token

            if (userIdClaim == null)
                return Unauthorized("You have been Logged Out.");

            _img.sender_id = int.Parse(userIdClaim.Value);
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
            var userIdClaim = User.FindFirst("user_id"); // custom claim name from token

            if (userIdClaim == null)
                return Unauthorized("You have been Logged Out.");

            _audio.sender_id = int.Parse(userIdClaim.Value);

            if (_audio.voice == null || _audio.voice.Length == 0)
            {
                return BadRequest(new { message = "no audio recieved" });
            }


            var allowedTypes = new[] { "audio/mpeg", "audio/wav", "audio/ogg", "audio/mp4" };
            if (!allowedTypes.Contains(_audio.voice.ContentType.ToLower()))
                return BadRequest(new { message = "Only audio files are allowed (MP3, WAV, etc.)" });

            _audio.duration=LengthService.GetAudioDuration(_audio.voice);
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

        [HttpPost]
        [Route("sendvideo")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> sendvideoMessage([FromForm] Video_msg _video)
        {
            var userIdClaim = User.FindFirst("user_id"); // custom claim name from token

            if (userIdClaim == null)
                return Unauthorized("You have been Logged Out.");

            _video.sender_id = int.Parse(userIdClaim.Value);

            if (_video.video == null || _video.video.Length == 0)
            {
                return BadRequest(new { message = "no video recieved" });
            }


            var allowedVideoTypes = new[] { "video/mp4", "video/x-msvideo", "video/x-matroska", "video/webm", "video/quicktime" };

            if (!allowedVideoTypes.Contains(_video.video.ContentType.ToLower()))
                return BadRequest(new { message = "Only video files are allowed (MP4, AVI, MKV, etc.)" });

            _video.duration = LengthService.GetVideoDuration(_video.video);
          
            Console.WriteLine(_video.duration);
            _video.video_url = await _azure.sendVideo(_video.video);

            if (_video.video_url == null)
            {
                return BadRequest(new { message = "error in sending video" });
            }

            bool isSend = _msg_dl.sendvideoMessage(_video);
            if (isSend)
            {
                return Ok(isSend);
            }
            else
            {
                return BadRequest(isSend);
            }
        }

        [HttpGet]
        [Route("getMessage")]
        public IActionResult getMessage([FromQuery] int id, [FromQuery] int ofsset)
        {
            Console.WriteLine("messages called");
            var userIdClaim = User.FindFirst("user_id"); // custom claim name from token

            if (userIdClaim == null)
                return Unauthorized("You have been Logged Out.");

            int user_id = int.Parse(userIdClaim.Value);
            if (user_id == null)
                return Unauthorized("You have been Logged Out.");


            var messages = _msg_dl.getMessages(user_id, id, ofsset);
            if (messages == null)
            {
                Console.WriteLine("got no messages (null)");
                return Ok(new List<object>()); // Return empty list to prevent Flutter errors
            }
            Console.WriteLine("got messages "+messages.Count);
            return Ok(messages);
        }


    }
}
