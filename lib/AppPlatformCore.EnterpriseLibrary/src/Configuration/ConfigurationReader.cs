using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using AppPlatform.Core.EnterpriseLibrary.Diagnostics;

namespace AppPlatform.Core.EnterpriseLibrary.Configuration
{
    /// <summary>
    /// Provides access to simple configuration reading
    /// </summary>
    public static class ConfigurationReader
    {
        /// <summary>
        /// Reads a string from section <para>sectionName</para> specified by <para>key</para> and returns its value.
        /// If section does not exist, returns empty string.
        /// </summary>
        /// <param name="sectionName">Section name. Section is hadled by System.Configuration.NameValueSectionHandler, System</param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static string Read(string sectionName, string keyName)
        {
            var settings = ConfigurationManager.GetSection(sectionName) as NameValueCollection;
            if (settings == null)
            {
                return String.Empty;
            }

            return settings[keyName];
        }

        /// <summary>
        /// Reads a string from section <para>sectionName</para> specified by <para>key</para> and tries to interpret it as 
        /// a boolean value.
        /// If section does not exist, or conversion to boolean is not possible, returns false.
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static bool ReadBool(string sectionName, string keyName)
        {
            var settings = ConfigurationManager.GetSection(sectionName) as NameValueCollection;
            if (settings == null)
            {
                return false;
            }

            var value = settings[keyName];
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            bool result;
            if (Boolean.TryParse(value, out result))
            {
                return result;
            }

            return false;
        }

        public static T Read<T>(string sectionName, string keyName, T defaultValue)
        {
            return Read(sectionName, keyName, defaultValue, rawValue => (T)Convert.ChangeType(rawValue, typeof(T)));
        }

        public static T Read<T>(string sectionName, string keyName, T defaultValue, Func<string, T> parsingFunction)
        {
            var settings = ConfigurationManager.GetSection(sectionName) as NameValueCollection;
            if (settings == null)
            {
                return defaultValue;
            }

            var value = settings[keyName];
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            try
            {
                return parsingFunction(value);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static NameValueCollection Read(string sectionName)
        {
            NameValueCollection settings = ConfigurationManager.GetSection(sectionName) as NameValueCollection;
            if (settings == null)
            {
                return new NameValueCollection();
            }
            return settings;
        }


        /// <summary>
        /// Reads a string from section appSettings specified by <para>key</para> and returns its value.
        /// If section does not exist, returns empty string.
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static string ReadAppSettings(string keyName)
        {
            return Read("appSettings", keyName);
        }

        /// <summary>
        /// Returns if value of appSetting with key is true.
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static bool ReadBoolAppSettings(string keyName)
        {
            string settingValue = ReadAppSettings(keyName);
            return !String.IsNullOrEmpty(settingValue) && settingValue.ToUpper().Equals("TRUE");
        }

        /// <summary>
        /// Retrieve string values for multiple keys in specified section
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="keyNames"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ReadMultipleValues(string sectionName, List<string> keyNames)
        {
            if (!Safe.HasItems(keyNames))
            {
                return null;
            }

            var settings = ConfigurationManager.GetSection(sectionName) as NameValueCollection;
            if (settings == null)
            {
                return null;
            }

            var values = new Dictionary<string, string>();

            foreach (var keyName in keyNames)
            {
                var value = settings[keyName];

                if (Safe.IsNotNull(value))
                {
                    values.Add(keyName, value);
                }
            }

            return values;
        }
    }
}
