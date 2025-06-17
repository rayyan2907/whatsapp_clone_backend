using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using whatsapp_clone_backend.Data;
using whatsapp_clone_backend.Models;

namespace whatsapp_clone_backend.Controllers
{
    [ApiController]
    //[Authorize]
    public class MessageController : ControllerBase
    {
        private readonly Message_DL _msg_dl;
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

    }
}
