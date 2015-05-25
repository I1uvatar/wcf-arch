using System;

namespace AppPlatform.Core.EnterpriseLibrary.Extensions.DateTimeExtensions
{
    /// <summary>
    /// DateTime extensions.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Safe cast to universal time.
        /// </summary>
        /// <param name="dateTime">Nullable DateTime value.</param>
        /// <returns>Universal time value.</returns>
        public static DateTime? ToUniversalTime(this DateTime? dateTime)
        {
            return dateTime.HasValue
                       ? dateTime.Value.ToUniversalTime()
                       : (DateTime?)null;
        }

        /// <summary>
        /// Safe cast to local time.
        /// </summary>
        /// <param name="dateTime">Nullable DateTime value.</param>
        /// <returns>Local time value.</returns>
        public static DateTime? ToLocalTime(this DateTime? dateTime)
        {
            return dateTime.HasValue
                       ? dateTime.Value.ToLocalTime()
                       : (DateTime?)null;
        }

        /// <summary>
        /// Get the first day of the month in local DateTimeKind for any full date submitted
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>First day of the month for instaced date.</returns>
        public static DateTime ToFirstDayOfMonth(this DateTime dateTime)
        {
            //if (!dateTime.HasValue)
            //{
            //    return null;
            //}
            var dt = dateTime;
            dt = dt.AddDays(-(dt.Day - 1));

            return new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0, DateTimeKind.Local);
        }

        /// <summary>
        /// Gets the last day of the month in local DateTimeKind for any full date.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>Last day of the month for instaced date.</returns>
        public static DateTime ToLastDayOfMonth(this DateTime dateTime)
        {
            //if (!dateTime.HasValue)
            //{
            //    return null;
            //}
            var dt = dateTime;
            dt = dt.AddMonths(1);
            dt = dt.AddDays(-(dt.Day));

            return new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0, DateTimeKind.Local).AddDays(1).AddTicks(-1);
        }

        public static DateTime ToFirstDayOfYear(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, 1, 1, 0, 0, 0, DateTimeKind.Local);
        }


        public static DateTime ToLastDayOfYear(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year + 1, 1, 1, 0, 0, 0, DateTimeKind.Local).AddTicks(-1);
        }

        /// <summary>
        /// Get end of day for specified dateTime
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime EndOfDay(this DateTime dateTime)
        {
            var dt = dateTime;
            dt = dt.AddDays(1);
            return new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0, DateTimeKind.Local).AddTicks(-1);
        }

        /// <summary>
        /// Get end of day for specified dateTime
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime? EndOfDay(this DateTime? dateTime)
        {
            return dateTime != null ? (DateTime?)dateTime.Value.EndOfDay() : null;
        }

        /// <summary>
        /// Returns date for the specified datetime or null if datetime is null
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        public static DateTime? DateOrNull(this DateTime? dateTime)
        {
            return dateTime != null ? (DateTime?)dateTime.Value.Date : null;
        }

        /// <summary>
        /// Get years between dates
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public static int? GetYearDiff(this DateTime dateTime, DateTime? dateTo)
        {
            if (!dateTo.HasValue)
            {
                return null;
            }

            var dayOfYearFrom = dateTime.DayOfYear;
            var dayOfYearTo = dateTo.Value.DayOfYear;
            var refYear = dateTo.Value.Year;

            return dayOfYearTo >= dayOfYearFrom ?
                (refYear - dateTime.Year) :
                (refYear - dateTime.Year - 1);
        }

        /// <summary>
        /// Get years between dates
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public static int? GetYearDiff(this DateTime? dateTime, DateTime? dateTo)
        {
            if (!dateTime.HasValue
                || !dateTo.HasValue)
            {
                return null;
            }

            return dateTime.Value.GetYearDiff(dateTo);
        }

        /// <summary>
        /// Determinate if instance is inside of interval.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="intervalFrom">Interval from.</param>
        /// <param name="intervalTo">Interval to.</param>
        /// <returns>If instance is inside of interval return TRUE, otherwise return FALSE.</returns>
        public static bool InsideOfInterval(this DateTime instance, DateTime intervalFrom, DateTime intervalTo)
        {
            return instance >= intervalFrom && instance <= intervalTo;
        }
    }

    public static class DateTimeHelper
    {

        /// <summary>
        /// Concats the specified date and time value. Return new DateTime with DateTimKind same as in object <see cref="timeValue"/>.
        /// </summary>
        /// <param name="dateValue">Date value object.</param>
        /// <param name="timeValue">Time value object.</param>
        /// <returns>DateTime object with DateTimKind same as in object <see cref="timeValue"/>.</returns>
        public static DateTime Concat(DateTime dateValue, DateTime timeValue)
        {
            return Concat(dateValue, timeValue, timeValue.Kind);
            //return new DateTime(
            //    dateValue.Year,
            //    dateValue.Month,
            //    dateValue.Day,
            //    timeValue.Hour,
            //    timeValue.Minute,
            //    timeValue.Second,
            //    timeValue.Millisecond,
            //    timeValue.Kind
            //    );
        }

        /// <summary>
        /// Concats the specified date and time value.
        /// </summary>
        /// <param name="dateValue">Date value object.</param>
        /// <param name="timeValue">Time value object.</param>
        /// <param name="kind">New DateTime object kind.</param>
        /// <returns>Concatenated DateTime object.</returns>
        public static DateTime Concat(DateTime dateValue, DateTime timeValue, DateTimeKind kind)
        {
            return new DateTime(
                dateValue.Year,
                dateValue.Month,
                dateValue.Day,
                timeValue.Hour,
                timeValue.Minute,
                timeValue.Second,
                timeValue.Millisecond,
                kind
                );
        }
    }
}