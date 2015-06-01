using System;
using System.IO;
using System.Serialisation;
using System.Serialisation.Advanced;
using System.Serialisation.Advanced.Deserializing;
using System.Serialisation.Advanced.Serializing;
using System.Serialisation.Advanced.Xml;
using System.Serialisation.Core;
using System.Serialisation.Deserializing;
using System.Serialisation.Serializing;
using System.Xml;
using System.Runtime.CompilerServices;

namespace System.Serialisation
{
    /// <summary>
    ///   This is the main class of SharpSerializer. It serializes and deserializes objects.
    /// </summary>
    public sealed class SharpSerializer
    {
        private IPropertyDeserializer _deserializer;
        private PropertyProvider _propertyProvider;
        private string _rootName;
        private IPropertySerializer _serializer;

        /// <summary>
        ///   Standard Constructor. Default is Xml serializing
        /// </summary>
        public SharpSerializer()
        {
            initialize(new SharpSerializerXmlSettings());
        }

        /// <summary>
        ///   Overloaded constructor
        /// </summary>
        /// <param name = "binarySerialization">true - binary serialization with SizeOptimized mode, false - xml. For more options use other overloaded constructors.</param>
        public SharpSerializer(bool binarySerialization)
        {
            if (binarySerialization)
            {
                initialize(new SharpSerializerBinarySettings());
            }
            else
            {
                initialize(new SharpSerializerXmlSettings());
            }
        }

        /// <summary>
        ///   Xml serialization with custom settings
        /// </summary>
        /// <param name = "settings"></param>
        public SharpSerializer(SharpSerializerXmlSettings settings)
        {
            if (settings == null) throw new ArgumentNullException("settings");
            initialize(settings);
        }

        /// <summary>
        ///   Binary serialization with custom settings
        /// </summary>
        /// <param name = "settings"></param>
        public SharpSerializer(SharpSerializerBinarySettings settings)
        {
            if (settings == null) throw new ArgumentNullException("settings");
            initialize(settings);
        }

        /// <summary>
        ///   Custom serializer and deserializer
        /// </summary>
        /// <param name = "serializer"></param>
        /// <param name = "deserializer"></param>
        public SharpSerializer(IPropertySerializer serializer, IPropertyDeserializer deserializer)
        {
            if (serializer == null) throw new ArgumentNullException("serializer");
            if (deserializer == null) throw new ArgumentNullException("deserializer");
            _serializer = serializer;
            _deserializer = deserializer;
        }

        /// <summary>
        ///   Default it is an instance of PropertyProvider. It provides all properties to serialize.
        ///   You can use an Ihneritor and overwrite its GetAllProperties and IgnoreProperty methods to implement your custom rules.
        /// </summary>
        public PropertyProvider PropertyProvider
        {
            get
            {
                if (_propertyProvider == null) _propertyProvider = new PropertyProvider();
                return _propertyProvider;
            }
            set { _propertyProvider = value; }
        }

        /// <summary>
        ///   What name should have the root property. Default is "Root".
        /// </summary>
        public string RootName
        {
            get
            {
                if (_rootName == null) _rootName = "Root";
                return _rootName;
            }
            set { _rootName = value; }
        }

        private void initialize(SharpSerializerXmlSettings settings)
        {
            // PropertiesToIgnore
            PropertyProvider.PropertiesToIgnore = settings.AdvancedSettings.PropertiesToIgnore;
            PropertyProvider.AttributesToIgnore = settings.AdvancedSettings.AttributesToIgnore;
            //RootName
            RootName = settings.AdvancedSettings.RootName;
            // TypeNameConverter)
            ITypeNameConverter typeNameConverter = settings.AdvancedSettings.TypeNameConverter ??
                                                   DefaultInitializer.GetTypeNameConverter(
                                                       settings.IncludeAssemblyVersionInTypeName,
                                                       settings.IncludeCultureInTypeName,
                                                       settings.IncludePublicKeyTokenInTypeName);
            // SimpleValueConverter
            ISimpleValueConverter simpleValueConverter = settings.AdvancedSettings.SimpleValueConverter ??
                                                         DefaultInitializer.GetSimpleValueConverter(settings.Culture, typeNameConverter);
            // XmlWriterSettings
            XmlWriterSettings xmlWriterSettings = DefaultInitializer.GetXmlWriterSettings(settings.Encoding);
            // XmlReaderSettings
            XmlReaderSettings xmlReaderSettings = DefaultInitializer.GetXmlReaderSettings();

            // Create Serializer and Deserializer
            var reader = new DefaultXmlReader(typeNameConverter, simpleValueConverter, xmlReaderSettings);
            var writer = new DefaultXmlWriter(typeNameConverter, simpleValueConverter, xmlWriterSettings);

            _serializer = new XmlPropertySerializer(writer);
            _deserializer = new XmlPropertyDeserializer(reader);
        }

