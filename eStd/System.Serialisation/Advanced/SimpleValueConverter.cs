using System;
using System.Globalization;
using System.Serialisation.Advanced;
using System.Serialisation.Advanced.Serializing;
using System.Serialisation.Advanced.Xml;
using System.Serialisation.Core;

namespace System.Serialisation.Advanced
{
    /// <summary>
    ///   Converts simple types to/from their text representation
    /// </summary>
    /// <remarks>
    ///   It is important to use the same ISimpleValueConverter during serialization and deserialization
    ///   Especially Format of the DateTime and float types can be differently converted in different cultures.
    ///   To customize it, please use the Constructor with the specified CultureInfo, 
    ///   or inherit your own converter from ISimpleValueConverter
    /// </remarks>
    public sealed class SimpleValueConverter : ISimpleValueConverter
    {
        private readonly CultureInfo _cultureInfo;
        private readonly ITypeNameConverter _typeNameConverter;
        private const char NullChar = (char) 0;
        private const string NullCharAsString = "&#x0;";

        /// <summary>
        ///   Default is CultureInfo.InvariantCulture used
        /// </summary>
        public SimpleValueConverter()
        {
            _cultureInfo = CultureInfo.InvariantCulture;
            _typeNameConverter = new TypeNameConverter();
            // Alternatively
            //_cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
        }

        /// <summary>
        ///   Here you can customize the culture. I.e. System.Threading.Thread.CurrentThread.CurrentCulture
        /// </summary>
        /// <param name = "cultureInfo"></param>
        /// <param name="typeNameConverter"></param>
        public SimpleValueConverter(CultureInfo cultureInfo, ITypeNameConverter typeNameConverter)
        {
            _cultureInfo = cultureInfo;
            _typeNameConverter = typeNameConverter;
        }

        #region ISimpleValueConverter Members

        /// <summary>
        /// </summary>
        /// <param name = "value"></param>
        /// <returns>string.Empty if the value is null</returns>
        public string ConvertToString(object value)
        {
            if (value == null) return string.Empty;

            // Array of byte
            if (value.GetType() == typeof(byte[]))
            {
                return Convert.ToBase64String((byte[])value);
            }

            // Type
            if (isType(value))
                return _typeNameConverter.ConvertToTypeName((Type)value);

            // Char which is \0
            if (value.Equals(NullChar))
                return NullCharAsString;

            return Convert.ToString(value, _cultureInfo);
        }


        /// <summary>
        /// </summary>
        /// <param name = "text"></param>
        /// <param name = "type">expected type. Result should be of this type.</param>
        /// <returns>null if the text is null</returns>
        public object ConvertFromString(string text, Type type)
        {
            try
            {
                if (type == typeof (string)) return text;
                if (type == typeof (Boolean)) return Convert.ToBoolean(text, _cultureInfo);
                if (type == typeof (Byte)) return Convert.ToByte(text, _cultureInfo);
                if (type == typeof (Char))
                {
                    if (text == NullCharAsString)
                        // this is a null termination
                        return NullChar;
                    //other chars
                    return Convert.ToChar(text, _cultureInfo);
                }
                    
                if (type == typeof (DateTime)) return Convert.ToDateTime(text, _cultureInfo);
                if (type == typeof (Decimal)) return Convert.ToDecimal(text, _cultureInfo);
                if (type == typeof (Double)) return Convert.ToDouble(text, _cultureInfo);
                if (type == typeof (Int16)) return Convert.ToInt16(text, _cultureInfo);
                if (type == typeof (Int32)) return Convert.ToInt32(text, _cultureInfo);
                if (type == typeof (Int64)) return Convert.ToInt64(text, _cultureInfo);
                if (type == typeof (SByte)) return Convert.ToSByte(text, _cultureInfo);
                if (type == typeof (Single)) return Convert.ToSingle(text, _cultureInfo);
                if (type == typeof (UInt16)) return Convert.ToUInt16(text, _cultureInfo);
                if (type == typeof (UInt32)) return Convert.ToUInt32(text, _cultureInfo);
                if (type == typeof (UInt64)) return Convert.ToUInt64(text, _cultureInfo);

                if (type == typeof (TimeSpan)) return TimeSpan.Parse(text);

                if (type == typeof(Guid)) return new Guid(text);
                // Enumeration
                if (type.IsEnum) return Enum.Parse(type, text, true);
                // Array of byte
                if (type == typeof(byte[]))
                {
                    return Convert.FromBase64String(text);
                }
                // Type-check must be last
                if (isType(type))
                {
                    return _typeNameConverter.ConvertToType(text);
                } 

                throw new InvalidOperationException(string.Format("Unknown simple type: {0}", type.FullName));
            }
            catch (Exception ex)
            {
                throw new SimpleValueParsingException(
                    string.Format("Invalid value: {0}. See details in the inner exception.", text), ex);
            }
        }

        #endregion

        private static bool isType(object value)
        {
            return (value as Type) != null;
        }
    }
}