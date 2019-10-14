// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* NumericUtility.cs -- helper routines for numeric values
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using JetBrains.Annotations;

#endregion

namespace AM
{
    /// <summary>
    /// Helper methods for numeric values.
    /// </summary>
    [PublicAPI]
    public static class NumericUtility
    {
        #region Public methods

        /// <summary>
        /// Transformation of integer number set into string representation,
        /// taking into account presence of sequential number chains,
        /// which are formatted as ranges.
        /// </summary>
        /// <param name="n">Источник целых чисел.</param>
        /// <remarks>Источник должен поддерживать многократное считывание.
        /// Числа предполагаются предварительно упорядоченные. Повторения чисел
        /// не допускаются. Пропуски в последовательностях допустимы.
        /// Числа допускаются только неотрицательные.
        /// </remarks>
        /// <returns>Строковое представление набора чисел.</returns>
        public static string CompressRange
            (
                IEnumerable<int>? n
            )
        {
            // TODO rewrite without .Any()

            if (ReferenceEquals(n, null))
            {
                return string.Empty;
            }

            // ReSharper disable PossibleMultipleEnumeration
            if (!n.Any())
            {
                return string.Empty;
            }

            var result = new StringBuilder();
            var first = true;
            var previous = n.First();
            var last = previous;
            foreach (var i in n.Skip(1))
            {
                if (i != last + 1)
                {
                    result.AppendFormat("{0}{1}", first ? "" : ", ",
                        FormatRange(previous, last));
                    previous = i;
                    first = false;
                }
                last = i;
            }
            result.AppendFormat("{0}{1}", first ? "" : ", ",
                FormatRange(previous, last));

            return result.ToString();
            // ReSharper restore PossibleMultipleEnumeration
        }

        /// <summary>
        /// Convert the floating point number text representation
        /// to <see cref="CultureInfo.InvariantCulture"/>.
        /// </summary>
        public static string ConvertFloatToInvariant
            (
                string text
            )
        {
            if (text.Contains(','))
            {
                text = text.Contains('.')
                    ? text.Replace(",", string.Empty)
                    : text.Replace(',', '.');
            }

            return text;
        }

        /// <summary>
        /// Convert the integer number text representation
        /// to <see cref="CultureInfo.InvariantCulture"/>.
        /// </summary>
        public static string ConvertIntegerToInvariant
            (
                string text
            )
        {
            if (text.Contains(','))
            {
                text = text.Replace(",", string.Empty);
            }

            return text;
        }

        /// <summary>
        /// Форматирование диапазона целых чисел.
        /// </summary>
        /// <remarks>Границы диапазона могут совпадать, однако
        /// левая не должна превышать правую.</remarks>
        /// <param name="first">Левая граница диапазона.</param>
        /// <param name="last">Правая граница диапазона.</param>
        /// <returns>Строковое представление диапазона.</returns>
        public static string FormatRange
            (
                int first,
                int last
            )
        {
            if (first == last)
            {
                return first.ToInvariantString();
            }
            if (first == last - 1)
            {
                return first.ToInvariantString()
                    + ", "
                    + last.ToInvariantString();
            }

            return first.ToInvariantString()
                + "-"
                + last.ToInvariantString();
        }

        /// <summary>
        /// Представляет ли строка положительное целое число.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPositiveInteger
            (
                this string text
            )
        {
            return text.SafeToInt32() > 0;
        }

