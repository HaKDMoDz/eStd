using System;
using System.Drawing;

namespace System.DrawingEx
{
    public class HslColor
    {
        // Private data members below are on scale 0-1
        // They are scaled for use externally based on scale
        private const double scale = 240.0;
        private double hue = 1.0;
        private double luminosity = 1.0;
        private double saturation = 1.0;

        public HslColor()
        {
        }

        public HslColor(Color color)
        {
            SetRGB(color.R, color.G, color.B);
        }

        public HslColor(int red, int green, int blue)
        {
            SetRGB(red, green, blue);
        }

        public HslColor(double hue, double saturation, double luminosity)
        {
            Hue = hue;
            Saturation = saturation;
            Luminosity = luminosity;
        }

        public double Hue
        {
            get { return hue*scale; }
            set { hue = CheckRange(value / scale); }
        }

        public double Saturation
        {
            get { return saturation*scale; }
            set { saturation = CheckRange(value / scale); }
        }

        public double Luminosity
        {
            get { return luminosity*scale; }
            set { luminosity = CheckRange(value / scale); }
        }

        public byte Alpha { get; set; }

        private double CheckRange(double Content)
        {
            if (Content < 0.0)
                Content = 0.0;
            else if (Content > 1.0)
                Content = 1.0;
            return Content;
        }

        public override string ToString()
        {
            return String.Format("H: {0:#0.##} S: {1:#0.##} L: {2:#0.##} A: {3:#0.##}", Hue, Saturation, Luminosity,
                                 Alpha);
        }

        public string ToRgbString()
        {
            Color color = this;
            return String.Format("R: {0:#0.##} G: {1:#0.##} B: {2:#0.##} A: {3:#0.##}", color.R, color.G, color.B,
                                 color.A);
        }

        public void SetRGB(int red, int green, int blue)
        {
            HslColor hslColor = Color.FromArgb(red, green, blue);
            hue = hslColor.hue;
            saturation = hslColor.saturation;
            luminosity = hslColor.luminosity;
        }

        #region Casts to/from System.Drawing.Color

        public static implicit operator Color(HslColor hslColor)
        {
            double r = 0, g = 0, b = 0;
            if (hslColor.luminosity != 0)
            {
                if (hslColor.saturation == 0)
                    r = g = b = hslColor.luminosity;
                else
                {
                    double temp2 = GetTemp2(hslColor);
                    double temp1 = 2.0*hslColor.luminosity - temp2;

                    r = GetColorComponent(temp1, temp2, hslColor.hue + 1.0/3.0);
                    g = GetColorComponent(temp1, temp2, hslColor.hue);
                    b = GetColorComponent(temp1, temp2, hslColor.hue - 1.0/3.0);
                }
            }
            return Color.FromArgb(hslColor.Alpha, (int) (255*r), (int) (255*g), (int) (255*b));
        }

        private static double GetColorComponent(double temp1, double temp2, double temp3)
        {
            temp3 = MoveIntoRange(temp3);
            if (temp3 < 1.0/6.0)
                return temp1 + (temp2 - temp1)*6.0*temp3;
            else if (temp3 < 0.5)
                return temp2;
            else if (temp3 < 2.0/3.0)
                return temp1 + ((temp2 - temp1)*((2.0/3.0) - temp3)*6.0);
            else
                return temp1;
        }

        private static double MoveIntoRange(double temp3)
        {
            if (temp3 < 0.0)
                temp3 += 1.0;
            else if (temp3 > 1.0)
                temp3 -= 1.0;
            return temp3;
        }

        private static double GetTemp2(HslColor hslColor)
        {
            double temp2;
            if (hslColor.luminosity < 0.5) //<=??
                temp2 = hslColor.luminosity*(1.0 + hslColor.saturation);
            else
                temp2 = hslColor.luminosity + hslColor.saturation - (hslColor.luminosity*hslColor.saturation);
            return temp2;
        }

        public static implicit operator HslColor(Color color)
        {
            var hslColor = new HslColor();
            hslColor.hue = color.GetHue()/360.0; // we store hue as 0-1 as opposed to 0-360 
            hslColor.luminosity = color.GetBrightness();
            hslColor.saturation = color.GetSaturation();
            hslColor.Alpha = color.A;
            return hslColor;
        }

        #endregion
    }
}