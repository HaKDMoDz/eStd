using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Linq.Expressions;

namespace System.Data.Access
{
    class ACCDBQueryProvider<T> : QueryProvider where T : new()
    {
        string joinClause;
        string topClause;
        string whereClause;
        string orderByClause; 
        string groupByClause;
        string table;
        bool hasFirst;
        int tableno = 0;
        ACCDBContext context;
        private string generatedquery;

        public ACCDBQueryProvider(string table, ACCDBContext context)
        {
            this.table = table;
            this.context = context;
        }

        Dictionary<Type, Dictionary<string, RawSQL>> joinTableTypeFields = new Dictionary<Type, Dictionary<string, RawSQL>>();
        Dictionary<Type, string> joinTableTypeTables = new Dictionary<Type, string>();


        Stack<string> memberaccessstack = new Stack<string>();

        protected string ToSQL(object result)
        {
            if (result == null) return "NULL";
            if (result.GetType() == typeof(string)) return "'" + result.ToString() + "'";
            if (result.GetType() == typeof(DateTime)) return "#" + result.ToString() + "#";
            if ((lastJoin != null) && (result.GetType() == typeof(RawSQL)) && (((RawSQL)result).Reference != null)) return lastJoin.getField(result.ToString());
            return result.ToString();
        }
        int tablecount = 0;
        Type lastJoinType;
        Join lastJoin;

