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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

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

        #region Public methods

        /// <summary>
        /// Сравнивает две строки с точностью до регистра символов.
        /// Нечувствительно к культуре.
        /// </summary>
        [Pure]
        public static int CompareNoCase
            (
                [CanBeNull] string left,
                [CanBeNull] string right
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
        /// Сравнивает два символа с точностью до регистра.
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
        /// Gets the first char of the text.
        /// </summary>
        public static char FirstChar
            (
                [CanBeNull] this string text
            )
        {
            return ReferenceEquals(text, null) || text.Length == 0
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
        [CanBeNull]
        public static string SafeSubstring
            (
                [CanBeNull] this string text,
                int offset,
                int width
            )
        {
            if (ReferenceEquals(text, null) || text.Length == 0)
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
        [Pure]
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
