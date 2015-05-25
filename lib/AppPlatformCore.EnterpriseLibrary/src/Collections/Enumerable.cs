using System;
using System.Collections;
using System.Collections.Generic;

namespace AppPlatform.Core.EnterpriseLibrary.Collections
{
    /// <summary>
    /// Enumerable helper class
    /// </summary>
    public static class Enumerable
    {
        /// <summary>
        /// Enumerate throught collection and returns typed collection enumeration
        /// </summary>
        /// <typeparam name="T">Collection ellement type</typeparam>
        /// <param name="collection">Colection to enumerate</param>
        /// <returns></returns>
        public static IEnumerable<T> Enumerate<T>(this IEnumerable collection)
        {
            foreach (var o in collection)
            {
                yield return (T)o;
            }
        }
    }
}
