// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* Upc12.cs --
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
    // https://ru.wikipedia.org/wiki/Universal_Product_Code
    //
    // UPC или Universal Product Code (универсальный код
    // товара) — американский стандарт штрихкода,
    // предназначенный для отслеживания товаров в магазинах.
    // UPC был разработан в 1973 году Джорджем Джосефом
    // Лорером (George Joseph Laurer), работавшим инженером
    // в корпорации IBM. В июне 1974 года первый UPC сканер
    // производства корпорации NCR был установлен
    // в супермаркете Марш (Marsh) в городе Трой (Troy)
    // штата Огайо. 26 июня 1974 года кассиром этого
    // супермаркета был просканирован первыйтовар — блок
    // 10 фруктовых жевательных резинок компании Wrigley.
    //
    // Пример проверки контрольной суммы
    // 041689300494 (бензин для зажигалки «Zippo») — код UPC-12.
    // 0x3 + 4x1 + 1x3 + 6x1 + 8x3 + 9x1 + 3x3 + 0x1 + 0x3 + 4x1 + 9x3 + 4x1=
    // 0 + 4 + 3 + 6 + 24 + 9 + 9 + 0 + 0 + 4 + 27 + 4= 90.
    // Контрольная сумма = 0 — номер правильный.
    //

    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public sealed class Upc12
    {
        #region Public data

        /// <summary>
        /// Coefficients for control digit calculation.
        /// </summary>
        private static int[] _coefficients = { 3, 1, 3, 1, 3, 1, 3, 1, 3, 1, 3, 1 };

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
            for (int i = 0; i < 11; i++)
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
            for (int i = 0; i < 12; i++)
            {
                sum = sum + (digits[i] - '0') * _coefficients[i];
            }
            bool result = sum % 10 == 0;

            return result;
        }

        #endregion
    }
}
