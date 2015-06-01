using System;

namespace System.Serialisation.Core.Xml
{
    /// <summary>
    ///   These elements are used as tags during the xml serialization.
    /// </summary>
    public static class Elements
    {
        ///<summary>
        ///</summary>
        public const string Collection = "Collection";

        ///<summary>
        ///</summary>
        public const string ComplexObject = "Complex";

        ///<summary>
        /// internal used as an id for referencing already serialized items
        /// Since v.2.12 Elements.Reference is used instead.
        ///</summary>
        public const string OldReference = "ComplexReference";
        ///<summary>
        /// used as an id for referencing already serialized items
        ///</summary>
        public const string Reference = "Reference";

        ///<summary>
        ///</summary>
        public const string Dictionary = "Dictionary";

        ///<summary>
        ///</summary>
        public const string MultiArray = "MultiArray";

        ///<summary>
        ///</summary>
        public const string Null = "Null";

        ///<summary>
        ///</summary>
        public const string SimpleObject = "Simple";

        ///<summary>
        ///</summary>
        public const string SingleArray = "SingleArray";
    }

    /// <summary>
    ///   These elements are used as tags during the xml serialization.
    /// </summary>
    public static class SubElements
    {
        ///<summary>
        ///</summary>
        public const string Dimension = "Dimension";

        ///<summary>
        ///</summary>
        public const string Dimensions = "Dimensions";

        ///<summary>
        ///</summary>
        public const string Item = "Item";

        ///<summary>
        ///</summary>
        public const string Items = "Items";

        ///<summary>
        ///</summary>
        public const string Properties = "Properties";
    }

    /// <summary>
    ///   These attributes are used during the xml serialization.
    /// </summary>
    public static class Attributes
    {
        ///<summary>
        ///</summary>
        public const string DimensionCount = "dimensionCount";

        ///<summary>
        ///</summary>
        public const string ElementType = "elementType";

        ///<summary>
        ///</summary>
        public const string Indexes = "indexes";

        ///<summary>
        ///</summary>
        public const string KeyType = "keyType";

        ///<summary>
        ///</summary>
        public const string Length = "length";

        ///<summary>
        ///</summary>
        public const string LowerBound = "lowerBound";

        ///<summary>
        ///</summary>
        public const string Name = "name";

        ///<summary>
        ///</summary>
        public const string Type = "type";

        ///<summary>
        ///</summary>
        public const string Value = "value";

        ///<summary>
        ///</summary>
        public const string ValueType = "valueType";

        ///<summary>
        /// used as an id to identify and refere already serialized items
        ///</summary>
        public const string ReferenceId = "id";
    }
}