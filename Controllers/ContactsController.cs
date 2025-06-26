using Microsoft.AspNetCore.Mvc;

namespace whatsapp_clone_backend.Controllers
{
    public class ContactsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
