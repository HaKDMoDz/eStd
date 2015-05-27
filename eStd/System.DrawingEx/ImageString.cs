namespace System.DrawingEx
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    public struct ImageString
    {
        internal ImageString(string imageToBase64String) : this()
        {
            this.Image = ImageFromBase64String(imageToBase64String);
            this.Base64 = imageToBase64String;
        }

        private Image Image { get; set; }

        private string Base64 { get; set; }

        public override string ToString()
        {
            return this.Image != null ? ImageToBase64String(this.Image, ImageFormat.Jpeg) : "";
        }

        public static implicit operator ImageString(Image img)
        {
            return new ImageString(ImageToBase64String(img, ImageFormat.Jpeg));
        }

        public static implicit operator Image(ImageString iS)
        {
            return ImageFromBase64String(iS.Base64);
        }

        private static string ImageToBase64String(Image image, ImageFormat format)
        {
            var memory = new MemoryStream();
            image.Save(memory, format);
            var base64 = Convert.ToBase64String(memory.ToArray());
            memory.Close();

            return base64;
        }

        private static Image ImageFromBase64String(string base64)
        {
            var memory = new MemoryStream(Convert.FromBase64String(base64));
            var result = Image.FromStream(memory);
            memory.Close();

            return result;
        }
    }
}