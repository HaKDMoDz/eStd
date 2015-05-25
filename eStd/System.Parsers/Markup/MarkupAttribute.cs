namespace Creek.Parsers.Markup
{
    /// <summary>
    /// This class represents an attribute item from a markup tag.
    /// </summary>
    public class MarkupAttribute
    {
        private readonly int indexBegin;
        private readonly int length;
        private readonly int nameIndexBegin;
        private readonly int nameLength;
        private readonly MarkupTag tag;
        private readonly int valueIndexBegin;
        private readonly int valueLength;

        /// <summary>
        /// This is the only constructor for this class.
        /// </summary>
        /// <param name="tag">The MarkupTag to which this attribute belongs.</param>
        /// <param name="indexBegin">The index in the document text at which this attribute begins.</param>
        /// <param name="length">The text length of this attribute.</param>
        /// <param name="nameIndexBegin">The index in the document text at which the name of this attribute begins.</param>
        /// <param name="nameLength">The text length of the name of this attribute.</param>
        /// <param name="valueIndexBegin">The index in the document text at which the value of this attribute begins.</param>
        /// <param name="valueLength">The text length of the value of this attribute.</param>
        internal MarkupAttribute(MarkupTag tag, int indexBegin, int length, int nameIndexBegin, int nameLength,
                                 int valueIndexBegin, int valueLength)
        {
            this.tag = tag;
            this.indexBegin = indexBegin;
            this.length = length;
            this.nameIndexBegin = nameIndexBegin;
            this.nameLength = nameLength;
            this.valueIndexBegin = valueIndexBegin;
            this.valueLength = valueLength;
        }

        /// <summary>
        /// The text string of the attribute from the document text.
        /// </summary>
        public string Text
        {
            get { return tag.Document.Text.Substring(indexBegin, length); }
        }

        /// <summary>
        /// The name of the attribute.
        /// </summary>
        public string Name
        {
            get { return tag.Document.Text.Substring(nameIndexBegin, nameLength); }
        }

        /// <summary>
        /// The value of the attribute, without quotes.
        /// </summary>
        public string Value
        {
            get { return RemoveSurroundingQuotes(tag.Document.Text.Substring(valueIndexBegin, valueLength)); }
        }

        public override string ToString()
        {
            return Text;
        }

        /// <summary>
        /// Internal function to handle the removal of quotes from an attribute value; handles single and double quotes.
        /// </summary>
        /// <param name="value">The value string to work with.</param>
        /// <returns>The value string with the surrounding quotes, if any, removed.</returns>
        private static string RemoveSurroundingQuotes(string value)
        {
            if ((value[0] == '\'' && value[value.Length - 1] == '\'') ||
                (value[0] == '\"' && value[value.Length - 1] == '\"'))
                return value.Substring(1, value.Length - 2);
            return value;
        }
    }
}