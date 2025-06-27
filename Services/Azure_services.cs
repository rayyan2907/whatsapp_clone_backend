using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;

namespace whatsapp_clone_backend.Services
{

    public class Azure_services
    {
        private readonly string connectionstring;

        public Azure_services()
        {
            connectionstring = "DefaultEndpointsProtocol=https;AccountName=whatsap;AccountKey=fmLjDC9VnNZXIL9/dEXLFWO9CyzlRCfTwh/0V9GTdIUf8RMHVkc8hNSrUsUBoZD1v7kUUN/fvwae+AStVWFeAw==;EndpointSuffix=core.windows.net";
        }

        public async Task<string> UploadProfilePic(byte[] imageBytes)
        {
            try
            {
                string containerName = "profilepics";
                var blobServiceClient = new BlobServiceClient(connectionstring);
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                var fileName = Guid.NewGuid().ToString() + ".png"; // Always PNG
                var blobClient = containerClient.GetBlobClient(fileName);

                using (var stream = new MemoryStream(imageBytes))
                {
                    await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = "image/png" });
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

        public async Task<string> sendVideo(IFormFile file)
        {
            try
            {
                string containerName = "sentvideos";
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
        public async Task<bool> DeleteFile(string fileUrl)
        {
            try
            {
                var uri = new Uri(fileUrl);
                string containerName = uri.Segments[1].TrimEnd('/'); // e.g. "profilepics"
                string blobName = string.Join("", uri.Segments.Skip(2)); // remaining path after container

                var blobServiceClient = new BlobServiceClient(connectionstring);
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(blobName);

                var response = await blobClient.DeleteIfExistsAsync();

                Console.WriteLine($"Deleted: {fileUrl} -> {response.Value}");
                return response.Value; // true if deleted, false if blob didn't exist
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete error: {ex.Message}");
                return false;
            }
        }


    }
}
    