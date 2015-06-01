using System.Globalization;
using System.Serialisation.Advanced;
using System.Serialisation.Advanced.Serializing;
using System.Serialisation.Advanced.Xml;
using System.Text;
using System.Xml;

namespace System.Serialisation.Core
{
    /// <summary>
    ///   Gives standard settings for the framework. Is used only internally.
    /// </summary>
    internal static class DefaultInitializer
    {
        public static XmlWriterSettings GetXmlWriterSettings()
        {
            return GetXmlWriterSettings(Encoding.UTF8);
        }


        public static XmlWriterSettings GetXmlWriterSettings(Encoding encoding)
        {
            var settings = new XmlWriterSettings();
            settings.Encoding = encoding;
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            return settings;
        }

        public static XmlReaderSettings GetXmlReaderSettings()
        {
            var settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;
            return settings;
        }

        public static ITypeNameConverter GetTypeNameConverter(bool includeAssemblyVersion, bool includeCulture,
                                                              bool includePublicKeyToken)
        {
            return new TypeNameConverter(includeAssemblyVersion, includeCulture, includePublicKeyToken);
        }

        public static ISimpleValueConverter GetSimpleValueConverter(CultureInfo cultureInfo, ITypeNameConverter typeNameConverter)
        {
            return new SimpleValueConverter(cultureInfo, typeNameConverter);
        }
    }
}