using System;
using System.Collections.Generic;
using System.Linq;

namespace AppPlatform.Core.EnterpriseLibrary.Extensions
{
    /// <summary>
    /// DictionaryExtensions
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Typed getter, when the key is a type of the value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dependencies"></param>
        /// <returns></returns>
        public static T Get<T>(this IDictionary<Type, object> dependencies)
        {
            if (dependencies == null)
            {
                return default(T);
            }

            if (dependencies.ContainsKey(typeof(T)))
            {
                return (T)dependencies[typeof(T)];
            }

            return default(T);
        }

        /// <summary>
        /// Typed setter, when the key is a type of the value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dependencies"></param>
        /// <param name="value"></param>
        public static void Set<T>(this IDictionary<Type, object> dependencies, T value)
        {
            dependencies.Add(typeof(T), value);
        }

        /// <summary>
        /// Factory method
        /// </summary>
        /// <returns></returns>
        public static IDictionary<Type, object> CreateTypedContainer()
        {
            return new Dictionary<Type, object>();
        }

        public static List<TKey> GetKeyByValue<TKey, TValue>(this Dictionary<TKey, TValue> dependencies, TValue value)
        {
            if (dependencies == null)
            {
                return null;
    }
            
            //var keyList = dependencies.ContainsValue(value)
            //                  ? dependencies.Where(r => r.Value.Equals(value)).Select(p => p.Key).ToList()
            //                  : null;
            var keyList = dependencies.Where(r => r.Value.Equals(value)).Select(p => p.Key).ToList();
            
            return keyList;
}
    }
}
