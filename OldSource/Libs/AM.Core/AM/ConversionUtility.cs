// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ConversionUtility.cs -- set of type conversion routines.
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.ComponentModel;

using AM.Logging;

using JetBrains.Annotations;

#endregion

namespace AM
{
    /// <summary>
    /// Type conversion helpers.
    /// </summary>
    public static class ConversionUtility
    {
        #region Public methods

        /// <summary>
        /// Determines whether given value can be converted to
        /// the specified type.
        /// </summary>
        /// <param name="value">Value to be converted.</param>
        /// <returns>
        /// <c>true</c> if value can be converted;
        /// otherwise, <c>false</c>.
        /// </returns>
        public static bool CanConvertTo<T>
            (
                [CanBeNull] object value
            )
        {
            if (value != null)
            {
                Type sourceType = value.GetType();
                Type targetType = typeof(T);

                if (ReferenceEquals(targetType, sourceType))
                {
                    return true;
                }

                if (targetType.IsAssignableFrom(sourceType))
                {
                    return true;
                }

                IConvertible convertible = value as IConvertible;
                if (!ReferenceEquals(convertible, null))
                {
                    return true; // ???
                }

                TypeConverter converterFrom = TypeDescriptor.GetConverter(value);
                if (converterFrom.CanConvertTo(targetType))
                {
                    return true;
                }

                TypeConverter converterTo = TypeDescriptor.GetConverter(targetType);
                if (converterTo.CanConvertFrom(sourceType))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Converts given value to the specified type.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <returns>Converted value.</returns>
        public static T ConvertTo<T>
            (
                [CanBeNull] object value
            )
        {
            if (ReferenceEquals(value, null))
            {
                return default(T);
            }

            Type sourceType = value.GetType();
            Type targetType = typeof(T);

            if (targetType == typeof(string))
            {
                return (T)(object)value.ToString();
            }

            if (targetType.IsAssignableFrom(sourceType))
            {
                return (T)value;
            }

            if (value is IConvertible)
            {
                return (T)Convert.ChangeType(value, targetType);
            }

            TypeConverter converterFrom = TypeDescriptor.GetConverter(value);
            if (!ReferenceEquals(converterFrom, null)
                && converterFrom.CanConvertTo(targetType))
            {
                return (T)converterFrom.ConvertTo
                            (
                                value,
                                targetType
                            );
            }

            TypeConverter converterTo = TypeDescriptor.GetConverter(targetType);
            if (!ReferenceEquals(converterTo, null)
                && converterTo.CanConvertFrom(sourceType))
            {
                return (T)converterTo.ConvertFrom(value);
            }

            throw new ArsMagnaException();
        }

        /// <summary>
        /// Converts given object to boolean value.
        /// </summary>
        /// <param name="value">Object to be converted.</param>
        /// <returns>Converted value.</returns>
        /// <exception cref="FormatException">
        /// Value can't be converted.
        /// </exception>
        public static bool ToBoolean
            (
                [NotNull] object value
            )
        {
            Sure.NotNull(value, "value");

            if (value is bool)
            {
                return (bool)value;
            }

            try
            {
                bool result = bool.Parse(value as string);

                return result;
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
                // Pass through
            }

            string svalue = value as string;
            if (!ReferenceEquals(svalue, null))
            {
                svalue = svalue.ToLowerInvariant();

                if (svalue == "false"
                    || svalue == "0"
                    || svalue == "no"
                    || svalue == "n"
                    || svalue == "off"
                    || svalue == "negative"
                    || svalue == "neg"
                    || svalue == "disabled"
                    || svalue == "incorrect"
                    || svalue == "wrong"
                    || svalue == "нет"
                )
                {
                    return false;
                }

                if (svalue == "true"
                    || svalue == "1"
                    || svalue == "yes"
                    || svalue == "y"
                    || svalue == "on"
                    || svalue == "positiva"
                    || svalue == "pos"
                    || svalue == "enabled"
                    || svalue == "correct"
                    || svalue == "right"
                    || svalue == "да"
                )
                {
                    return true;
                }
            }

            if (value is IConvertible)
            {
                return Convert.ToBoolean(value);
            }

            TypeConverter converterFrom = TypeDescriptor.GetConverter(value);
            if (!ReferenceEquals(converterFrom, null)
                && converterFrom.CanConvertTo(typeof(bool)))
            {
                return (bool)converterFrom.ConvertTo
                    (
                        value,
                        typeof(bool)
                    );
            }

            Log.Error
                (
                    nameof(ConversionUtility) + "::" + nameof(ToBoolean)
                    + "bad value="
                    + value
                );

            throw new FormatException
                (
                    "Bad value " + value
                );
        }

        #endregion
    }
}