        protected object Evaluate(Expression expression)
        {
            object returnval = null;
            Dictionary<string, RawSQL> fields;
            switch (expression.NodeType)
            {
                case ExpressionType.Parameter:
                    returnval = expression.Type;
                    break;
                case ExpressionType.AndAlso:
                case ExpressionType.And:
                    {
                        BinaryExpression binexp = ((BinaryExpression)expression);
                        returnval = new RawSQL("(" + ToSQL(Evaluate(binexp.Left)) + ") AND (" + ToSQL(Evaluate(binexp.Right)) + ")");
                    }
                    break;

                case ExpressionType.OrElse:
                case ExpressionType.Or:
                    {
                        BinaryExpression binexp = ((BinaryExpression)expression);
                        returnval = new RawSQL("(" + ToSQL(Evaluate(binexp.Left)) + ") OR (" + ToSQL(Evaluate(binexp.Right)) + ")");
                    }
                    break;

                case ExpressionType.Add:
                    {
                        BinaryExpression binexp = ((BinaryExpression)expression);
                        returnval = new RawSQL(ToSQL(Evaluate(binexp.Left)) + " + " + ToSQL(Evaluate(binexp.Right)));
                    }
                    break;


                case ExpressionType.Equal:
                    {
                        BinaryExpression binexp = ((BinaryExpression)expression);
                        returnval = new RawSQL(ToSQL(Evaluate(binexp.Left)) + " = " + ToSQL(Evaluate(binexp.Right)));
                    }
                    break;

                case ExpressionType.NotEqual:
                    {
                        BinaryExpression binexp = ((BinaryExpression)expression);
                        returnval = new RawSQL(ToSQL(Evaluate(binexp.Left)) + " <> " + ToSQL(Evaluate(binexp.Right)));
                    }
                    break;

                case ExpressionType.LessThan:
                    {
                        BinaryExpression binexp = ((BinaryExpression)expression);
                        returnval = new RawSQL(ToSQL(Evaluate(binexp.Left)) + " < " + ToSQL(Evaluate(binexp.Right)));
                    }
                    break;

                case ExpressionType.LessThanOrEqual:
                    {
                        BinaryExpression binexp = ((BinaryExpression)expression);
                        returnval = new RawSQL(ToSQL(Evaluate(binexp.Left)) + " <= " + ToSQL(Evaluate(binexp.Right)));
                    }
                    break;

                case ExpressionType.GreaterThan:
                    {
                        BinaryExpression binexp = ((BinaryExpression)expression);
                        returnval = new RawSQL(ToSQL(Evaluate(binexp.Left)) + " > " + ToSQL(Evaluate(binexp.Right)));
                    }
                    break;

                case ExpressionType.GreaterThanOrEqual:
                    {
                        BinaryExpression binexp = ((BinaryExpression)expression);
                        returnval = new RawSQL(ToSQL(Evaluate(binexp.Left)) + " >= " + ToSQL(Evaluate(binexp.Right)));
                    }
                    break;

                case ExpressionType.MemberInit:
                    MemberInitExpression miexp = (MemberInitExpression)expression;
                    fields = new Dictionary<string, RawSQL>();
                    foreach (MemberBinding binding in miexp.Bindings)
                    {
                        fields.Add(binding.Member.Name, (RawSQL)Evaluate(((MemberAssignment)binding).Expression));
                    }
                    returnval = fields;
                    break;

                case ExpressionType.New:
                    NewExpression newexp = (NewExpression)expression;
                    fields = new Dictionary<string, RawSQL>();
                    if (newexp.Arguments[0].NodeType == ExpressionType.MemberAccess)
                    {
                        if (newexp.Arguments.Count > 0)
                        {

                            for (int i = 0; i < newexp.Arguments.Count; i++)
                            {
                                fields.Add(newexp.Members[i].Name, (RawSQL)Evaluate(newexp.Arguments[i]));
                            }

                        }
                    }
                    else
                    {
                        for (int i = 1; i < newexp.Arguments.Count; i++)
                        {
                            Type t = (Type)Evaluate(newexp.Arguments[i]);
                            foreach (PropertyInfo pi in t.GetProperties())
                            {
                                fields.Add(pi.Name, new RawSQL(ModelMapper.GetTableName(t), ModelMapper.GetFieldName(t, pi.Name)));
                            }
                        }
                    }
                    returnval = fields;
                    break;

                case ExpressionType.Quote:
                    {
                        LambdaExpression callexp = (LambdaExpression)((UnaryExpression)expression).Operand;
                        returnval = new Lambda(callexp, Evaluate(callexp.Body));
                    }
                    break;

                case ExpressionType.MemberAccess:
                    {
                        MemberExpression propexp = (MemberExpression)expression;
                        string propertyName = propexp.Member.Name;

                        if (propexp.Expression != null)
                        {
                            if (propexp.Expression.Type.GetType().IsRuntimeType())
                            {
                                // If this is a runtime type, then we probably need to convert it into an SQL statement
                                if (propexp.Expression.NodeType == ExpressionType.Call)
                                {
                                    object result = Evaluate(propexp.Expression);

                                    if (propexp.Expression.Type == typeof(TimeSpan))
                                    {
                                        string timespan = "";
                                        switch (propertyName)
                                        {
                                            case "Seconds": timespan = "s"; break;
                                            case "Days": timespan = "d"; break;
                                            case "Hours": timespan = "h"; break;
                                            case "Months": timespan = "m"; break;
                                            case "Years": timespan = "y"; break;
                                        }
                                        returnval = new RawSQL(string.Format(result.ToString(), timespan));
                                    }
                                    else
                                    {
                                        returnval = result;
                                    }
                                }
                                else if (propexp.Expression.NodeType == ExpressionType.Parameter)
                                {
                                    //if (((ParameterExpression)propexp.Expression).Name.Contains("Transparent"))
                                    //{
                                    //    returnval = propexp.Member;
                                      
                                    //}
                                    //else
                                    //{
                                        returnval = new RawSQL(((ParameterExpression)propexp.Expression).Name, ModelMapper.GetFieldName(propexp.Expression.Type, propertyName));
                                    //}
                                }
                                else if (propexp.Expression.NodeType == ExpressionType.MemberAccess)
                                {
                                    object obj = Evaluate(propexp.Expression);
                                    //if (obj.GetType().BaseType==typeof(PropertyInfo))
                                    //{
                                    //    return new RawSQL(
                                    //        ((ParameterExpression)((MemberExpression)propexp.Expression).Expression).Name, 
                                    //        ModelMapper.GetFieldName(((PropertyInfo)obj).PropertyType, propertyName));
                                    //}
                                    if (propexp.Expression.Type.IsGenericType && propexp.Expression.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
                                    {
                                        returnval = obj;
                                    }
                                    else
                                    {
                                        //returnval = Traverse(propexp.Expression);
                                        Type type = null;

                                        if (propexp.Member.MemberType == MemberTypes.Field)
                                        {
                                            FieldInfo field = obj.GetType().GetField(propexp.Member.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField | BindingFlags.FlattenHierarchy);
                                            if (field != null)
                                            {
                                                returnval = field.GetValue(obj);
                                                type = field.FieldType;
                                            }
                                            else
                                            {
                                                throw new MemberAccessException(string.Format("Member '{0}' was not found on type '{1}'", propexp.Member.Name, obj.GetType().Name));
                                            };

                                        }
                                        else if (propexp.Member.MemberType == MemberTypes.Property)
                                        {
                                            PropertyInfo prop = obj.GetType().GetProperty(propexp.Member.Name);
                                            if (prop != null)
                                            {
                                                returnval = prop.GetValue(obj, null);
                                                type = prop.PropertyType;
                                            }
                                            else
                                            {
                                                throw new MemberAccessException(string.Format("Member '{0}' was not found on type '{1}'", propexp.Member.Name, obj.GetType().Name));
                                            };
                                        }
                                    }
                                }
                                else if (propexp.Expression.NodeType == ExpressionType.Constant)
                                {
                                    Type type = null;
                                    object obj = Evaluate(propexp.Expression);
                                    if (propexp.Member.MemberType == MemberTypes.Field)
                                    {
                                        FieldInfo field = obj.GetType().GetField(propexp.Member.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField | BindingFlags.FlattenHierarchy);
                                        if (field != null)
                                        {
                                            returnval = field.GetValue(obj);
                                            type = field.FieldType;
                                        }
                                        else
                                        {
                                            throw new MemberAccessException(string.Format("Member '{0}' was not found on type '{1}'", propexp.Member.Name, obj.GetType().Name));
                                        };

                                    }
                                    else if (propexp.Member.MemberType == MemberTypes.Property)
                                    {
                                        PropertyInfo prop = obj.GetType().GetProperty(propexp.Member.Name);
                                        if (prop != null)
                                        {
                                            returnval = prop.GetValue(obj, null);
                                            type = prop.PropertyType;
                                        }
                                        else
                                        {
                                            throw new MemberAccessException(string.Format("Member '{0}' was not found on type '{1}'", propexp.Member.Name, obj.GetType().Name));
                                        };
                                    }
                                }
                                else
                                {
                                    throw new NotSupportedException();
                                }
                            }
                            else
                            {
                                Type type = null;
                                object obj = Evaluate(propexp.Expression);
                                if (propexp.Member.MemberType == MemberTypes.Field)
                                {
                                    FieldInfo field = obj.GetType().GetField(propexp.Member.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField | BindingFlags.FlattenHierarchy);
                                    if (field != null)
                                    {
                                        returnval = field.GetValue(obj);
                                        type = field.GetType();
                                    }
                                    else
                                    {
                                        throw new MemberAccessException(string.Format("Member '{0}' was not found on type '{1}'", propexp.Member.Name, obj.GetType().Name));
                                    };

                                }
                                else if (propexp.Member.MemberType == MemberTypes.Property)
                                {
                                    PropertyInfo prop = obj.GetType().GetProperty(propexp.Member.Name);
                                    if (prop != null)
                                    {
                                        returnval = prop.GetValue(obj, null);
                                        type = prop.GetType();
                                    }
                                    else
                                    {
                                        throw new MemberAccessException(string.Format("Member '{0}' was not found on type '{1}'", propexp.Member.Name, obj.GetType().Name));
                                    };
                                }
                                if (type == typeof(string)) returnval = "'" + returnval + "'";
                                if (type == typeof(DateTime)) returnval = "#" + returnval.ToString() + "#";

                            }
                        }
                        else
                        {
                            // If we have a null expression then we are accessing a Type, not an object
                            if (propexp.Type == typeof(DateTime))
                            {
                                if (propexp.Member.Name == "Now")
                                    returnval = "DATE()";
                                else
                                    throw new NotSupportedException();
                            }
                            else
                            {
                                throw new NotSupportedException();
                            }
                        }

                    }
                    break;
                case ExpressionType.Constant:

                    string quotes = "";
                    ConstantExpression constexp = ((ConstantExpression)expression);
                    if (constexp.Type.IsGenericType)
                    {
                        var p = constexp.Type.GetGenericArguments()[0];
                        //returnval = new RawSQL(ModelMapper.GetTableName(p));
                        returnval = constexp;
                    }
                    else
                    {
                        return constexp.Value;
                    }
                    break;

                case ExpressionType.Call:
                    {
                        MethodCallExpression callexp = (MethodCallExpression)expression;

                        if (callexp.Type == typeof(String))
                        {
                            switch (callexp.Method.Name)
                            {
                                case "ToString":
                                    returnval = Evaluate(callexp.Object);
                                    break;
                            }
                        }
                        else if (typeof(IQueryable).IsAssignableFrom(callexp.Arguments[0].Type))
                        {
                            // LINQ Method calls
                            switch (callexp.Method.Name)
                            {
                                case "First":
                                case "FirstOrDefault":
                                    topClause = "1";
                                    Evaluate(callexp.Arguments[0]);
                                    hasFirst = true;
                                    break;
                                case "Distinct":
                                    Evaluate(callexp.Arguments[0]);
                                    hasDistinct = true;
                                    break;

                                case "OrderBy":
                                    Evaluate(callexp.Arguments[0]);
                                    if (selectExpression != null)
                                    {
                                        orderByClause = getSelectFields(false);
                                    }
                                    else
                                    {
                                        orderByClause = ((RawSQL)((Lambda)Evaluate(callexp.Arguments[1])).p).ToString();
                                    }
                                    hasOrderBy = true;
                                    break;


                                case "GroupBy":
                                    Evaluate(callexp.Arguments[0]);
                                    if (selectExpression != null)
                                    {
                                        throw new NotImplementedException();
                                        //groupByClause = getSelectFields(false);
                                    }
                                    else
                                    {
                                        groupByClause = ((RawSQL)((Lambda)Evaluate(callexp.Arguments[1])).p).ToString();
                                    }
                                    hasGroupBy = true;
                                    break;

                                case "OrderByDescending":
                                    object r = Evaluate(callexp.Arguments[0]);
                                    if (r.GetType() == typeof(Join))
                                        orderByClause = ((Join)r).getField(((RawSQL)((Lambda)Evaluate(callexp.Arguments[1])).p).ToString()) + " DESC ";
                                    else
                                        orderByClause = ((RawSQL)((Lambda)Evaluate(callexp.Arguments[1])).p).ToString() + " DESC ";
                                    hasOrderBy = true;

                                    break;

                                case "Count":
                                    Evaluate(callexp.Arguments[0]);
                                    hasCount = true;
                                    break;



                                case "Take":
                                    topClause = ToSQL(Evaluate(callexp.Arguments[1]));
                                    Evaluate(callexp.Arguments[0]);
                                    break;

                                case "Join":
                                    Join j = new Join(Evaluate(callexp.Arguments[0]),
                                                    Evaluate(callexp.Arguments[1]),
                                                    Evaluate(callexp.Arguments[2]),
                                                    Evaluate(callexp.Arguments[3]),
                                                    Evaluate(callexp.Arguments[4]), ref tablecount);

                                    joinClause += j.getJoin();

                                    joinFields = j.fields.Select(k => k.Value + " AS [" + k.Key + "]");

                                    lastJoinType = ((LambdaExpression)((UnaryExpression)callexp.Arguments[4]).Operand).ReturnType;

                                    lastJoin = j;

                                    returnval = j;
                                    break;

                                case "Select":
                                    Evaluate(callexp.Arguments[0]);
                                    //hasSelect = true;
                                    selectExpression = ((UnaryExpression)callexp.Arguments[1]).Operand;
                                    break;

                                case "Where":
                                    wheredepth++;
                                    string query = string.Empty;
                                    object ret = Evaluate(callexp.Arguments[0]);

                                    for (int i = 1; i < callexp.Arguments.Count; i++)
                                    {
                                        var clause = callexp.Arguments[i];
                                        object result = Evaluate(clause);

                                        if ((result != null) && (clause.NodeType != ExpressionType.Constant))
                                        {
                                            if (clause.NodeType == ExpressionType.Quote)
                                            {
                                                //if (typeof(Join) == ret.GetType())
                                                //{
                                                //query += " AND " + ((Join)ret).getField(Evaluate(clause).ToString());
                                                query += " AND " + ((RawSQL)((Lambda)result).p).ToString();
                                                //}
                                            }
                                            else
                                            {
                                                query = ((RawSQL)((Lambda)result).p).ToString();
                                            }
                                        }
                                    }
                                    wheredepth--;

                                    if (wheredepth == 0 && query.Length > 5)
                                        whereClause = query.Remove(0, 5) + " " + whereClause;
                                    else
                                        whereClause = query + " " + whereClause;

                                    returnval = ret;
                                    break;

                                default:
                                    throw new Exception(string.Format("Unsupported LINQ method call: '{0}'", callexp.Method.Name));
                            }
                        }
                        else
                        {
                            if (callexp.Object != null)
                            {

                                List<object> arguments = new List<object>();
                                foreach (var args in callexp.Arguments)
                                {
                                    arguments.Add(Evaluate(args));
                                }

                                string le = (string)Evaluate(callexp.Object);
                                switch (callexp.Object.Type.Name)
                                {
                                    case "DateTime":
                                        switch (callexp.Method.Name)
                                        {
                                            case "Subtract":
                                                returnval = new RawSQL("DATEDIFF('{0}', " + lastJoin.getField(arguments[0].ToString()) + ", " + le + ")");
                                                break;
                                            case "Add":
                                                returnval = new RawSQL("DATEADD('{0}', " + lastJoin.getField(arguments[0].ToString()) + ", " + le + ")");
                                                break;

                                            default:
                                                throw new Exception(string.Format("Unsupported method call: '{0}'", callexp.Method.Name));
                                        }
                                        break;

                                    default:
                                        throw new Exception(string.Format("Unsupported type: '{0}'", callexp.Object.Type.Name));

                                }
                            }
                            else
                            {

                                List<object> arguments = new List<object>();
                                foreach (var args in callexp.Arguments)
                                {
                                    arguments.Add(Evaluate(args));
                                }

                                returnval = callexp.Method.Invoke(null, arguments.ToArray());


                            }
                        }
                    }
                    break;

                case ExpressionType.Convert:
                    returnval = Evaluate(((UnaryExpression)expression).Operand);
                    break;

                default:
                    throw new Exception(string.Format("Unsupported type: '{0}'", Enum.GetName(typeof(ExpressionType), expression.NodeType)));
            }
            return returnval;
        }

