using System.Text;
using System.Windows.Forms;
using System;
using System.Runtime.InteropServices;

namespace Creek.IO
{
    public static class Keyboard
    {
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(Keys vKey); 

        public static bool IsKeyDown(Keys keys)
        {
            return Convert.ToBoolean(GetAsyncKeyState(keys));
        }

        public static void Push(params object[] keys)
        {
            var sb = new StringBuilder();

            foreach (var key in keys)
            {
                if(key is Keys)
                {
                    sb.Append("{" + Enum.GetName(typeof(Keys), key) + "}");
                }
                else
                {
                    sb.Append(key);
                }
            }
            SendKeys.Send(sb.ToString());
        }



    }
}