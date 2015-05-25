using System;

namespace AppPlatform.Core.EnterpriseLibrary.Configuration
{
    /// <summary>
    /// Configuration setting constants for integers
    /// </summary>
    public class IntegerSetting
    {
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        public IntegerSetting(string name, int defaultValue)
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
        public int Default { get; private set; }
    }
}
