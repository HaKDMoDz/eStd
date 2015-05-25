using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Creek.Dynamics
{
    public class Instance
    {
        public static dynamic FromXml(string src)
        {
            XDocument xmlDocument = XDocument.Parse(src);
            dynamic xmlContent = new ExpandoObject();
            ExpandoObjectHelper.Parse(xmlContent, xmlDocument.Root, null);

            return xmlContent;
        }

        public static dynamic FromXmlFile(string filename)
        {
            return FromXml(File.ReadAllText(filename));
        }

        public static dynamic FromFunctionBlock(string fb)
        {
            dynamic d = new ExpandoObject();

            foreach (string line in fb.Split(';', '\r', '\n'))
            {
                string[] n = line.Split('(');
                if (n[0] == "define")
                {
                    string[] l = n[1].Remove(n[1].Length - 1, 1).Split(',');

                    string k = l[0];
                    object v = ParseType(l[1]);

                    (d as IDictionary<string, object>)[k] = v;
                }
            }

            return d;
        }

        public static dynamic FromFunction(string s)
        {
            dynamic r = new ExpandoObject();

            string[] n = s.Split('(');
            if (n[0] == "define")
            {
                string[] l = n[1].Remove(n[1].Length - 1, 1).Split(',');

                string k = l[0];
                object v = ParseType(l[1]);

                (r as IDictionary<string, object>)[k] = v;
            }

            return r;
        }

        public static dynamic FromString(string src)
        {
            dynamic c = new ExpandoObject();

            foreach (var k in src.Split(';').Select(s => s.Split('=')))
            {
                if (k[1].StartsWith("{") && k[1].EndsWith("}"))
                {
                    FromStringParseDictionary(k, c);
                }
                else if (k[1].StartsWith("[") && k[1].EndsWith("]"))
                {
                    string r = k[1].Remove(0, 1);
                    r = r.Remove(r.Length - 1, 1);

                    (c as IDictionary<string, object>)[k[0]] = r.Split(',');
                }
                else
                {
                    (c as IDictionary<string, object>)[k[0]] = ParseType(k[1]);
                }
            }

            return c;
        }

        public static dynamic FromAssembly(string filename, string name)
        {
            Assembly ass = Assembly.LoadFile(filename);
            return ass.CreateInstance(name);
        }

        public static IEnumerable<dynamic> FromRegex(string pattern, string input)
        {
            return MatchesDynamic<dynamic>(new Regex(pattern), input);
        }

        #region "Private Functions + Class"

        private static IEnumerable<T> MatchesDynamic<T>(Regex regex, string input, ICustomConverter<T> converter = null)
            where T : class, new()
        {
            string[] groupNames = regex.GetGroupNames();

            foreach (Match m in regex.Matches(input).OfType<Match>().Where(f => f.Success))
            {
                dynamic itm = new DynamicResult(m, groupNames);

                for (int i = 1; i < m.Groups.Count; ++i)
                {
                    string groupName = regex.GroupNameFromNumber(i);

                    if (converter != null)
                        converter.Convert(groupName, m.Groups[i], itm);
                }

                yield return itm;
            }
        }

        private static void FromStringParseDictionary(string[] k, dynamic parent)
        {
            string r = k[1].Replace("{", "").Remove(0, 1);
            r = r.Remove(r.Length - 1, 1);
            dynamic d = new ExpandoObject();
            foreach (string ri in r.Split(','))
            {
                string[] rik = ri.Split(':');
                (d as IDictionary<string, object>)[rik[0]] = ParseType(rik[1]);
            }
            (parent as IDictionary<string, object>)[k[0]] = d;
        }

        private static object ParseType(string s)
        {
            int ir;
            double dr;
            bool br;

            if (int.TryParse(s, out ir))
                return int.Parse(s);
            if (double.TryParse(s, out dr))
                return double.Parse(s);
            if (bool.TryParse(s, out br))
                return bool.Parse(s);
            return s;
        }

        #region Nested type: DynamicResult

        private sealed class DynamicResult : DynamicObject
        {
            private readonly Dictionary<string, string> values;

            public DynamicResult(Match match, string[] groupNames)
            {
                values = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

                foreach (string grpName in groupNames)
                    values.Add(grpName, match.Groups[grpName].Value);
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                string grpValue = null;

                result = values.TryGetValue(binder.Name, out grpValue) ? grpValue : null;
                return result != null;
            }

            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                values[binder.Name] = value.ToString();
                return true;
            }
        }

        #endregion

        #region Nested type: ICustomConverter

        public interface ICustomConverter<T>
        {
            // Liefert true, falls manuell konvertiert wurde, ansonsten false
            bool Convert(string groupName, Group group, T item);
        }

        #endregion

        #endregion
    }
}