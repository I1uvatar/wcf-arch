using Microsoft.Practices.EnterpriseLibrary.Caching;
using System;
using System.Collections.Generic;

namespace AppPlatform.Core.EnterpriseLibrary.DatabaseUtility
{
    public class MultiDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, List<TValue>> dict = new Dictionary<TKey, List<TValue>>();
        private static object locked = new object();

        public void Add(TKey key, TValue value)
        {
            lock (locked)
            {
                List<TValue> list;
                if (dict.TryGetValue(key, out list))
                {
                    list.Add(value);
                }
                else
                {
                    list = new List<TValue> {value};
                    dict.Add(key, list);
                }
            }
        }

        public bool ContainsKey(TKey key)
        {
            return dict.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get
            {
                return dict.Keys;
            }
        }

        public bool Remove(TKey key)
        {
            lock (locked)
            {
                return dict.Remove(key);
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                List<TValue> values = new List<TValue>();

                Dictionary<TKey, List<TValue>>.Enumerator enumerator = dict.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    values.AddRange(enumerator.Current.Value);
                }
                return values;
            }
        }


        bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
        {
            throw new NotSupportedException("TryGetValue is not supported");
        }

        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get
            {
                throw new NotSupportedException(
                      "accessing elements by key is not supported");
            }
            set
            {
                throw new NotSupportedException(
                      "accessing elements by key is not supported");
            }
        }

        public IList<TValue> this[TKey key]
        {
            get
            {
                return dict[key];
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            dict.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            List<TValue> list;
            if (!dict.TryGetValue(item.Key, out list))
            {
                return false;
            }
            else
            {
                return list.Contains(item.Value);
            }
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {

            if (array == null)
                throw new ArgumentNullException("array");
            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException("array index out of range");
            if (array.Length - arrayIndex < this.Count)
                throw new ArgumentException("Array too small");

            Dictionary<TKey, List<TValue>>.Enumerator enumerator = dict.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<TKey, List<TValue>> mapPair = enumerator.Current;
                foreach (TValue val in mapPair.Value)
                {
                    array[arrayIndex++] = new KeyValuePair<TKey, TValue>(mapPair.Key, val);
                }
            }
        }
        public int Count
        {
            get
            {
                int count = 0;

                Dictionary<TKey, List<TValue>>.Enumerator enumerator = dict.GetEnumerator();
                while (enumerator.MoveNext())
                {

                    KeyValuePair<TKey, List<TValue>> pair = enumerator.Current;
                    count += pair.Value.Count;
                }
                return count;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            List<TValue> list;
            if (dict.TryGetValue(item.Key, out list))
            {
                return list.Remove(item.Value);
            }
            else
            {
                return false;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            Dictionary<TKey, List<TValue>>.Enumerator enumerateKeys = dict.GetEnumerator();
            while (enumerateKeys.MoveNext())
            {
                foreach (TValue val in enumerateKeys.Current.Value)
                {
                    KeyValuePair<TKey, TValue> pair = new KeyValuePair<TKey, TValue>(
                       enumerateKeys.Current.Key, val);
                    yield return pair;
                }
            }
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

     
    
}
