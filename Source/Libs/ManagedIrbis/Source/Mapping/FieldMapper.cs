// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* FieldMapper.cs -- отображение поля на свойство
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AM;

using JetBrains.Annotations;

using Newtonsoft.Json;

#endregion

namespace ManagedIrbis.Mapping
{
    /// <summary>
    /// Отображение поля на свойство.
    /// </summary>
    [PublicAPI]
    public static class FieldMapper
    {
        #region Public methods

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
        /// Преобразование в число с плавающей точкой
        /// двойной точностью.
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
        /// одинарной точности.
        /// </summary>
        public static float ToSingle
            (
                [NotNull] RecordField field
            )
        {
            NumericUtility.TryParseFloat(field.Value, out float result);

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

        #endregion
    }
}
