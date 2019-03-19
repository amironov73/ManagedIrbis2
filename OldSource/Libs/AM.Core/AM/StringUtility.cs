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

            foreach (string other in others)
            {
                if (!string.IsNullOrEmpty(other))
                {
                    return other;
                }
            }

            return null;
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
        /// Replace control characters in the text.
        /// </summary>
        [CanBeNull]
        public static string ReplaceControlCharacters
            (
                [CanBeNull] string text
            )
        {
            return ReplaceControlCharacters(text, ' ');
        }

        /// <summary>
        /// Replace control characters in the text.
        /// </summary>
        [CanBeNull]
        public static string ReplaceControlCharacters
            (
                [CanBeNull] string text,
                char substitute
            )
        {
            if (ReferenceEquals(text, null) || text.Length == 0)
            {
                return text;
            }

            bool needReplace = false;
            foreach (char c in text)
            {
                if (c < ' ')
                {
                    needReplace = true;
                    break;
                }
            }

            if (!needReplace)
            {
                return text;
            }

            StringBuilder result = new StringBuilder(text.Length);

            foreach (char c in text)
            {
                result.Append
                (
                    c < ' ' ? substitute : c
                );
            }

            return result.ToString();
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
            for (int i = 0; i < list.Length; i++)
            {
                string s = list[i];
                if (!string.IsNullOrEmpty(s))
                {
                    if (value.Contains(s.ToLowerInvariant()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Разбивает строку по указанному разделителю.
        /// </summary>
        [NotNull]
        public static string[] SplitFirst
            (
                [NotNull] this string line,
                char delimiter
            )
        {
            int index = line.IndexOf(delimiter);
            string[] result = index < 0
                ? new[] { line }
                : new[]
                {
                    line.Substring(0, index),
                    line.Substring(index + 1)
                };

            return result;
        }

        /// <summary>
        /// Разбивка текста на отдельные строки.
        /// </summary>
        /// <remarks>Пустые строки не удаляются.</remarks>
        /// <param name="text">Текст для разбиения.</param>
        /// <returns>Массив строк.</returns>
        [NotNull]
        public static string[] SplitLines
            (
                [NotNull] this string text
            )
        {
            text = text.Replace("\r\n", "\n");

            string[] result = text.Split('\n');

            return result;
        }


        #endregion
    }
}
