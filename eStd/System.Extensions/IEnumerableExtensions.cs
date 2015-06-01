using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace System.Extensions
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Applies the action to each element in the list.
        /// </summary>
        /// <typeparam name="T">The enumerable item's type.</typeparam>
        /// <param name="enumerable">The elements to enumerate.</param>
        /// <param name="action">The action to apply to each item in the list.</param>
        public static void Apply<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
                action(item);
        }

        /// <summary>
        /// Toes the binding list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns></returns>
        public static BindingList<T> ToBindingList<T>(this IEnumerable<T> enumerable)
        {
            var bl = new BindingList<T>();
            if (enumerable != null && enumerable.Any())
            {
                enumerable.ToList().ForEach(i => bl.Add(i));
            }
            return bl;
        }

    }
}