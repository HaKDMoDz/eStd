using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace System.Parsers.Markup
{
    public static class MarkupExtensions
    {
        public static MarkupTag GetElementByTagName(this List<MarkupTag> lm, string name)
        {
            return (from ml in lm where ml.Tag == name select ml).FirstOrDefault();
        }
        public static IEnumerable<MarkupTag> GetElementsByTagName(this List<MarkupTag> lm, string name)
        {
            return lm.Where(markupTag => markupTag.Tag == name);
        }

        public static string GetAttributeByName(this NameValueCollection lm, string name)
        {
            return lm[name];
        }
        public static string GetAttributeByName(this MarkupTag t, string name)
        {
            return GetAttributeByName(t.Attributes, name);
        }
        public static bool HasAttribute(this NameValueCollection lm, string name)
        {
            return lm[name] != null;
        }
        public static bool HasAttribute(this MarkupTag t, string name)
        {
            return HasAttribute(t.Attributes, name);
        }

    }
}