// <copyright file="CollectionExtensions.cs" company="Edge Extensions Project">
// Copyright (c) 2009 All Rights Reserved
// </copyright>
// <author>Kevin Nessland</author>
// <email>kevinnessland@gmail.com</email>
// <date>2009-07-08</date>
// <summary>Contains collection-related extensions.</summary>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Creek.Extensions
{
    /// <summary>
    /// Contains collection-related extensions.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Adds a range of values to an existing <see cref="string">List</see>.
        /// </summary>
        /// <typeparam name="T">Type of the items in the <see cref="string">List</see>.</typeparam>
        /// <param name="list"><see cref="string">List</see> to append.</param>
        /// <param name="values">Range of values to add.</param>
        /// <example>
        ///     <code language="c#">   
        ///         List&lt;string&gt; items = new List&lt;string&gt;();
        ///         items.Add("a");
        ///         items.Add("b");
        ///         items.AddRange("c", "d", "e");
        ///     </code>
        /// </example>
        public static void AddRange<T>(this List<T> list, params T[] values)
        {
            foreach (T value in values)
            {
                list.Add(value);
            }
        }

        /// <summary>
        /// Averages the values of an integer array.
        /// </summary>
        /// <param name="array"><see cref="Array">Array</see> to average.</param>
        /// <returns>The average value of the <see cref="Array">Array</see> contents.</returns>
        /// <example>
        ///     <code language="c#">          
        ///         int[] array = new int[] { 1, 2, 3, 4, 5 };
        ///         float average = array.Average(); -> results in 3
        ///     </code>
        /// </example>
        public static float Average(this int[] array)
        {
            if (array.Length == 0)
            {
                throw new ArgumentOutOfRangeException("array");
            }

            float average = 0;

            for (int i = 0; i < array.Length; i++)
            {
                average += (int)array.GetValue(i);
            }

            return average / array.Length;
        }

        /// <summary>
        /// Checks whether an <see cref="IEnumerable">IEnumerable</see> contains at least a certain number of items.
        /// </summary>
        /// <typeparam name="T">Type of the <see cref="IEnumerable">IEnumerable</see>.</typeparam>
        /// <param name="enumeration"><see cref="IEnumerable">IEnumerable</see> to inspect.</param>
        /// <param name="count">Number to use in the comparison.</param>
        /// <returns>Indicator if the <see cref="IEnumerable">IEnumerable</see> is at least the specified length.</returns>
        /// <example>
        ///     <code language="c#">
        ///         List&lt;int&gt; list = new List&lt;int&gt;();
        ///         list.Add(1);
        ///         list.Add(2);
        ///         list.Add(3);
        ///         bool valid = list.ContainsAtLeast&lt;int&gt;(2);
        ///     </code>
        /// </example>
        public static bool ContainsAtLeast<T>(this IEnumerable<T> enumeration, int count)
        {
            // Check to see that enumeration is not null
            if (enumeration == null)
            {
                throw new ArgumentNullException("enumeration");
            }

            return (from t in enumeration.Take(count)
                    select t)
                    .Count() == count;
        }

        /// <summary>
        /// Iterates each item in an IEnumerable of a type and performs an operation.
        /// </summary>
        /// <typeparam name="T">Type to enumerate.</typeparam>
        /// <param name="enumeration">List of IEnumerable objects.</param>
        /// <param name="mapFunction">Function to execute for each type.</param>
        /// <example>
        ///     <code language="c#">
        ///         buttons.ForEach(b => b.Click());
        ///     </code>
        /// </example>
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> mapFunction)
        {
            foreach (var item in enumeration)
            {
                mapFunction(item);
            }
        }

        /// <summary>
        /// Creates an array of IEnumerables grouped.
        /// </summary>
        /// <typeparam name="T">Type of the <see cref="IEnumerable">IEnumerable</see>.</typeparam>
        /// <param name="enumeration"><see cref="IEnumerable">IEnumerable</see> to group.</param>
        /// <param name="count">Number of elements in each group.</param>
        /// <returns>An <see cref="IEnumerable">IEnumerable</see> array.</returns>
        /// <example>
        ///     <code language="c#">
        ///         List&lt;int&gt; list = new List&lt;int&gt;();
        ///         for (int i = 0; i &lt; 10; i++)
        ///         {
        ///             list.Add(i);
        ///         }
        ///         IEnumerable&lt;int[]&gt; groupedList = list.GroupEvery&lt;int&gt;(2);
        ///     </code>
        /// </example>
        public static IEnumerable<T[]> GroupEvery<T>(this IEnumerable<T> enumeration, int count)
        {
            if (enumeration == null)
            {
                throw new ArgumentNullException("IEnumerable cannot be null.");
            }

            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException("The count parameter must be greater than zero.");
            }

            int current = 0;

            T[] array = new T[count];

            foreach (var item in enumeration)
            {
                array[current++] = item;

                if (current == count)
                {
                    yield return array;

                    current = 0;

                    array = new T[count];
                }
            }

            if (current != 0)
            {
                yield return array;
            }
        }

        /// <summary>
        /// Returns the index of the first occurrence of a value in a sequence by using the default EqualityComparer.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="enumeration">A sequence in which to locate a value.</param>
        /// <param name="value">The object to locate in the sequence.</param>
        /// <returns>The zero-based index of the first occurrence of value within the entire sequence, if found; otherwise, –1.</returns>
        /// <example>
        ///     <code language="c#">
        ///         List&lt;string&gt; items = new List&lt;string&gt;();
        ///         items.Push("a");
        ///         items.Push("b");
        ///         items.Push("c");
        ///         int index = items.IndexOf&lt;string&gt;("c");
        ///     </code>
        /// </example>
        public static int IndexOf<T>(this IEnumerable<T> enumeration, T value) where T : IEquatable<T>
        {
            return enumeration.IndexOf<T>(value, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Returns the index of the first occurrence in a sequence by using a specified IEqualityComparer.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the IEnumerable.</typeparam>
        /// <param name="enumeration">A sequence in which to locate a value.</param>
        /// <param name="value">The value to locate in the sequence.</param>
        /// <param name="comparer">An equality comparer to compare values.</param>
        /// <returns>The zero-based index of the first occurrence of value within the entire sequence, if found; otherwise, –1.</returns>
        /// <example>
        ///     <code language="c#">
        ///         List&lt;string&gt; items = new List&lt;string&gt;();
        ///         items.Push("a");
        ///         items.Push("b");
        ///         items.Push("c");
        ///         int index = items.IndexOf&lt;string&gt;("c", myComparer);
        ///     </code>
        /// </example>
        public static int IndexOf<T>(this IEnumerable<T> enumeration, T value, IEqualityComparer<T> comparer)
        {
            int index = 0;

            foreach (var item in enumeration)
            {
                if (comparer.Equals(item, value))
                {
                    return index;
                }

                index++;
            }

            return -1;
        }

        /// <summary>
        /// Returns the index of the first occurrence, starting at the indicated start index, in a sequence by using the default equality comparer.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the IEnumerable.</typeparam>
        /// <param name="enumeration">A sequence in which to locate a value.</param>
        /// <param name="value">The value to locate in the sequence.</param>
        /// <param name="startIndex">The index at which to begin the search.</param>
        /// <returns>The zero-based index of the first occurrence of value within the entire sequence, if found; otherwise, –1.</returns>
        /// <example>
        ///     <code language="c#">
        ///         List&lt;string&gt; items = new List&lt;string&gt;();
        ///         items.Push("a");
        ///         items.Push("b");
        ///         items.Push("c");
        ///         int index = items.IndexOf&lt;string&gt;("c", 1);
        ///     </code>
        /// </example>
        public static int IndexOf<T>(this IEnumerable<T> enumeration, T value, int startIndex) where T : IEquatable<T>
        {
            return enumeration.IndexOf<T>(value, startIndex, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Returns the index of the first occurrence in a sequence by using a specified IEqualityComparer.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="enumeration">A sequence in which to locate a value.</param>
        /// <param name="value">The value to locate in the sequence.</param>
        /// <param name="startIndex">The index at which to begin the search.</param>
        /// <param name="comparer">An equality comparer to compare values.</param>
        /// <returns>The zero-based index of the first occurrence of value within the entire sequence, if found; otherwise, –1.</returns>
        /// <example>
        ///     <code language="c#">
        ///         List&lt;string&gt; items = new List&lt;string&gt;();
        ///         items.Push("a");
        ///         items.Push("b");
        ///         items.Push("c");
        ///         int index = items.IndexOf&lt;string&gt;("c", 1, myComparer);
        ///     </code>
        /// </example>
        public static int IndexOf<T>(this IEnumerable<T> enumeration, T value, int startIndex, IEqualityComparer<T> comparer)
        {
            for (int i = startIndex; i < enumeration.Count(); i++)
            {
                T item = enumeration.ElementAt(i);

                if (comparer.Equals(item, value))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Retrieves the index of the next line in a string list that contains the specified value.
        /// </summary>
        /// <param name="items">The list of strings to inspect.</param>
        /// <param name="value">Value to search for.</param>
        /// <param name="startIndex">Line at which to begin the search.</param>
        /// <returns>A FileInfo array of the search results.</returns>
        /// <example>
        ///     <code language="c#">
        ///         List&lt;string&gt; items = new List&lt;string&gt;();
        ///         items.Push("Hello, ");
        ///         items.Push("World!");
        ///         items.Push("How ");
        ///         items.Push("are ");
        ///         items.Push("you?");
        ///         int index = items.IndexOfNextContaining("re", 2);
        ///     </code>
        /// </example>
        public static int IndexOfNextContaining(this List<string> items, string value, int startIndex)
        {
            for (int i = startIndex; i < items.Count; i++)
            {
                if (items[i].Contains(value))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Retrieves the index of the previous line in a string list that contains the specified value.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="items">The list of strings to inspect.</param>
        /// <param name="value">Value to search for.</param>
        /// <param name="fromIndex">Line at which to begin the search.</param>
        /// <returns>A FileInfo array of the search results.</returns>
        /// <example>
        ///     <code language="c#">
        ///         List&lt;string&gt; items = new List&lt;string&gt;();
        ///         items.Push("a");
        ///         items.Push("b");
        ///         items.Push("c");
        ///         int indexOfA = items.IndexOfPrevious("a", 2);
        ///     </code>
        /// </example>
        public static int IndexOfPrevious<T>(this IEnumerable<T> items, T value, int fromIndex)
        {
            for (int i = fromIndex - 1; i > -1; i--)
            {
                T item = items.ElementAt(i);

                if (EqualityComparer<T>.Default.Equals(item, value))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Retrieves the index of the previous line in a string list that contains the specified value.
        /// </summary>
        /// <param name="items">The list of strings to inspect.</param>
        /// <param name="value">Value to search for.</param>
        /// <param name="fromIndex">Line at which to begin the search.</param>
        /// <returns>A FileInfo array of the search results.</returns>
        /// <example>
        ///     <code language="c#">
        ///         List&lt;string&gt; items = new List&lt;string&gt;();
        ///         items.Push("Hello, ");
        ///         items.Push("World!");
        ///         items.Push("How ");
        ///         items.Push("are ");
        ///         items.Push("you?");
        ///         int index = items.IndexOfPreviousContaining("Wor", 3);
        ///     </code>
        /// </example>
        public static int IndexOfPreviousContaining(this List<string> items, string value, int fromIndex)
        {
            for (int i = fromIndex - 1; i > -1; i--)
            {
                if (items[i].Contains(value))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns a Boolean indicating whether the Array is null or empty.
        /// </summary>
        /// <param name="array">The Array to inspect.</param>
        /// <returns>Returns a boolean indicating whether the Array is null empty.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string[] array = new string[] { };
        ///         bool isEmpty = array.IsNullOrEmpty();-> results in true
        ///     </code>
        /// </example>
        public static bool IsNullOrEmpty(this Array array)
        {
            return (array == null) || (array.Length == 0);
        }

        /// <summary>
        /// Returns a Boolean indicating whether the ArrayList is null or empty..
        /// </summary>
        /// <param name="list">The ArrayList to inspect.</param>
        /// <returns>Returns a Boolean indicating whether the ArrayList is null or empty.</returns>
        /// <returns>Array with only unique items.</returns>
        /// <example>
        ///     <code language="c#">
        ///         if (!list.IsNullOrEmpty())
        ///         {
        ///             // do operation involving list
        ///         }
        ///     </code>
        /// </example>
        public static bool IsNullOrEmpty(this ArrayList list)
        {
            return (list == null) || (list.Count == 0);
        }

        /// <summary>
        /// Gets the last value from a list.
        /// </summary>
        /// <typeparam name="T">Type of items in the list.</typeparam>
        /// <param name="list">List to retrieve the value from.</param>
        /// <returns>The last item in the list.</returns>
        /// <example>
        ///     <code language="c#">
        ///          var list = new List&lt;int&gt;();
        ///          for (int i = 1; i &lt;= 10; i++)
        ///          {
        ///              list.Push(i);
        ///          }
        ///          while (list.Count > 0)
        ///          {
        ///              Console.WriteLine(list.Pop().ToString());
        ///          }
        ///     </code>
        /// </example>
        public static T Pop<T>(this List<T> list)
        {
            if (list.Count == 0)
            {
                throw new Exception("Cannot pop items from an empty list.");
            }

            int lastPosition = list.Count - 1;

            T result = list[lastPosition];

            list.RemoveAt(lastPosition);

            return result;
        }

        /// <summary>
        /// Adds a value to the end of a list.
        /// </summary>
        /// <typeparam name="T">Type of the items in the list.</typeparam>
        /// <param name="list">List to add the new value to.</param>
        /// <param name="value">Value to place at the end of the list.</param>
        /// <example>
        ///     <code language="c#">
        ///          var list = new List&lt;int&gt;();
        ///          for (int i = 1; i &lt;= 10; i++)
        ///          {
        ///              list.Push(i);
        ///          }
        ///          while (list.Count > 0)
        ///          {
        ///              Console.WriteLine(list.Pop());
        ///          }
        ///     </code>
        /// </example>
        public static void Push<T>(this List<T> list, T value)
        {
            list.Add(value);
        }

        /// <summary>
        /// Removes duplicate strings from an array.
        /// </summary>
        /// <typeparam name="T">Type of array.</typeparam>
        /// <param name="array">Array for which to remove duplicates.</param>
        /// <returns>Array with only unique items.</returns>
        /// <example>
        ///     <code language="c#">
        ///         string[] array = new string[] { "test1", "test2", "test1" };
        ///         string[] filtered = array.RemoveDuplicates(); -> results in string[] { "test1", "test2" };
        ///     </code>
        /// </example>
        public static T[] RemoveDuplicates<T>(this T[] array)
        {
            ArrayList al = new ArrayList();

            for (int i = 0; i < array.Length; i++)
            {
                if (!al.Contains(array[i]))
                {
                    al.Add(array[i]);
                }
            }

            return (T[])al.ToArray(typeof(T));
        }

        /// <summary>
        /// Get the array slice between the two indexes.
        /// Inclusive for start index, exclusive for end index.
        /// </summary>
        /// <typeparam name="T">Type of the objects in the array.</typeparam>
        /// <param name="array">Array to slice.</param>
        /// <param name="start">Start index. Inclusive.</param>
        /// <param name="end">End index.  Non-inclusive.</param>
        /// <returns>A slice of an array.</returns>
        /// <example>
        ///     <para>The following example slices an array that looks like the following:  int[] { 2, 3, 4 }</para>
        ///     <code language="c#">
        ///         int[] array = new int[] { 1, 2, 3, 4, 5 };
        ///         int[] slice = array.Slice&lt;int&gt;(1, 3);
        ///     </code>
        /// </example>
        public static T[] Slice<T>(this T[] array, int start, int end)
        {
            // Handles negative ends
            if (end < 0)
            {
                end = array.Length - start - end - 1;
            }

            int len = end - start;

            // Return new array
            T[] res = new T[len];
            for (int i = 0; i < len; i++)
            {
                res[i] = array[i + start];
            }

            return res;
        }

        /// <summary> 
        /// Return the element that the specified property's value is contained in the specifiec values 
        /// </summary> 
        /// <typeparam name="TElement">The type of the element.</typeparam> 
        /// <typeparam name="TValue">The type of the values.</typeparam> 
        /// <param name="source">The source.</param> 
        /// <param name="propertySelector">The property to be tested.</param> 
        /// <param name="values">The accepted values of the property.</param> 
        /// <returns>The accepted elements.</returns> 
        public static IQueryable<TElement> WhereIn<TElement, TValue>(this IQueryable<TElement> source, Expression<Func<TElement, TValue>> propertySelector, params TValue[] values)
        {
            return source.Where(GetWhereInExpression(propertySelector, values));
        }

        /// <summary> 
        /// Returns the element that the specified property's value is contained in the specifiec values.
        /// </summary> 
        /// <typeparam name="TElement">The type of the element.</typeparam> 
        /// <typeparam name="TValue">The type of the values.</typeparam> 
        /// <param name="source">The source.</param> 
        /// <param name="propertySelector">The property to be tested.</param> 
        /// <param name="values">The accepted values of the property.</param> 
        /// <returns>The accepted elements.</returns> 
        public static IQueryable<TElement> WhereIn<TElement, TValue>(this IQueryable<TElement> source, Expression<Func<TElement, TValue>> propertySelector, IEnumerable<TValue> values)
        {
            return source.Where(GetWhereInExpression(propertySelector, values));
        }

        /// <summary>
        /// Returns the element that the specified property's value is not contained in the specifiec values.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam> 
        /// <typeparam name="TValue">The type of the values.</typeparam> 
        /// <param name="source">The source.</param> 
        /// <param name="propertySelector">The property to be tested.</param> 
        /// <param name="values">The accepted values of the property.</param> 
        /// <returns>The accepted elements.</returns>
        public static IQueryable<TElement> WhereNotIn<TElement, TValue>(this IQueryable<TElement> source, Expression<Func<TElement, TValue>> propertySelector, params TValue[] values)
        {
            return source.Where(GetWhereNotInExpression(propertySelector, values));
        }

        /// <summary>
        /// Returns the element that the specified property's value is not contained in the specifiec values.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam> 
        /// <typeparam name="TValue">The type of the values.</typeparam> 
        /// <param name="source">The source.</param> 
        /// <param name="propertySelector">The property to be tested.</param> 
        /// <param name="values">The accepted values of the property.</param> 
        /// <returns>The accepted elements.</returns>
        public static IQueryable<TElement> WhereNotIn<TElement, TValue>(this IQueryable<TElement> source, Expression<Func<TElement, TValue>> propertySelector, IEnumerable<TValue> values)
        {
            return source.Where(GetWhereNotInExpression(propertySelector, values));
        }

        /// <summary>
        /// Returns an expression that will assist in retrieving values included in a collection.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam> 
        /// <typeparam name="TValue">The type of the values.</typeparam> 
        /// <param name="propertySelector">The property to be tested.</param> 
        /// <param name="values">The accepted values of the property.</param> 
        /// <returns>A LINQ expression.</returns>
        private static Expression<Func<TElement, bool>> GetWhereInExpression<TElement, TValue>(Expression<Func<TElement, TValue>> propertySelector, IEnumerable<TValue> values)
        {
            ParameterExpression p = propertySelector.Parameters.Single();

            if (!values.Any())
            {
                return e => false;
            }

            var equals = values.Select(value => (Expression)Expression.Equal(propertySelector.Body, Expression.Constant(value, typeof(TValue))));
            var body = equals.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal));

            return Expression.Lambda<Func<TElement, bool>>(body, p);
        }

        /// <summary>
        /// Returns an expression that will assist in retrieving values not included in a collection.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam> 
        /// <typeparam name="TValue">The type of the values.</typeparam> 
        /// <param name="propertySelector">The property to be tested.</param> 
        /// <param name="values">The accepted values of the property.</param> 
        /// <returns>A LINQ expression.</returns>
        private static Expression<Func<TElement, bool>> GetWhereNotInExpression<TElement, TValue>(Expression<Func<TElement, TValue>> propertySelector, IEnumerable<TValue> values)
        {
            ParameterExpression p = propertySelector.Parameters.Single();

            if (!values.Any())
            {
                return e => true;
            }

            var unequals = values.Select(value => (Expression)Expression.NotEqual(propertySelector.Body, Expression.Constant(value, typeof(TValue))));

            var body = unequals.Aggregate<Expression>((accumulate, unequal) => Expression.And(accumulate, unequal));

            return Expression.Lambda<Func<TElement, bool>>(body, p);
        }
    }
}