        int wheredepth = 0;
        private Expression selectExpression;
        private bool hasCount;
        private bool hasDistinct;
        private IEnumerable<string> joinFields;

        //CachedQueryProvider cqp = new CachedQueryProvider();

        internal string GenerateQuery(Expression expression)
        {
            if (generatedquery == null)
            {
                Evaluate(expression);

                List<string> fields = ModelMapper.GetFields(typeof(T));

                generatedquery = string.Format("SELECT " +
                    (hasDistinct ? " DISTINCT " : "") +
                    (topClause != null ? " TOP " + topClause + " " : "") +
                    (selectExpression != null ? getSelectFields() :
                    (joinClause != null ?
                     string.Join(",", joinFields)
                     :
                    "[t0." + string.Join("], [t0.", fields.ToArray()) + "] "
                    )) +
                    " FROM " + (tablecount - 1 > 0 ? "".PadLeft(tablecount - 1, '(') : "") + table + " t0 " +
                    (joinClause != null ? joinClause : "") +
                    (whereClause != null ? " WHERE " + whereClause : "") +
                    (orderByClause != null ? " ORDER BY " + orderByClause : "") +
                    (groupByClause != null ? " GROUP BY " + groupByClause : "")
                    );

            }
            return generatedquery;
        }

        public override string ToString()
        {
            return generatedquery;
        }

