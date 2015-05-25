using System;
using System.Xml.Linq;

namespace AppPlatform.Core.EnterpriseLibrary.Xml
{
    /// <summary>
    /// Helper class, used to safely extract sub-XElement values of a given XElement.
    /// </summary>
    public class XElementValueExtractor
    {
        private readonly XElement sourceElement;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sourceElement">Data source</param>
        public XElementValueExtractor(XElement sourceElement)
        {
            this.sourceElement = sourceElement;
        }

        private T SafeGetValue<T>(string childElementName, Func<string, T> convertString)
        {
            var childElement = this.sourceElement.Element(childElementName);
            if (childElement == null || String.IsNullOrEmpty(childElement.Value))
            {
                return default(T);
            }

            try
            {
                return convertString(childElement.Value);
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// Get a string value.
        /// </summary>
        /// <param name="childElementName"></param>
        /// <returns></returns>
        public string GetString(string childElementName)
        {
            return this.SafeGetValue(childElementName, s => s);
        }

        /// <summary>
        /// Get an integer value.
        /// </summary>
        /// <param name="childElementName"></param>
        /// <returns></returns>
        public int GetInt(string childElementName)
        {
            return this.SafeGetValue(childElementName, s => Int32.Parse(s));
        }

        /// <summary>
        /// Get enum value.
        /// </summary>
        /// <typeparam name="T">Enum, containing the value</typeparam>
        /// <param name="childElementName"></param>
        /// <returns></returns>
        public T GetEnum<T>(string childElementName)
        {
            return this.SafeGetValue(childElementName, s => (T)Enum.Parse(typeof(T), s));
        }

        /// <summary>
        /// Get GUID Value.
        /// </summary>
        /// <param name="childElementName"></param>
        /// <returns></returns>
        public Guid GetGuid(string childElementName)
        {
            return this.SafeGetValue(childElementName, s => { try { return new Guid(s); } catch { return Guid.Empty; } });
        }

        /// <summary>
        /// Get Datetime value.
        /// </summary>
        /// <param name="childElementName"></param>
        /// <returns></returns>
        public DateTime GetDateTime(string childElementName)
        {
            return this.SafeGetValue(childElementName, s => Convert.ToDateTime(s));
        }
    }
}
