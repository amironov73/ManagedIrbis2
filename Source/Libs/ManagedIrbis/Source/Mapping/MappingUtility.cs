// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* MappingUtility.cs -- common routines for record/field mapping
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;

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
        #region Public methods

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
