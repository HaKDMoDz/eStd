using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Extensions
{
    public class Tools
    {

        public static Font LoadFont(Assembly a, string resource)
        {
            var fontStream = a.GetManifestResourceStream(resource);
            var data = Marshal.AllocCoTaskMem((int)fontStream.Length);
            var fontdata = new byte[fontStream.Length];
            fontStream.Read(fontdata, 0, (int)fontStream.Length);
            Marshal.Copy(fontdata, 0, data, (int)fontStream.Length);
            var private_fonts = new PrivateFontCollection();
            private_fonts.AddMemoryFont(data, (int)fontStream.Length);
            fontStream.Close();
            Marshal.FreeCoTaskMem(data);
            return new Font(private_fonts.Families[0], 12);
        }

        public static string AutoUpper(string input)
        {
	        var returns = new List<char>(input.ToCharArray());

	        for (var l = 0; l <= returns.Count - 1; l++) {
		        if (!(char.IsWhiteSpace(returns[l]))) {
			        returns[l] = Convert.ToChar(returns[l].ToString().ToUpper());
			        break;
		        }
	        }

	        for (var i = 0; i <= returns.Count - 1; i++) {
		        if (i != returns.Count - 1) {
			        if ((returns[i].ToString() == ".")) {
				        if (input[i + 1].ToString() == " ") {
					        returns[i + 2] = Convert.ToChar(returns[i + 2].ToString().ToUpper());
				        } else {
					        returns[i + 1] = Convert.ToChar(returns[i + 1].ToString().ToUpper());
				        }
			        }
		        }
	        }

	        var sb = new StringBuilder();
	        foreach (var c in returns) {
		        sb.Append(c);
	        }

	        return sb.ToString();
        }

    }
}