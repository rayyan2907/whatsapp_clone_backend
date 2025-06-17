using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace whatsapp_clone_backend.Controllers
{
    [ApiController]
    [Authorize]
    public class MessageController : ControllerBase
    {
    }
}
