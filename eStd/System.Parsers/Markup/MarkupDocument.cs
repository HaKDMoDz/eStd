using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace System.Parsers.Markup
{
    /// <summary>
    /// This class represents a document of markup text with a hierarchy structure of markup tags.
    /// </summary>
    public class MarkupDocument
    {
        protected static readonly int CACHE_BOUNDARY = 4096; // 4096 Bytes = 4 KB

        #region Static Known Inline members

        private static List<string> knownInlineTags;

        /// <summary>
        /// This property is the list of known inline tag names.
        /// </summary>
        public static List<string> KnownInlineTags
        {
            get
            {
                if (knownInlineTags == null)
                {
                    knownInlineTags = new List<string>();
                    // Put additional items here as needed...
                    knownInlineTags.Add("?xml"); // flag the <?xml .... > tag as a known inline tag
                }
                return knownInlineTags;
            }
        }

        /// <summary>
        /// This internal function will compare the supplied tag name against the list of known inline tags to see if there is a match.
        /// </summary>
        /// <param name="tag">The tag name to test.</param>
        /// <returns>True or false to indicate whether the supplied tag name is a known inline tag.</returns>
        internal static bool IsKnownInlineTag(string tag, StringComparison stringComparison)
        {
            foreach (string knownInlineTag in KnownInlineTags)
            {
                if (tag.Equals(knownInlineTag, stringComparison))
                    return true;
            }
            return false;
        }

        #endregion

        private readonly bool fixBadlyFormedInlineTags;
        private readonly List<MarkupTag> rootTags = new List<MarkupTag>();
        private bool caseSensitiveComparisons;
        private StringComparison stringComparison = StringComparison.OrdinalIgnoreCase;
        private string text;
        private bool useCaching;

        #region Constructors

        /// <summary>
        /// This is an alternate constructor that defaults the FixBadlyFormedInlineTags to true and UseCaching to false. This constructor is best used with small and medium sized HTML documents.
        /// </summary>
        /// <param name="markupText">The string that contains the markup text to be parsed.</param>
        public MarkupDocument(string markupText) : this(markupText, true, false, false)
        {
        }

        /// <summary>
        /// This is the main constructor for the class.
        /// </summary>
        /// <param name="markupText">The string that contains the markup text to be parsed.</param>
        /// <param name="fixBadlyFormedInlineTags">Set this option to have the parser find and handle badly formed inline tags. Tags that have an opening tag, but no closing tag. This is somewhat common in HTML, but not usually found in XML. Turning off this option will improve the parse time.</param>
        /// <param name="useCaching">Set this option to have the parser cache copies of the string values it works with. This will improve the parsing performance, but at the cost of increased memory usage. Note that after the document has been created and the parsing is complete the cache is cleared.</param>
        /// <param name="caseSensitiveComparisons">Set this option to have textual comparisons (for matching opening and closing tags, etc.) to be case-sensitive.</param>
        public MarkupDocument(string markupText, bool fixBadlyFormedInlineTags, bool useCaching,
                              bool caseSensitiveComparisons)
        {
            CaseSensitiveComparisons = caseSensitiveComparisons;
            this.fixBadlyFormedInlineTags = fixBadlyFormedInlineTags;
            this.useCaching = useCaching;
            Text = markupText;
        }

        #endregion

        public bool FixBadlyFormedInlineTags
        {
            get { return fixBadlyFormedInlineTags; }
        }

        public bool UseCaching
        {
            get { return useCaching; }
            set { useCaching = value; }
        }

        public bool CaseSensitiveComparisons
        {
            get { return caseSensitiveComparisons; }
            internal set
            {
                caseSensitiveComparisons = value;
                if (caseSensitiveComparisons)
                    stringComparison = StringComparison.Ordinal;
                else
                    stringComparison = StringComparison.OrdinalIgnoreCase;
            }
        }

        internal StringComparison StringComparison
        {
            get { return stringComparison; }
        }

        /// <summary>
        /// The markup text that was loaded into the document.
        /// </summary>
        public string Text
        {
            get { return text; }
            private set
            {
                text = value;
                ParseMarkup();
            }
        }

        /// <summary>
        /// String indexer that indexes into the document's RootTags collection and returns an array of MarkupTags matching the supplied tag name (case-insensitive).
        /// </summary>
        /// <param name="tagName">The name of the desired tag.</param>
        /// <returns>An array of matching MarkupTags (empty if none were found).</returns>
        public MarkupTag[] this[string tagName]
        {
            get
            {
                var retval = new List<MarkupTag>();
                foreach (MarkupTag tag in rootTags)
                {
                    if (tag.Tag.Equals(tagName, stringComparison))
                        retval.Add(tag);
                }
                return retval.ToArray();
            }
        }

        /// <summary>
        /// Integer indexer that indexes into the document's RootTags collection.
        /// </summary>
        /// <param name="index">The index of the desired item.</param>
        /// <returns>The MarkupTag at the supplied index.</returns>
        public MarkupTag this[int index]
        {
            get { return rootTags[index]; }
        }

        /// <summary>
        /// Collection of MarkupTags that are at the root level of the document.
        /// </summary>
        public List<MarkupTag> RootTags
        {
            get { return rootTags; }
        }

        public override string ToString()
        {
            return Text;
        }

        /// <summary>
        /// This is the internal function that does all the work in parsing the markup text and constructing the document objects.
        /// </summary>
        private void ParseMarkup()
        {
#if DEBUG
            Stopwatch sw1, sw2, sw3, sw4, sw5, sw6, sw7;
            TimeSpan totalElapsed;
#endif

            #region Create initial tag objects

#if DEBUG
            sw1 = Stopwatch.StartNew();
#endif
            Match n = null;
            // parse the text into markup tag objects
            var tags = new List<MarkupTag>();
            foreach (Match m in MarkupRegexHelper.ParseMarkupTags(text))
            {
                n = MarkupRegexHelper.GetMarkupTagName(m.Value);
                if (n != null && n.Groups.Count >= 1) // this would indicate an empty tag (<>)
                    tags.Add(new MarkupTag(this, m.Index, m.Length, m.Index + n.Groups[1].Index, n.Groups[1].Length));
            }
#if DEBUG
            sw1.Stop();
#endif

            #endregion

            #region Correct faulty inline tags

#if DEBUG
            sw2 = Stopwatch.StartNew();
#endif
            int tagCount = tags.Count - 1;
            MarkupTag ti = null, tj = null;
            var usedClosingTags = new List<int>();
            if (fixBadlyFormedInlineTags)
            {
                // handle faulty inline tags
                bool closingTagFound;
                for (int i = 0; i <= tagCount; i++)
                {
                    closingTagFound = false;
                    // zip through tags that we aren't interested in
                    do
                    {
                        ti = tags[i];
                        if (ti.Inline || ti.Comment || ti.IsClosingTag)
                            i++;
                        else
                            break;
                    } while (i <= tagCount);
                    // if this was the last tag and it was a closing tag stop here
                    if (i > tagCount && ti.IsClosingTag)
                        break;
                    for (int j = i + 1; j <= tagCount; j++)
                    {
                        // zip through tags that we aren't interested in; we want to find a closing tag that hasn't been used
                        do
                        {
                            tj = tags[j];
                            if (tj.IsClosingTag && !usedClosingTags.Contains(j))
                                break;
                            else
                                j++;
                        } while (j <= tagCount);
                        if (tj.Tag.Equals("/" + ti.Tag, stringComparison))
                        {
                            usedClosingTags.Add(j);
                            closingTagFound = true;
                            // skip ahead one if we have a closing tag immediately after it's opening tag
                            if (j == i + 1)
                                i++;
                            break;
                        }
                    }
                    // no closing tag was found, so we'll mark the tag as an inline tag
                    if (!closingTagFound)
                        ti.Inline = true;
                }
                usedClosingTags.Clear();
                //usedClosingTags = null;
                //ti = null;
                //tj = null;
            }
#if DEBUG
            sw2.Stop();
#endif

            #endregion

            #region Construct document hierarchy

#if DEBUG
            sw3 = Stopwatch.StartNew();
#endif
            // now process the tag objects to form the document hierarchy
            var tagQueue = new Queue<MarkupTag>(tags);
            var tagStack = new Stack<MarkupTag>(1);
            var processedTags = new List<MarkupTag>(tagCount + 1);
            MarkupTag currentTag = null, parentTag = null;
            var closedTagIndices = new List<int>(tagCount + 1);
            int nestingLevel = 0;
            while (tagQueue.Count > 0)
            {
                currentTag = tagQueue.Dequeue();
                if (tagStack.Count > 0)
                    parentTag = tagStack.Pop();
                if (currentTag.Inline || currentTag.Comment) // inline and comment tags
                {
                    currentTag.Parent = parentTag;
                    currentTag.NestingLevel = nestingLevel;
                    processedTags.Add(currentTag);
                    if (parentTag != null)
                    {
                        parentTag.Children.Add(currentTag);
                        tagStack.Push(parentTag);
                    }
                    else
                        rootTags.Add(currentTag);
                }
                else if (currentTag.IsClosingTag) // normal close tag
                {
                    // find the corresponding opening tag and use it's parent for the parent of this closing tag
                    for (int i = processedTags.Count - 1; i >= 0; i--)
                    {
                        while (processedTags[i].Inline || processedTags[i].Comment || processedTags[i].IsClosingTag)
                        {
                            i--;
                        }
                        if (currentTag.Tag.Equals("/" + processedTags[i].Tag, stringComparison) &&
                            !closedTagIndices.Contains(i))
                        {
                            parentTag = processedTags[i].Parent;
                            closedTagIndices.Add(i);
                            break;
                        }
                    }
                    if (parentTag != null)
                        nestingLevel = parentTag.NestingLevel + 1;
                    else
                        nestingLevel = 0;
                    currentTag.NestingLevel = nestingLevel;
                    // parentTag = parentTag.Parent;
                    if (parentTag == null) // end of processing!
                    {
                        rootTags.Add(currentTag);
                        break;
                    }
                    currentTag.Parent = parentTag;
                    parentTag.Children.Add(currentTag);
                    processedTags.Add(currentTag);
                    tagStack.Push(parentTag);
                }
                else // normal open tag
                {
                    currentTag.NestingLevel = nestingLevel;
                    nestingLevel++;
                    currentTag.Parent = parentTag;
                    processedTags.Add(currentTag);
                    if (parentTag != null)
                        parentTag.Children.Add(currentTag);
                    else
                        rootTags.Add(currentTag);
                    tagStack.Push(currentTag);
                }
            }
            // these items are no longer needed
            processedTags.Clear();
            processedTags = null;
            tagStack.Clear();
            tagStack = null;
            tagQueue.Clear();
            tagQueue = null;
            closedTagIndices.Clear();
            closedTagIndices = null;
            currentTag = null;
            parentTag = null;
#if DEBUG
            sw3.Stop();
#endif

            #endregion

            #region Associate normal opening tags with their corresponding closing tags

#if DEBUG
            sw4 = Stopwatch.StartNew();
#endif
            usedClosingTags = new List<int>();
            // use the initial list that still has references to all of the tag objects for this work
            for (int i = 0; i < tagCount; i++)
            {
                // skip these kinds of tags; we're looking for a normal opening tag to process
                do
                {
                    ti = tags[i];
                } while ((ti.Inline || ti.Comment || ti.ClosingTag != null || ti.IsClosingTag) && ++i < tagCount);
                // skip these kinds of tags; we're looking for a normal opening tag to process
                for (int j = i + 1; j <= tagCount; j++)
                {
                    // skip these kinds of tags; we're looking for a normal closing tag for the current normal opening tag
                    do
                    {
                        tj = tags[j];
                    } while ((!tj.IsClosingTag || tj.NestingLevel != ti.NestingLevel || usedClosingTags.Contains(j)) &&
                             ++j <= tagCount);
                    // skip these kinds of tags; we're looking for a normal closing tag for the current normal opening tag
                    // this will be a matching closing tag for the current opening tag
                    if (tj.Tag.Equals("/" + ti.Tag, stringComparison))
                    {
                        ti.ClosingTag = tj;
                        usedClosingTags.Add(j);
                        break;
                    }
                }
            }
            usedClosingTags.Clear();
            usedClosingTags = null;
            ti = null;
            tj = null;
#if DEBUG
            sw4.Stop();
#endif

            #endregion

            #region Validate the markup

#if DEBUG
            sw5 = Stopwatch.StartNew();
#endif
            bool valid = IsValidMarkup(tags);
#if DEBUG
            sw5.Stop();
#endif
            if (!valid)
                throw new Exception("The supplied markup text is invalid.");

            #endregion

            #region Remove closing tags from the root tags collection and from all child objects

#if DEBUG
            sw6 = Stopwatch.StartNew();
#endif
            // handle the root tags
            var tagsToRemove = new List<MarkupTag>();
            foreach (MarkupTag t in rootTags)
            {
                if (t.IsClosingTag)
                    tagsToRemove.Add(t);
            }
            foreach (MarkupTag t in tagsToRemove)
            {
                rootTags.Remove(t);
            }
            // now all the child tags
            tagStack = new Stack<MarkupTag>();
            processedTags = new List<MarkupTag>(tagCount);
            foreach (MarkupTag rootTag in rootTags)
            {
                if (rootTag.Children.Count > 0)
                    tagStack.Push(rootTag);
            }
            while (tagStack.Count > 0)
            {
                currentTag = tagStack.Pop();
                if (processedTags.Contains(currentTag))
                    continue;
                tagsToRemove.Clear();
                foreach (MarkupTag child in currentTag.Children)
                {
                    // if it's a closing tag flag it to be removed, otherwise if it has children we'll put it on the stack
                    if (child.IsClosingTag)
                        tagsToRemove.Add(child);
                    else if (child.Children.Count > 0)
                        tagStack.Push(child);
                }
                foreach (MarkupTag t in tagsToRemove)
                {
                    currentTag.Children.Remove(t);
                }
            }
#if DEBUG
            sw6.Stop();
#endif

            #endregion

            #region Clear the cache that was used for parsing

#if DEBUG
            sw7 = Stopwatch.StartNew();
#endif
            if (useCaching)
                ClearCache();
#if DEBUG
            sw7.Stop();
#endif

            #endregion

#if DEBUG
            totalElapsed = sw1.Elapsed + sw2.Elapsed + sw3.Elapsed + sw4.Elapsed + sw5.Elapsed + sw6.Elapsed +
                           sw7.Elapsed;
#endif
        }

        /// <summary>
        /// Clears the cache from all objects in the document.
        /// </summary>
        public void ClearCache()
        {
            foreach (MarkupTag rootTag in rootTags)
            {
                rootTag.ClearCache();
            }
        }

        /// <summary>
        /// Returns the MarkupTag with the supplied id attribute.
        /// </summary>
        /// <param name="id">The id value of the desired tag.</param>
        /// <returns>The tag whose id attribute matches the supplied value (case-insensitive), or null if not found.</returns>
        public MarkupTag GetTagById(string id)
        {
            MarkupTag retval = null;
            MarkupTag[] tags = GetTagsByAttribute("id", id);
            if (tags.Length > 0)
                retval = tags[0];
            return retval;
        }

        /// <summary>
        /// Returns the MarkupTag with the supplied name attribute.
        /// </summary>
        /// <param name="name">The name value of the desired tag.</param>
        /// <returns>The tag whose name attribute matches the supplied value (case-insensitive), or null if not found.</returns>
        public MarkupTag GetTagByName(string name)
        {
            MarkupTag retval = null;
            MarkupTag[] tags = GetTagsByAttribute("name", name);
            if (tags.Length > 0)
                retval = tags[0];
            return retval;
        }

        /// <summary>
        /// Returns an array of MarkupTags whose attributes match the supplied criteria.
        /// </summary>
        /// <param name="attributeName">The name of the desired attribute to compare against (case-insensitive).</param>
        /// <param name="attributeValue">The value of the desired attribute to compare against (case-insensitive).</param>
        /// <returns>An array of MarkupTags that have attributes matching the supplied criteria.</returns>
        private MarkupTag[] GetTagsByAttribute(string attributeName, string attributeValue)
        {
            List<MarkupTag> results = new List<MarkupTag>(), processedTags = new List<MarkupTag>();
            var processStack = new Stack<MarkupTag>();
            MarkupTag rootTag = null, t = null, currentTag = null;
            int i;
            for (i = 0; i < rootTags.Count; i++)
            {
                rootTag = rootTags[i];
                if (rootTag.Comment || rootTag.IsClosingTag)
                    continue;
                processStack.Push(rootTag);
            }
            while (processStack.Count > 0)
            {
                currentTag = processStack.Pop();
                if (processedTags.Contains(currentTag))
                    continue;
                // search current tag to see if it matches what we're looking for
                foreach (string testName in currentTag.Attributes.Keys)
                {
                    if (testName.Equals(attributeName, stringComparison))
                    {
                        // check for a value match
                        if (currentTag.Attributes[testName].Equals(attributeValue, stringComparison))
                            results.Add(currentTag);
                        // had the attribute name, but value was not a match; or we had a match; either way break here
                        break;
                    }
                }
                processedTags.Add(currentTag);
                // now search the tag's children
                for (i = 0; i < currentTag.Children.Count; i++)
                {
                    t = currentTag.Children[i];
                    if (t.Comment || t.IsClosingTag || (t.Children.Count == 0 && t.Attributes.Count == 0))
                        // we'll skip these types as they won't have attributes to search
                        continue;
                    processStack.Push(t);
                }
            }
            return results.ToArray();
        }

        private static bool IsValidMarkup(List<MarkupTag> tags)
        {
            for (int i = 0; i < tags.Count; i++)
            {
                if (tags[i].Inline || tags[i].Comment)
                    continue;
                if (!tags[i].IsClosingTag && tags[i].ClosingTag == null)
                    return false;
            }
            return true;
        }

        #region Static Factory Methods

        /// <summary>
        /// Static factory method that will set all the appropriate options for parsing markup data.
        /// </summary>
        /// <param name="htmlText">The text to be parsed.</param>
        /// <returns>A MarkupDocument configured with appropriate options.</returns>
        public static MarkupDocument Load(string markupText)
        {
            // always use fixBadlyFormedInlineTags for markup data
            return LoadCore(markupText, true);
        }

        /// <summary>
        /// Internal factory method that will set the caching option if the text is greater than 4K.
        /// </summary>
        /// <param name="markupText">The markup to be parsed.</param>
        /// <param name="fixBadlyFormedInlineTags">Whether or not to account for badly formed inline tags (primarily for HTML data).</param>
        /// <returns>A MarkupDocument configured with the supplied options.</returns>
        private static MarkupDocument LoadCore(string markupText, bool fixBadlyFormedInlineTags)
        {
            // if more that 4K of text, use caching
            return new MarkupDocument(markupText, fixBadlyFormedInlineTags,
                                      (markupText.Length > CACHE_BOUNDARY ? true : false), true);
        }

        #endregion
    }
}