using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using whatsapp_clone_backend.Data;
using whatsapp_clone_backend.Models;
using whatsapp_clone_backend.Services;

namespace whatsapp_clone_backend.Controllers
{
    [ApiController]
    [Authorize]

    public class DpUpdateController : ControllerBase
    {


        private readonly Azure_services _azure= new Azure_services();
        private readonly Image_crop_service _img_service= new Image_crop_service();
        private readonly DpSet_DL _dp_dl;

        public DpUpdateController (DpSet_DL dpSet_DL)
        {
            _dp_dl=dpSet_DL;
        }




        [HttpPost]
        [Route("dpUpdate")]
        public async Task<IActionResult> uploadDP([FromForm] ProfilePic pic)
        {
            Console.WriteLine("fumction called");
            var userIdClaim = User.FindFirst("user_id"); // custom claim name from token

            if (userIdClaim == null)
                return Unauthorized("You have been Logged Out.");

            int user_id = int.Parse(userIdClaim.Value);
            if (user_id == null)
                return Unauthorized("You have been Logged Out.");

            string oldUrl = _dp_dl.getOldDp(user_id);

            string url;
            if (pic.Pic == null || pic.Pic.Length == 0)
            {
                url = null;
            }
            else
            {

                using (var ms = new MemoryStream())
                {
                    await pic.Pic.CopyToAsync(ms);
                    byte[] originalBytes = ms.ToArray();

                    // 2. Crop the image to square PNG
                    byte[] croppedBytes = _img_service.CropImageToSquarePng(originalBytes);

                    // 3. Upload cropped image to Azure
                    url = await _azure.UploadProfilePic(croppedBytes); // Update this to accept byte[]
                }
            }

            if (url == null)
            {
                return Ok(new { message = "No Profile Photo Added" });
            }

            else
            {
                bool isDpUpload = _dp_dl.changePicture(url,user_id);
                if (isDpUpload)
                {
                    if(oldUrl!= null)
                    {
                        _=_azure.DeleteFile(oldUrl);
                    }
                    return Ok(new { message = "Profile Photo Uploaded" });
                }
                else
                {
                    return BadRequest(new { message = "Error in changing profile photo" });
                }

            }
        }
    }
}