        /// <summary>
        /// One of many?
        /// </summary>
        public static bool OneOf
            (
                this int one,
                params int[] many
            )
        {
            foreach (var i in many)
            {
                if (i == one)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// One of many?
        /// </summary>
        public static bool OneOf
            (
                this int one,
                IEnumerable<int> many
            )
        {
            foreach (var i in many)
            {
                if (i == one)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Parse decimal in standard manner.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal ParseDecimal
            (
                string text
            )
        {
            Sure.NotNullNorEmpty(text, nameof(text));

            text = ConvertFloatToInvariant(text);

            var result = decimal.Parse
                (
                    text,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture
                );

            return result;
        }

        /// <summary>
        /// Parse double in standard manner.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ParseDouble
            (
                string text
            )
        {
            Sure.NotNullNorEmpty(text, nameof(text));

            text = ConvertFloatToInvariant(text);
            var result = double.Parse
                (
                    text,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture
                );
            return result;
        }

        /// <summary>
        /// Parse short integer in standard manner.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ParseInt16
            (
                string text
            )
        {
            Sure.NotNullNorEmpty(text, nameof(text));

            text = ConvertIntegerToInvariant(text);
            var result = short.Parse
                (
                    text,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture
                );
            return result;
        }

        /// <summary>
        /// Parse integer in standard manner.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ParseInt32
            (
                string text
            )
        {
            Sure.NotNullNorEmpty(text, nameof(text));

            text = ConvertIntegerToInvariant(text);
            var result = int.Parse
                (
                    text,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture
                );

            return result;
        }

        /// <summary>
        /// Parse integer in standard manner.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ParseInt32
            (
                ReadOnlyMemory<char> memory
            )
        {
            var text = ConvertIntegerToInvariant(memory.ToString());
            var result = int.Parse
                (
                    text,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture
                );
            return result;
        }

        /// <summary>
        /// Parse long integer in standard manner.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ParseInt64
            (
                string text
            )
        {
            Sure.NotNullNorEmpty(text, nameof(text));

            text = ConvertFloatToInvariant(text);
            var result = long.Parse
                (
                    text,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture
                );
            return result;
        }

        /// <summary>
        /// Безопасное преобразование строки
        /// в число с фиксированной точкой.
        /// </summary>
        public static decimal SafeToDecimal
            (
                this string? text,
                decimal defaultValue
            )
        {
            if (string.IsNullOrEmpty(text))
            {
                return defaultValue;
            }

            if (!TryParseDecimal(text, out var result))
            {
                result = defaultValue;
            }
            return result;
        }

        /// <summary>
        /// Безопасное преобразование строки
        /// в число с плавающей точкой.
        /// </summary>
        public static double SafeToDouble
            (
                this string? text,
                double defaultValue
            )
        {
            if (string.IsNullOrEmpty(text))
            {
                return defaultValue;
            }

            if (!TryParseDouble(text, out var result))
            {
                result = defaultValue;
            }
            return result;
        }

        /// <summary>
        /// Безопасное преобразование строки в целое.
        /// </summary>
        public static int SafeToInt32
            (
                this string? text,
                int defaultValue,
                int minValue,
                int maxValue
            )
        {
            if (string.IsNullOrEmpty(text))
            {
                return defaultValue;
            }

            if (!TryParseInt32(text, out var result))
            {
                result = defaultValue;
            }

            if (result < minValue
                || result > maxValue)
            {
                result = defaultValue;
            }

            return result;
        }

        /// <summary>
        /// Безопасное преобразование строки в целое.
        /// </summary>
        public static int SafeToInt32
            (
                this string? text,
                int defaultValue
            )
        {
            if (string.IsNullOrEmpty(text))
            {
                return defaultValue;
            }

            if (!TryParseInt32(text, out var result))
            {
                result = defaultValue;
            }

            return result;
        }

        /// <summary>
        /// Безопасное преобразование строки в целое.
        /// </summary>
        public static int SafeToInt32
            (
                this string? text
            )
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }

            if (!TryParseInt32(text, out var result))
            {
                result = 0;
            }

            return result;
        }

        /// <summary>
        /// Безопасное преобразование строки в целое.
        /// </summary>
        public static long SafeToInt64
            (
                this string? text
            )
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }

            if (!TryParseInt64(text, out var result))
            {
                result = 0;
            }

            return result;
        }

        /// <summary>
        /// Преобразование числа в строку по правилам инвариантной
        /// (не зависящей от региона) культуры.
        /// </summary>
        /// <param name="value">Число для преобразования.</param>
        /// <returns>Строковое представление числа.</returns>
        public static string ToInvariantString
            (
                this short value
            )
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }


        /// <summary>
        /// Преобразование числа в строку по правилам инвариантной
        /// (не зависящей от региона) культуры.
        /// </summary>
        /// <param name="value">Число для преобразования.</param>
        /// <returns>Строковое представление числа.</returns>
        public static string ToInvariantString
            (
                this int value
            )
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Преобразование числа в строку по правилам инвариантной
        /// (не зависящей от региона) культуры.
        /// </summary>
        /// <param name="value">Число для преобразования.</param>
        /// <returns>Строковое представление числа.</returns>
        public static string ToInvariantString
            (
                this double value
            )
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Convert double to string using InvariantCulture.
        /// </summary>
        public static string ToInvariantString
            (
                this double value,
                string format
            )
        {
            Sure.NotNullNorEmpty(format, nameof(format));

            return value.ToString
                (
                    format,
                    CultureInfo.InvariantCulture
                );
        }

