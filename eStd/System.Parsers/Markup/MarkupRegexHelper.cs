using System.Text.RegularExpressions;

namespace Creek.Parsers.Markup
{
    public class MarkupRegexHelper
    {
        /// <summary>
        /// This RegEx string will identify all text elements that match a markup tag pattern.
        /// </summary>
        private static readonly string MarkupDocumentTagsRegEx = "<[/\\?\\!\\-]*[\\w][\\w\\s\\d=\"\'.,:;/\\?_\\-~!@#$%\\^\\&\\*\\(\\)\\[\\]\\{\\}]*\\s*>";
        /// <summary>
        /// This RegEx string will identify the name of a tag from the entire text of the tag.
        /// </summary>
        private static readonly string MarkupTagNameRegEx = "<\\s*[\\!]*([/\\?]?[a-zA-Z][a-zA-Z0-9]*)[\\w\\s\"\':;!.,\\?=@#$%()\\*/\\\\&\\-\\+\\^\\{\\}\\[\\]~]*>";

        /// <summary>
        /// This RegEx string will identify the attributes of a tag form the entire text of the tag.
        /// </summary>
        private static readonly string MarkupTagAttributesRegEx = @"(\S+)=[""']?((?:.(?![""']?)?\s+(?:\S+)(=|[>""']))+.)[""']?"; //"\\b([a-zA-Z][a-zA-Z_0-9]*)\\s*=\\s*(\"([\\w\\s\'.,?/\\:;!@#$%^&*()=\\-\\+\\^\\{\\}\\[\\]~]*)\"|\'([\\w\\s\".,?/\\:;!@#$%^&*()=\\-\\+\\^\\{\\}\\[\\]~]*)\'|([\\w.,?/\\:;!@#$%^&*()=\\-\\+\\^\\{\\}\\[\\]~]*))";
        /// <summary>
        /// This RegEx options object is set to use all the options we'll need to make comparisons against markup text.
        /// </summary>
        private static readonly RegexOptions STANDARD_REGEX_OPTIONS = RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase;

        /// <summary>
        /// This helper function runs a predefined RegEx against the supplied markup tag text to help with identifying the tag's name.
        /// </summary>
        /// <param name="markupTagText"></param>
        /// <returns></returns>
        public static Match GetMarkupTagName(string markupTagText)
        {
            return RunStandardSingleRegEx(markupTagText, MarkupTagNameRegEx);
        }

        /// <summary>
        /// This helper function runs a predefined RegEx against the supplied markup tag text to help with identifying the tag's attributes.
        /// </summary>
        /// <param name="tagText"></param>
        /// <returns></returns>
        public static MatchCollection ParseTagAttributes(string markupTagText)
        {
            return RunStandardMultiRegEx(markupTagText, MarkupTagAttributesRegEx);
        }

        /// <summary>
        /// This helper function runs a predefined RegEx against the supplied markup document text to help with identifying the tags in the document.
        /// </summary>
        /// <param name="documentText"></param>
        /// <returns></returns>
        public static MatchCollection ParseMarkupTags(string documentText)
        {
            return RunStandardMultiRegEx(documentText, MarkupDocumentTagsRegEx);
        }

        /// <summary>
        /// This helper function runs the supplied RegEx pattern against the supplied text and returns a single Match.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private static Match RunStandardSingleRegEx(string text, string pattern)
        {
            return Regex.Match(text, pattern, STANDARD_REGEX_OPTIONS);
        }

        /// <summary>
        /// This helper function runs the supplied RegEx pattern against the supplied text and returns a MatchCollection.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private static MatchCollection RunStandardMultiRegEx(string text, string pattern)
        {
            return Regex.Matches(text, pattern, STANDARD_REGEX_OPTIONS);
        }

        /// <summary>
        /// This helper function runs the supplied RegEx pattern against the supplied text and returns whether or not a match was found.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private static bool CheckStandardRegEx(string text, string pattern)
        {
            return Regex.IsMatch(text, pattern, STANDARD_REGEX_OPTIONS);
        }
    }
}
