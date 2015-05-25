using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Xml;
using AppPlatform.Core.EnterpriseLibrary.Configuration;

namespace AppPlatform.Core.EnterpriseLibrary.Logging
{
    /// <summary>
    /// Creates a writer for a HslLogEntry
    /// </summary>
    public class DbWriterFactory
    {
        private readonly Dictionary<string, string> writers = new Dictionary<string, string>();


        public DbWriterFactory()
        {
            this.Initialize();
        }

        private void Initialize()
        {
            // Read configuration from config file
            NameValueCollection collection = ConfigurationReader.Read("categorySources");
            foreach(String key in collection.AllKeys)
            {
                writers.Add(key, collection[key]);
            }
        }

        public IDbLogWriter GetWriter(object anEntry)
        {
            var hEntry = anEntry as HslLogEntry;
            if (hEntry == null)
            {
                return new NullDbWriter();
            }

            foreach (String category in hEntry.CategoriesStrings)
            {
                if (this.writers.ContainsKey(category))
                {
                    var writerType = Type.GetType(this.writers[category]);
                    return (IDbLogWriter) Activator.CreateInstance(writerType);
                }
            }

            return new NullDbWriter();
        }
    }
}
