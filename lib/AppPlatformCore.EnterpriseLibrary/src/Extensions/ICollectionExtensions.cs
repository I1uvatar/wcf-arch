using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using AppPlatform.Core.EnterpriseLibrary.Diagnostics;

namespace AppPlatform.Core.EnterpriseLibrary.Extensions
{
    /// <summary>
    /// Handy extension methods for ICollection
    /// </summary>
    public static class ICollectionExtensions
    {
        /// <summary>
        /// Finds out if collection is empty
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static bool IsEmpty(this ICollection collection)
        {
            if (collection == null || collection.Count == 0)
            {
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// Finds out if collection is not empty
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns>
        ///   <c>true</c> if specified collection is not empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNotEmpty(this ICollection collection)
        {
            return !IsEmpty(collection);
        }

        /// <summary>
        /// Finds out if element exists in the collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static bool Exists<T>(this ICollection collection, Func<T, bool>match)
        {
            if (collection.IsEmpty())
            {
                return false;
            }

            foreach(T element in collection)
            {
                if (match(element))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Finds out the first element in the collection.
        /// Returns null when such element does not exist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static T First<T>(this ICollection collection, Func<T, bool>match)
        {
            if (collection.IsEmpty())
            {
                return default(T);
            }

            foreach (T element in collection)
            {
                if (match(element))
                {
                    return element;
                }
            }

            return default(T);
        }

        /// <summary>
        /// Safe add item into source collection <paramref name="sourceCollection"/>.
        /// If <paramref name="sourceCollection"/> is null do not nothing.
        /// If <paramref name="item"/> is null do not add null value to source collection.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="sourceCollection">Source collection. If <paramref name="sourceCollection"/> is null do not nothing.</param>
        /// <param name="item">Item to add into source collection.</param>
        /// <returns>True if item successfully added, else false.</returns>
        public static bool AddIfNotNull<T>(this ICollection<T> sourceCollection, T item) where T : class
        {
            if (sourceCollection == null)
            {
                return false;
    }
            if (item != null)
            {
                sourceCollection.Add(item);
                return true;
}
            return false;
        }

        /// <summary>
        /// Iterates action for each element of collection.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TElem">The type to filter the elements of source.</typeparam>
        /// <param name="source">An System.Collections.Generic.IEnumerable&lt;TSource&gt; to iterate through.</param>
        /// <param name="predicate">The System.Func&lt;TElem, bool&gt; delegate that defines the conditions of the elements to search for</param>
        /// <param name="action">The System.Action&lt;TElem&gt; delegate to perform on each element of the System.Collections.Generic.IEnumerable&lt;TSource&gt;.</param>
        public static void IterateThrowList<TSource, TElem>(this ICollection<TSource> source, Func<TElem, bool> predicate, Action<TElem> action)
            where TSource : class
            where TElem : TSource
        {
            var newList = source.OfType<TElem>().Where(elem => predicate == null || predicate(elem));
            if (action != null && newList != null)
            {
                foreach (var element in newList)
                {
                    action(element);
                }
            }
        }

        /// <summary>
        /// Iterates action for each element of collection.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">An System.Collections.Generic.IEnumerable&lt;TSource&gt; to iterate through.</param>
        /// <param name="predicate">The System.Func&lt;TElem, bool&gt; delegate that defines the conditions of the elements to search for</param>
        /// <param name="action">The System.Action&lt;TElem&gt; delegate to perform on each element of the System.Collections.Generic.IEnumerable&lt;TSource&gt;.</param>
        public static void IterateThrowList<TSource>(this ICollection<TSource> source, Func<TSource, bool> predicate, Action<TSource> action)
            where TSource : class
        {
            if (source==null)
            {
                return;
            }

            var newList = source.Where(elem => predicate == null || predicate(elem));
            if (action != null && newList != null)
            {
                foreach (var element in newList)
                {
                    action(element);
                }
            }
        }

        public static string ToStringWithSeparatedValue<T>(this ICollection<T> collection, string valueSeparator, Func<T, string> objectGetter)
        {
            if (collection == null || collection.Count == 0 || objectGetter == null)
            {
                return null;
            }

            var builder = new StringBuilder();
            foreach (var obj in collection)
            {
                var stringObj = objectGetter(obj);
                if (!string.IsNullOrEmpty(stringObj))
                {
                    //TODO sta akoje valueSeparator==null? Da li ce se pojaviti exception?
                    builder.Append(stringObj).Append(valueSeparator);
                }
            }

            if (builder.Length == 0)
            {
                return null;
            }
            if (!string.IsNullOrEmpty(valueSeparator))
            {
                return builder.ToString(0, builder.Length - valueSeparator.Length);
            }
            return builder.ToString();
        }
    }
}
