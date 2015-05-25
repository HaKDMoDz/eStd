// #define TEST_KNOWN_INLINE_TAGS

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace Creek.Parsers.Markup
{
    /// <summary>
    /// This class represents an individual tag element from the markup text.
    /// </summary>
    public class MarkupTag
    {
        private static readonly string INLINE_ENDSWITH_TEXT = "/>",
            COMMENT_BEGINS_WITH_TEXT = "<!--";

        private bool inline = false, comment = false;
        private int tagIndexBegin, tagLength, tagNameIndexBegin, tagNameLength, nestingLevel;
        private string tagCache = null, tagNameCache = null, innerTextCache = null;
        private List<MarkupAttribute> attributes = new List<MarkupAttribute>();
        private List<MarkupTag> children = new List<MarkupTag>();
        private MarkupTag parent = null, closingTag = null;
        private MarkupDocument document = null;
        private NameValueCollection attributeCache = null;

        /// <summary>
        /// This is the only constructor for this class.
        /// </summary>
        /// <param name="document">The owning MarkupDocument object.</param>
        /// <param name="tagIndexBegin">The index in the document text at which this tag begins.</param>
        /// <param name="tagLength">The text length of this tag.</param>
        /// <param name="tagNameIndexBegin">The index in the document text at which the name of this tag begins.</param>
        /// <param name="tagNameLength">The text length of the name of this tag.</param>
        internal MarkupTag(MarkupDocument document, int tagIndexBegin, int tagLength, int tagNameIndexBegin, int tagNameLength)
        {
            this.document = document;
            this.tagIndexBegin = tagIndexBegin;
            this.tagLength = tagLength;
            this.tagNameIndexBegin = tagNameIndexBegin;
            this.tagNameLength = tagNameLength;
            string text = this.Text.Trim();
            inline = text.EndsWith(INLINE_ENDSWITH_TEXT) || MarkupDocument.IsKnownInlineTag(Tag, document.StringComparison);
            comment = text.StartsWith(COMMENT_BEGINS_WITH_TEXT);
            ParseAttributes();
        }

        /// <summary>
        /// Internal function that determines if this tag contains a specific index position in the document text; similar to a hit-test.
        /// </summary>
        /// <param name="indexPosition">The position to be tested.</param>
        /// <returns>True or false to indicate whether the supplied index is within the textual boundaries of this tag.</returns>
        internal bool Contains(int indexPosition)
        {
            return this.tagIndexBegin <= indexPosition && indexPosition <= tagIndexBegin + tagLength;
        }

        /// <summary>
        /// The MarkupDocument object to which this tag belongs.
        /// </summary>
        public MarkupDocument Document
        {
            get { return document; }
        }

        /// <summary>
        /// The text string of the tag from the document text.
        /// </summary>
        public string Text
        {
            get 
            {
                if (document.UseCaching)
                {
                    if (tagCache == null)
                        tagCache = document.Text.Substring(tagIndexBegin, tagLength);
                    return tagCache;
                }
                else
                    return document.Text.Substring(tagIndexBegin, tagLength);
            }
        }

        /// <summary>
        /// The name of the tag.
        /// </summary>
        public string Tag
        {
            get 
            {
                if (document.UseCaching)
                {
                    if (tagNameCache == null)
                        tagNameCache = document.Text.Substring(tagNameIndexBegin, tagNameLength);
                    return tagNameCache;
                }
                else
                    return document.Text.Substring(tagNameIndexBegin, tagNameLength);
            }
        }

        public override string ToString()
        {
            return Text;
        }

        /// <summary>
        /// Returns a NameValueCollection of the tag's attributes.
        /// </summary>
        /// <returns></returns>
        public NameValueCollection Attributes
        {
            get
            {
                NameValueCollection attributeCacheTemp = null;
                if (document.UseCaching)
                {
                    if (attributeCache == null)
                    {
                        attributeCache = new NameValueCollection(attributes.Count);
                        foreach (MarkupAttribute a in attributes)
                        {
                            attributeCache.Add(a.Name, a.Value);
                        }
                    }
                    attributeCacheTemp = attributeCache;
                }
                else
                {
                    attributeCacheTemp = new NameValueCollection(attributes.Count);
                    foreach (MarkupAttribute a in attributes)
                    {
                        attributeCacheTemp.Add(a.Name, a.Value);
                    }
                }
                return attributeCacheTemp;
            }
        }

        /// <summary>
        /// Whether or not this is considered an inline tag (i.e. a tag that does not have a corresponsing closing tag).
        /// </summary>
        public bool Inline
        {
            get { return inline; }
            internal set { inline = value; }
        }

        /// <summary>
        /// Whether or not this tag is considered a comment.
        /// </summary>
        public bool Comment
        {
            get { return comment; }
        }

        /// <summary>
        /// Whether this tag is considered a closing tag.
        /// </summary>
        public bool IsClosingTag
        {
            get { return (Tag.Length > 0 && Tag[0] == '/'); }
        }

        /// <summary>
        /// Internal function that analyzes the tag text and identifies the attributes of the tag.
        /// </summary>
        private void ParseAttributes()
        {
            int indexBegin, length, nameIndexBegin, nameLength, valueIndexBegin, valueLength;
            indexBegin = length = nameIndexBegin = nameLength = valueIndexBegin = valueLength = 0;
            foreach (Match m in MarkupRegexHelper.ParseTagAttributes(Text))
            {
                if (m.Length > 0 && m.Groups.Count >= 3 && m.Groups[1].Length > 0 && m.Groups[2].Length > 0)
                {
                    indexBegin = m.Index;
                    length = m.Length;
                    nameIndexBegin = m.Groups[1].Index;
                    nameLength = m.Groups[1].Length;
                    valueIndexBegin = m.Groups[2].Index;
                    valueLength = m.Groups[2].Length;
                    indexBegin += tagIndexBegin;
                    nameIndexBegin += tagIndexBegin;
                    valueIndexBegin += tagIndexBegin;
                    attributes.Add(new MarkupAttribute(this, indexBegin, length, nameIndexBegin, nameLength, valueIndexBegin, valueLength));
                }
            }
        }

        /// <summary>
        /// MarkupTags that are considered children of (contained within) this tag in the document hierarchy. If there are no children it is a leaf tag in the document.
        /// </summary>
        public List<MarkupTag> Children
        {
            get { return children; }
        }

        /// <summary>
        /// The MarkupTag that is the parent of this tag (this tag is contained within the parent tag). If there is no parent it is a root tag in the document.
        /// </summary>
        public MarkupTag Parent
        {
            get { return parent; }
            internal set { parent = value; }
        }

        /// <summary>
        /// String indexer that indexes into the tag's Children collection and returns an array of MarkupTags matching the supplied tag name (case-insensitive).
        /// </summary>
        /// <param name="tagName">The name of the desired tag.</param>
        /// <returns>An array of matching MarkupTags (empty if none were found).</returns>
        public MarkupTag[] this[string tagName]
        {
            get
            {
                List<MarkupTag> retval = new List<MarkupTag>();
                foreach (MarkupTag tag in children)
                {
                    if (tag.Tag.Equals(tagName, document.StringComparison))
                        retval.Add(tag);
                }
                return retval.ToArray();
            }
        }

        /// <summary>
        /// Integer indexer that indexes into the tag's Children collection.
        /// </summary>
        /// <param name="index">The index of the desired item.</param>
        /// <returns>The MarkupTag at the supplied index.</returns>
        public MarkupTag this[int index]
        {
            get { return children[index]; }
        }

        /// <summary>
        /// A reference to this tag's closing tag. Will be null for items that have no closing tag (inline tags, comments, etc.)
        /// </summary>
        public MarkupTag ClosingTag
        {
            get { return closingTag; }
            internal set { closingTag = value; }
        }

        /// <summary>
        /// The 0-based level of this tag in the document hierarchy.
        /// </summary>
        public int NestingLevel
        {
            get { return nestingLevel; }
            internal set { nestingLevel = value; }
        }

        /// <summary>
        /// The text contained within this tag and it's closing tag. This will be an empty string is the tag has no closing tag.
        /// </summary>
        public string InnerText
        {
            get
            {
                string retval = String.Empty;
                if (document.UseCaching)
                {
                    if (innerTextCache == null)
                    {
                        innerTextCache = String.Empty;
                        if (closingTag != null) // this should limit it to only normal opening tags
                        {
                            int innerTextIndexBegin = (tagIndexBegin + tagLength), innerTextLength = closingTag.tagIndexBegin - innerTextIndexBegin;
                            if (innerTextLength > 0)
                                innerTextCache = document.Text.Substring(innerTextIndexBegin, innerTextLength);
                        }
                    }
                    retval = innerTextCache;
                }
                else
                {
                    if (closingTag != null) // this should limit it to only normal opening tags
                    {
                        int innerTextIndexBegin = (tagIndexBegin + tagLength), innerTextLength = closingTag.tagIndexBegin - innerTextIndexBegin;
                        if (innerTextLength > 0)
                            retval = document.Text.Substring(innerTextIndexBegin, innerTextLength);
                    }
                }
                return retval;
            }
        }

        /// <summary>
        /// Returns the MarkupAttribute with the supplied name (case-insensitive), or null if not found.
        /// </summary>
        /// <param name="attributeName">The name of the desired attribute.</param>
        /// <returns>The MarkupAttribute that was found.</returns>
        public MarkupAttribute GetAttribute(string attributeName)
        {
            MarkupAttribute retval = null;
            foreach (MarkupAttribute a in attributes)
            {
                if (a.Name.Equals(attributeName, document.StringComparison))
                {
                    retval = a;
                    break;
                }
            }
            return retval;
        }

        /// <summary>
        /// Attempts to get the attribute of the supplied name.
        /// </summary>
        /// <param name="attributeName">The name of the desired attributes.</param>
        /// <param name="attribute">The attribute parameter that will be filled by the operation.</param>
        /// <returns>True or false to indicate success or failure.</returns>
        public bool TryGetAttribute(string attributeName, out MarkupAttribute attribute)
        {
            attribute = GetAttribute(attributeName);
            return attribute != null;
        }

        /// <summary>
        /// Internal function to clear this tag's cache and the cache of all child tags.
        /// </summary>
        internal void ClearCache()
        {
            tagCache = tagNameCache = innerTextCache = null;
            if (attributeCache != null)
            {
                attributeCache.Clear();
                attributeCache = null;
            }
            foreach (MarkupTag childTag in children)
            {
                childTag.ClearCache();
            }
        }
    }
}
