using System;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;

namespace GH.DAL.Helpers
{
    public static class ImageExtension
    {
        public static byte[] CreateThumbnail(Image img)
        {
            int newWidth = 100;
            int newHeight = 100;
            double ratio = 0;

            if (img.Width > img.Height)
            {
                ratio = img.Width / (double)img.Height;
                newHeight = (int)(newHeight / ratio);
            }
            else
            {
                ratio = img.Height / (double)img.Width;
                newWidth = (int)(newWidth / ratio);
            }

            Image bmp1 = img.GetThumbnailImage(newWidth, newHeight, null, IntPtr.Zero);

            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(bmp1, typeof(byte[]));
        }

        public static byte[] ConvertImageToByteArray(Image imageIn)
        {
            MemoryStream ms = new MemoryStream();

            imageIn.Save(ms, ImageFormat.Jpeg);

            return ms.ToArray();
        }

        public static Image byteArrayToImage(byte[] bmpBytes)
        {
            Image image = null;
            using (MemoryStream stream = new MemoryStream(bmpBytes))
            {
                image = Image.FromStream(stream);
            }
            return image;
        }
    }
    
}
