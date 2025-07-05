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
    // [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly Message_DL _msg_dl;
        private Azure_services _azure = new Azure_services();

        public MessageController(Message_DL msg_dl)
        {
            _msg_dl = msg_dl;
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
            Console.WriteLine("got messages " + messages.Count);
            return Ok(messages);
        }


        [HttpPost]
        [Route("sendimg")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> sendimgMessage([FromForm] Image_msg _img)
        {
            var userIdClaim = User.FindFirst("user_id");
            if (userIdClaim == null)
                return Unauthorized("You have been logged out.");

            int userId = int.Parse(userIdClaim.Value);
            _img.sender_id = userId; // Assign sender_id from token

            if (_img.image == null || _img.image.Length == 0)
                return BadRequest("No image file provided.");

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
            if (!allowedTypes.Contains(_img.image.ContentType.ToLower()))
                return BadRequest("Unsupported image type.");

            var azureService = new Azure_services();
            _img.img_url = await azureService.sendImg(_img.image);

            if (string.IsNullOrEmpty(_img.img_url))
                return StatusCode(500, "Image upload failed.");

            bool isSend = _msg_dl.sendimgMessage(_img);

            if (isSend)
                return Ok(_img.img_url);
            else
                return StatusCode(500, "Database insert failed.");
        }

        [HttpPost]
        [Route("sendvoice")]
        public async Task<IActionResult> SendVoiceMessage([FromBody] Audio_msg _audio)
        {
            Console.WriteLine("sendvoice called");

            var userIdClaim = User.FindFirst("user_id");
            if (userIdClaim == null)
                return Unauthorized("You have been logged out.");

            _audio.sender_id = int.Parse(userIdClaim.Value);
            _audio.sender_id = 19;



            try
            {
                // Decode base64 to byte array
                byte[] voiceBytes = Convert.FromBase64String(_audio.voice_byte!);

                // Convert to MemoryStream -> IFormFile
                var stream = new MemoryStream(voiceBytes);
                _audio.voice = new FormFile(stream, 0, voiceBytes.Length, "voice", _audio.file_name)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "audio/aac"
                };
            }
            catch (FormatException)
            {
                return BadRequest("Invalid base64 string.");
            }


            if (_audio.voice == null || _audio.voice.Length == 0)
                return BadRequest("No voice file provided.");

            var allowedTypes = new[] { "audio/mpeg", "audio/wav", "audio/ogg", "audio/aac" };
            if (!allowedTypes.Contains(_audio.voice.ContentType.ToLower()))
                Console.WriteLine("format is " + _audio.voice.ContentType);
            //return BadRequest("Unsupported audio format.");

            _audio.duration = await LengthService.GetAudioDuration(_audio.voice);
            _audio.voice_url = await _azure.sendVoice(_audio.voice);

            if (string.IsNullOrEmpty(_audio.voice_url))
                return StatusCode(500, "Audio upload failed.");

            bool isSent = _msg_dl.sendvoiceMessage(_audio);

            if (!isSent)
                return StatusCode(500, "Database insert failed.");

            // return full message object (can be modified as needed)


            return Ok(new
            {
                recieverId = _audio.reciever_id,
                type = _audio.type,
                voice_url = _audio.voice_url,
                time = _audio.time,
                is_seen = _audio.is_seen,
            });
        }

        

    }
}
