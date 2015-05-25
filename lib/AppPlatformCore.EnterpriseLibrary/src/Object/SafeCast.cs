using System;
using System.Globalization;

namespace AppPlatform.Core.EnterpriseLibrary.Object
{
    public class SafeCast
    {
        #region Object to Long
        
        /// <summary>
        /// Cast object to long. In case of error returns 0 or throws 
        /// exception if throwException is true.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="throwException"></param>
        /// <returns></returns>
        public static long GetLong(object value, bool throwException)
        {
            if (value == null || value == DBNull.Value)
                return 0;

            try
            {
                return (long)value;
            }
            catch (InvalidCastException)
            {
                // TODO - put some logging here
                if (throwException)
                    throw;
            }

            return 0;
        }

        /// <summary>
        /// Cast object to long or null. In case of error returns null or throws 
        /// exception if throwException is true.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="throwException"></param>
        /// <returns></returns>
        public static long? GetLongOrNull(object value, bool throwException)
        {
            if (value == null || value == DBNull.Value)
                return null;

            try
            {
                return (long)value;
            }
            catch (InvalidCastException)
            {
                // TODO - put some logging here
                if (throwException)
                    throw;
            }

            return null;
        }

        /// <summary>
        /// Cast object to long or null. In case of error returns false;
        /// </summary>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <returns>Cast successful.</returns>
        public static bool GetLongOrNull(object value, out long? result)
        {
            result = null;

            if (value == null || value == DBNull.Value)
                return true;

            try
            {
                result = (long)value;
            }
            catch (InvalidCastException)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Object to Int32

        /// <summary>
        /// Cast object to long. In case of error returns 0 or throws 
        /// exception if throwException is true.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="throwException"></param>
        /// <returns></returns>
        public static int GetInt(object value, bool throwException)
        {
            if (value == null || value == DBNull.Value)
                return 0;

            try
            {
                return (int)value;
            }
            catch (InvalidCastException)
            {
                // TODO - put some logging here
                if (throwException)
                    throw;
            }

            return 0;
        }

        /// <summary>
        /// Cast object to long or null. In case of error returns null or throws 
        /// exception if throwException is true.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="throwException"></param>
        /// <returns></returns>
        public static int? GetIntOrNull(object value, bool throwException)
        {
            if (value == null || value == DBNull.Value)
                return null;

            try
            {
                return (int)value;
            }
            catch (InvalidCastException)
            {
                // TODO - put some logging here
                if (throwException)
                    throw;
            }

            return null;
        }

        #endregion

        #region String to Long
        
        /// <summary>
        /// Safe casting of string to long
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long GetLong(string value)
        {
            if (value == null)
                return 0;

            long result;
            if (long.TryParse(value, out result))
            {
                return result;
            }

            return 0;
        }

        /// <summary>
        /// Safe casting of string to long or null
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long? GetLongOrNull(string value)
        {
            if (value == null)
                return null;

            long result;
            if (long.TryParse(value, out result))
            {
                return result;
            }

            return null;
        } 
        
        #endregion

        #region String to Int32
        
        /// <summary>
        /// Safe casting of string to int
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetInt(string value)
        {
            if (value == null)
                return 0;

            int result;
            if (int.TryParse(value, out result))
            {
                return result;
            }

            return 0;
        }

        /// <summary>
        /// Safe casting of string to int or null
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int? GetIntOrNull(string value)
        {
            if (value == null)
                return null;

            int result;
            if (int.TryParse(value, out result))
            {
                return result;
            }

            return null;
        } 
        
        #endregion

        #region Long to String

        public static string GetStringOrNull(long? value)
        {
            return value.HasValue ? value.Value.ToString() : null;
        } 
        
        #endregion

        #region String to DateTime

        /// <summary>
        /// Returns datetime.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        /// <param name="throwException"></param>
        /// <returns></returns>
        public static DateTime GetDateTime(string value, string format, bool throwException)
        {
            DateTime date = DateTime.MinValue;
            try
            {
                date = DateTime.ParseExact(value, format, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                // TODO - put some logging here
                if (throwException)
                {
                    throw;
                }
            }
            return date;
        } 
        
        /// <summary>
        /// Returns datetime or null.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        /// <param name="throwException"></param>
        /// <returns></returns>
        public static DateTime? GetDateTimeOrNull(string value, string format, bool throwException)
        {
            try
            {
                return DateTime.ParseExact(value, format, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                // TODO - put some logging here     
                if (throwException)
                {
                    throw;
                }
            }
            return null;
        }

        #endregion

        #region String conversions
        
        /// <summary>
        /// To string conversion.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetString(object value)
        {
            if (value == null || value == DBNull.Value)
                return null; 
            
            return value.ToString();
        }

        /// <summary>
        /// To string conversion. Returns "" if value is null.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetStringOrEmpty(object value)
        {
            if (value == null)
                return String.Empty;

            return GetString(value);
        }

        /// <summary>
        /// To string conversion. Object must be string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetStringOrNull(object value)
        {
            return GetValueOrDefault<string>(value);
        }

        /// <summary>
        /// Returns null if string is empty or null.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetNullIfEmpty(string value)
        {
            if (String.IsNullOrEmpty(value))
                return null;

            return value;
        }

        /// <summary>
        /// Returns empty if string is null.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEmptyIfNull(string value)
        {
            if (value == null)
                return String.Empty;

            return value;
        }

        #endregion

        #region General

        /// <summary>
        /// Gets the value or default.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static T GetValueOrDefault<T>(object value)
        {
            if (value == null)
            {
                return default(T);
            }

            return (T)value;
        }

        #endregion
    }
}