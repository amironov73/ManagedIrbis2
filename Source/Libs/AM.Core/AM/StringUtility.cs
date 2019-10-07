// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* StringUtility.cs -- string manipulation routines
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

using AM.Collections;

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
        public static readonly string[] EmptyArray = new string[0];

        #endregion

        #region Public methods

        /// <summary>
        /// Compares two strings ignoring case of the characters.
        /// Culture-tolerant.
        /// </summary>
        [Pure]
        public static int CompareNoCase
            (
                string? left,
                string? right
            )
        {
            return string.Compare
                (
                    left,
                    right,
                    StringComparison.OrdinalIgnoreCase
                );
        }

        /// <summary>
        /// Compares two symbols ignoring case of the characters.
        /// </summary>
        [Pure]
        public static int CompareNoCase
            (
                char left,
                char right
            )
        {
            return char.ToUpperInvariant(left) - char.ToUpperInvariant(right);
        }

        /// <summary>
        /// Converts empty string to <c>null</c>.
        /// </summary>
        public static string? EmptyToNull
            (
                this string? value
            )
        {
            return string.IsNullOrEmpty(value)
                ? null
                : value;
        }

        /// <summary>
        /// Gets the first char of the text.
        /// </summary>
        public static char FirstChar
            (
                this string? text
            )
        {
            return string.IsNullOrEmpty(text)
                ? '\0'
                : text[0];
        }

        /// <summary>
        /// Gets the last char of the text.
        /// </summary>
        public static char LastChar
            (
                [CanBeNull] this string text
            )
        {
            return ReferenceEquals(text, null) || text.Length == 0
                ? '\0'
                : text[text.Length - 1];
        }

        /// <summary>
        /// Mangle given text with the escape character.
        /// </summary>
        public static string? Mangle
            (
                string? text,
                char escape,
                char[] badCharacters
            )
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            var result = new StringBuilder(text.Length);
            foreach (char c in text)
            {
                if (c.OneOf(badCharacters) || c == escape)
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
                IEnumerable<string> many
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
        /// Поиск начала строки.
        /// </summary>
        public static bool SafeStarts
            (
                [CanBeNull] this string text,
                [CanBeNull] string begin
            )
        {
            if (ReferenceEquals(text, null)
                || text.Length == 0
                || ReferenceEquals(begin, null)
                || begin.Length == 0)
            {
                return false;
            }

            return text.ToLowerInvariant()
                .StartsWith(begin.ToLowerInvariant());
        }

        /// <summary>
        ///
        /// </summary>
        public static string? SafeSubstring
            (
                this string? text,
                int offset,
                int width
            )
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            int length = text.Length;
            if (offset < 0
                || offset >= length
                || width <= 0)
            {
                return string.Empty;
            }

            if (offset + width > length)
            {
                width = length - offset;
            }

            string result = text.Substring(offset, width);

            return result;
        }

        /// <summary>
        /// Сравнивает строки с точностью до регистра.
        /// </summary>
        /// <param name="one">Первая строка.</param>
        /// <param name="two">Вторая строка.</param>
        /// <returns>Строки совпадают с точностью до регистра.</returns>
        [Pure]
        public static bool SameString
            (
                this string? one,
                string? two
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
        [Pure]
        public static bool SameStringSensitive
            (
                this string? one,
                string? two
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
        ///
        /// </summary>
        public static ReadOnlyMemory<char>[] SplitToSpan
            (
                string text,
                char delimiter
            )
        {
            var result = new LocalList<ReadOnlyMemory<char>>();
            var start = 0;
            var characters = text.ToCharArray();
            var offset = 0;
            for (; offset < characters.Length; offset++)
            {
                if (characters[offset] == delimiter)
                {
                    if (offset != start)
                    {
                        var item = new ReadOnlyMemory<char>(characters, start, offset - start);
                        result.Add(item);
                    }
                    start = offset + 1;
                }
            }

            if (start < characters.Length && offset != start)
            {
                var item = new ReadOnlyMemory<char>(characters, start, offset - start);
                result.Add(item);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Превращает строку в видимую.
        /// Пример: "(null)".
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToVisibleString
            (
                this string? text
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
