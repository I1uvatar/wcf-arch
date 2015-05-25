using System;
using System.Linq;
using AppPlatform.Core.EnterpriseLibrary.Extensions;

namespace AppPlatform.Core.EnterpriseLibrary.Logging
{
    /// <summary>
    /// Selects messages to log
    /// </summary>
    public class LoggingFilter
    {
        private readonly bool isTurnedOn;
        private readonly string[] excludeMethods;
        private readonly string[] excludeStartsWithMethods;
        private readonly string[] excludeEndsWithMethods;
        private readonly string[] includeHosts;
        private readonly bool useLog4Net;

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="isTurnedOn">if set to <c>true</c> [is turned on].</param>
        /// <param name="excludeMethods">The exclude methods.</param>
        /// <param name="excludeStartsWithMethods">The exclude starts with methods.</param>
        /// <param name="excludeEndsWithMethods">The exclude ends with methods.</param>
        /// <param name="includeHosts">The include hosts.</param>
        /// <param name="useLog4Net">if set to <c>true</c> [use log4 net].</param>
        public LoggingFilter(bool isTurnedOn, string excludeMethods, string excludeStartsWithMethods, string excludeEndsWithMethods, string includeHosts, bool useLog4Net)
        {
            this.isTurnedOn = isTurnedOn;
            this.excludeMethods = ParseList(excludeMethods);
            this.excludeStartsWithMethods = ParseList(excludeStartsWithMethods);
            this.excludeEndsWithMethods = ParseList(excludeEndsWithMethods);
            this.includeHosts = ParseList(includeHosts);
            this.useLog4Net = useLog4Net;
        }

        private static string[] ParseList(string commaSeparatedStringList)
        {
            if (string.IsNullOrEmpty(commaSeparatedStringList))
            {
                return new string[0];
            }

            return commaSeparatedStringList
                .Split(',')
                .Select(s => s.Trim())
                .ToArray();
        }

        /// <summary>
        /// Is logging turned on at all?
        /// </summary>
        /// <returns></returns>
        public bool ShallLog()
        {
            return this.isTurnedOn;
        }

        /// <summary>
        /// Shalls the log4 net.
        /// </summary>
        /// <returns></returns>
        public bool UseLog4Net()
        {
            return this.useLog4Net;
        }

        /// <summary>
        /// Decides whether to log or not based on method name
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="hostName"></param>
        /// <returns></returns>
        public bool ShallLog(string methodName, string hostName)
        {
            if (!this.isTurnedOn)
            {
                return false;
            }

            if (!this.includeHosts.IsEmpty() && !String.IsNullOrEmpty(hostName))
            {
                if (!this.includeHosts.Exists<string>(host => host.Equals(hostName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return false;
                }
            }

            if (this.excludeStartsWithMethods.Where(methodName.StartsWith).Count() > 0)
            {
                return false;
            }

            if (this.excludeMethods.Contains(methodName))
            {
                return false;
            }

            if (this.excludeEndsWithMethods.Where(methodName.EndsWith).Count() > 0)
            {
                return false;
            }

            return true;
        }
    }
}
