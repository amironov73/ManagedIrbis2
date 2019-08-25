// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* CharUtility.cs -- helpers for System.Char
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Collections.Generic;

using JetBrains.Annotations;

#endregion

namespace AM
{
    /// <summary>
    /// Helpers for <see cref="System.Char"/>.
    /// </summary>
    [PublicAPI]
    public static class CharUtility
    {
        #region Public methods

        /// <summary>
        /// Is digit from 0 to 9?
        /// </summary>
        public static bool IsArabicDigit(this char c)
            => c >= '0' && c <= '9';

        /// <summary>
        /// Is letter from A to Z or a to z?
        /// </summary>
        public static bool IsLatinLetter(this char c)
            => c >= 'A' && c <= 'Z'
               || c >= 'a' && c <= 'z';

        /// <summary>
        /// Is digit from 0 to 9
        /// or letter from A to Z or a to z?
        /// </summary>
        public static bool IsLatinLetterOrArabicDigit(this char c)
            => c >= '0' && c <= '9'
               || c >= 'A' && c <= 'Z'
               || c >= 'a' && c <= 'z';

        /// <summary>
        /// Is letter from А to Я or а to я?
        /// </summary>
        public static bool IsRussianLetter(this char c)
            => c >= 'А' && c <= 'я'
               || c == 'Ё' || c == 'ё';

        /// <summary>
        /// Is URL-safe char?
        /// </summary>
        /// <remarks>Set of safe chars, from RFC 1738.4 minus '+'</remarks>
        public static bool IsUrlSafeChar
            (
                char ch
            )
        {
            if (ch >= 'a' && ch <= 'z'
                || ch >= 'A' && ch <= 'Z'
                || ch >= '0' && ch <= '9'
                )
            {
                return true;
            }

            switch (ch)
            {
                case '-':
                case '_':
                case '.':
                case '!':
                case '*':
                case '(':
                case ')':
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
        public static bool SameChar(this char one, char two)
            => char.ToUpperInvariant(one) == char.ToUpperInvariant(two);

        #endregion
    }
}