        /// <summary>
        /// Преобразование числа в строку по правилам инвариантной
        /// (не зависящей от региона) культуры.
        /// </summary>
        /// <param name="value">Число для преобразования.</param>
        /// <returns>Строковое представление числа.</returns>
        public static string ToInvariantString
            (
                this decimal value
            )
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Convert decimal value to string using InvariantCulture.
        /// </summary>
        public static string ToInvariantString
            (
                this decimal value,
                string format
            )
        {
            Sure.NotNullNorEmpty(format, nameof(format));

            return value.ToString
                (
                    format,
                    CultureInfo.InvariantCulture
                );
        }

        /// <summary>
        /// Преобразование числа в строку по правилам инвариантной
        /// (не зависящей от региона) культуры.
        /// </summary>
        /// <param name="value">Число для преобразования.</param>
        /// <returns>Строковое представление числа.</returns>
        public static string ToInvariantString
            (
                this long value
            )
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Convert to <see cref="System.String"/>
        /// using <see cref="CultureInfo.InvariantCulture"/>.
        /// </summary>
        public static string ToInvariantString
            (
                this char value
            )
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Try parse decimal value in standard manner.
        /// </summary>
        public static bool TryParseDecimal
            (
                string? text,
                out decimal value
            )
        {
            if (string.IsNullOrEmpty(text))
            {
                value = 0m;
                return false;
            }

            text = ConvertFloatToInvariant(text);
            var result = decimal.TryParse
                (
                    text,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out value
                );

            return result;
        }

        /// <summary>
        /// Try parse double precision value in standard manner.
        /// </summary>
        public static bool TryParseDouble
            (
                string? text,
                out double value
            )
        {
            if (string.IsNullOrEmpty(text))
            {
                value = 0.0;
                return false;
            }

            text = ConvertFloatToInvariant(text);
            var result = double.TryParse
                (
                    text,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out value
                );

            return result;
        }

        /// <summary>
        /// Try parse single precision value in standard manner.
        /// </summary>
        public static bool TryParseSingle
            (
                string? text,
                out float value
            )
        {
            if (string.IsNullOrEmpty(text))
            {
                value = 0.0f;
                return false;
            }

            text = ConvertFloatToInvariant(text);
            var result = float.TryParse
                (
                    text,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out value
                );

            return result;
        }

        /// <summary>
        /// Try parse integer in standard manner.
        /// </summary>
        public static bool TryParseInt16
            (
                string? text,
                out short value
            )
        {
            if (string.IsNullOrEmpty(text))
            {
                value = 0;
                return false;
            }

            text = ConvertIntegerToInvariant(text);
            var result = short.TryParse
                (
                    text,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out value
                );

            return result;
        }

        /// <summary>
        /// Try to parse unsigned integer in standard manner.
        /// </summary>
        [CLSCompliant(false)]
        public static bool TryParseUInt16
            (
                string? text,
                out ushort value
            )
        {
            if (string.IsNullOrEmpty(text))
            {
                value = 0;
                return false;
            }

            text = ConvertIntegerToInvariant(text);
            var result = ushort.TryParse
                (
                    text,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out value
                );

            return result;
        }

        /// <summary>
        /// Try parse integer in standard manner.
        /// </summary>
        public static bool TryParseInt32
            (
                string? text,
                out int value
            )
        {
            if (string.IsNullOrEmpty(text))
            {
                value = 0;
                return false;
            }

            text = ConvertIntegerToInvariant(text);
            var result = int.TryParse
                (
                    text,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out value
                );

            return result;
        }

        /// <summary>
        /// Try to parse unsigned integer in standard manner.
        /// </summary>
        [CLSCompliant(false)]
        public static bool TryParseUInt32
            (
                string? text,
                out uint value
            )
        {
            if (string.IsNullOrEmpty(text))
            {
                value = 0;
                return false;
            }

            text = ConvertIntegerToInvariant(text);
            var result = uint.TryParse
                (
                    text,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out value
                );

            return result;
        }

        /// <summary>
        /// Try parse integer in standard manner.
        /// </summary>
        public static bool TryParseInt64
            (
                string? text,
                out long value
            )
        {
            if (string.IsNullOrEmpty(text))
            {
                value = 0;
                return false;
            }

            text = ConvertIntegerToInvariant(text);
            var result = long.TryParse
                (
                    text,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out value
                );

            return result;
        }

        /// <summary>
        /// Try to parse unsigned integer in standard manner.
        /// </summary>
        [CLSCompliant(false)]
        public static bool TryParseUInt64
            (
                string? text,
                out ulong value
            )
        {
            if (string.IsNullOrEmpty(text))
            {
                value = 0;
                return false;
            }

            text = ConvertIntegerToInvariant(text);
            var result = ulong.TryParse
                (
                    text,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out value
                );

            return result;
        }

        #endregion
    }
}
