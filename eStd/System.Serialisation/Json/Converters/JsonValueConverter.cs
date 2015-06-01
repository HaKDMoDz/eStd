#if NETFX_CORE
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security;
using Newtonsoft.Json.Utilities;
using Windows.Data.Json;

namespace Newtonsoft.Json.Converters
{
    /// <summary>
    /// Converts a <see cref="IJsonValue"/> to and from JSON.
    /// </summary>
    public class JsonValueConverter : JsonConverter
    {
        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            WriteJsonValue(writer, (IJsonValue)value);
        }

        private void WriteJsonValue(JsonWriter writer, IJsonValue value)
        {
            switch (value.ValueType)
            {
                case JsonValueType.Array:
                {
                    JsonArray a = value.GetArray();
                    writer.WriteStartArray();
                    for (int i = 0; i < a.Count; i++)
                    {
                        WriteJsonValue(writer, a[i]);
                    }
                    writer.WriteEndArray();
                }
                    break;
                case JsonValueType.Boolean:
                {
                    writer.WriteValue(value.GetBoolean());
                }
                    break;
                case JsonValueType.Null:
                {
                    writer.WriteNull();
                }
                    break;
                case JsonValueType.Number:
                {
                    // JsonValue doesn't support integers
                    // serialize whole numbers without a decimal point
                    double d = value.GetNumber();
                    bool isInteger = (d % 1 == 0);
                    if (isInteger && d <= long.MaxValue && d >= long.MinValue)
                        writer.WriteValue(Convert.ToInt64(d));
                    else
                        writer.WriteValue(d);
                }
                    break;
                case JsonValueType.Object:
                {
                    JsonObject o = value.GetObject();
                    writer.WriteStartObject();
                    foreach (KeyValuePair<string, IJsonValue> v in o)
                    {
                        writer.WritePropertyName(v.Key);
                        WriteJsonValue(writer, v.Value);
                    }
                    writer.WriteEndObject();
                }
                    break;
                case JsonValueType.String:
                {
                    writer.WriteValue(value.GetString());
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("ValueType");
            }
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
            if (reader.TokenType == JsonToken.None)
                reader.Read();

            IJsonValue value = CreateJsonValue(reader);

            if (!objectType.IsAssignableFrom(value.GetType()))
                throw JsonSerializationException.Create(reader, "Could not convert '{0}' to '{1}'.".FormatWith(CultureInfo.InvariantCulture, value.GetType(), objectType));

            return value;
        }

        private IJsonValue CreateJsonValue(JsonReader reader)
        {
            while (reader.TokenType == JsonToken.Comment)
            {
                if (!reader.Read())
                    throw JsonSerializationException.Create(reader, "Unexpected end.");
            }

            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                {
                    return CreateJsonObject(reader);
                }
                case JsonToken.StartArray:
                {
                    JsonArray a = new JsonArray();

                    while (reader.Read())
                    {
                        switch (reader.TokenType)
                        {
                            case JsonToken.EndArray:
                                return a;
                            default:
                                IJsonValue value = CreateJsonValue(reader);
                                a.Add(value);
                                break;
                        }
                    }
                }
                    break;
                case JsonToken.Integer:
                case JsonToken.Float:
                    return JsonValue.CreateNumberValue(Convert.ToDouble(reader.Value, CultureInfo.InvariantCulture));
                case JsonToken.String:
                    return JsonValue.CreateStringValue(reader.Value.ToString());
                case JsonToken.Boolean:
                    return JsonValue.CreateBooleanValue(Convert.ToBoolean(reader.Value, CultureInfo.InvariantCulture));
                case JsonToken.Null:
                    // surely there is a better way to create a null value than this?
                    return JsonValue.Parse("null");
                case JsonToken.Date:
                    return JsonValue.CreateStringValue(reader.Value.ToString());
                case JsonToken.Bytes:
                    return JsonValue.CreateStringValue(reader.Value.ToString());
                default:
                    throw JsonSerializationException.Create(reader, "Unexpected or unsupported token: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
            }

            throw JsonSerializationException.Create(reader, "Unexpected end.");
        }

        private JsonObject CreateJsonObject(JsonReader reader)
        {
            JsonObject o = new JsonObject();
            string propertyName = null;

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        propertyName = (string)reader.Value;
                        break;
                    case JsonToken.EndObject:
                        return o;
                    case JsonToken.Comment:
                        break;
                    default:
                        IJsonValue propertyValue = CreateJsonValue(reader);
                        o.Add(propertyName, propertyValue);
                        break;
                }
            }

            throw JsonSerializationException.Create(reader, "Unexpected end.");
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
            return typeof(IJsonValue).IsAssignableFrom(objectType);
        }
    }
}
#endif