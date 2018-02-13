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
        [Pure]
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
        [Pure]
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
        [Pure]
        public static bool ConsistOf
            (
                [CanBeNull] this string text,
                char c
            )
        {
            if (ReferenceEquals(text, null) || text.Length == 0)
            {
                return false;
            }

            foreach (char c1 in text)
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
        [Pure]
        public static bool ConsistOf
            (
                [CanBeNull] this string text,
                params char[] array
            )
        {
            if (ReferenceEquals(text, null) || text.Length == 0)
            {
                return false;
            }

            foreach (char c in text)
            {
                if (Array.IndexOf(array, c) < 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Mangle given text with the escape character.
        /// </summary>
        [CanBeNull]
        public static string Mangle
            (
                [CanBeNull] string text,
                char escape,
                [NotNull] char[] badCharacters
            )
        {
            if (ReferenceEquals(text, null) || text.Length == 0)
            {
                return text;
            }

            StringBuilder result = new StringBuilder(text.Length);

            foreach (char c in text)
            {
                if (c.OneOf(badCharacters)
                    || c == escape)
                {
                    result.Append(escape);
                }
                result.Append(c);
            }

            return result.ToString();
        }

        /// <summary>
        /// Проверяет, является ли искомая строка одной
        /// из перечисленных. Регистр символов не учитывается.
        /// </summary>
        /// <param name="one">Искомая строка.</param>
        /// <param name="many">Источник проверяемых строк.</param>
        /// <returns>Найдена ли искомая строка.</returns>
        public static bool OneOf
            (
                [CanBeNull] this string one,
                [NotNull] IEnumerable<string> many
            )
        {
            foreach (string s in many)
            {
                if (one.SameString(s))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Проверяет, является ли искомая строка одной
        /// из перечисленных. Регистр символов не учитывается.
        /// </summary>
        /// <param name="one">Искомая строка.</param>
        /// <param name="many">Массив проверяемых строк.</param>
        /// <returns>Найдена ли искомая строка.</returns>
        public static bool OneOf
            (
                [CanBeNull] this string one,
                params string[] many
            )
        {
            foreach (string s in many)
            {
                if (one.SameString(s))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Проверяет, является ли искомый символ одним
        /// из перечисленных. Регистр символов не учитывается.
        /// </summary>
        /// <param name="one">Искомый символ.</param>
        /// <param name="many">Массив проверяемых символов.</param>
        /// <returns>Найден ли искомый символ.</returns>
        public static bool OneOf
            (
                this char one,
                [NotNull] IEnumerable<char> many
            )
        {
            foreach (char s in many)
            {
                if (one.SameChar(s))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Проверяет, является ли искомый символ одним
        /// из перечисленных. Регистр символов не учитывается.
        /// </summary>
        /// <param name="one">Искомый символ.</param>
        /// <param name="many">Массив проверяемых символов.</param>
        /// <returns>Найден ли искомый символ.</returns>
        public static bool OneOf
            (
                this char one,
                params char[] many
            )
        {
            foreach (char s in many)
            {
                if (one.SameChar(s))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Сравнивает символы с точностью до регистра.
        /// </summary>
        /// <param name="one">Первый символ.</param>
        /// <param name="two">Второй символ.</param>
        /// <returns>Символы совпадают с точностью до регистра.</returns>
        public static bool SameChar
            (
                this char one,
                char two
            )
        {
            return char.ToUpperInvariant(one) == char.ToUpperInvariant(two);
        }

        /// <summary>
        /// Сравнивает строки с точностью до регистра.
        /// </summary>
        /// <param name="one">Первая строка.</param>
        /// <param name="two">Вторая строка.</param>
        /// <returns>Строки совпадают с точностью до регистра.</returns>
        public static bool SameString
            (
                [CanBeNull] this string one,
                [CanBeNull] string two
            )
        {
            return string.Compare
                   (
                       one,
                       two,
                       StringComparison.OrdinalIgnoreCase
                   ) == 0;
        }

        /// <summary>
        /// Сравнивает строки.
        /// </summary>
        public static bool SameStringSensitive
            (
                [CanBeNull] this string one,
                [CanBeNull] string two
            )
        {
            return string.Compare
                   (
                       one,
                       two,
                       StringComparison.Ordinal
                   ) == 0;
        }

        /// <summary>
        /// Превращает строку в видимую.
        /// Пример: "(null)".
        /// </summary>
        [Pure]
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
