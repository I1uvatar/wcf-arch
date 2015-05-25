using System;

namespace AppPlatform.Core.EnterpriseLibrary.Configuration
{
    /// <summary>
    /// General purpose configuration setting constants
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConfigurationSetting<T>
    {
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        public ConfigurationSetting(string name, T defaultValue)
        {
            this.Key = name;     
            this.Default = defaultValue;
        }

        /// <summary>
        /// Configuration key
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Default value
        /// </summary>
        public T Default { get; private set; }
    }
}
