// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* TimeSpanUtility.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Globalization;

using JetBrains.Annotations;

#endregion

namespace AM
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public static class TimeSpanUtility
    {
        #region Properties

        /// <summary>
        /// One day.
        /// </summary>
        public static TimeSpan OneDay => new TimeSpan(1, 0, 0, 0);

        /// <summary>
        /// One hour.
        /// </summary>
        public static TimeSpan OneHour => new TimeSpan(1, 0, 0);

        /// <summary>
        /// One minute.
        /// </summary>
        public static TimeSpan OneMinute => new TimeSpan(0, 1, 0);

        /// <summary>
        /// One second.
        /// </summary>
        public static TimeSpan OneSecond => new TimeSpan(0, 0, 1);

        #endregion

        #region Private members

        private static IFormatProvider FormatProvider => CultureInfo.InvariantCulture;

        #endregion

        #region Public methods

        /// <summary>
        /// Is zero-length time span?
        /// </summary>
        public static bool IsZero(this TimeSpan timeSpan)
            => TimeSpan.Compare(timeSpan, TimeSpan.Zero) == 0;

        /// <summary>
        /// Is zero-length or less?
        /// </summary>
        public static bool IsZeroOrLess(this TimeSpan timeSpan)
            => TimeSpan.Compare(timeSpan, TimeSpan.Zero) <= 0;

        /// <summary>
        /// Is length of the time span less than zero?
        /// </summary>
        public static bool LessThanZero(this TimeSpan timeSpan)
            => TimeSpan.Compare(timeSpan, TimeSpan.Zero) < 0;

        /// <summary>
        /// Converts time span to string
        /// automatically selecting format
        /// according duration of the span.
        /// </summary>
        public static string ToAutoString
            (
                this TimeSpan span
            )
        {
            if (span >= OneDay)
            {
                return span.ToDayString();
            }

            if (span >= OneHour)
            {
                return span.ToHourString();
            }

            if (span >= OneMinute)
            {
                return span.ToMinuteString();
            }

            return span.ToSecondString();
        }

        /// <summary>
        /// Converts time span using format 'dd:hh:mm:ss'
        /// </summary>
        public static string ToDayString
            (
                this TimeSpan span
            )
        {
            return string.Format
                (
                    FormatProvider,
                    "{0:00} d {1:00} h {2:00} m {3:00} s",
                    span.Days,
                    span.Hours,
                    span.Minutes,
                    span.Seconds
                );
        }

        /// <summary>
        /// Converts time span using format 'hh:mm:ss'
        /// </summary>
        public static string ToHourString
            (
                this TimeSpan span
            )
        {
            return string.Format
                (
                    FormatProvider,
                    "{0:00}:{1:00}:{2:00}",
                    span.Hours + span.Days * 60,
                    span.Minutes,
                    span.Seconds
                );
        }

        /// <summary>
        /// Converts time span using format 'mm:ss'
        /// </summary>
        public static string ToMinuteString
            (
                this TimeSpan span
            )
        {
            var totalMinutes = span.TotalMinutes;
            var minutes = (int)totalMinutes;
            var seconds = (int)((totalMinutes - minutes) * 60.0);

            return string.Format
                (
                    FormatProvider,
                    "{0:00}:{1:00}",
                    minutes,
                    seconds
                );
        }

        /// <summary>
        /// Converts time span using format 's.ff'
        /// </summary>
        [NotNull]
        public static string ToSecondString
            (
                this TimeSpan span
            )
        {
            return span.TotalSeconds.ToString
                (
                    "F2",
                    FormatProvider
                );
        }

        /// <summary>
        /// Converts time span using format 's'
        /// </summary>
        [NotNull]
        public static string ToWholeSecondsString
            (
                this TimeSpan span
            )
        {
            return span.TotalSeconds.ToString
                (
                    "F0",
                    FormatProvider
                );
        }

        #endregion
    }
}
