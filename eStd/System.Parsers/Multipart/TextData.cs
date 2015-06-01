using System.Parsers.Multipart;

namespace System.Parsers.Multipart
{
    /// <summary>
    /// A textual part of a multipart request
    /// </summary>
    public class TextData : MultipartData {
        /// <summary>
        /// The part's data
        /// </summary>
        public string Data { get; set; }

        public TextData() {
            IsBinary = false;
        }
    }
}