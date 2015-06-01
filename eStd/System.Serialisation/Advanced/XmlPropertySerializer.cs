using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Serialisation.Advanced.Serializing;
using System.Serialisation.Advanced.Xml;
using System.Serialisation.Core;
using System.Serialisation.Core.Xml;

namespace System.Serialisation.Advanced
{
    /// <summary>
    ///   Serializes properties to xml or any other target which supports node/attribute notation
    /// </summary>
    /// <remarks>
    ///   Use an instance of your own IXmlWriter in the constructor to target other storage standards
    /// </remarks>
    public sealed class XmlPropertySerializer : PropertySerializer
    {
        private readonly IXmlWriter _writer;

        ///<summary>
        ///</summary>
        ///<param name = "writer"></param>
        public XmlPropertySerializer(IXmlWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            _writer = writer;
        }

        /// <summary>
        /// </summary>
        /// <param name = "property"></param>
        protected override void SerializeNullProperty(System.Serialisation.Serializing.PropertyTypeInfo<NullProperty> property)
        {
            // nulls must be serialized also 
            writeStartProperty(Elements.Null, property.Name, property.ValueType);
            writeEndProperty();
        }

        /// <summary>
        /// </summary>
        /// <param name = "property"></param>
        protected override void SerializeSimpleProperty(System.Serialisation.Serializing.PropertyTypeInfo<SimpleProperty> property)
        {
            if (property.Property.Value == null)
                return;

            writeStartProperty(Elements.SimpleObject, property.Name, property.ValueType);

            _writer.WriteAttribute(Attributes.Value, property.Property.Value);

            writeEndProperty();
        }

        private void writeEndProperty()
        {
            _writer.WriteEndElement();
        }

