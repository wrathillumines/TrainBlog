using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;

namespace HunterW_Blog.Utilities
{
    public class ImageUploadValidator
    {
        public static bool IsWebFriendlyImage(HttpPostedFileBase file)
        {
            if (file == null)
                return false;

            if (file.ContentLength > 2 * 1024 * 1024 || file.ContentLength < 1024)
                return false;

            try
            {
                using (var img = Image.FromStream(file.InputStream))
                {
                    return ImageFormat.Jpeg.Equals(img.RawFormat) ||
                           ImageFormat.Png.Equals(img.RawFormat);
                }
            }

            catch
            {
                return false;
            }
        }

        internal static bool IsWebFriendlyImage(object image)
        {
            throw new NotImplementedException();
        }
    }
}