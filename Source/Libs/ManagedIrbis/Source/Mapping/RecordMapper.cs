// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* RecordMapper.cs -- отображение полей записи на свойства объекта
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Linq;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Mapping
{
    /// <summary>
    /// Отображение полей записи на свойства объекта.
    /// </summary>
    [PublicAPI]
    public static class RecordMapper
    {
        #region Public methods

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
                          && FieldMapper.ToBoolean(field);

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
                          && SubFieldMapper.ToBoolean(subField);

            return result;
        }

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
                .Select(FieldMapper.ToBoolean)
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
                .Select(SubFieldMapper.ToBoolean)
                .ToArray();
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
                ? '\0' : FieldMapper.ToChar(field);

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
                ? '\0' : SubFieldMapper.ToChar(subField);

            return result;
        }

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
                .Select(FieldMapper.ToChar)
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
                .Select(SubFieldMapper.ToChar)
                .ToArray();
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
                : FieldMapper.ToDateTime(field);

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
                : SubFieldMapper.ToDateTime(subField);

            return result;
        }

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
                .Select(FieldMapper.ToDateTime)
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
                .Select(SubFieldMapper.ToDateTime)
                .ToArray();
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
                : FieldMapper.ToDecimal(field);

            return result;
        }

        /// <summary>
        /// Преобразование в число с плавающей точкой двойной точности.
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
                : FieldMapper.ToDouble(field);

            return result;
        }

        /// <summary>
        /// Преобразование в число с плавающей точкой одинарной точности.
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
                : FieldMapper.ToSingle(field);

            return result;
        }

        /// <summary>
        /// Преобразование в число с плавающей точкой одинарной точности.
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
                : SubFieldMapper.ToSingle(subField);

            return result;
        }

        /// <summary>
        /// Преобразование в массив чисел с плавающей точкой одинарной точности.
        /// </summary>
        [NotNull]
        public static float[] ToSingleArray
            (
                [NotNull] MarcRecord record,
                int tag
            )
        {
            return record.Fields.GetField(tag)
                .Select(FieldMapper.ToSingle)
                .ToArray();
        }

        /// <summary>
        /// Преобразование в массив чисел с плавающей точкой одинарной точности.
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
                .Select(SubFieldMapper.ToSingle)
                .ToArray();
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
                : FieldMapper.ToInt16(field);

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
                : SubFieldMapper.ToInt16(subField);

            return result;
        }

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
                .Select(FieldMapper.ToInt16)
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
                .Select(SubFieldMapper.ToInt16)
                .ToArray();
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
                : FieldMapper.ToInt32(field);

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
                : SubFieldMapper.ToInt32(subField);

            return result;
        }

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
                .Select(FieldMapper.ToInt32)
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
                .Select(SubFieldMapper.ToInt32)
                .ToArray();
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
                : FieldMapper.ToInt64(field);

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
                : SubFieldMapper.ToInt64(subField);

            return result;
        }

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
                .Select(SubFieldMapper.ToInt64)
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
                .Select(FieldMapper.ToInt64)
                .ToArray();
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
                : FieldMapper.ToString(field);

            return result;
        }

        ///// <summary>
        ///// Преобразование в строку (тривиальное).
        ///// </summary>
        //[CanBeNull]
        //public static string ToString
        //    (
        //        [NotNull] MarcRecord record,
        //        int tag,
        //        char code
        //    )
        //{
        //    SubField subField = record.Fields.GetFirstSubField(tag, code);
        //    string result = ReferenceEquals(subField, null)
        //        ? null
        //        : SubFieldMapper.ToString(subField);

        //    return result;
        //}

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
                .Select(FieldMapper.ToString)
                .ToArray();
        }

        ///// <summary>
        ///// Преобразование в массив строк.
        ///// </summary>
        //[NotNull]
        //[ItemCanBeNull]
        //public static string[] ToStringArray
        //    (
        //        [NotNull] MarcRecord record,
        //        int tag,
        //        char code
        //    )
        //{
        //    return record.Fields.GetSubField(tag, code)
        //        .Select(SubFieldMapper.ToString)
        //        .ToArray();
        //}

        #endregion
    }
}
