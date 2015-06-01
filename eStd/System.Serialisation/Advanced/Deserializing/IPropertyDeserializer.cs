using System.IO;
using System.Serialisation.Core;

namespace System.Serialisation.Advanced.Deserializing
{
    /// <summary>
    ///   Deserializes a stream and gives back a Property
    /// </summary>
    public interface IPropertyDeserializer
    {
        /// <summary>
        ///   Open the stream to read
        /// </summary>
        /// <param name = "stream"></param>
        void Open(Stream stream);

        /// <summary>
        ///   Reading the stream
        /// </summary>
        /// <returns></returns>
        Property Deserialize();

        /// <summary>
        ///   Cleans all
        /// </summary>
        void Close();
    }
}