using System;

namespace Furesoft.Creek.Office.Access
{
    public static class TypeExtensions
    {
        public static bool IsRuntimeType(this Type type)
        {
            return type.UnderlyingSystemType.Name == "RuntimeType";
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DBFieldMapping : Attribute
    {
        public string field { get; set; }
        public bool primarykey { get; set; }

        public DBFieldMapping(string field)
        {
            this.field = field;
        }

        public DBFieldMapping(string field, bool primarykey)
        {
            this.field = field;
            this.primarykey = primarykey;
        }

    }

    [AttributeUsage(AttributeTargets.Class)]
    public class DBTableMapping : Attribute
    {
        public string table { get; set; }

        public DBTableMapping(string table)
        {
            this.table = table;
        }

    }
}
