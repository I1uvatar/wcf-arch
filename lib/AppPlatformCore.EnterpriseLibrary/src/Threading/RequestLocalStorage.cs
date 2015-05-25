using System;
using System.Runtime.Remoting.Messaging;
using System.Web;

namespace AppPlatform.Core.EnterpriseLibrary.Threading
{
    /// <summary>
    /// Defines interface for storing "per request" data
    /// </summary>
    public interface IThreadLocalStorage
    {
        /// <summary>
        /// Save
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Save(string key, object value);

        /// <summary>
        /// Get by key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get<T>(string key) where T : class;
    }


    /// <summary>
    /// Implements IThreadLocalStorage using HttpContext
    /// </summary>
    public class WebThreadLocal : IThreadLocalStorage
    {
        public void Save(string key, object value)
        {
            HttpContext.Current.Items[key] = value;
        }

        public T Get<T>(string key) where T : class
        {
            if (HttpContext.Current.Items.Contains(key))
            {
                return (T)HttpContext.Current.Items[key];
            }
            return null;
        }
    }


    /// <summary>
    /// Implements IThreadLocalStorage using CallContext
    /// </summary>
    public class CallContextThreadLocal : IThreadLocalStorage
    {
        public void Save(string key, object value)
        {
            CallContext.SetData(key, value);
        }

        public T Get<T>(string key) where T : class
        {
            return (T)CallContext.GetData(key);
        }
    }


    /// <summary>
    /// Provides "per request" storage of data
    /// </summary>
    public static class RequestLocalStorage
    {
        private static readonly IThreadLocalStorage storage;


        static RequestLocalStorage()
        {
            if (HttpContext.Current != null)
            {
                storage = new WebThreadLocal();
            }
            else
            {
                storage = new CallContextThreadLocal();
            }
        }

        /// <summary>
        /// Save
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Save(string key, object value)
        {
            storage.Save(key, value);
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(string key) where T : class
        {
            return storage.Get<T>(key);
        }
    }
}
