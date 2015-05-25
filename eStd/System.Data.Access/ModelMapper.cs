using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;
using Furesoft.Creek.Office.Access;

namespace Furesoft.Creek.Office.Access
{
    public static class TypeExtension
    {

        public static Boolean IsAnonymousType(this Type type)
        {
            Boolean hasCompilerGeneratedAttribute = type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Count() > 0;
            Boolean nameContainsAnonymousType = type.FullName.Contains("AnonymousType");
            Boolean isAnonymousType = hasCompilerGeneratedAttribute && nameContainsAnonymousType;

            return isAnonymousType;
        }
    }

    class ModelMapper
    {

        public static void Map(Type type, IDataReader datareader, object obj)
        {
            PropertyInfo[] props = type.GetProperties();
            bool foundAttribute = false;
            foreach (PropertyInfo prop in props)
            {
                foundAttribute = false;
                object[] attributes = prop.GetCustomAttributes(false);
                foreach (object attribute in attributes)
                {
                    if (attribute.GetType() == typeof(DBFieldMapping))
                    {
                        DBFieldMapping mapping = (DBFieldMapping)attribute;
                        prop.SetValue(obj, getValue(datareader, mapping.field, prop.PropertyType), null);
                        foundAttribute = true;
                    }
                }
                if (!foundAttribute)
                    prop.SetValue(obj, getValue(datareader, prop.Name, prop.PropertyType), null);
            }
        }

        class ByteBuilder
        {
            byte[] buffer;
            long maxLength = 65536;
            int current = 0;
            public ByteBuilder()
            {
                buffer = new byte[maxLength];
            }

            public void Append(byte[] source)
            {
                if (current + source.Length > maxLength)
                {
                    maxLength *= 2;
                    byte[] newbuffer = new byte[maxLength];
                    Buffer.BlockCopy(buffer, 0, newbuffer, 0, current);
                    buffer = newbuffer;
                }

                Buffer.BlockCopy(source, 0, buffer, current, source.Length);
                current += source.Length;
            }

            public byte[] GetBytes()
            {
                byte[] newbuffer = new byte[current];
                Buffer.BlockCopy(buffer, 0, newbuffer, 0, current);
                return newbuffer;
            }
        }

        private static object getRealValue(IDataReader datareader, int ordinal, Type t)
        {

            if (t == typeof(Int16)) return !datareader.IsDBNull(ordinal) ? (object)datareader.GetInt16(ordinal) : null;
            
            if (t == typeof(Int32)) return !datareader.IsDBNull(ordinal) ? (object)datareader.GetInt32(ordinal) : null;
            
            if (t == typeof(Int64)) return !datareader.IsDBNull(ordinal) ? (object)datareader.GetInt32(ordinal) : null;
            
            if (t == typeof(Byte[]))
            {
                if (datareader.IsDBNull(ordinal)) return null;
                int bufferSize = 1024;
                int startIndex = 0;
                byte[] outbyte = new byte[bufferSize];
                ByteBuilder bb = new ByteBuilder();
                long retval = datareader.GetBytes(ordinal, startIndex, outbyte, 0, bufferSize);
                while (retval == bufferSize)
                {
                    bb.Append(outbyte);
                    startIndex += bufferSize;
                    retval = datareader.GetBytes(ordinal, startIndex, outbyte, 0, bufferSize);
                }
                if (retval > 0)
                    bb.Append(outbyte);
                return bb.GetBytes();
            }
            
            if (t == typeof(String)) return !datareader.IsDBNull(ordinal) ? datareader.GetString(ordinal) : null;
            
            if (t == typeof(DateTime)) return !datareader.IsDBNull(ordinal) ? (object)datareader.GetDateTime(ordinal) : null;
            
            if (t == typeof(Boolean)) return !datareader.IsDBNull(ordinal) ? (object)datareader.GetBoolean(ordinal) : null;

            if (t == typeof(Double)) return !datareader.IsDBNull(ordinal) ? (object)datareader.GetDouble(ordinal) : null;

            if (t == typeof(Single)) return !datareader.IsDBNull(ordinal) ? (object)datareader.GetFloat(ordinal) : null;

            if (t == typeof(Guid)) return !datareader.IsDBNull(ordinal) ? (object)datareader.GetGuid(ordinal) : null;

            if (t == typeof(Decimal)) return !datareader.IsDBNull(ordinal) ? (object)datareader.GetDecimal(ordinal) : null;

