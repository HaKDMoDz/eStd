using System;
using System.IO;

namespace System.Serialisation.Advanced.Xml
{
    /// <summary>
    ///   Writes data to xml or other node oriented format
    /// </summary>
    public interface IXmlWriter
    {
        ///<summary>
        ///  Writes start tag/node/element
        ///</summary>
        ///<param name = "elementId"></param>
        void WriteStartElement(string elementId);

        ///<summary>
        ///  Writes end tag/node/element
        ///</summary>
        void WriteEndElement();

        ///<summary>
        ///  Writes attribute of type string
        ///</summary>
        ///<param name = "attributeId"></param>
        ///<param name = "text"></param>
        void WriteAttribute(string attributeId, string text);

        ///<summary>
        ///  Writes attribute of type Type
        ///</summary>
        ///<param name = "attributeId"></param>
        ///<param name = "type"></param>
        void WriteAttribute(string attributeId, Type type);

        ///<summary>
        ///  Writes attribute of type integer
        ///</summary>
        ///<param name = "attributeId"></param>
        ///<param name = "number"></param>
        void WriteAttribute(string attributeId, int number);

        ///<summary>
        ///  Writes attribute of type array of int
        ///</summary>
        ///<param name = "attributeId"></param>
        ///<param name = "numbers"></param>
        void WriteAttribute(string attributeId, int[] numbers);

        ///<summary>
        ///  Writes attribute of a simple type (value of a SimpleProperty)
        ///</summary>
        ///<param name = "attributeId"></param>
        ///<param name = "value"></param>
        void WriteAttribute(string attributeId, object value);

        ///<summary>
        ///  Opens the stream
        ///</summary>
        ///<param name = "stream"></param>
        void Open(Stream stream);

        /// <summary>
        ///   Writes all data to the stream, the stream can be further used.
        /// </summary>
        void Close();
    }
}