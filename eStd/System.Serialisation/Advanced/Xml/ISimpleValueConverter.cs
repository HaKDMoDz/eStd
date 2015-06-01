using System;

namespace System.Serialisation.Advanced.Xml
{
    /// <summary>
    ///   Converts values of SimpleProperty to/from string
    /// </summary>
    public interface ISimpleValueConverter
    {
        /// <summary>
        /// </summary>
        /// <param name = "value"></param>
        /// <returns>string.Empty if the value is null</returns>
        string ConvertToString(object value);

        /// <summary>
        /// </summary>
        /// <param name = "text"></param>
        /// <param name = "type">expected type. Result should be of this type.</param>
        /// <returns>null if the text is null</returns>
        object ConvertFromString(string text, Type type);
    }
}