            if (t == typeof(Byte)) return !datareader.IsDBNull(ordinal) ? (object)datareader.GetByte(ordinal) : null;

            return null;
        }

        public static object getValue(IDataReader datareader, string name, Type type)
        {
            // Add additional data types here
            int ordinal = datareader.GetOrdinal(name);
            Type t = datareader.GetFieldType(ordinal);
            object retval = getRealValue(datareader, ordinal, t);
            // fugly ToString hack...
            if (retval !=null && type != t && type == typeof(string))
            {
                retval = retval.ToString();
            }
            return retval;
        }

        public static List<string> GetFields(Type type, bool includePrimary = true)
        {
            PropertyInfo[] props = type.GetProperties();
            List<string> fieldnames = new List<string>();
            bool foundAttribute = false;
            foreach (PropertyInfo prop in props)
            {
                foundAttribute = false;
                object[] attributes = prop.GetCustomAttributes(false);
                foreach (object attribute in attributes)
                {
                    if (attribute.GetType() == typeof(DBFieldMapping))
                    {
                        DBFieldMapping mapping = (DBFieldMapping)attribute;
                        //if (!mapping.primarykey || (includePrimary && mapping.primarykey))
                            fieldnames.Add(mapping.field);
                        foundAttribute = true;
                    }
                }
                if (!foundAttribute)
                    fieldnames.Add(prop.Name);
            }
            return fieldnames;
        }

        public static Dictionary<string, string> GetFieldMappings(Type type, bool includePrimary = true)
        {
            PropertyInfo[] props = type.GetProperties();
            Dictionary<string, string> mappings = new Dictionary<string, string>();
            bool foundAttribute = false;
            foreach (PropertyInfo prop in props)
            {
                foundAttribute = false;
                object[] attributes = prop.GetCustomAttributes(false);
                foreach (object attribute in attributes)
                {
                    if (attribute.GetType() == typeof(DBFieldMapping))
                    {
                        DBFieldMapping mapping = (DBFieldMapping)attribute;
                        if (!mapping.primarykey || (includePrimary && mapping.primarykey))
                            mappings.Add(prop.Name, mapping.field);
                        foundAttribute = true;
                    }
                }
                if (!foundAttribute)
                    mappings.Add(prop.Name, prop.Name);
            }
            return mappings;
        }

        public static string GetPrimaryKey(Type type)
        {
            PropertyInfo[] props = type.GetProperties();
            List<string> fieldnames = new List<string>();
            foreach (PropertyInfo prop in props)
            {
                object[] attributes = prop.GetCustomAttributes(false);
                foreach (object attribute in attributes)
                {
                    if (attribute.GetType() == typeof(DBFieldMapping))
                    {
                        DBFieldMapping mapping = (DBFieldMapping)attribute;
                        if (mapping.primarykey) return mapping.field;
                    }
                }
            }
            return null;
        }

        public static PropertyInfo GetPrimaryProperty(Type type)
        {
            PropertyInfo[] props = type.GetProperties();
            List<string> fieldnames = new List<string>();
            foreach (PropertyInfo prop in props)
            {
                object[] attributes = prop.GetCustomAttributes(false);
                foreach (object attribute in attributes)
                {
                    if (attribute.GetType() == typeof(DBFieldMapping))
                    {
                        DBFieldMapping mapping = (DBFieldMapping)attribute;
                        if (mapping.primarykey) return prop;
                    }
                }
            }
            return null;
        }

        public static string GetTableName(Type type)
        {
            object[] attributes = type.GetCustomAttributes(false);
            foreach (object attribute in attributes)
            {
                if (attribute.GetType() == typeof(DBTableMapping))
                {
                    DBTableMapping mapping = (DBTableMapping)attribute;
                    return mapping.table;
                }
            }
            return type.Name;
        }

        public static string GetFieldName(Type type, string propertyName)
        {
            PropertyInfo[] props = type.GetProperties();
            List<string> fieldnames = new List<string>();
            foreach (PropertyInfo prop in props)
            {
                object[] attributes = prop.GetCustomAttributes(false);
                if (prop.Name == propertyName)
                {
                    foreach (object attribute in attributes)
                    {
                        if (attribute.GetType() == typeof(DBFieldMapping))
                        {
                            DBFieldMapping mapping = (DBFieldMapping)attribute;
                            return mapping.field;
                        }
                    }
                    return prop.Name;
                }
            }
            return null;
        }

    }
}
