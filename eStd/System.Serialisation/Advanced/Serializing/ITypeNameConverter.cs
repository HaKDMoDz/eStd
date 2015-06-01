using System;

namespace System.Serialisation.Advanced.Serializing
{
    /// <summary>
    ///   Converts Type to its string representation and vice versa. The default instance used in the Framework is TypeNameConverter
    /// </summary>
    public interface ITypeNameConverter
    {
        /// <summary>
        ///   Gives back Type as text.
        /// </summary>
        /// <param name = "type"></param>
        /// <returns>string.Empty if the type is null</returns>
        string ConvertToTypeName(Type type);

        /// <summary>
        ///   Gives back Type from the text.
        /// </summary>
        /// <param name = "typeName"></param>
        /// <returns></returns>
        Type ConvertToType(string typeName);
    }
}