using System;
using System.Collections.Generic;
using HermesSoftLab.EnterpriseLibrary.DatabaseUtility;

namespace HermesSoftLab.EnterpriseLibrary.DatabaseUtility
{
    public class DatabaseListenersPool : IDisposable
    {
        private DatabaseListenersPool(){}

        private static List<IDatabaseListener> pool = new List<IDatabaseListener>();

        public void AddListener(IDatabaseListener listener)
        {
            pool.Add(listener);
        }

        public static void Clear()
        {
            foreach (IDatabaseListener listener in pool)
            {
                listener.Dispose();
            }

            pool.Clear();
        }

        #region IDisposable Members

        public void Dispose()
        {
            Clear();
        }

        #endregion
    }
}