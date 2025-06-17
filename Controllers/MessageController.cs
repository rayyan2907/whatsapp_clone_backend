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





    }
}
