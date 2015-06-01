using System;

namespace System.Parsers.Markup
{
    /// <summary>
    /// Class that represents an HTML specific MarkupDocument implementation.
    /// </summary>
    public class HTMLDocument : MarkupDocument
    {
        /// <summary>
        /// Private constructor to be called from the static Load() function.
        /// </summary>
        /// <param name="htmlText">The html text to be loaded into the document.</param>
        private HTMLDocument(string htmlText)
            : base(htmlText, true, (htmlText.Length > CACHE_BOUNDARY ? true : false), true)
        {
        }

        /// <summary>
        /// Internal reference to the document's &lt;HTML&gt; tag.
        /// </summary>
        private MarkupTag Html
        {
            get { return this["html"][0]; }
        }

        /// <summary>
        /// Reference to the document's &lt;HEAD&gt; tag.
        /// </summary>
        public MarkupTag Head
        {
            get { return Html["head"][0]; }
        }

        /// <summary>
        /// Reference to the document's &lt;BODY&gt; tag.
        /// </summary>
        public MarkupTag Body
        {
            get { return Html["body"][0]; }
        }

        /// <summary>
        /// References to all &lt;FORM&gt; tags in the document's &lt;BODY&gt; tag.
        /// </summary>
        public MarkupTag[] Forms
        {
            get { return Body["FORM"]; }
        }

        /// <summary>
        /// Internal static function that validates a markup document object as being a valid HTML document.
        /// </summary>
        /// <param name="document">The document to examine.</param>
        /// <returns>True to false to indicate validity.</returns>
        private static bool ValidateAsHTML(MarkupDocument document)
        {
            bool valid = false;
            valid = document["html"].Length == 1; // should only be 1 HTML tag
            valid = valid && document["html"][0]["head"].Length < 2; // can be 0 or 1 HEAD tag
            valid = valid && document["html"][0]["body"].Length == 1; // should only be 1 BODY tag
            return valid;
        }

        /// <summary>
        /// Static factor method for creating an HTMLDocument object using the supplied text.
        /// </summary>
        /// <param name="htmlText">The html text to load into the document.</param>
        /// <returns>A new HTMLDocument object with the supplied text. Throws an exception if the text is not valid HTML.</returns>
        public new static HTMLDocument Load(string htmlText)
        {
            var doc = new HTMLDocument(htmlText);
            if (!ValidateAsHTML(doc))
            {
                doc = null;
                throw new Exception("The supplied text could not be validated as HTML.");
            }
            return doc;
        }
    }
}