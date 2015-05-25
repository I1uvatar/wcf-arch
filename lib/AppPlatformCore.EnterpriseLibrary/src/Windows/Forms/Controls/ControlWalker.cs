using System;
using System.Collections.Generic;
using System.Collections;
using System.Windows.Forms;

namespace AppPlatform.Core.EnterpriseLibrary.Windows.Forms.Controls
{
    /// <summary>
    /// This is a helper class, which can be used to traverse the controls and execute actions on them.
    /// </summary>
    public static class ControlWalker
    {
        private static Predicate<T> AlwaysTrue<T>() { return delegate { return true; }; }

        /// <summary>
        /// Given an enumerable collection, returns an enumerator for all elements of type <para>T</para> in the collection.
        /// </summary>
        /// <typeparam name="T">Type of the elements to be filtered out</typeparam>
        /// <param name="controls">Collection of objects to be traversed</param>
        /// <returns>Enumerator of elements of type <para>T</para></returns>
        public static IEnumerable<T> Filter<T>(IEnumerable controls) where T : class
        {
            return Filter(controls, AlwaysTrue<T>());
        }

        /// <summary>
        /// Given an enumerable collection, returns an enumerator for all elements of type <para>T</para> in the collection,
        /// matching the condition expressed in <para>criteria</para>.
        /// </summary>
        /// <typeparam name="T">Type of the elements to be filtered out</typeparam>
        /// <param name="controls">Collection of objects to be traversed</param>
        /// <param name="criteria">Additional criteria that must be met by an element from <para>controls</para></param>
        /// <returns>Enumerator of elements of type <para>T</para></returns>
        public static IEnumerable<T> Filter<T>(IEnumerable controls, Predicate<T> criteria) where T : class
        {
            foreach (Control c in controls)
            {
                T target = c as T;
                if (target != null && criteria(target))
                {
                    yield return target;
                }
            }
        }

        /// <summary>
        /// Given an enumerable collection, the first element type <para>T</para> from the collection,
        /// matching the condition expressed in <para>criteria</para>.
        /// </summary>
        /// <typeparam name="T">Type of the element to be filtered out</typeparam>
        /// <param name="controls">Collection of objects to be traversed</param>
        /// <param name="criteria">Additional criteria that must be met by an element from <para>controls</para></param>
        /// <returns>First matching element</returns>
        public static T GetFirst<T>(IEnumerable controls, Predicate<T> criteria) where T : class
        {
            foreach (Control c in controls)
            {
                T target = c as T;
                if (target != null && criteria(target))
                {
                    return target;
                }
            }

            return default(T);
        }

        /// <summary>
        /// Given an enumerable collection, traverses all elements of type <para>T</para> and executes
        /// the action <para>execute</para> on them.
        /// </summary>
        /// <typeparam name="T">Type of the elements to be filtered out</typeparam>
        /// <param name="controls">Collection of objects to be traversed</param>
        /// <param name="execute">Action to be executed on every element of type <para>T</para>from collection</param>
        public static void Traverse<T>(IEnumerable controls, Action<T> execute) where T : class
        {
            Traverse(controls, AlwaysTrue<T>(), execute);
        }

        /// <summary>
        /// Given an enumerable collection, traverses all elements of type <para>T</para> and executes
        /// the action <para>execute</para> on every element matching the <para>criteria</para>.
        /// </summary>
        /// <typeparam name="T">Type of the elements to be filtered out</typeparam>
        /// <param name="controls">Collection of objects to be traversed</param>
        /// <param name="criteria">Filter for selecting the elements being processed</param>
        /// <param name="execute">Action to be executed on every element of type <para>T</para>from collection</param>
        public static void Traverse<T>(IEnumerable controls, Predicate<T> criteria, Action<T> execute) where T : class
        {
            foreach (T control in Filter<T>(controls))
            {
                if (criteria(control))
                {
                    execute(control);
                }
            }
        }

        /// <summary>
        /// TraverseCheck
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controls"></param>
        /// <param name="checkAction"></param>
        /// <returns></returns>
        public static bool TraverseCheck<T>(IEnumerable controls, Predicate<T> checkAction) where T : class
        {
            bool check = true;

            foreach (T control in Filter<T>(controls))
            {
                check &= checkAction(control);
            }

            return check;
        }

        /// <summary>
        /// Given an enumerable collection <para>controls</para>, traverses all elements, their children 
        /// <para>children</para> and so forth recursively. 
        /// On every element matching the <para>criteria</para> the action <para>execute</para> is executed.
        /// </summary>
        /// <typeparam name="T">Type of the elements to be filtered out</typeparam>
        /// <param name="controls">Collection of objects to be traversed</param>
        /// <param name="children">Collection of child objects to be traversed</param>
        /// <param name="criteria">Filter for selecting the elements being processed</param>
        /// <param name="execute">Action to be executed on every element of type <para>T</para>from collection</param>
        public static void DeepTraverse<T>(IEnumerable controls, Func<T, IEnumerable> children, Predicate<T> criteria, Action<T> execute) where T : class
        {
            foreach (T control in controls)
            {
                if (criteria(control))
                {
                    execute(control);
                }
                DeepTraverse<T>(children(control), children, criteria, execute);
            }
        }
    }
}