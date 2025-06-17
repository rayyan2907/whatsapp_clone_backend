using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace whatsapp_clone_backend.Services
{

    public class Azure_services
    {
        private readonly string connectionstring;

        public Azure_services()
        {
            connectionstring = "DefaultEndpointsProtocol=https;AccountName=whatsap;AccountKey=fmLjDC9VnNZXIL9/dEXLFWO9CyzlRCfTwh/0V9GTdIUf8RMHVkc8hNSrUsUBoZD1v7kUUN/fvwae+AStVWFeAw==;EndpointSuffix=core.windows.net";
        }

        public async Task<string> UploadProfilePic(IFormFile file)
        {
            try
            {
                string containerName = "profilepics";
                var blobServiceClient = new BlobServiceClient(connectionstring);
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var blobClient = containerClient.GetBlobClient(fileName);

                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });                }

                Console.WriteLine(blobClient.Uri.ToString());
                return blobClient.Uri.ToString();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public async Task<string> sendImg(IFormFile file)
        {
            try
            {
                string containerName = "sentimgs";
                var blobServiceClient = new BlobServiceClient(connectionstring);
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var blobClient = containerClient.GetBlobClient(fileName);

                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
                }

                Console.WriteLine(blobClient.Uri.ToString());
                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public async Task<string> sendVoice(IFormFile file)
        {
            try
            {
                string containerName = "sentvoice";
                var blobServiceClient = new BlobServiceClient(connectionstring);
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var blobClient = containerClient.GetBlobClient(fileName);

                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
                }

                Console.WriteLine(blobClient.Uri.ToString());
                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

    }
}
    