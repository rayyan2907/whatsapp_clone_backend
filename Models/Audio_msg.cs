﻿namespace whatsapp_clone_backend.Models
{
    public class Audio_msg : Message_model
    {
        public IFormFile? voice {  get; set; }
        public string? voice_url { get; set; }
        public string? duration { get; set; }
        public string? voice_byte { get; set; }
        public string? file_name { get; set; }

    }
}
