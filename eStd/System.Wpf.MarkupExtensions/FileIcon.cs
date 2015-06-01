using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Wpf.MarkupExtensions.API;

namespace System.Wpf.MarkupExtensions
{
    public class FileIcon : MarkupExtension
    {
        private Icon _icon;

        private string filename;

        public string Filename
        {
            get
            {
                return this.filename;
            }
            set
            {
                this.filename = value;
                _icon = Icons.IconFromExtension(value, Icons.SystemIconSize.Large);
            }
        }

        public FileIcon()
        {

        }

        public FileIcon(string filename)
        {
            Filename = filename;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var ms = new MemoryStream();
            _icon.Save(ms);

            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.StreamSource = ms;
            bmp.EndInit();

            return bmp;
        }
    }
}