using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Creek.Extensions
{
    /// <summary>
    /// Provides various image-related extensions.
    /// </summary>
    public static class ImageExtensions
    {
        /// <summary>
        /// Appends a border to an existing image.
        /// </summary>
        /// <param name="image">Image to append the border to.</param>
        /// <param name="borderWidth">Width of the border.</param>
        /// <param name="color">Color of the border.</param>
        /// <returns>The resulting image.</returns>
        public static Image AppendBorder(this Image image, int borderWidth, Color color)
        {
            var newSize = new Size(image.Width + (borderWidth * 2), image.Height + (borderWidth * 2));

            var img = new Bitmap(newSize.Width, newSize.Height);

            var g = Graphics.FromImage(img);

            g.Clear(color);
            g.DrawImage(image, new Point(borderWidth, borderWidth));
            g.Dispose();

            return img;
        }

        /// <summary>
        /// Modifies the brightness of an image.
        /// </summary>
        /// <param name="bitmap">Image to modify.</param>
        /// <param name="brightness">Brightness value.  Must be between 0 and 255.</param>
        /// <returns>Success indicator.</returns>
        public static bool Brightness(this Bitmap bitmap, int brightness)
        {
            if (brightness < -255 || brightness > 255)
            {
                return false;
            }

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bitmapData.Stride;
            System.IntPtr scan0 = bitmapData.Scan0;

            int val = 0;

            unsafe
            {
                byte* p = (byte*)(void*)scan0;

                int offset = stride - (bitmap.Width * 3);
                int width = bitmap.Width * 3;

                for (int y = 0; y < bitmap.Height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        val = (int)(p[0] + brightness);

                        if (val < 0)
                        {
                            val = 0;
                        }

                        if (val > 255)
                        {
                            val = 255;
                        }

                        p[0] = (byte)val;

                        ++p;
                    }

                    p += offset;
                }
            }

            bitmap.UnlockBits(bitmapData);

            return true;
        }

        /// <summary>
        /// Colorizes an image.
        /// </summary>
        /// <param name="bitmap">Image to colorize.</param>
        /// <param name="red">Red value.  Must be between 0 and 255.</param>
        /// <param name="green">Green value.  Must be between 0 and 255.</param>
        /// <param name="blue">Blue value.  Must be between 0 and 255.</param>
        /// <returns>Success indicator.</returns>
        public static bool Color(this Bitmap bitmap, int red, int green, int blue)
        {
            if (red < -255 || red > 255)
            {
                return false;
            }

            if (green < -255 || green > 255)
            {
                return false;
            }

            if (blue < -255 || blue > 255)
            {
                return false;
            }

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bitmapData.Stride;
            IntPtr scan0 = bitmapData.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)scan0;

                int offset = stride - (bitmap.Width * 3);

                int pixel;

                for (int y = 0; y < bitmap.Height; ++y)
                {
                    for (int x = 0; x < bitmap.Width; ++x)
                    {
                        pixel = p[2] + red;
                        pixel = Math.Max(pixel, 0);

                        p[2] = (byte)Math.Min(255, pixel);

                        pixel = p[1] + green;
                        pixel = Math.Max(pixel, 0);
                        p[1] = (byte)Math.Min(255, pixel);

                        pixel = p[0] + blue;
                        pixel = Math.Max(pixel, 0);
                        p[0] = (byte)Math.Min(255, pixel);

                        p += 3;
                    }

                    p += offset;
                }
            }

            bitmap.UnlockBits(bitmapData);

            return true;
        }

        /// <summary>
        /// Sets the contrast of an image.
        /// </summary>
        /// <param name="bitmap">Image to adjust.</param>
        /// <param name="contrast">Contrast setting.</param>
        /// <returns>Success indicator.</returns>
        public static bool Contrast(this Bitmap bitmap, sbyte contrast)
        {
            if (contrast < -100)
            {
                return false;
            }

            if (contrast > 100)
            {
                return false;
            }

            double pixel = 0, pixelContrast = (100.0 + contrast) / 100.0;

            pixelContrast *= pixelContrast;

            int red, green, blue;

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bitmapData.Stride;
            IntPtr scan0 = bitmapData.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)scan0;

                int offset = stride - (bitmap.Width * 3);

                for (int y = 0; y < bitmap.Height; ++y)
                {
                    for (int x = 0; x < bitmap.Width; ++x)
                    {
                        blue = p[0];
                        green = p[1];
                        red = p[2];

                        pixel = red / 255.0;
                        pixel -= 0.5;
                        pixel *= pixelContrast;
                        pixel += 0.5;
                        pixel *= 255;

                        if (pixel < 0)
                        {
                            pixel = 0;
                        }

                        if (pixel > 255) 
                        {
                            pixel = 255;
                        }

                        p[2] = (byte)pixel;

                        pixel = green / 255.0;
                        pixel -= 0.5;
                        pixel *= pixelContrast;
                        pixel += 0.5;
                        pixel *= 255;
                        if (pixel < 0)
                        {
                            pixel = 0;
                        }

                        if (pixel > 255)
                        {
                            pixel = 255;
                        }

                        p[1] = (byte)pixel;

                        pixel = blue / 255.0;
                        pixel -= 0.5;
                        pixel *= pixelContrast;
                        pixel += 0.5;
                        pixel *= 255;

                        if (pixel < 0)
                        {
                            pixel = 0;
                        }

                        if (pixel > 255)
                        {
                            pixel = 255;
                        }

                        p[0] = (byte)pixel;

                        p += 3;
                    }

                    p += offset;
                }
            }

            bitmap.UnlockBits(bitmapData);

            return true;
        }

        /// <summary>
        /// Creates a copy of a bitmap.
        /// </summary>
        /// <param name="bitmap">Original bitmap.</param>
        /// <returns>Bitmap copy.</returns>
        public static Bitmap Copy(this Bitmap bitmap)
        {
            return new Bitmap(bitmap);
        }

        /// <summary>
        /// Crops an image file.
        /// </summary>
        /// <param name="image">Image to crop.</param>
        /// <param name="targetW">Target width.</param>
        /// <param name="targetH">Target height.</param>
        /// <param name="targetX">Target left.</param>
        /// <param name="targetY">Target top.</param>
        /// <returns>Cropped image byte array.</returns>
        public static Image Crop(this Image image, int targetW, int targetH, int targetX, int targetY)
        {
            byte[] imageFile = image.ToByteArray(image.RawFormat);

            System.Drawing.Image imgPicture = System.Drawing.Image.FromStream(new MemoryStream(imageFile));
            Bitmap bitMap = new Bitmap(targetW, targetH, PixelFormat.Format24bppRgb);
            bitMap.SetResolution(72, 72);

            Graphics graphics = Graphics.FromImage(bitMap);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.High;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.DrawImage(imgPicture, new Rectangle(0, 0, targetW, targetH), targetX, targetY, targetW, targetH, GraphicsUnit.Pixel);

            MemoryStream mm = new MemoryStream();

            ImageCodecInfo[] info = ImageCodecInfo.GetImageEncoders();

            EncoderParameters parameters = new EncoderParameters(1);
            parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);

            bitMap.Save(mm, info[1], parameters);

            imgPicture.Dispose();
            bitMap.Dispose();
            graphics.Dispose();

            return mm.GetBuffer().ToImage();
        }

        /// <summary>
        /// Adjusts the gamma of an image.
        /// </summary>
        /// <param name="bitmap">Image to adjust.</param>
        /// <param name="red">Red gamma value.</param>
        /// <param name="green">Green gamma value.</param>
        /// <param name="blue">Blue gamma value.</param>
        /// <returns>Success indicator.</returns>
        public static bool Gamma(this Bitmap bitmap, double red, double green, double blue)
        {
            if (red < .2 || red > 5)
            {
                return false;
            }

            if (green < .2 || green > 5)
            {
                return false;
            }

            if (blue < .2 || blue > 5)
            {
                return false;
            }

            byte[] redGamma = new byte[256];
            byte[] greenGamma = new byte[256];
            byte[] blueGamma = new byte[256];

            for (int i = 0; i < 256; ++i)
            {
                redGamma[i] = (byte)Math.Min(255, (int)((255.0 * Math.Pow(i / 255.0, 1.0 / red)) + 0.5));
                greenGamma[i] = (byte)Math.Min(255, (int)((255.0 * Math.Pow(i / 255.0, 1.0 / green)) + 0.5));
                blueGamma[i] = (byte)Math.Min(255, (int)((255.0 * Math.Pow(i / 255.0, 1.0 / blue)) + 0.5));
            }

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bitmapData.Stride;
            IntPtr scan0 = bitmapData.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)scan0;

                int offset = stride - (bitmap.Width * 3);

                for (int y = 0; y < bitmap.Height; ++y)
                {
                    for (int x = 0; x < bitmap.Width; ++x)
                    {
                        p[2] = redGamma[p[2]];
                        p[1] = greenGamma[p[1]];
                        p[0] = blueGamma[p[0]];

                        p += 3;
                    }

                    p += offset;
                }
            }

            bitmap.UnlockBits(bitmapData);

            return true;
        }

        /// <summary>
        /// Sets an image to grayscale.
        /// </summary>
        /// <param name="bitmap">Image to adjust.</param>
        /// <returns>Success indicator.</returns>
        public static bool GrayScale(this Bitmap bitmap)
        {
            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bitmapData.Stride;
            IntPtr scan0 = bitmapData.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)scan0;

                int offset = stride - (bitmap.Width * 3);

                byte red, green, blue;

                for (int y = 0; y < bitmap.Height; ++y)
                {
                    for (int x = 0; x < bitmap.Width; ++x)
                    {
                        blue = p[0];
                        green = p[1];
                        red = p[2];

                        p[0] = p[1] = p[2] = (byte)((.299 * red) + (.587 * green) + (.114 * blue));

                        p += 3;
                    }

                    p += offset;
                }
            }

            bitmap.UnlockBits(bitmapData);

            return true;
        }

        /// <summary>
        /// Inverts an image.
        /// </summary>
        /// <param name="bitmap">Image to invert.</param>
        /// <returns>Success indicator.</returns>
        public static bool Invert(this Bitmap bitmap)
        {
            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bitmapData.Stride;

            IntPtr scan0 = bitmapData.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)scan0;

                int offset = stride - (bitmap.Width * 3);
                int width = bitmap.Width * 3;

                for (int y = 0; y < bitmap.Height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        p[0] = (byte)(255 - p[0]);
                        ++p;
                    }

                    p += offset;
                }
            }

            bitmap.UnlockBits(bitmapData);

            return true;
        }

        /// <summary>
        /// Check if the image is grayscale.
        /// </summary>
        /// <param name="bitmap">Image to check.</param>
        /// <returns>Returns <b>true</b> if the image is grayscale or <b>false</b> otherwise.</returns>
        /// <remarks>The methods check if the image is a grayscale image of 256 gradients.
        /// The method first examines if the image's pixel format is
        /// <see cref="System.Drawing.Imaging.PixelFormat">Format8bppIndexed</see>
        /// and then it examines its palette to check if the image is grayscale or not.</remarks>
        public static bool IsGrayscale(this Bitmap bitmap)
        {
            bool ret = false;

            // check pixel format
            if (bitmap.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                ret = true;

                // check palette
                ColorPalette cp = bitmap.Palette;

                Color c;

                // init palette
                for (int i = 0; i < 256; i++)
                {
                    c = cp.Entries[i];
                    if ((c.R != i) || (c.G != i) || (c.B != i))
                    {
                        ret = false;
                        break;
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Resizes a bitmap.
        /// </summary>
        /// <param name="bitmap">Bitmap to resize</param>
        /// <param name="width">Width to resize the bitmap to</param>
        /// <param name="height">Height to resize the bitmap to</param>
        /// <returns>A resized bitmap</returns>
        public static Bitmap Resize(this Bitmap bitmap, int width, int height)
        {
            Bitmap output = new Bitmap(width, height);
            Rectangle r = new Rectangle(0, 0, width, height);
            Graphics g = Graphics.FromImage(output);

            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(bitmap, r);

            bitmap.Dispose();

            return output;
        }

        /// <summary>
        /// Resizes an image to a particular height.
        /// </summary>
        /// <param name="image">File to resize.</param>
        /// <param name="targetHeight">Height to resize the image to.</param>
        /// <returns>Resized image file.</returns>
        public static Image ResizeToHeight(this Image image, int targetHeight)
        {
            byte[] imageBytes = image.ToByteArray();

            Image original = Image.FromStream(new MemoryStream(imageBytes));

            if (original.Height < targetHeight)
            {
                targetHeight = original.Height;
            }

            int targetW = (int)(original.Width * ((float)targetHeight / (float)original.Height));

            Image imgPicture = Image.FromStream(new MemoryStream(imageBytes));

            // Create a new blank canvas.  The resized image will be drawn on this canvas.
            Bitmap bitMap = new Bitmap(targetW, targetHeight, PixelFormat.Format32bppRgb);

            Graphics graphics = Graphics.FromImage(bitMap);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.High;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.DrawImage(imgPicture, new Rectangle(0, 0, targetW, targetHeight), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel);

            MemoryStream mm = new MemoryStream();

            ImageCodecInfo[] info = ImageCodecInfo.GetImageEncoders();
            EncoderParameters parameters = new EncoderParameters(1);
            parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);

            bitMap.Save(mm, info[1], parameters);

            original.Dispose();
            imgPicture.Dispose();
            bitMap.Dispose();
            graphics.Dispose();

            return mm.GetBuffer().ToImage();
        }

        /// <summary>
        /// Resizes an image file to a particular width.
        /// </summary>
        /// <param name="image">Image file to resize.</param>
        /// <param name="targetWidth">Target width.</param>
        /// <returns>Resized image file.</returns>
        public static Image ResizeToWidth(this Image image, int targetWidth)
        {
            byte[] imageBytes = image.ToByteArray();

            Image original = Image.FromStream(new MemoryStream(imageBytes));

            if (original.Width < targetWidth)
            {
                targetWidth = original.Width;
            }

            int targetH = (int)(original.Height * ((float)targetWidth / (float)original.Width));

            Image imgPicture = Image.FromStream(new MemoryStream(imageBytes));

            // Create a new blank canvas.  The resized image will be drawn on this canvas.
            Bitmap bitMap = new Bitmap(targetWidth, targetH, PixelFormat.Format32bppRgb);

            Graphics graphics = Graphics.FromImage(bitMap);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.High;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.DrawImage(imgPicture, new Rectangle(0, 0, targetWidth, targetH), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel);

            MemoryStream mm = new MemoryStream();

            ImageCodecInfo[] info = ImageCodecInfo.GetImageEncoders();
            EncoderParameters parameters = new EncoderParameters(1);
            parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);

            bitMap.Save(mm, info[1], parameters);

            original.Dispose();
            imgPicture.Dispose();
            bitMap.Dispose();
            graphics.Dispose();

            return mm.GetBuffer().ToImage();
        }

        /// <summary>
        /// Resizes an image and creates a copy under the new filename.
        /// </summary>
        /// <param name="image">Image to resize</param>
        /// <param name="width">Target width.</param>
        /// <param name="height">Target height</param>
        /// <returns>Resized image.</returns>
        public static Image ResizeWithTransparency(this Image image, int width, int height)
        {
            Size newSize = new Size(width, height);

            string tempDirPath = Path.GetTempPath();

            Directory.CreateDirectory(tempDirPath);

            string newImagePath = tempDirPath + StringUtilities.GetRandomString(10) + ".png";

            using (image)
            {
                PixelFormat format = image.PixelFormat;

                if (format.ToString().Contains("Indexed"))
                {
                    format = PixelFormat.Format24bppRgb;
                }

                using (Bitmap newImage = new Bitmap(newSize.Width, newSize.Height, image.PixelFormat))
                {
                    using (Graphics canvas = Graphics.FromImage(newImage))
                    {
                        canvas.SmoothingMode = SmoothingMode.AntiAlias;
                        canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        canvas.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        canvas.DrawImage(image, new Rectangle(new Point(0, 0), newSize));

                        newImage.Save(newImagePath, image.RawFormat);

                        newImage.Dispose();
                    }
                }

                Image returnImage = Image.FromFile(newImagePath);

                return returnImage;
            }
        }

        /// <summary>
        /// Converts an image to a byte array.
        /// </summary>
        /// <param name="image">Image to convert.</param>
        /// <returns>Image byte array.</returns>
        public static byte[] ToByteArray(this Image image)
        {
            return image.ToByteArray(image.RawFormat);
        }

        /// <summary>
        /// Converts an image to a byte array in the specified format.
        /// </summary>
        /// <param name="image">Image to convert.</param>
        /// <param name="format">Format of image to return.</param>
        /// <returns>Image byte array.</returns>
        public static byte[] ToByteArray(this Image image, ImageFormat format)
        {
            byte[] ret;

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, format);

                    ret = ms.ToArray();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return ret;
        }

        /// <summary>
        /// Returns a thumbnail of a transparent image.
        /// </summary>
        /// <param name="image">File to convert to a thumbnail.</param>
        /// <param name="maxSize">Maximum width or height, whichever is largest.</param>
        /// <returns>Thumbnail of the original.</returns>
        public static byte[] ToTransparentThumbnail(this Image image, int maxSize)
        {
            int height = 0;
            int width = 0;

            // if the image is taller than it is wide,
            // calculate the width and resize the height
            // to the maxSize
            if (image.Height > image.Width)
            {
                double ratio = Convert.ToDouble(image.Height) / Convert.ToDouble(image.Width);

                height = maxSize;

                width = Convert.ToInt32(maxSize / ratio);
            }
            else
            {
                double ratio = Convert.ToDouble(image.Width) / Convert.ToDouble(image.Height);

                width = maxSize;

                height = Convert.ToInt32(maxSize / ratio);
            }

            Image resized = image.ResizeWithTransparency(width, height);

            byte[] ret = resized.ToByteArray();

            image.Dispose();
            resized.Dispose();

            return ret;
        }
    }
}
