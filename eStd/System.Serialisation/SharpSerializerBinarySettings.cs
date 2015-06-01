using System.Serialisation.Core;
using System.Text;

namespace System.Serialisation
{
    /// <summary>
    ///   All the most important settings for binary serialization
    /// </summary>
    public sealed class SharpSerializerBinarySettings : SharpSerializerSettings<AdvancedSharpSerializerBinarySettings>
    {
        /// <summary>
        ///   Default constructor. Serialization in SizeOptimized mode. For other modes choose an overloaded constructor
        /// </summary>
        public SharpSerializerBinarySettings()
        {
            Encoding = Encoding.UTF8;
        }

        /// <summary>
        ///   Overloaded constructor. Chooses mode in which the data is serialized.
        /// </summary>
        /// <param name = "mode">SizeOptimized - all types are stored in a header, objects only reference these types (better for collections). Burst - all types are serialized with their objects (better for serializing of single objects).</param>
        public SharpSerializerBinarySettings(BinarySerializationMode mode)
        {
            Encoding = Encoding.UTF8;
            Mode = mode;
        }

        /// <summary>
        ///   How are strings serialized. Default is UTF-8.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        ///   Default is SizeOptimized - Types and property names are stored in a header. The opposite is Burst mode when all types are serialized with their objects.
        /// </summary>
        public BinarySerializationMode Mode { get; set; }
    }
}