        protected override object ExecuteExpression(Expression expression)
        {
            object returnval = null;

            string sqlcommand = GenerateQuery(expression);

            using (IDbConnection conn = context.CreateConnection())
            {

                System.Diagnostics.Debug.WriteLine(sqlcommand);

                conn.Open();
                IDbCommand comm = conn.CreateCommand();
                comm.CommandText = sqlcommand;
                comm.CommandType = CommandType.Text;

                IDataReader rdr = comm.ExecuteReader();

                if (selectExpression != null)
                {
                    Type selectType = ((LambdaExpression)selectExpression).ReturnType;

                    var outputgenerictype = typeof(List<>).MakeGenericType(selectType);
                    object iitems = Activator.CreateInstance(outputgenerictype);
                    ConstructorInfo constructor = null;
                    ParameterInfo[] parameterInfoes = null;
                    bool isAnonymousType = selectType.IsAnonymousType();
                    MethodInfo mListAdd = outputgenerictype.GetMethod("Add");

                    if (isAnonymousType)
                    {
                        constructor = selectType.GetConstructors()[0];
                        parameterInfoes = constructor.GetParameters();
                    }

                    while (rdr.Read())
                    {
                        var paramlist = new List<object>();
                        object obj = null;
                        // Anonymous types have a parameterized constructor
                        if (isAnonymousType)
                        {
                            // Create the parameter array by mapping the joined field names to the constructor parameter names
                            foreach (ParameterInfo parameterInfo in parameterInfoes)
                            {
                                string mappedfield = parameterInfo.Name;
                                paramlist.Add(ModelMapper.getValue(rdr, mappedfield, parameterInfo.ParameterType));
                            }
                            obj = constructor.Invoke(paramlist.ToArray());

                        }
                        else
                        {
                            if (selectType.GetType().IsRuntimeType())
                            {
                                object result = Evaluate(((LambdaExpression)selectExpression).Body);
                                if (result.GetType() == typeof(Dictionary<string, RawSQL>))
                                {
                                    Dictionary<string, RawSQL> r = (Dictionary<string, RawSQL>)result;
                                    obj = Activator.CreateInstance(selectType);
                                    foreach (PropertyInfo pi in selectType.GetProperties())
                                    {
                                        if (r.ContainsKey(pi.Name))
                                        {
                                            string mappedfield = r[pi.Name].SQL;
                                            object getval = ModelMapper.getValue(rdr, mappedfield, pi.PropertyType);
                                            pi.SetValue(obj, getval, null);
                                        }
                                    }
                                }
                                else
                                {
                                    string fullname = ((RawSQL)result).ToString();
                                    string mappedfield = fullname.Substring(fullname.IndexOf(".") + 1);
                                    obj = ModelMapper.getValue(rdr, mappedfield, selectType);

                                }


                            }
                            else
                            {
                                obj = Activator.CreateInstance(selectType);
                                foreach (PropertyInfo pi in selectType.GetProperties())
                                {
                                    string mappedfield = pi.Name;
                                    pi.SetValue(obj, ModelMapper.getValue(rdr, mappedfield, pi.PropertyType), null);
                                }
                            }

                        }
                        mListAdd.Invoke(iitems, new object[] { obj });
                    }

                    returnval = iitems;

                }
                else if (joinClause != null)
                {
                    // Create a 

                    var outputgenerictype = typeof(List<>).MakeGenericType(lastJoinType);
                    object iitems = Activator.CreateInstance(outputgenerictype);
                    ConstructorInfo constructor = null;
                    ParameterInfo[] parameterInfoes = null;
                    bool isAnonymousType = lastJoinType.IsAnonymousType();
                    MethodInfo mListAdd = outputgenerictype.GetMethod("Add");

                    if (isAnonymousType)
                    {
                        constructor = lastJoinType.GetConstructors()[0];
                        parameterInfoes = constructor.GetParameters();
                    }

                    while (rdr.Read())
                    {
                        var paramlist = new List<object>();
                        object obj;
                        // Anonymous types have a parameterized constructor
                        if (isAnonymousType)
                        {
                            // Create the parameter array by mapping the joined field names to the constructor parameter names
                            foreach (ParameterInfo parameterInfo in parameterInfoes)
                            {
                                string mappedfield = parameterInfo.Name;
                                paramlist.Add(ModelMapper.getValue(rdr, mappedfield, parameterInfo.ParameterType));
                            }
                            obj = constructor.Invoke(paramlist.ToArray());

                        }
                        else
                        {
                            obj = Activator.CreateInstance(lastJoinType);
                            foreach (PropertyInfo pi in lastJoinType.GetProperties())
                            {
                                if (lastJoin.hasField(pi.Name))
                                {
                                    pi.SetValue(obj, ModelMapper.getValue(rdr, pi.Name, pi.PropertyType), null);
                                }
                            }

                        }
                        mListAdd.Invoke(iitems, new object[] { obj });
                    }

                    returnval = getReturnVal(iitems);
                }
                else
                {
                    var items = new List<T>();
                    while (rdr.Read())
                    {
                        T obj = new T();
                        ModelMapper.Map(typeof(T), rdr, obj);
                        items.Add(obj);
                    }
                    returnval = getReturnVal(items);
                }

                conn.Close();
            }
            return returnval;
        }


