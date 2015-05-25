using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Linq.Expressions;

namespace Furesoft.Creek.Office.Access
{
    public class Query<T> : IOrderedQueryable<T>
    {
        protected QueryProvider provider;
        protected Expression expression;

        public Query()
        {
            this.expression = Expression.Constant(this);
        }

        public Query(QueryProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            this.provider = provider;
            this.expression = Expression.Constant(this);
        }

        public Query(QueryProvider provider, Expression expression)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentOutOfRangeException("expression");
            }
            this.provider = provider;
            this.expression = expression;
        }

        #region IEnumerable<T> Members
        IEnumerable<T> collection = null;

        IEnumerable<T> lastCollection;

        protected virtual IEnumerable<T> Collection
        {
            get
            {
                if (collection == null) collection = ((IEnumerable<T>)this.provider.Execute(this.expression));
                //lastCollection = collection.ToList<T>();
                return collection;
            }
        }


        public void Update()
        {
            //PropertyInfo[] props = typeof(T).GetProperties();
            //foreach (T item in lastCollection)
            //{
            //    bool changed = false;
            //    T comp = collection.Where(q => q.GetHashCode() == item.GetHashCode()).First();
            //    if (comp != null)
            //    {
            //        foreach (PropertyInfo prop in props)
            //        {
            //            if (!prop.GetValue(item, null).Equals(prop.GetValue(comp, null)))
            //            {
            //                changed = true;
            //                break;
            //            }
            //        }
            //    }
            //    if (changed)
            //    {
            //        int x = 1;
            //    }
            //}
        }


        public IEnumerator<T> GetEnumerator()
        {
            return Collection.GetEnumerator();
        }

        public override string ToString()
        {
            return this.provider.ToString();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.provider.Execute(this.expression)).GetEnumerator();
        }

        #endregion

        #region IQueryable Members

        public Type ElementType
        {
            get { return typeof(T); }
        }

        public Expression Expression
        {
            get { return this.expression; }
        }

        public IQueryProvider Provider
        {
            get { return this.provider; }
        }

        #endregion
    }
}
