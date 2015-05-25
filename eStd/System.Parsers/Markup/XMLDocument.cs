namespace Creek.Parsers.Markup
{
    public class XMLDocument : MarkupDocument
    {
        private XMLDocument(string xmlText) : base(xmlText, false, (xmlText.Length > CACHE_BOUNDARY), true) { }

        public new static XMLDocument Load(string xmlText)
        {
            return new XMLDocument(xmlText);
        }
    }
}