        private string getSelectFields()
        {
            return getSelectFields(true);
        }

        private string getSelectFields(bool IncludeAlias)
        {
            List<string> fieldList = new List<string>(); ;
            if (selectExpression != null)
            {
                Type selectType = ((LambdaExpression)selectExpression).ReturnType;

                ConstructorInfo constructor = null;
                ParameterInfo[] parameterInfoes = null;
                bool isAnonymousType = selectType.IsAnonymousType();

                if (isAnonymousType)
                {
                    constructor = selectType.GetConstructors()[0];
                    parameterInfoes = constructor.GetParameters();
                }

                // Anonymous types have a parameterized constructor
                if (isAnonymousType)
                {

                    var c = ((NewExpression)((LambdaExpression)selectExpression).Body);
                    // Create the parameter array by mapping the joined field names to the constructor parameter names
                    int i = 0;
                    foreach (ParameterInfo parameterInfo in parameterInfoes)
                    {
                        fieldList.Add(Evaluate(c.Arguments[i++]) + (IncludeAlias ? " AS [" + parameterInfo.Name + "]" : ""));
                    }

                }
                else
                {
                    if (selectType.GetType().IsRuntimeType())
                    {
                        object result = Evaluate(((LambdaExpression)selectExpression).Body);
                        if (result.GetType() == typeof(Dictionary<string, RawSQL>))
                        {

                            foreach (RawSQL field in ((Dictionary<string, RawSQL>)result).Values)
                            {
                                fieldList.Add(field.SQL);
                            }
                        }
                        else
                        {
                            string fullname = ((RawSQL)result).ToString();
                            fieldList.Add(fullname.Substring(fullname.IndexOf(".") + 1));
                        }
                    }
                    else
                    {
                        foreach (PropertyInfo pi in selectType.GetProperties())
                        {
                            fieldList.Add("[" + pi.Name + "]");
                        }
                    }

                }

            }
            return string.Join(", ", fieldList.ToArray());
        }

        protected object getReturnVal(object items)
        {
            object returnval = null;

            IList listitems = (IList)items;
            if (hasCount)
                returnval = listitems.Count;
            else if (hasFirst)
                if (listitems.Count > 0)
                    returnval = listitems[0];
                else
                    returnval = default(T);
            else
                returnval = items;

            return returnval;
        }


        private bool hasOrderBy;
        private bool hasGroupBy;
    }
}
