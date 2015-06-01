using System.IO;
using System.Serialisation.Core;

namespace System.Serialisation.Advanced.Serializing
{
    /// <summary>
    ///   Serializes property to a stream
    /// </summary>
    public interface IPropertySerializer
    {
        /// <summary>
        ///   Open the stream for writing
        /// </summary>
        /// <param name = "stream"></param>
        void Open(Stream stream);

        /// <summary>
        ///   Serializes property
        /// </summary>
        /// <param name = "property"></param>
        void Serialize(Property property);

        /// <summary>
        ///   Cleaning, but the stream can be used further
        /// </summary>
        void Close();
    }
}