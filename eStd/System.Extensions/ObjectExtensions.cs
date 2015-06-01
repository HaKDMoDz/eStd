using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Serialization;

namespace System.Extensions
{
    /// <summary>
    /// Contains generic object-related extensions.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Serializes an object to a byte array.
        /// </summary>
        /// <param name="value">Object to serialize.</param>
        /// <returns>Serialized object.</returns>
        public static byte[] BinarySerialize(this object value)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf1 = new BinaryFormatter();
            bf1.Serialize(ms, value);
            return ms.ToArray();
        }

        /// <summary>
        /// Converts an object to the specified target type or returns the default value.
        /// </summary>
        /// <typeparam name="T">Type of original object.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The target type</returns>
        public static T ConvertTo<T>(this object value, T defaultValue)
        {
            if (value != null)
            {
                var targetType = typeof(T);

                var converter = TypeDescriptor.GetConverter(value);

                if (converter != null)
                {
                    if (converter.CanConvertTo(targetType))
                    {
                        return (T)converter.ConvertTo(value, targetType);
                    }
                }

                converter = TypeDescriptor.GetConverter(targetType);

                if (converter != null)
                {
                    if (converter.CanConvertFrom(value.GetType()))
                    {
                        return (T)converter.ConvertFrom(value);
                    }
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// Copies the readable and writable public property values from the source object to the target.
        /// </summary>
        /// <remarks>The source and target objects must be of the same type.</remarks>
        /// <param name="target">The target object.</param>
        /// <param name="source">The source object.</param>
        public static void CopyPropertiesFrom(this object target, object source)
        {
            CopyPropertiesFrom(target, source, string.Empty);
        }

        /// <summary>
        /// Copies the readable and writable public property values from the source object to the target and
        /// optionally allows for the ignoring of a single property.
        /// </summary>
        /// <remarks>The source and target objects must be of the same type.</remarks>
        /// <param name="target">The target object</param>
        /// <param name="source">The source object</param>
        /// <param name="ignoreProperty">A single property name to ignore</param>
        public static void CopyPropertiesFrom(this object target, object source, string ignoreProperty)
        {
            CopyPropertiesFrom(target, source, new string[] { ignoreProperty });
        }

        /// <summary>
        /// Copies the readable and writable public property values from the source object to the target and
        /// optionally allows for the ignoring of any number of properties.
        /// </summary>
        /// <remarks>The source and target objects must be of the same type.</remarks>
        /// <param name="target">The target object</param>
        /// <param name="source">The source object</param>
        /// <param name="ignoreProperties">An array of property names to ignore</param>
        public static void CopyPropertiesFrom(this object target, object source, string[] ignoreProperties)
        {
            // Get and check the object types
            Type type = source.GetType();
            if (target.GetType() != type)
            {
                throw new ArgumentException("The source type must be the same as the target");
            }

            // Build a clean list of property names to ignore
            List<string> ignoreList = new List<string>();
            foreach (string item in ignoreProperties)
            {
                if (!string.IsNullOrEmpty(item) && !ignoreList.Contains(item))
                {
                    ignoreList.Add(item);
                }
            }

            // Copy the properties
            foreach (PropertyInfo property in type.GetProperties())
            {
                if (property.CanWrite
                    && property.CanRead
                    && !ignoreList.Contains(property.Name))
                {
                    object val = property.GetValue(source, null);
                    property.SetValue(target, val, null);
                }
            }
        }

        /// <summary>
        /// Returns a Boolean value indicating whether a variable is of the indicated type.
        /// </summary>
        /// <param name="obj">Object instance.</param>
        /// <param name="type">The Type to check the object against.</param>
        /// <returns>Result of the comparison.</returns>
        public static object IsType(this object obj, Type type)
        {
            return obj.GetType().Equals(type);
        }

        /// <summary>
        /// Serializes an object to a JSON object.
        /// </summary>
        /// <param name="obj">Object to serialize.</param>
        /// <returns>JSON string object.</returns>
        /// <example>
        ///     <code language="c#">
        ///         Employee emp = new Employee("Bob Smith");
        ///         string s = emp.ToJson();
        ///         emp = null;
        ///         emp = s.FromJson&lt;Employeen&gt;();
        ///     </code>
        /// </example>
        public static string ToJson(this object obj)
        {
            var serializer = new DataContractJsonSerializer(obj.GetType());

            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, obj);

                return Encoding.Default.GetString(stream.ToArray());
            }
        }

        /// <summary>
        /// Serializes an object to a JSON object.  For complex objects that require recursion,
        /// known types my be supplied.
        /// </summary>
        /// <param name="obj">Object to serialize.</param>
        /// <param name="knownTypes">An IEnumerable of known types.  Useful for complex objects.</param>
        /// <returns>JSON string object.</returns>
        /// <example>
        ///     <code language="c#">
        ///         Employee emp = new Employee("Bob Smith");
        ///         string s = emp.ToJson();
        ///         emp = null;
        ///         emp = s.FromJsonn&lt;Employeen&gt;();
        ///     </code>
        /// </example>
        public static string ToJson(this object obj, IEnumerable<Type> knownTypes)
        {
            var serializer = new DataContractJsonSerializer(obj.GetType(), knownTypes);

            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, obj);

                return Encoding.Default.GetString(stream.ToArray());
            }
        }
        
        /// <summary>
        /// Serializes a JSON object to an object.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <param name="obj">JSON string object.</param>
        /// <returns>The resulting object.</returns>
        /// <example>
        ///     <code language="c#">
        ///         Employee emp = new Employee("Bob Smith");
        ///         string s = emp.ToJson();
        ///         emp = null;
        ///         emp = s.FromJsonn&lt;Employeen&gt;();
        ///     </code>
        /// </example>
        public static T FromJson<T>(this object obj)
        {
            var returnObject = Activator.CreateInstance<T>();

            var serializer = new DataContractJsonSerializer(returnObject.GetType());
            
            using (var stream = new MemoryStream(Encoding.Default.GetBytes(obj.ToString())))
            {
                return (T)serializer.ReadObject(stream);
            }
        }

        /// <summary>
        /// Serializes a JSON object to an object.  For complex objects that require recursion,
        /// known types my be supplied.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <param name="obj">JSON string object.</param>
        /// <param name="knownTypes">An IEnumerable of known types.  Useful for complex objects.</param>
        /// <returns>The resulting object.</returns>
        /// <example>
        ///     <code language="c#">
        ///         Employee emp = new Employee("Bob Smith");
        ///         string s = emp.ToJson();
        ///         emp = null;
        ///         emp = s.FromJsonn&lt;Employeen&gt;();
        ///     </code>
        /// </example>
        public static T FromJson<T>(this object obj, IEnumerable<Type> knownTypes)
        {
            var returnObject = Activator.CreateInstance<T>();

            var serializer = new DataContractJsonSerializer(returnObject.GetType(), knownTypes);

            using (var stream = new MemoryStream(Encoding.Default.GetBytes(obj.ToString())))
            {
                return (T)serializer.ReadObject(stream);
            }
        }

        /// <summary>
        /// Returns a string representation of the objects property values.
        /// </summary>
        /// <param name="source">The object for the string representation.</param>
        /// <returns>A string of properties.</returns>
        public static string ToPropertiesString(this object source)
        {
            return ToPropertiesString(source, System.Environment.NewLine);
        }

        /// <summary>
        /// Returns a string representation of the objects property values, with a delimiter between values.
        /// </summary>
        /// <param name="source">The object for the string representation.</param>
        /// <param name="delimiter">The line terminstor string to use between properties.</param>
        /// <returns>A string of properties.</returns>
        public static string ToPropertiesString(this object source, string delimiter)
        {
            if (source == null)
            {
                return string.Empty;
            }

            Type type = source.GetType();

            StringBuilder sb = new StringBuilder(type.Name);
            sb.Append(delimiter);

            foreach (PropertyInfo property in type.GetProperties())
            {
                if (property.CanWrite
                    && property.CanRead)
                {
                    object val = property.GetValue(source, null);
                    sb.Append(property.Name);
                    sb.Append(": ");
                    sb.Append(val == null ? "[NULL]" : val.ToString());
                    sb.Append(delimiter);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Serializes the object into an XML string
        /// </summary>
        /// <remarks>
        /// The object to be serialized should be decorated with the 
        /// <see cref="SerializableAttribute"/>, or implement the <see cref="ISerializable"/> interface.
        /// </remarks>
        /// <param name="source">The object to serialize</param>
        /// <param name="encoding">The Encoding scheme to use when serializing the data to XML</param>
        /// <returns>An XML encoded string representation of the source object</returns>
        public static string ToXml(this object source, Encoding encoding)
        {
            if (source == null)
            {
                throw new ArgumentException("The source object cannot be null.");
            }

            if (encoding == null)
            {
                throw new Exception("You must specify an encoder to use for serialization.");
            }

            using (MemoryStream stream = new MemoryStream())
            {
                XmlSerializer serializer = new XmlSerializer(source.GetType());
                serializer.Serialize(stream, source);
                stream.Position = 0;
                return encoding.GetString(stream.ToArray());
            }
        }
    }
}
