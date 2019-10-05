// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ConfigurationUtility.cs -- some useful routines for System.Configuration
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */


#region Using directives

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using AM.Logging;
using AM.Runtime;

using JetBrains.Annotations;

using CM = System.Configuration.ConfigurationManager;

#endregion

// ReSharper disable CommentTypo

namespace AM.Configuration
{
    /// <summary>
    /// Some useful routines for System.Configuration.
    /// </summary>
    [PublicAPI]
    [ExcludeFromCodeCoverage]
    public static class ConfigurationUtility
    {
        #region Private members

        private static IFormatProvider InvariantCulture => CultureInfo.InvariantCulture;

        #endregion

        #region Public methods

        /// <summary>
        /// Application.exe.config file name with full path.
        /// </summary>
        public static string ConfigFileName => string.Concat
            (
                RuntimeUtility.ExecutableFileName,
                ".config"
            );

        /// <summary>
        /// Получаем сеттинг из возможных кандидатов.
        /// </summary>
        public static string? FindSetting
            (
                params string[] candidates
            )
        {
            foreach (string candidate in candidates)
            {
                if (!string.IsNullOrEmpty(candidate))
                {
                    var setting = CM.AppSettings[candidate];
                    if (!string.IsNullOrEmpty(setting))
                    {
                        return setting;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get boolean value from application configuration.
        /// </summary>
        public static bool GetBoolean
            (
                string key,
                bool defaultValue = false
            )
        {
            var result = defaultValue;
            var setting = CM.AppSettings[key];
            if (!string.IsNullOrEmpty(setting))
            {
                result = ConversionUtility.ToBoolean(setting);
            }

            return result;
        }

        /// <summary>
        /// Get 16-bit integer value from application configuration.
        /// </summary>
        public static short GetInt16
            (
                string key,
                short defaultValue = 0
            )
        {
            var setting = CM.AppSettings[key];
            if (!NumericUtility.TryParseInt16(setting, out var result))
            {
                result = defaultValue;
            }

            return result;
        }

        /// <summary>
        /// Get unsigned 16-bit integer.
        /// </summary>
        [CLSCompliant(false)]
        public static ushort GetUInt16
            (
                string key,
                ushort defaultValue = 0
            )
        {
            var setting = CM.AppSettings[key];
            if (!NumericUtility.TryParseUInt16(setting, out var result))
            {
                result = defaultValue;
            }

            return result;
        }

        /// <summary>
        /// Get 32-bit integer value
        /// from application configuration.
        /// </summary>
        public static int GetInt32
            (
                string key,
                int defaultValue = 0
            )
        {
            var setting = CM.AppSettings[key];
            if (!NumericUtility.TryParseInt32(setting, out var result))
            {
                result = defaultValue;
            }

            return result;
        }

        /// <summary>
        /// Get unsigned 32-bit integer value
        /// from application configuration.
        /// </summary>
        [CLSCompliant (false)]
        public static uint GetUInt32
            (
                string key,
                uint defaultValue = 0
            )
        {
            var setting = CM.AppSettings[key];
            if (!NumericUtility.TryParseUInt32(setting, out var result))
            {
                result = defaultValue;
            }

            return result;
        }

        /// <summary>
        /// Get 64-bit integer value
        /// from application configuration.
        /// </summary>
        public static long GetInt64
            (
                string key,
                long defaultValue = 0L
            )
        {
            var setting = CM.AppSettings[key];
            if (!NumericUtility.TryParseInt64(setting, out var result))
            {
                result = defaultValue;
            }

            return result;
        }

        /// <summary>
        /// Get usingned 64-bit integer value
        /// from application configuration.
        /// </summary>
        [CLSCompliant(false)]
        public static ulong GetUInt64
            (
                string key,
                ulong defaultValue
            )
        {
            var s = CM.AppSettings[key];
            if (!NumericUtility.TryParseUInt64(s, out var result))
            {
                result = defaultValue;
            }

            return result;
        }

        /// <summary>
        /// Get single-precision float value from application configuration.
        /// </summary>
        public static float GetSingle
            (
                string key,
                float defaultValue = 0.0f
            )
        {
            var setting = CM.AppSettings[key];
            if (!NumericUtility.TryParseSingle(setting, out var result))
            {
                result = defaultValue;
            }

            return result;
        }

        /// <summary>
        /// Get double-precision float value from application configuration.
        /// </summary>
        public static double GetDouble
            (
                string key,
                double defaultValue = 0.0
            )
        {
            var setting = CM.AppSettings[key];
            if (!NumericUtility.TryParseDouble(setting, out var result))
            {
                result = defaultValue;
            }

            return result;
        }

        /// <summary>
        /// Get decimal value from application configuration.
        /// </summary>
        public static decimal GetDecimal
            (
                string key,
                decimal defaultValue = 0.0m
            )
        {
            var setting = CM.AppSettings[key];
            if (!NumericUtility.TryParseDecimal(setting, out var result))
            {
                result = defaultValue;
            }

            return result;
        }

        /// <summary>
        /// Get string value from application configuration.
        /// </summary>
        public static string? GetString
            (
                string key,
                string? defaultValue = null
            )
        {
            Sure.NotNullNorEmpty(key, nameof(key));

            var result = defaultValue;
            var s = CM.AppSettings[key];

            if (!string.IsNullOrEmpty(s))
            {
                result = s;
            }

            return result;
        }

        /// <summary>
        /// Get string value from application configuration.
        /// </summary>
        public static string RequireString
            (
                string key
            )
        {
            Sure.NotNullNorEmpty(key, nameof(key));

            var result = GetString(key);
            if (ReferenceEquals(result, null))
            {
                Log.Error
                    (
                        "ConfigurationUtility::RequireString: "
                        + "key '"
                        + key
                        + "' not set"
                    );

                throw new ArgumentNullException("configuration key '" + key + "' not set");
            }

            return result;
        }

        /// <summary>
        /// Get date or time value from application configuration.
        /// </summary>
        public static DateTime GetDateTime
            (
                string key,
                DateTime defaultValue
            )
        {
            var setting = CM.AppSettings[key];
            if (!string.IsNullOrEmpty(setting))
            {
                defaultValue = DateTime.Parse(setting, InvariantCulture);
            }

            return defaultValue;
        }

        #endregion
    }
}

