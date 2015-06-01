using System.Globalization;
using System.Serialisation.Core;
using System.Text;

namespace System.Serialisation
{
    /// <summary>
    ///   All the most important settings for xml serialization
    /// </summary>
    public sealed class SharpSerializerXmlSettings : SharpSerializerSettings<AdvancedSharpSerializerXmlSettings>
    {
        ///<summary>
        ///  Standard constructor with Culture=InvariantCulture and Encoding=UTF8
        ///</summary>
        public SharpSerializerXmlSettings()
        {
            Culture = CultureInfo.InvariantCulture;
            Encoding = Encoding.UTF8;
        }

        /// <summary>
        ///   All float numbers and date/time values are stored as text according to the Culture. Default is CultureInfo.InvariantCulture.
        ///   This setting is overridden if you set AdvancedSettings.SimpleValueConverter
        /// </summary>
        public CultureInfo Culture { get; set; }

        /// <summary>
        ///   Describes format in which the xml file is stored. Default is UTF-8.
        ///   This setting is overridden if you set AdvancedSettings.XmlWriterSettings
        /// </summary>
        public Encoding Encoding { get; set; }
    }
}