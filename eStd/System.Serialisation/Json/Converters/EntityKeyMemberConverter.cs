#if !(NET20 || NETFX_CORE || PORTABLE40 || PORTABLE)
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Globalization;
using Newtonsoft.Json.Utilities;

namespace System.Serialisation.Json.Converters
{
    /// <summary>
    /// Converts an Entity Framework EntityKey to and from JSON.
    /// </summary>
    public class EntityKeyMemberConverter : JsonConverter
    {
        private const string EntityKeyMemberFullTypeName = "System.Data.EntityKeyMember";

        private const string KeyPropertyName = "Key";
        private const string TypePropertyName = "Type";
        private const string ValuePropertyName = "Value";

        private static ReflectionObject _reflectionObject;

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            EnsureReflectionObject(value.GetType());

            DefaultContractResolver resolver = serializer.ContractResolver as DefaultContractResolver;

            string keyName = (string)_reflectionObject.GetValue(value, KeyPropertyName);
            object keyValue = _reflectionObject.GetValue(value, ValuePropertyName);

            Type keyValueType = (keyValue != null) ? keyValue.GetType() : null;

            writer.WriteStartObject();
            writer.WritePropertyName((resolver != null) ? resolver.GetResolvedPropertyName(KeyPropertyName) : KeyPropertyName);
            writer.WriteValue(keyName);
            writer.WritePropertyName((resolver != null) ? resolver.GetResolvedPropertyName(TypePropertyName) : TypePropertyName);
            writer.WriteValue((keyValueType != null) ? keyValueType.FullName : null);

            writer.WritePropertyName((resolver != null) ? resolver.GetResolvedPropertyName(ValuePropertyName) : ValuePropertyName);

            if (keyValueType != null)
            {
                string valueJson;
                if (JsonSerializerInternalWriter.TryConvertToString(keyValue, keyValueType, out valueJson))
                    writer.WriteValue(valueJson);
                else
                    writer.WriteValue(keyValue);
            }
            else
            {
                writer.WriteNull();
            }

            writer.WriteEndObject();
        }

        private static void ReadAndAssertProperty(JsonReader reader, string propertyName)
        {
            ReadAndAssert(reader);

            if (reader.TokenType != JsonToken.PropertyName || !string.Equals(reader.Value.ToString(), propertyName, StringComparison.OrdinalIgnoreCase))
                throw new JsonSerializationException("Expected JSON property '{0}'.".FormatWith(CultureInfo.InvariantCulture, propertyName));
        }

        private static void ReadAndAssert(JsonReader reader)
        {
            if (!reader.Read())
                throw new JsonSerializationException("Unexpected end.");
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            EnsureReflectionObject(objectType);

            object entityKeyMember = _reflectionObject.Creator();

            ReadAndAssertProperty(reader, KeyPropertyName);
            ReadAndAssert(reader);
            _reflectionObject.SetValue(entityKeyMember, KeyPropertyName, reader.Value.ToString());

            ReadAndAssertProperty(reader, TypePropertyName);
            ReadAndAssert(reader);
            string type = reader.Value.ToString();

            Type t = Type.GetType(type);

            ReadAndAssertProperty(reader, ValuePropertyName);
            ReadAndAssert(reader);
            _reflectionObject.SetValue(entityKeyMember, ValuePropertyName, serializer.Deserialize(reader, t));

            ReadAndAssert(reader);

            return entityKeyMember;
        }

        private static void EnsureReflectionObject(Type objectType)
        {
            if (_reflectionObject == null)
                _reflectionObject = ReflectionObject.Create(objectType, KeyPropertyName, ValuePropertyName);
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType.AssignableToTypeName(EntityKeyMemberFullTypeName);
        }
    }
}
#endif