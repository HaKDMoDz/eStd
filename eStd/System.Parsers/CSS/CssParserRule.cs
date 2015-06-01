using System.Collections.Generic;
using System.Parsers.CSS;

namespace System.Parsers.CSS
{
    /// <summary>
    /// Class to hold information for single CSS rule.
    /// </summary>
    public class CssParserRule
    {
        public CssParserRule(string media)
        {
            Selectors = new List<string>();
            Declarations = new List<CssParserDeclaration>();
            Media = media;
        }

        public string Media { get; set; }
        public IEnumerable<string> Selectors { get; set; }
        public IEnumerable<CssParserDeclaration> Declarations { get; set; }
    }
}