// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* SubFieldMapper.cs -- отображение подполя на свойство
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AM;

using JetBrains.Annotations;

using Newtonsoft.Json;

#endregion

// ReSharper disable ConvertClosureToMethodGroup

namespace ManagedIrbis.Mapping
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public sealed class SubFieldMapper
    {
        #region Public methods

        /// <summary>
        /// Преобразование из булевого значения в строку.
        /// </summary>
        [CanBeNull]
        public static string FromBoolean
            (
                bool value
            )
        {
            return value ? "1" : null;
        }

        /// <summary>
        /// Преобразование из символа в строку.
        /// </summary>
        [CanBeNull]
        public static string FromChar
            (
                char value
            )
        {
            return value == '\0' ? null : new string(value, 1);
        }

        /// <summary>
        /// Преобразование из даты в строку.
        /// </summary>
        [CanBeNull]
        public static string FromDateTime
            (
                DateTime value
            )
        {
            return value == DateTime.MinValue ? null : IrbisDate.ConvertDateToString(value);
        }

        /// <summary>
        /// Преобразвание из числа с фиксированной точкой в строку.
        /// </summary>
        [CanBeNull]
        public static string FromDecimal
            (
                decimal value
            )
        {
            return value == 0m ? null : value.ToInvariantString();
        }

        /// <summary>
        /// Преобразвание из числа с плавающей точкой двойной точности в строку.
        /// </summary>
        [CanBeNull]
        public static string FromDouble
            (
                double value
            )
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return value == 0.0 ? null : value.ToInvariantString();
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        /// Преобразвание из числа с плавающей точкой одинарной точности в строку.
        /// </summary>
        [CanBeNull]
        public static string FromSingle
            (
                float value
            )
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return value == 0.0 ? null : value.ToString(CultureInfo.InvariantCulture);
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        /// Преобразвание из 16-битного целого в строку.
        /// </summary>
        [CanBeNull]
        public static string FromInt16
            (
                short value
            )
        {
            return value == 0 ? null : value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Преобразвание из 32-битного целого в строку.
        /// </summary>
        [CanBeNull]
        public static string FromInt32
            (
                int value
            )
        {
            return value == 0 ? null : value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Преобразвание из 64-битного целого в строку.
        /// </summary>
        [CanBeNull]
        public static string FromInt64
            (
                long value
            )
        {
            return value == 0 ? null : value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Преобразвание из строки в строку (тривиальное).
        /// </summary>
        [CanBeNull]
        public static string FromString
            (
                [CanBeNull] string value
            )
        {
            return string.IsNullOrEmpty(value) ? null : value;
        }

        /// <summary>
        /// Преобразование в булево значение.
        /// </summary>
        public static bool ToBoolean
            (
                [NotNull] SubField subField
            )
        {
            return !string.IsNullOrEmpty(subField.Value);
        }

        /// <summary>
        /// Преобразование в булево значение.
        /// </summary>
        public static bool ToBoolean
            (
                [NotNull] RecordField field,
                char code
            )
        {
            SubField subField = field.GetFirstSubField(code);
            bool result = !ReferenceEquals(subField, null) && ToBoolean(subField);

            return result;
        }

        /// <summary>
        /// Преобразование в символ.
        /// </summary>
        public static char ToChar
            (
                [NotNull] SubField subField
            )
        {
             return subField.Value.FirstChar();
        }

        /// <summary>
        /// Преобразование в символ.
        /// </summary>
        public static char ToChar
            (
                [NotNull] RecordField field,
                char code
            )
        {
            SubField subField = field.GetFirstSubField(code);

            return subField?.Value.FirstChar() ?? '\0';
        }

        /// <summary>
        /// Преобразование в дату.
        /// </summary>
        public static DateTime ToDateTime
            (
                [NotNull] SubField subField
            )
        {
            return IrbisDate.ConvertStringToDate(subField.Value);
        }

        ///// <summary>
        ///// Преобразование в дату.
        ///// </summary>
        //[CanBeNull]
        //public static DateTime? ToDateTime
        //    (
        //        [NotNull] RecordField field,
        //        char code
        //    )
        //{
        //    SubField subField = field.GetFirstSubField(code);
        //    DateTime? result = null;
        //    if (!ReferenceEquals(subField, null))
        //    {
        //        result = ToDateTime(subField);
        //    }

        //    return result;
        //}

        /// <summary>
        /// Преобразование в число с фиксированной точкой.
        /// </summary>
        public static decimal ToDecimal
            (
                [NotNull] SubField subField
            )
        {
            NumericUtility.TryParseDecimal(subField.Value, out decimal result);

            return result;
        }

        /// <summary>
        /// Преобразование в число с плавающей точкой
        /// двойной точностью.
        /// </summary>
        public static double ToDouble
            (
                [NotNull] SubField subField
            )
        {
            NumericUtility.TryParseDouble(subField.Value, out double result);

            return result;
        }

        /// <summary>
        /// Преобразование в число с плавающей точкой
        /// одинарной точности.
        /// </summary>
        public static float ToSingle
            (
                [NotNull] SubField subField
            )
        {
            NumericUtility.TryParseFloat(subField.Value, out float result);

            return result;
        }

        /// <summary>
        /// Преобразование в 16-битное целое со знаком.
        /// </summary>
        public static short ToInt16
            (
                [NotNull] SubField subField
            )
        {
            NumericUtility.TryParseInt16(subField.Value, out short result);

            return result;
        }

        /// <summary>
        /// Преобразование в 32-битное целое со знаком.
        /// </summary>
        public static int ToInt32
            (
                [NotNull] SubField subField
            )
        {
            NumericUtility.TryParseInt32(subField.Value, out int result);

            return result;
        }

        /// <summary>
        /// Преобразование в 32-битное целое со знаком.
        /// </summary>
        public static int ToInt32
            (
                [NotNull] RecordField field,
                char code
            )
        {
            SubField subField = field.GetFirstSubField(code);
            int result = 0;
            if (!ReferenceEquals(subField, null))
            {
                result = ToInt32(subField);
            }

            return result;
        }

        /// <summary>
        /// Преобразование в 64-битное целое со знаком.
        /// </summary>
        public static long ToInt64
            (
                [NotNull] SubField subField
            )
        {
            NumericUtility.TryParseInt64(subField.Value, out long result);

            return result;
        }

        ///// <summary>
        ///// Преобразование в 64-битное целое со знаком.
        ///// </summary>
        //public static long? ToInt64
        //    (
        //        [NotNull] RecordField field,
        //        char code
        //    )
        //{
        //    SubField subField = field.GetFirstSubField(code);
        //    long? result = null;
        //    if (!ReferenceEquals(subField, null))
        //    {
        //        result = ToInt64(subField);
        //    }

        //    return result;
        //}

        ///// <summary>
        ///// Преобразование в строку (тривиальное).
        ///// </summary>
        //[CanBeNull]
        //public static string ToString
        //    (
        //        [NotNull] SubField subField
        //    )
        //{
        //    return subField.Value;
        //}

        /// <summary>
        /// Преобразование в строку (тривиальное).
        /// </summary>
        [CanBeNull]
        public static string ToString
            (
                [NotNull] RecordField field,
                char code
            )
        {
            SubField subField = field.GetFirstSubField(code);
            string result = subField?.Value;

            return result;
        }

        #endregion
    }
}