        private void writeStartProperty(string elementId, string propertyName, Type propertyType)
        {
            _writer.WriteStartElement(elementId);

            // Name
            if (!string.IsNullOrEmpty(propertyName))
            {
                _writer.WriteAttribute(Attributes.Name, propertyName);
            }

            // Type
            if (propertyType != null)
            {
                _writer.WriteAttribute(Attributes.Type, propertyType);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name = "property"></param>
        protected override void SerializeMultiDimensionalArrayProperty(
            System.Serialisation.Serializing.PropertyTypeInfo<MultiDimensionalArrayProperty> property)
        {
            writeStartProperty(Elements.MultiArray, property.Name, property.ValueType);

            // additional attribute with referenceId
            if (property.Property.Reference.Count > 1)
            {
                _writer.WriteAttribute(Attributes.ReferenceId, property.Property.Reference.Id);
            }

            // DimensionInfos
            writeDimensionInfos(property.Property.DimensionInfos);

            // Einträge
            writeMultiDimensionalArrayItems(property.Property.Items, property.Property.ElementType);

            writeEndProperty();
        }

        private void writeMultiDimensionalArrayItems(IEnumerable<MultiDimensionalArrayItem> items, Type defaultItemType)
        {
            _writer.WriteStartElement(SubElements.Items);
            foreach (MultiDimensionalArrayItem item in items)
            {
                writeMultiDimensionalArrayItem(item, defaultItemType);
            }
            _writer.WriteEndElement();
        }

        private void writeMultiDimensionalArrayItem(MultiDimensionalArrayItem item, Type defaultTypeOfItemValue)
        {
            _writer.WriteStartElement(SubElements.Item);

            // Write Indexes
            _writer.WriteAttribute(Attributes.Indexes, item.Indexes);

            // Write Data
            SerializeCore(new System.Serialisation.Serializing.PropertyTypeInfo<Property>(item.Value, defaultTypeOfItemValue));

            _writer.WriteEndElement();
        }

        private void writeDimensionInfos(IEnumerable<DimensionInfo> infos)
        {
            _writer.WriteStartElement(SubElements.Dimensions);
            foreach (DimensionInfo info in infos)
            {
                writeDimensionInfo(info);
            }
            _writer.WriteEndElement();
        }

        /// <summary>
        /// </summary>
        /// <param name = "property"></param>
        protected override void SerializeSingleDimensionalArrayProperty(
            System.Serialisation.Serializing.PropertyTypeInfo<SingleDimensionalArrayProperty> property)
        {
            writeStartProperty(Elements.SingleArray, property.Name, property.ValueType);

            // additional attribute with referenceId
            if (property.Property.Reference.Count > 1)
            {
                _writer.WriteAttribute(Attributes.ReferenceId, property.Property.Reference.Id);
            }

            // LowerBound
            if (property.Property.LowerBound != 0)
            {
                _writer.WriteAttribute(Attributes.LowerBound, property.Property.LowerBound);
            }

            // items
            writeItems(property.Property.Items, property.Property.ElementType);

            writeEndProperty();
        }

        private void writeDimensionInfo(DimensionInfo info)
        {
            _writer.WriteStartElement(SubElements.Dimension);
            if (info.Length != 0)
            {
                _writer.WriteAttribute(Attributes.Length, info.Length);
            }
            if (info.LowerBound != 0)
            {
                _writer.WriteAttribute(Attributes.LowerBound, info.LowerBound);
            }

            _writer.WriteEndElement();
        }

        /// <summary>
        /// </summary>
        /// <param name = "property"></param>
        protected override void SerializeDictionaryProperty(System.Serialisation.Serializing.PropertyTypeInfo<DictionaryProperty> property)
        {
            writeStartProperty(Elements.Dictionary, property.Name, property.ValueType);

            // additional attribute with referenceId
            if (property.Property.Reference.Count > 1)
            {
                _writer.WriteAttribute(Attributes.ReferenceId, property.Property.Reference.Id);
            }

            // Properties
            writeProperties(property.Property.Properties, property.Property.Type);

            // Items
            writeDictionaryItems(property.Property.Items, property.Property.KeyType, property.Property.ValueType);

            writeEndProperty();
        }

        private void writeDictionaryItems(IEnumerable<KeyValueItem> items, Type defaultKeyType, Type defaultValueType)
        {
            _writer.WriteStartElement(SubElements.Items);
            foreach (KeyValueItem item in items)
            {
                writeDictionaryItem(item, defaultKeyType, defaultValueType);
            }
            _writer.WriteEndElement();
        }

        private void writeDictionaryItem(KeyValueItem item, Type defaultKeyType, Type defaultValueType)
        {
            _writer.WriteStartElement(SubElements.Item);
            SerializeCore(new System.Serialisation.Serializing.PropertyTypeInfo<Property>(item.Key, defaultKeyType));
            SerializeCore(new System.Serialisation.Serializing.PropertyTypeInfo<Property>(item.Value, defaultValueType));
            _writer.WriteEndElement();
        }

        private void writeValueType(Type type)
        {
            if (type != null)
            {
                _writer.WriteAttribute(Attributes.ValueType, type);
            }
        }

        private void writeKeyType(Type type)
        {
            if (type != null)
            {
                _writer.WriteAttribute(Attributes.KeyType, type);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name = "property"></param>
        protected override void SerializeCollectionProperty(System.Serialisation.Serializing.PropertyTypeInfo<CollectionProperty> property)
        {
            writeStartProperty(Elements.Collection, property.Name, property.ValueType);

            // additional attribute with referenceId
            if (property.Property.Reference.Count > 1)
            {
                _writer.WriteAttribute(Attributes.ReferenceId, property.Property.Reference.Id);
            }

            // Properties
            writeProperties(property.Property.Properties, property.Property.Type);

            //Items
            writeItems(property.Property.Items, property.Property.ElementType);

            writeEndProperty();
        }

        private void writeItems(IEnumerable<Property> properties, Type defaultItemType)
        {
            _writer.WriteStartElement(SubElements.Items);
            foreach (Property item in properties)
            {
                SerializeCore(new System.Serialisation.Serializing.PropertyTypeInfo<Property>(item, defaultItemType));
            }
            _writer.WriteEndElement();
        }

        /// <summary>
        ///   Properties are only saved if at least one property exists
        /// </summary>
        /// <param name = "properties"></param>
        /// <param name = "ownerType">to which type belong the properties</param>
        private void writeProperties(ICollection<Property> properties, Type ownerType)
        {
            // check if there are properties
            if (properties.Count == 0)
                return;

            _writer.WriteStartElement(SubElements.Properties);
            foreach (Property property in properties)
            {
                PropertyInfo propertyInfo = ownerType.GetProperty(property.Name);
                if (propertyInfo != null)
                {
                    SerializeCore(new System.Serialisation.Serializing.PropertyTypeInfo<Property>(property, propertyInfo.PropertyType));
                }
                else
                {
                    SerializeCore(new System.Serialisation.Serializing.PropertyTypeInfo<Property>(property, null));
                }
            }
            _writer.WriteEndElement();
        }

        /// <summary>
        /// </summary>
        /// <param name = "property"></param>
        protected override void SerializeComplexProperty(System.Serialisation.Serializing.PropertyTypeInfo<ComplexProperty> property)
        {
            writeStartProperty(Elements.ComplexObject, property.Name, property.ValueType);

            // additional attribute with referenceId
            if (property.Property.Reference.Count>1)
            {
                _writer.WriteAttribute(Attributes.ReferenceId, property.Property.Reference.Id);
            }

            // Properties
            writeProperties(property.Property.Properties, property.Property.Type);

            writeEndProperty();
        }

        /// <summary>
        /// Stores only reference to an object, not the object itself
        /// </summary>
        /// <param name="referenceTarget"></param>
        protected override void SerializeReference(ReferenceTargetProperty referenceTarget)
        {
            writeStartProperty(Elements.Reference, referenceTarget.Name, null);
            _writer.WriteAttribute(Attributes.ReferenceId, referenceTarget.Reference.Id);
            writeEndProperty();
        }

        /// <summary>
        ///   Open the writer
        /// </summary>
        /// <param name = "stream"></param>
        public override void Open(Stream stream)
        {
            _writer.Open(stream);
        }

        /// <summary>
        ///   Close the Write, but do not close the stream
        /// </summary>
        public override void Close()
        {
            _writer.Close();
        }
    }
}