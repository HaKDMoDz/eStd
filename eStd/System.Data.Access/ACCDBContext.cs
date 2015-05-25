using System;
using System.Linq;
using System.Data;
using System.Reflection;

namespace Furesoft.Creek.Office.Access
{
    public abstract class ACCDBContext : IDisposable
    {
        protected string connectionString;
        protected IDbConnection connection;

        public virtual void Init()
        {
            FieldInfo[] fields = this.GetType().GetFields();
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsGenericType && (field.FieldType.GetGenericTypeDefinition() == typeof(ACCDBSet<>)))
                {
                    Type[] types = field.FieldType.GetGenericArguments();
                    field.SetValue(this, Activator.CreateInstance(typeof(ACCDBSet<>).MakeGenericType(types[0]), new object[] { this }));
                }
            }
        }

        public ACCDBSet<TEntity> Set<TEntity>(string table) where TEntity : new()
        {
            return new ACCDBSet<TEntity>(this, table);
        }

        public ACCDBSet<TEntity> Set<TEntity>() where TEntity : new()
        {
            return new ACCDBSet<TEntity>(this);
        }

        public ACCDBContext(IDbConnection connection)
        {
            this.connection = connection;
            Init();
        }

        public ACCDBContext(string connectionString)
        {
            this.connectionString = connectionString;
            Init();
        }

        public void AddObject(string entitySetName, object entity)
        {
            PropertyInfo[] fields = this.GetType().GetProperties();
            foreach (PropertyInfo field in fields)
            {
                if (field.PropertyType.IsGenericType)
                {
                    Type[] types = field.PropertyType.GetGenericArguments();
                    if (types[0].Name == entitySetName)
                    {
                        dynamic set = field.GetValue(this, null);
                        set.AddObject(entity);
                    }
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
            }
        }

        #endregion

        public abstract IDbConnection CreateConnection();

        internal object Execute(string commandText)
        {
            object retval = null;
            using (IDbConnection connection = CreateConnection())
            {
                connection.Open();
                IDbCommand cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = commandText;
                cmd.ExecuteNonQuery();
                if (commandText.ToUpper().StartsWith("INSERT"))
                {
                    cmd.CommandText = "Select @@Identity";
                    retval = cmd.ExecuteScalar();
                }
                connection.Close();
                return retval;
            }
        }

        public void SaveChanges()
        {
        }
    }
}
