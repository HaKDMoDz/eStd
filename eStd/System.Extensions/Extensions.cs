using System;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace Creek.Extensions
{
    public static class Extensions
    {
        public static object IIf(bool Expression, object TruePart, object FalsePart)
        {
            return Expression ? TruePart : FalsePart;
        }

        public static T IIf<T>(bool Condition, T TruePart, T FalsePart)
        {
            return Condition ? TruePart : FalsePart;
        }

        public static string ClassPropertiesToString(object obj)
        {
            var retVal = obj.GetType() + "\n";
            const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;
            var sourceProperties = obj.GetType().GetProperties(Flags);

            return sourceProperties.Where(pi => pi.PropertyType.Namespace == "System").Aggregate(retVal, (current, pi) => current + string.Format("{0}:{1}\n", pi.Name, pi.GetValue(obj, null)));
        }

        public static string ReplaceMany(this string s, params string[] kv)
        {
            return kv.Aggregate(s, (current, kvs) => current.Replace(kvs[0], kvs[1]));
        }

        public static void Form_in_Form(this System.Windows.Forms.Form Form, System.Windows.Forms.Form added_Form)
        {
            added_Form.TopLevel = false;
            added_Form.Visible = true;
            Form.Controls.Add(added_Form);
        }

        public static Color percent(this Color s, int percentage)
        {
                return Color.FromArgb(s.ToArgb() /100 * percentage);
            
        }

        public static string randomize(string input, int charamount)
        {
            var r = new Random();
            var strb = new System.Text.StringBuilder();
            var chararray = input.ToCharArray();
            for (var i = 1; i <= charamount; i++)
            {
                var ti = r.Next(0, chararray.Length);
                strb.Append(chararray[ti]);
            }
            return strb.ToString();
        }

        public static string Middle(string str, string startchar, string endchar)
        {
            var strStart = (str.IndexOf(startchar) + 1);
            var strEnd = str.LastIndexOf(endchar);
            //- 1
            return str.Substring(Convert.ToInt32(strStart), strEnd - strStart);
        }

        public static void Raise<T>(this EventHandler<T> eventHandler, Object sender, T e) where T : EventArgs
        {
            if (eventHandler != null)
            {
                eventHandler(sender, e);
            }
        }
    }
}
