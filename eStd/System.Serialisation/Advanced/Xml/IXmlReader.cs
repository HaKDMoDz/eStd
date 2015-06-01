using System;
using System.Collections.Generic;
using System.IO;

namespace System.Serialisation.Advanced.Xml
{
    /// <summary>
    ///   Reads data from Xml or other node oriented format
    /// </summary>
    public interface IXmlReader
    {
        /// <summary>
        ///   Reads next valid element
        /// </summary>
        /// <returns>null if nothing was found</returns>
        string ReadElement();

        /// <summary>
        ///   Reads all sub elements of the current element
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> ReadSubElements();

        /// <summary>
        ///   Reads attribute as string
        /// </summary>
        /// <param name = "attributeName"></param>
        /// <returns>null if nothing was found</returns>
        string GetAttributeAsString(string attributeName);

        /// <summary>
        ///   Reads attribute and converts it to type
        /// </summary>
        /// <param name = "attributeName"></param>
        /// <returns>null if nothing found</returns>
        Type GetAttributeAsType(string attributeName);

        /// <summary>
        ///   Reads attribute and converts it to integer
        /// </summary>
        /// <param name = "attributeName"></param>
        /// <returns>0 if nothing found</returns>
        int GetAttributeAsInt(string attributeName);

        /// <summary>
        ///   Reads attribute and converts it as array of int
        /// </summary>
        /// <param name = "attributeName"></param>
        /// <returns>empty array if nothing found</returns>
        int[] GetAttributeAsArrayOfInt(string attributeName);

        /// <summary>
        ///   Reads attribute and converts it to object of the expectedType
        /// </summary>
        /// <param name = "attributeName"></param>
        /// <param name = "expectedType"></param>
        /// <returns></returns>
        object GetAttributeAsObject(string attributeName, Type expectedType);

        /// <summary>
        ///   Open the stream
        /// </summary>
        /// <param name = "stream"></param>
        void Open(Stream stream);

        /// <summary>
        ///   Stream can be further used
        /// </summary>
        void Close();
    }
}