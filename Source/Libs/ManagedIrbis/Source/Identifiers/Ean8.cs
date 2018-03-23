// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* Ean8.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using AM;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Identifiers
{
    //
    // Information from Wikipedia
    // https://ru.wikipedia.org/wiki/European_Article_Number
    //
    // European Article Number, EAN (европейский номер
    // товара) — европейский стандарт штрихкода,
    // предназначенный для кодирования идентификатора
    // товара и производителя. Является надмножеством
    // американского стандарта UPC.
    //
    // Пример проверки контрольной суммы
    // 46009333 (папиросы «Беломорканал») — код EAN-8.
    // 4x3 + 6x1 + 0x3 + 0x1 + 9x3 + 3x1 + 3x3 + 3x1=
    // 12 + 6 + 0 + 0 + 27 + 3 + 9 + 3= 60.
    // Контрольная сумма = 0 — номер правильный.
    //

    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public sealed class Ean8
    {
        #region Private data

        /// <summary>
        /// Coefficients for control digit calculation.
        /// </summary>
        private static int[] _coefficients = { 3, 1, 3, 1, 3, 1, 3, 1 };

        #endregion

        #region Public methods

        /// <summary>
        /// Compute check digit.
        /// </summary>
        public static char ComputeCheckDigit
            (
                [NotNull] char[] digits
            )
        {
            Sure.NotNull(digits, nameof(digits));

            int sum = 0;
            for (int i = 0; i < 7; i++)
            {
                sum = sum + (digits[i] - '0') * _coefficients[i];
            }
            char result = (char)(10 - sum % 10 + '0');

            return result;
        }

        /// <summary>
        /// Check control digit.
        /// </summary>
        public static bool CheckControlDigit
            (
                [NotNull] char[] digits
            )
        {
            Sure.NotNull(digits, nameof(digits));

            int sum = 0;
            for (int i = 0; i < 8; i++)
            {
                sum = sum + (digits[i] - '0') * _coefficients[i];
            }
            bool result = sum % 10 == 0;

            return result;
        }

        #endregion
    }
}
