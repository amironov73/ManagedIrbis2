// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* StringUtility.cs -- string manipulation routines
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

using JetBrains.Annotations;

#endregion

namespace AM
{
    /// <summary>
    /// String manipulation routines.
    /// </summary>
    [PublicAPI]
    public static class StringUtility
    {
        #region Properties or fields

        /// <summary>
        /// Empty array of <see cref="string"/>.
        /// </summary>
        [NotNull]
        public static readonly string[] EmptyArray = new string[0];

        #endregion

        #region Private members

        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
                                     + "abcdefghijklmnopqrstuvwxyz";

        private const string Symbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
                                       + "abcdefghijklmnopqrstuvwxyz"
                                       + "0123456789"
                                       + "_";

        private static readonly Random _random = new Random();

        private static readonly char[] _quotes = { '\'', '"', '[', ']' };

        #endregion

        #region Public methods

        /// <summary>
        /// Сравнивает две строки с точностью до регистра символов.
        /// </summary>
        public static bool CompareNoCase
            (
                [CanBeNull] string left,
                [CanBeNull] string right
            )
        {
            return CultureInfo.InvariantCulture.CompareInfo.Compare
                   (
                       left,
                       right,
                       CompareOptions.IgnoreCase
                   ) == 0;
        }

        /// <summary>
        /// Сравнивает два символа с точностью до регистра.
        /// </summary>
        public static bool CompareNoCase
            (
                char left,
                char right
            )
        {
            return char.ToUpperInvariant(left)
                   == char.ToUpperInvariant(right);
        }

        /// <summary>
        /// Состоит ли строка только из указанного символа.
        /// </summary>
        public static bool ConsistOf
            (
                [CanBeNull] this string value,
                char c
            )
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            foreach (char c1 in value)
            {
                if (c1 != c)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Состоит ли строка только из указанных символов.
        /// </summary>
        public static bool ConsistOf
            (
                [CanBeNull] this string value,
                params char[] array
            )
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            foreach (char c in value)
            {
                if (Array.IndexOf(array, c) < 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Превращает строку в видимую.
        /// Пример: "(null)".
        /// </summary>
        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToVisibleString
            (
                [CanBeNull] this string text
            )
        {
            if (ReferenceEquals(text, null))
            {
                return "(null)";
            }
            if (string.IsNullOrEmpty(text))
            {
                return "(empty)";
            }

            return text;
        }

        #endregion
    }
}
