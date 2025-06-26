using System.Drawing;
using System.Drawing.Imaging;

namespace whatsapp_clone_backend.Services
{
    public class Image_crop_service
    {
        public byte[] CropImageToSquarePng(byte[] imageBytes)
        {
            using (var ms = new MemoryStream(imageBytes))
            using (var originalImage = Image.FromStream(ms))
            {
                int side = Math.Min(originalImage.Width, originalImage.Height);
                int x = (originalImage.Width - side) / 2;
                int y = (originalImage.Height - side) / 2;

                using (var squareImage = new Bitmap(side, side))
                using (var g = Graphics.FromImage(squareImage))
                {
                    g.Clear(Color.Transparent);
                    g.DrawImage(originalImage, new Rectangle(0, 0, side, side), new Rectangle(x, y, side, side), GraphicsUnit.Pixel);

                    using (var resultStream = new MemoryStream())
                    {
                        squareImage.Save(resultStream, ImageFormat.Png);
                        return resultStream.ToArray();
                    }
                }
            }
        }

    }
}