        private void initialize(SharpSerializerBinarySettings settings)
        {
            // PropertiesToIgnore
            PropertyProvider.PropertiesToIgnore = settings.AdvancedSettings.PropertiesToIgnore;
            PropertyProvider.AttributesToIgnore = settings.AdvancedSettings.AttributesToIgnore;

            //RootName
            RootName = settings.AdvancedSettings.RootName;

            // TypeNameConverter)
            ITypeNameConverter typeNameConverter = settings.AdvancedSettings.TypeNameConverter ??
                                                   DefaultInitializer.GetTypeNameConverter(
                                                       settings.IncludeAssemblyVersionInTypeName,
                                                       settings.IncludeCultureInTypeName,
                                                       settings.IncludePublicKeyTokenInTypeName);


            // Create Serializer and Deserializer
            System.Serialisation.Advanced.Binary.IBinaryReader reader = null;
            System.Serialisation.Advanced.Binary.IBinaryWriter writer = null;
            if (settings.Mode == BinarySerializationMode.Burst)
            {
                // Burst mode
                writer = new BurstBinaryWriter(typeNameConverter, settings.Encoding);
                reader = new BurstBinaryReader(typeNameConverter, settings.Encoding);
            }
            else
            {
                // Size optimized mode
                writer = new SizeOptimizedBinaryWriter(typeNameConverter, settings.Encoding);
                reader = new SizeOptimizedBinaryReader(typeNameConverter, settings.Encoding);
            }

            _deserializer = new BinaryPropertyDeserializer(reader);
            _serializer = new BinaryPropertySerializer(writer);
        }

        #region Serializing/Deserializing methods

#if !PORTABLE
        /// <summary>
        ///   Serializing to a file. File will be always new created and closed after the serialization.
        /// </summary>
        /// <param name = "data"></param>
        /// <param name = "filename"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Serialize(object data, string filename)
        {
            createDirectoryIfNeccessary(filename);
            using (Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                Serialize(data, stream);
            }
        }

        private void createDirectoryIfNeccessary(string filename)
        {
            var directory = Path.GetDirectoryName(filename);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
#endif

        /// <summary>
        ///   Serializing to the stream. After serialization the stream will NOT be closed.
        /// </summary>
        /// <param name = "data"></param>
        /// <param name = "stream"></param>
#if !PORTABLE
        [MethodImpl(MethodImplOptions.Synchronized)]
#endif
        public void Serialize(object data, Stream stream)
        {
            if (data == null) throw new ArgumentNullException("data");

            var factory = new PropertyFactory(PropertyProvider);

            Property property = factory.CreateProperty(RootName, data);

            try
            {
                _serializer.Open(stream);
                _serializer.Serialize(property);
            }
            finally
            {
                _serializer.Close();
            }
        }
#if !PORTABLE
        /// <summary>
        ///   Deserializing from the file. After deserialization the file will be closed.
        /// </summary>
        /// <param name = "filename"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public object Deserialize(string filename)
        {
            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                return Deserialize(stream);
            }
        }
#endif
        /// <summary>
        ///   Deserialization from the stream. After deserialization the stream will NOT be closed.
        /// </summary>
        /// <param name = "stream"></param>
        /// <returns></returns>
#if !PORTABLE
        [MethodImpl(MethodImplOptions.Synchronized)]
#endif
        public object Deserialize(Stream stream)
        {
            try
            {
                // Deserialize Property
                _deserializer.Open(stream);
                Property property = _deserializer.Deserialize();
                _deserializer.Close();

                // create object from Property
                var factory = new ObjectFactory();
                return factory.CreateObject(property);
            }
            catch (Exception exception)
            {
                // corrupted Stream
                throw new DeserializingException(
                    "An error occured during the deserialization. Details are in the inner exception.", exception);
            }
        }

        #endregion
    }
}