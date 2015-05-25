using System;

namespace AppPlatform.Core.EnterpriseLibrary.Extensions
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Determines whether date is work day.
        /// </summary>
        /// <param name="instance">Instance date.</param>
        /// <returns>
        /// 	<c>true</c> if day is work ; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsWorkDay(this DateTime instance)
        {
            return instance.DayOfWeek != DayOfWeek.Saturday && instance.DayOfWeek != DayOfWeek.Sunday;
        }

        /// <summary>
        /// Returns entered time, i.e. datetTime different than min value
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static DateTime? ToEnteredDateTime(this DateTime instance)
        {
            return instance.CompareTo(DateTime.MinValue) != 0 ? instance : (DateTime?)null;
        }

        /// <summary>
        /// Returns entered time, i.e. datetTime different than min value
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static DateTime? ToEnteredDateTime(this DateTime? instance)
        {
            return instance.HasValue && instance.Value.CompareTo(DateTime.MinValue) != 0 ? instance : (DateTime?)null;
        }

        /// <summary>
        /// Checks if DateTime is entered, i.e. different than minValue
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsEntered(this DateTime instance)
        {
            return instance.CompareTo(DateTime.MinValue) != 0;
        }

        /// <summary>
        /// Checks if DateTime is entered, i.e. different than minValue
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsEntered(this DateTime? instance)
        {
            return instance.HasValue && instance.Value.IsEntered();
        }

        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="value">The value.</param>
        /// <returns>0 - (equals or both null), -1 - instance is earlier than value, or instance is null, value not null, 1 - instance is later than value, or instance is not null, value is null</returns>
        public static int CompareTo(this DateTime? instance, DateTime? value)
        {
            if (instance == null)
            {
                if (value == null)
                {
                    return 0;
                }
                return -1;
            }
            
            if (value == null)
            {
                return 1;
            }

            return instance.Value.CompareTo(value.Value);
        }
    }
}
