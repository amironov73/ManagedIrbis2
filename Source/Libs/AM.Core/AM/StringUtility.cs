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

        #region Private members

        //private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
        //                             + "abcdefghijklmnopqrstuvwxyz";

        //private const string Symbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
        //                               + "abcdefghijklmnopqrstuvwxyz"
        //                               + "0123456789"
        //                               + "_";

        //private static readonly Random _random = new Random();

        //private static readonly char[] _quotes = { '\'', '"', '[', ']' };

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
        /// Содержит ли строка любую из перечисленных букв.
        /// </summary>
        public static bool ContainsAny
            (
                [CanBeNull] this string text,
                params char[] characters
            )
        {
            if (!ReferenceEquals(text, null))
            {
                foreach (char c in text)
                {
                    if (characters.Contains(c))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Содержит ли строка указанную букву.
        /// </summary>
        public static bool Contains
            (
                [CanBeNull] this string text,
                char character
            )
        {
            if (!ReferenceEquals(text, null))
            {
                foreach (char c in text)
                {
                    if (c == character)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Count of given substring in the text.
        /// </summary>
        public static int CountSubstring
            (
                [CanBeNull] this string text,
                [CanBeNull] string substring
            )
        {
            int result = 0;

            if (!ReferenceEquals(text, null) && text.Length != 0
                && !ReferenceEquals(substring, null) && substring.Length != 0)
            {
                int length = substring.Length;
                int offset = 0;

                while (true)
                {
                    int index = text.IndexOf
                        (
                            substring,
                            offset,
                            StringComparison.OrdinalIgnoreCase
                        );
                    if (index < 0)
                    {
                        break;
                    }
                    result++;
                    offset = index + length;
                }
            }

            return result;
        }

        /// <summary>
        /// Converts empty string to <c>null</c>.
        /// </summary>
        [CanBeNull]
        public static string EmptyToNull
            (
                [CanBeNull] this string value
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
                [CanBeNull] this string text
            )
        {
            return ReferenceEquals(text, null) || text.Length == 0
                ? '\0'
                : text[0];
        }

        /// <summary>
        /// 
        /// </summary>
        [CanBeNull]
        public static string IfEmpty
            (
                [CanBeNull] this string first,
                params string[] others
            )
        {
            if (!string.IsNullOrEmpty(first))
            {
                return first;
            }

            return others.FirstOrDefault(s => !string.IsNullOrEmpty(s));
        }

        /// <summary>
        /// Reports the index of the first occurrence in this instance
        /// of any string in a specified array.
        /// </summary>
        public static int IndexOfAny
            (
                [NotNull] this string text,
                out int which,
                params string[] anyOf
            )
        {
            Sure.NotNull(text, nameof(text));

            int result = -1;
            which = -1;

            for (int i = 0; i < anyOf.Length; i++)
            {
                string value = anyOf[i];
                int index = text.IndexOf(value, StringComparison.Ordinal);
                if (index >= 0)
                {
                    if (result >= 0)
                    {
                        if (index < result)
                        {
                            result = index;
                            which = i;
                        }
                    }
                    else
                    {
                        result = index;
                        which = i;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Join string representations of the given objects.
        /// </summary>
        [NotNull]
        public static string Join
            (
                [CanBeNull] string separator,
                [NotNull] IEnumerable objects
            )
        {
            Sure.NotNull(objects, nameof(objects));

            if (ReferenceEquals(separator, null))
            {
                separator = string.Empty;
            }

            StringBuilder result = new StringBuilder();
            IEnumerator enumerator = objects.GetEnumerator();
            if (enumerator.MoveNext())
            {
                bool flag = false;
                object o = enumerator.Current;
                if (!ReferenceEquals(o, null))
                {
                    result.Append(o);
                    flag = true;
                }

                while (enumerator.MoveNext())
                {
                    o = enumerator.Current;
                    if (!ReferenceEquals(o, null))
                    {
                        if (flag)
                        {
                            result.Append(separator);
                        }
                        result.Append(o);
                        flag = true;
                    }
                }
            }

            return result.ToString();
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
                if (c.OneOf(badCharacters) || c == escape)
                {
                    result.Append(escape);
                }
                result.Append(c);
            }

            return result.ToString();
        }

        /// <summary>
        /// Склейка строк в сплошной текст, разделенный переводами строки.
        /// </summary>
        /// <param name="lines">Строки для склейки.</param>
        /// <returns>Склеенный текст.</returns>
        public static string MergeLines
            (
                [NotNull] this IEnumerable<string> lines
            )
        {
            string result = string.Join
                (
                    Environment.NewLine,
                    lines.ToArray()
                );

            return result;
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
        /// Replicates given string.
        /// </summary>
        /// <param name="text">String to replicate.</param>
        /// <param name="times">How many times.</param>
        /// <returns>Replicated string.</returns>
        /// <remarks><c>Replicate ( null, AnyNumber )</c>
        /// yields <c>null</c>.
        /// </remarks>
        [CanBeNull]
        public static string Replicate
            (
                [CanBeNull] string text,
                int times
            )
        {
            if (ReferenceEquals(text, null))
            {
                return null;
            }

            unchecked
            {
                int length = text.Length * times;
                if (length <= 0)
                {
                    return string.Empty;
                }

                StringBuilder result = new StringBuilder(length);
                for (; times > 0; times--)
                {
                    result.Append(text);
                }

                return result.ToString();
            }
        }

        /// <summary>
        /// Сравнение строк. Нечувтствительно к культуре.
        /// </summary>
        public static int SafeCompare
            (
                [CanBeNull] this string s1,
                string s2
            )
        {
            return string.Compare
                (
                    s1,
                    s2,
                    StringComparison.InvariantCultureIgnoreCase
                );
        }

        /// <summary>
        /// Поиск подстроки.
        /// </summary>
        public static bool SafeContains
            (
                [CanBeNull] this string value,
                params string[] list
            )
        {
            if (ReferenceEquals(value, null) || value.Length == 0)
            {
                return false;
            }

            value = value.ToLowerInvariant();
            foreach (string s in list)
            {
                if (value.Contains(s.ToLowerInvariant()))
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
            if (ReferenceEquals(text, null) || text.Length == 0
                || ReferenceEquals(begin, null) || begin.Length == 0)
            {
                return false;
            }

            return text.ToLowerInvariant().StartsWith(begin.ToLowerInvariant());
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
            if (width <= 0)
            {
                return string.Empty;
            }

            string result = text.Substring(offset, width);

            return result;
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
