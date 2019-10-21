using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;

namespace TrainBlog.Helpers
{
    public class ImageUploadHelper
    {
        private static HttpServerUtilityWrapper Server = new HttpServerUtilityWrapper(HttpContext.Current.Server);

        //Image format checker
        public static bool IsWebFriendlyImage(HttpPostedFileBase file)
        {
            if (file == null)
                return false;

            if (file.ContentLength > 3 * 1024 * 1024 || file.ContentLength < 1024)
                return false;

            try
            {
                using (var img = Image.FromStream(file.InputStream))
                {
                    return ImageFormat.Jpeg.Equals(img.RawFormat)
                        || ImageFormat.Png.Equals(img.RawFormat);
                };
            }
            catch
            {
                return false;
            }
        }
    }
}