using System;
using System.Collections;

namespace System.Serialisation.Core
{
    /// <summary>
    ///   Some help functions for the serializing framework. As these functions are complexer
    ///   they can be converted to single classes.
    /// </summary>
    internal static class Tools
    {
        /// <summary>
        ///   Is the simple type (string, DateTime, TimeSpan, Decimal, Enumeration or other primitive type)
        /// </summary>
        /// <param name = "type"></param>
        /// <returns></returns>
        public static bool IsSimple(Type type)
        {
            if (type == typeof (string))
            {
                return true;
            }
            if (type == typeof (DateTime))
            {
                return true;
            }
            if (type == typeof (TimeSpan))
            {
                return true;
            }
            if (type == typeof (Decimal))
            {
                // new since the version 2
                return true;
            }
            if (type == typeof(Guid))
            {
                // new since the version 2.8
                return true;
            }
            if (type == typeof(Type) || type.IsSubclassOf(typeof(Type)))
            {
                // new since v.2.11
                return true;
            }
            if (type.IsEnum)
            {
                return true;
            }
            if (type==typeof(byte[]))
            {
                // since v.2.16 is byte[] a simple type
                return true;
            }

            return type.IsPrimitive;
        }

        /// <summary>
        ///   Is type an IEnumerable
        /// </summary>
        /// <param name = "type"></param>
        /// <returns></returns>
        public static bool IsEnumerable(Type type)
        {
            Type referenceType = typeof (IEnumerable);
            return referenceType.IsAssignableFrom(type);
        }

        /// <summary>
        ///   Is type ICollection
        /// </summary>
        /// <param name = "type"></param>
        /// <returns></returns>
        public static bool IsCollection(Type type)
        {
            Type referenceType = typeof (ICollection);
            return referenceType.IsAssignableFrom(type);
        }

        /// <summary>
        ///   Is type IDictionary
        /// </summary>
        /// <param name = "type"></param>
        /// <returns></returns>
        public static bool IsDictionary(Type type)
        {
            Type referenceType = typeof (IDictionary);
            return referenceType.IsAssignableFrom(type);
        }

        /// <summary>
        ///   Is it array? It does not matter if singledimensional or multidimensional
        /// </summary>
        /// <param name = "type"></param>
        /// <returns></returns>
        public static bool IsArray(Type type)
        {
            return type.IsArray;
        }

        /// <summary>
        ///   Creates instance from type. There must be a standard constructor (without parameters) in the type.
        /// </summary>
        /// <param name = "type"></param>
        /// <returns></returns>
        public static object CreateInstance(Type type)
        {
            if (type == null) return null;

            try
            {
                object result = Activator.CreateInstance(type);
                return result;
            }
            catch (Exception ex)
            {
                throw new CreatingInstanceException(
                    string.Format(
                        "Error during creating an object. Please check if the type \"{0}\" has public parameterless constructor, or if the settings IncludeAssemblyVersionInTypeName, IncludeCultureInTypeName, IncludePublicKeyTokenInTypeName are set to true. Details are in the inner exception.",
                        type.AssemblyQualifiedName), ex);
            }
        }
    }
}