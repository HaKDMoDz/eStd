using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System.Data.Access
{
    public class ACCDBSet<T> : Query<T> where T : new()
    {
        ACCDBContext context;

        public ACCDBSet(ACCDBContext context, string table)
        {
            this.context = context;
            provider = new ACCDBQueryProvider<T>(table, context);
        }

        public ACCDBSet(ACCDBContext context)
        {
            this.context = context;
            string table = ModelMapper.GetTableName(typeof(T));
            provider = new ACCDBQueryProvider<T>(table, context);
        }

        public override string ToString()
        {
            return ((ACCDBQueryProvider<T>)provider).GenerateQuery(this.expression);
        }

        public void AddObject(object entity)
        {
            string table = ModelMapper.GetTableName(typeof(T));
            Dictionary<string, string> mappings = ModelMapper.GetFieldMappings(typeof(T), false);
            List<string> sbFieldValues = new List<string>();
            List<string> sbFieldNames = new List<string>();
            PropertyInfo[] props = entity.GetType().GetProperties();
            foreach (PropertyInfo prop in props)
            {
                if (mappings.ContainsKey(prop.Name))
                {
                    sbFieldNames.Add("[" + mappings[prop.Name] + "]");
                    sbFieldValues.Add(ToSQL(prop.GetValue(entity, null)));
                }
            }
            string insertSql = string.Format("INSERT INTO {0} ({1}) SELECT {2}", table, string.Join(", ", sbFieldNames.ToArray()), string.Join(", ", sbFieldValues.ToArray()));
            object result = context.Execute(insertSql);
            PropertyInfo pk = ModelMapper.GetPrimaryProperty(typeof(T));
            if (pk != null)
                pk.SetValue(entity, result, null);
        }

        protected string ToSQL(object result)
        {
            if (result == null) return "NULL";
            if (result.GetType() == typeof(string)) return "'" + result.ToString() + "'";
            if (result.GetType() == typeof(DateTime)) return "#" + result.ToString() + "#";
            return result.ToString();
        }
    }
}
