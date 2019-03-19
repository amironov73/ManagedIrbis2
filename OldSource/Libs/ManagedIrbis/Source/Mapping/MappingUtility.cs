// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* MappingUtility.cs -- common routines for record/field mapping
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

using AM;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Mapping
{
    /// <summary>
    /// Common routines for record/field mapping.
    /// </summary>
    [PublicAPI]
    public static class MappingUtility
    {
        #region Mapping methods

        //===========================================================

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

        //===========================================================

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

        //===========================================================

        /// <summary>
        /// Преобразование из даты в строку.
        /// </summary>
        [CanBeNull]
        public static string FromDateTime
            (
                DateTime value
            )
        {
            return value == DateTime.MinValue
                ? null
                : IrbisDate.ConvertDateToString(value);
        }

        //===========================================================

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

        //===========================================================

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

        //===========================================================

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

        //===========================================================

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

        //===========================================================

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

        //===========================================================

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

        //===========================================================

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

        //===========================================================
        // Прямое преобразование
        //===========================================================

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
                [NotNull] RecordField field
            )
        {
            return !string.IsNullOrEmpty(field.Value);
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
            bool result = !ReferenceEquals(subField, null)
                          && !string.IsNullOrEmpty(subField.Value);

            return result;
        }

        /// <summary>
        /// Преобразование в булево значение.
        /// </summary>
        public static bool ToBoolean
            (
                [NotNull] MarcRecord record,
                int tag
            )
        {
            RecordField field = record.Fields.GetFirstField(tag);
            bool result = !ReferenceEquals(field, null)
                          && ToBoolean(field);

            return result;
        }

        /// <summary>
        /// Преобразование в булево значение.
        /// </summary>
        public static bool ToBoolean
            (
                [NotNull] MarcRecord record,
                int tag,
                char code
            )
        {
            SubField subField = record.Fields.GetFirstSubField(tag, code);
            bool result = !ReferenceEquals(subField, null)
                          && !string.IsNullOrEmpty(subField.Value);

            return result;
        }

        //===========================================================

        /// <summary>
        /// Преобразование в массив булевых значений.
        /// </summary>
        [NotNull]
        public static bool[] ToBooleanArray
            (
                [NotNull] MarcRecord record,
                int tag
            )
        {
            return record.Fields.GetField(tag)
                .Select(ToBoolean)
                .ToArray();
        }

        /// <summary>
        /// Преобразование в массив булевых значений.
        /// </summary>
        [NotNull]
        public static bool[] ToBooleanArray
            (
                [NotNull] MarcRecord record,
                int tag,
                char code
            )
        {
            return record.Fields.GetSubField(tag, code)
                .Select(subField => !string.IsNullOrEmpty(subField.Value))
                .ToArray();
        }

        //===========================================================

        /// <summary>
        /// Преобразование в символ.
        /// </summary>
        public static char ToChar
            (
                [NotNull] RecordField field
            )
        {
            return field.Value.FirstChar();
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
        /// Преобразование в символ.
        /// </summary>
        public static char ToChar
            (
                [NotNull] MarcRecord record,
                int tag
            )
        {
            RecordField field = record.Fields.GetFirstField(tag);
            char result = ReferenceEquals(field, null)
                ? '\0' : ToChar(field);

            return result;
        }

        /// <summary>
        /// Преобразование в символ.
        /// </summary>
        public static char ToChar
            (
                [NotNull] MarcRecord record,
                int tag,
                char code
            )
        {
            SubField subField = record.Fields.GetFirstSubField(tag, code);
            char result = ReferenceEquals(subField, null)
                ? '\0' : ToChar(subField);

            return result;
        }

        //===========================================================

        /// <summary>
        /// Преобразование в массив символов.
        /// </summary>
        [NotNull]
        public static char[] ToCharArray
            (
                [NotNull] MarcRecord record,
                int tag
            )
        {
            return record.Fields.GetField(tag)
                .Select(ToChar)
                .ToArray();
        }

        /// <summary>
        /// Преобразование в массив символов.
        /// </summary>
        [NotNull]
        public static char[] ToCharArray
            (
                [NotNull] MarcRecord record,
                int tag,
                char code
            )
        {
            return record.Fields.GetSubField(tag, code)
                .Select(ToChar)
                .ToArray();
        }

        //===========================================================

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

        /// <summary>
        /// Преобразование в дату.
        /// </summary>
        public static DateTime ToDateTime
            (
                [NotNull] RecordField field
            )
        {
            return IrbisDate.ConvertStringToDate(field.Value);
        }

        /// <summary>
        /// Преобразование в дату.
        /// </summary>
        public static DateTime ToDateTime
            (
                [NotNull] RecordField field,
                char code
            )
        {
            SubField subField = field.GetFirstSubField(code);
            DateTime result = DateTime.MinValue;
            if (!ReferenceEquals(subField, null))
            {
                result = IrbisDate.ConvertStringToDate(subField.Value);
            }

            return result;
        }

        /// <summary>
        /// Преобразование в дату.
        /// </summary>
        public static DateTime ToDateTime
            (
                [NotNull] MarcRecord record,
                int tag
            )
        {
            RecordField field = record.Fields.GetFirstField(tag);
            DateTime result = ReferenceEquals(field, null)
                ? DateTime.MinValue
                : ToDateTime(field);

            return result;
        }

        /// <summary>
        /// Преобразование в дату.
        /// </summary>
        public static DateTime ToDateTime
            (
                [NotNull] MarcRecord record,
                int tag,
                char code
            )
        {
            SubField subField = record.Fields.GetFirstSubField(tag, code);
            DateTime result = ReferenceEquals(subField, null)
                ? DateTime.MinValue
                : ToDateTime(subField);

            return result;
        }

        //===========================================================

        /// <summary>
        /// Преобразование в массив дат.
        /// </summary>
        [NotNull]
        public static DateTime[] ToDateTimeArray
            (
                [NotNull] MarcRecord record,
                int tag
            )
        {
            return record.Fields.GetField(tag)
                .Select(ToDateTime)
                .ToArray();
        }

        /// <summary>
        /// Преобразование в массив дат.
        /// </summary>
        [NotNull]
        public static DateTime[] ToDateTimeArray
            (
                [NotNull] MarcRecord record,
                int tag,
                char code
            )
        {
            return record.Fields.GetSubField(tag, code)
                .Select(ToDateTime)
                .ToArray();
        }

        //===========================================================

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
        /// Преобразование в число с фиксированной точкой.
        /// </summary>
        public static decimal ToDecimal
            (
                [NotNull] RecordField field
            )
        {
            NumericUtility.TryParseDecimal(field.Value, out decimal result);

            return result;
        }

        /// <summary>
        /// Преобразование в число с фиксированной точкой.
        /// </summary>
        public static decimal ToDecimal
            (
                [NotNull] RecordField field,
                char code
            )
        {
            SubField subField = field.GetFirstSubField(code);
            decimal result = 0;
            if (!ReferenceEquals(subField, null))
            {
                NumericUtility.TryParseDecimal(subField.Value, out result);
            }

            return result;
        }

        /// <summary>
        /// Преобразование в число с фиксированной точкой.
        /// </summary>
        public static decimal ToDecimal
            (
                [NotNull] MarcRecord record,
                int tag
            )
        {
            RecordField field = record.Fields.GetFirstField(tag);
            decimal result = ReferenceEquals(field, null)
                ? 0
                : ToDecimal(field);

            return result;
        }

        /// <summary>
        /// Преобразование в число с фиксированной точкой.
        /// </summary>
        public static decimal ToDecimal
            (
                [NotNull] MarcRecord record,
                int tag,
                char code
            )
        {
            SubField subField = record.Fields.GetFirstSubField(tag, code);
            decimal result = ReferenceEquals(subField, null)
                ? 0
                : ToDecimal(subField);

            return result;
        }

        //===========================================================

        /// <summary>
        /// Преобразование в массив чисел с фиксированной точкой.
        /// </summary>
        [NotNull]
        public static decimal[] ToDecimalArray
            (
                [NotNull] MarcRecord record,
                int tag
            )
        {
            return record.Fields.GetField(tag)
                .Select(ToDecimal)
                .ToArray();
        }

        /// <summary>
        /// Преобразование в массив чисел с фиксированной точкой.
        /// </summary>
        [NotNull]
        public static decimal[] ToDecimalArray
            (
                [NotNull] MarcRecord record,
                int tag,
                char code
            )
        {
            return record.Fields.GetSubField(tag, code)
                .Select(ToDecimal)
                .ToArray();
        }

        //===========================================================

        /// <summary>
        /// Преобразование в число с плавающей точкой
        /// одинарной точности.
        /// </summary>
        public static float ToSingle
            (
                [NotNull] SubField subField
            )
        {
            NumericUtility.TryParseSingle(subField.Value, out float result);

            return result;
        }

        /// <summary>
        /// Преобразование в число с плавающей точкой
        /// одинарной точности.
        /// </summary>
        public static float ToSingle
            (
                [NotNull] RecordField field
            )
        {
            NumericUtility.TryParseSingle(field.Value, out float result);

            return result;
        }

        /// <summary>
        /// Преобразование в число с плавающей точкой
        /// одинарной точности.
        /// </summary>
        public static float ToSingle
            (
                [NotNull] RecordField field,
                char code
            )
        {
            SubField subField = field.GetFirstSubField(code);
            float result = 0;
            if (!ReferenceEquals(subField, null))
            {
                NumericUtility.TryParseSingle(subField.Value, out result);
            }

            return result;
        }

        /// <summary>
        /// Преобразование в число с плавающей точкой
        /// одинарной точности.
        /// </summary>
        public static float ToSingle
            (
                [NotNull] MarcRecord record,
                int tag
            )
        {
            RecordField field = record.Fields.GetFirstField(tag);
            float result = ReferenceEquals(field, null)
                ? 0
                : ToSingle(field);

            return result;
        }

        /// <summary>
        /// Преобразование в число с плавающей точкой
        /// одинарной точности.
        /// </summary>
        public static float ToSingle
            (
                [NotNull] MarcRecord record,
                int tag,
                char code
            )
        {
            SubField subField = record.Fields.GetFirstSubField(tag, code);
            float result = ReferenceEquals(subField, null)
                ? 0
                : ToSingle(subField);

            return result;
        }

        //===========================================================

        /// <summary>
        /// Преобразование в массив чисел с плавающей точкой
        /// одинарной точности.
        /// </summary>
        [NotNull]
        public static float[] ToSingleArray
            (
                [NotNull] MarcRecord record,
                int tag
            )
        {
            return record.Fields.GetField(tag)
                .Select(ToSingle)
                .ToArray();
        }

        /// <summary>
        /// Преобразование в массив чисел с плавающей точкой
        /// одинарной точности.
        /// </summary>
        [NotNull]
        public static float[] ToSingleArray
            (
                [NotNull] MarcRecord record,
                int tag,
                char code
            )
        {
            return record.Fields.GetSubField(tag, code)
                .Select(ToSingle)
                .ToArray();
        }

        //===========================================================

        /// <summary>
        /// Преобразование в число с плавающей точкой
        /// двойной точности.
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
        /// двойной точности.
        /// </summary>
        public static double ToDouble
            (
                [NotNull] RecordField field
            )
        {
            NumericUtility.TryParseDouble(field.Value, out double result);

            return result;
        }

        /// <summary>
        /// Преобразование в число с плавающей точкой
        /// двойной точности.
        /// </summary>
        public static double ToDouble
            (
                [NotNull] RecordField field,
                char code
            )
        {
            SubField subField = field.GetFirstSubField(code);
            double result = 0;
            if (!ReferenceEquals(subField, null))
            {
                NumericUtility.TryParseDouble(subField.Value, out result);
            }

            return result;
        }

        /// <summary>
        /// Преобразование в число с плавающей точкой
        /// двойной точности.
        /// </summary>
        public static double ToDouble
            (
                [NotNull] MarcRecord record,
                int tag
            )
        {
            RecordField field = record.Fields.GetFirstField(tag);
            double result = ReferenceEquals(field, null)
                ? 0
                : ToDouble(field);

            return result;
        }

        /// <summary>
        /// Преобразование в число с плавающей точкой
        /// одинарной точности.
        /// </summary>
        public static double ToDouble
            (
                [NotNull] MarcRecord record,
                int tag,
                char code
            )
        {
            SubField subField = record.Fields.GetFirstSubField(tag, code);
            double result = ReferenceEquals(subField, null)
                ? 0
                : ToDouble(subField);

            return result;
        }

        //===========================================================

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
        /// Преобразование в 16-битное целое со знаком.
        /// </summary>
        public static short ToInt16
            (
                [NotNull] RecordField field
            )
        {
            NumericUtility.TryParseInt16(field.Value, out short result);

            return result;
        }

        /// <summary>
        /// Преобразование в 16-битное целое со знаком.
        /// </summary>
        public static short ToInt16
            (
                [NotNull] RecordField field,
                char code
            )
        {
            SubField subField = field.GetFirstSubField(code);
            short result = 0;
            if (!ReferenceEquals(subField, null))
            {
                NumericUtility.TryParseInt16(subField.Value, out result);
            }

            return result;
        }

        /// <summary>
        /// Преобразование в 16-битное целое со знаком.
        /// </summary>
        public static short ToInt16
            (
                [NotNull] MarcRecord record,
                int tag
            )
        {
            RecordField field = record.Fields.GetFirstField(tag);
            short result = ReferenceEquals(field, null)
                ? (short)0
                : ToInt16(field);

            return result;
        }

        /// <summary>
        /// Преобразование в 16-битное целое со знаком.
        /// </summary>
        public static short ToInt16
            (
                [NotNull] MarcRecord record,
                int tag,
                char code
            )
        {
            SubField subField = record.Fields.GetFirstSubField(tag, code);
            short result = ReferenceEquals(subField, null)
                ? (short)0
                : ToInt16(subField);

            return result;
        }

        //===========================================================

        /// <summary>
        /// Преобразование в массив 16-битных целых со знаком.
        /// </summary>
        [NotNull]
        public static short[] ToInt16Array
            (
                [NotNull] MarcRecord record,
                int tag
            )
        {
            return record.Fields.GetField(tag)
                .Select(ToInt16)
                .ToArray();
        }

        /// <summary>
        /// Преобразование в массив 16-битных целых со знаком.
        /// </summary>
        [NotNull]
        public static short[] ToInt16Array
            (
                [NotNull] MarcRecord record,
                int tag,
                char code
            )
        {
            return record.Fields.GetSubField(tag, code)
                .Select(ToInt16)
                .ToArray();
        }

        //===========================================================

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
                [NotNull] RecordField field
            )
        {
            NumericUtility.TryParseInt32(field.Value, out int result);

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
                NumericUtility.TryParseInt32(subField.Value, out result);
            }

            return result;
        }

        /// <summary>
        /// Преобразование в 32-битное целое со знаком.
        /// </summary>
        public static int ToInt32
            (
                [NotNull] MarcRecord record,
                int tag
            )
        {
            RecordField field = record.Fields.GetFirstField(tag);
            int result = ReferenceEquals(field, null)
                ? 0
                : ToInt32(field);

            return result;
        }

        /// <summary>
        /// Преобразование в 32-битное целое со знаком.
        /// </summary>
        public static int ToInt32
            (
                [NotNull] MarcRecord record,
                int tag,
                char code
            )
        {
            SubField subField = record.Fields.GetFirstSubField(tag, code);
            int result = ReferenceEquals(subField, null)
                ? 0
                : ToInt32(subField);

            return result;
        }

        //===========================================================

        /// <summary>
        /// Преобразование в массив 32-битных целых со знаком.
        /// </summary>
        [NotNull]
        public static int[] ToInt32Array
            (
                [NotNull] MarcRecord record,
                int tag
            )
        {
            return record.Fields.GetField(tag)
                .Select(ToInt32)
                .ToArray();
        }

        /// <summary>
        /// Преобразование в массив 32-битных целых со знаком.
        /// </summary>
        [NotNull]
        public static int[] ToInt32Array
            (
                [NotNull] MarcRecord record,
                int tag,
                char code
            )
        {
            return record.Fields.GetSubField(tag, code)
                .Select(ToInt32)
                .ToArray();
        }

        //===========================================================

        /// <summary>
        /// Преобразование в 64-битное целое со знаком.
        /// </summary>
        public static long ToInt64
            (
                [NotNull] RecordField field
            )
        {
            NumericUtility.TryParseInt64(field.Value, out long result);

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

        /// <summary>
        /// Преобразование в 64-битное целое со знаком.
        /// </summary>
        public static long ToInt64
            (
                [NotNull] RecordField field,
                char code
            )
        {
            SubField subField = field.GetFirstSubField(code);
            long result = 0;
            if (!ReferenceEquals(subField, null))
            {
                NumericUtility.TryParseInt64(subField.Value, out result);
            }

            return result;
        }

        /// <summary>
        /// Преобразование в 64-битное целое со знаком.
        /// </summary>
        public static long ToInt64
            (
                [NotNull] MarcRecord record,
                int tag
            )
        {
            RecordField field = record.Fields.GetFirstField(tag);
            long result = ReferenceEquals(field, null)
                ? 0
                : ToInt64(field);

            return result;
        }

        /// <summary>
        /// Преобразование в 64-битное целое со знаком.
        /// </summary>
        public static long ToInt64
            (
                [NotNull] MarcRecord record,
                int tag,
                char code
            )
        {
            SubField subField = record.Fields.GetFirstSubField(tag, code);
            long result = ReferenceEquals(subField, null)
                ? 0
                : ToInt64(subField);

            return result;
        }

        //===========================================================

        /// <summary>
        /// Преобразование в массив 64-битных целых со знаком.
        /// </summary>
        [NotNull]
        public static long[] ToInt64Array
            (
                [NotNull] MarcRecord record,
                int tag,
                char code
            )
        {
            return record.Fields.GetSubField(tag, code)
                .Select(ToInt64)
                .ToArray();
        }

        /// <summary>
        /// Преобразование в массив 64-битных целых со знаком.
        /// </summary>
        [NotNull]
        public static long[] ToInt64Array
            (
                [NotNull] MarcRecord record,
                int tag
            )
        {
            return record.Fields.GetField(tag)
                .Select(ToInt64)
                .ToArray();
        }

        //===========================================================

        /// <summary>
        /// Преобразование в строку (тривиальное).
        /// </summary>
        [CanBeNull]
        public static string ToString
            (
                [NotNull] SubField subField
            )
        {
            return subField.Value;
        }

        /// <summary>
        /// Преобразование в строку (тривиальное).
        /// </summary>
        [CanBeNull]
        public static string ToString
            (
                [NotNull] RecordField field
            )
        {
            return field.Value;
        }

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

        /// <summary>
        /// Преобразование в строку (тривиальное).
        /// </summary>
        [CanBeNull]
        public static string ToString
            (
                [NotNull] MarcRecord record,
                int tag
            )
        {
            RecordField field = record.Fields.GetFirstField(tag);
            string result = ReferenceEquals(field, null)
                ? null
                : ToString(field);

            return result;
        }

        /// <summary>
        /// Преобразование в строку (тривиальное).
        /// </summary>
        [CanBeNull]
        public static string ToString
            (
                [NotNull] MarcRecord record,
                int tag,
                char code
            )
        {
            SubField subField = record.Fields.GetFirstSubField(tag, code);
            string result = ReferenceEquals(subField, null)
                ? null
                : ToString(subField);

            return result;
        }

        //===========================================================

        /// <summary>
        /// Преобразование в массив строк.
        /// </summary>
        [NotNull]
        [ItemCanBeNull]
        public static string[] ToStringArray
            (
                [NotNull] MarcRecord record,
                int tag
            )
        {
            return record.Fields.GetField(tag)
                .Select(ToString)
                .ToArray();
        }

        /// <summary>
        /// Преобразование в массив строк.
        /// </summary>
        [NotNull]
        [ItemCanBeNull]
        public static string[] ToStringArray
            (
                [NotNull] MarcRecord record,
                int tag,
                char code
            )
        {
            return record.Fields.GetSubField(tag, code)
                .Select(ToString)
                .ToArray();
        }

        //===========================================================

        #endregion

        #region Public methods

        /// <summary>
        /// Create delegate for backward converter.
        /// </summary>
        [Pure]
        [NotNull]
        [MustUseReturnValue]
        public static Action<T, RecordField> CreateBackwardConverter<T>
            (
                [NotNull] SubFieldMapping<T> mapping
            )
        {
            Sure.NotNull(mapping, nameof(mapping));

            //
            // What we want to get:
            //
            // field.SetSubField(code, SubFieldMapper.FromXXX(source.Property);
            //

            var sourceParameter = Expression.Parameter(typeof(T), "source");
            var fieldParameter = Expression.Parameter(typeof(RecordField), "field");
            var codeConstant = Expression.Constant(mapping.Code);
            var property = Expression.Property(sourceParameter, mapping.Property);
            var propertyType = mapping.Property.PropertyType;
            var methodName = GetBackwardMethodName(propertyType);
            var backwardMethod = typeof(MappingUtility).GetMethod
                (
                    methodName,
                    new[] { propertyType }
                );
            var setSubField = typeof(RecordField).GetMethod
                (
                    "SetSubField",
                    new[] { typeof(char), typeof(object) }
                );
            var body = Expression.Call
                (
                    fieldParameter,
                    setSubField,
                    codeConstant,
                    Expression.Call(backwardMethod, property)
                );
            var expression = Expression.Lambda<Action<T, RecordField>>
                (
                    body,
                    sourceParameter,
                    fieldParameter
                );
            var result = expression.Compile();

            return result;
        }

        /// <summary>
        /// Create delegate for forward converter.
        /// </summary>
        [Pure]
        [NotNull]
        [MustUseReturnValue]
        public static Action<RecordField, T> CreateForwardConverter<T>
            (
                [NotNull] SubFieldMapping<T> mapping
            )
        {
            Sure.NotNull(mapping, nameof(mapping));

            //
            // What we want to get:
            //
            // target.Property = SubFieldMapper.ToXXX(field, code);
            //

            var fieldParameter = Expression.Parameter(typeof(RecordField), "field");
            var targetParameter = Expression.Parameter(typeof(T), "target");
            var codeConstant = Expression.Constant(mapping.Code);
            var property = Expression.Property(targetParameter, mapping.Property);
            var propertyType = mapping.Property.PropertyType;
            var methodName = GetForwardMethodName(propertyType);
            var methodInfo = typeof(MappingUtility).GetMethod
                (
                    methodName,
                    new[] { typeof(RecordField), typeof(char) }
                );
            var body = Expression.Assign
                (
                    property,
                    Expression.Call(methodInfo, fieldParameter, codeConstant)
                );
            var expression = Expression.Lambda<Action<RecordField, T>>
                (
                    body,
                    fieldParameter,
                    targetParameter
                );
            var result = expression.Compile();

            return result;
        }

        /// <summary>
        /// Get name for backward conversion.
        /// </summary>
        [NotNull]
        public static string GetBackwardMethodName
            (
                [NotNull] Type type
            )
        {
            Sure.NotNull(type, nameof(type));

            string result;
            TypeCode typeCode = Type.GetTypeCode(type);
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    result = "FromBoolean";
                    break;

                case TypeCode.Char:
                    result = "FromChar";
                    break;

                case TypeCode.DateTime:
                    result = "FromDateTime";
                    break;

                case TypeCode.Decimal:
                    result = "FromDecimal";
                    break;

                case TypeCode.Double:
                    result = "FromDouble";
                    break;

                case TypeCode.Int16:
                    result = "FromInt16";
                    break;

                case TypeCode.Int32:
                    result = "FromInt32";
                    break;

                case TypeCode.Int64:
                    result = "FromInt64";
                    break;

                case TypeCode.Single:
                    result = "FromSingle";
                    break;

                case TypeCode.String:
                    result = "FromString";
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(typeCode));
            }

            return result;
        }

        /// <summary>
        /// Get name for forward conversion.
        /// </summary>
        [NotNull]
        public static string GetForwardMethodName
            (
                [NotNull] Type type
            )
        {
            Sure.NotNull(type, nameof(type));

            string result;
            TypeCode typeCode = Type.GetTypeCode(type);
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    result = "ToBoolean";
                    break;

                case TypeCode.Char:
                    result = "ToChar";
                    break;

                case TypeCode.DateTime:
                    result = "ToDateTime";
                    break;

                case TypeCode.Decimal:
                    result = "ToDecimal";
                    break;

                case TypeCode.Double:
                    result = "ToDouble";
                    break;

                case TypeCode.Int16:
                    result = "ToInt16";
                    break;

                case TypeCode.Int32:
                    result = "ToInt32";
                    break;

                case TypeCode.Int64:
                    result = "ToInt64";
                    break;

                case TypeCode.Single:
                    result = "ToSingle";
                    break;

                case TypeCode.String:
                    result = "ToString";
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(typeCode));
            }

            return result;
        }

        #endregion
    }
}
