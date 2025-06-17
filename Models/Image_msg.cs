namespace whatsapp_clone_backend.Models
{
    public class Image_msg : Message_model
    {
        public string? img_url {  get; set; }
        public IFormFile image {  get; set; }
        public string? caption {  get; set; }

    }
}
