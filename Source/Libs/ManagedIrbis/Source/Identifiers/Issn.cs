﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* Issn.cs -- ISSN
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Collections.Generic;

using AM;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Identifiers
{
    //
    // Международный стандартный серийный номер
    // (англ. International Standard Serial Number — ISSN)
    // — уникальный номер, позволяющий идентифицировать любое
    // периодическое издание независимо от того, где оно издано,
    // на каком языке, на каком носителе. Состоит из восьми цифр.
    // Восьмая цифра — контрольное число, рассчитываемое
    // по предыдущим семи и модулю 11.
    //
    // История создания
    // Стандарт ISO 3297, определяющий правила присвоения ISSN,
    // был введён в 1975 году. Управление процессом присвоения ISSN
    // осуществляется из 75 национальных центров. Их координацию
    // осуществляет Международный центр, расположенный в Париже,
    // при поддержке ЮНЕСКО и правительства Франции.
    //
    // Национальное агентство ISSN в Российской книжной палате
    // В 1989 году в СССР был введён ГОСТ 7.56-89, который
    // с 1 января 2003 года был заменён на ГОСТ 7.56-2002.
    // С 1 января 2016 года в составе РКП начало работу
    // Национальное агентство ISSN в России.
    // Национальное агентство ISSN было учреждено 3 декабря 2015 года,
    // когда между Международным центром ISSN и ИТАР-ТАСС было
    // подписано рабочее соглашение об основании Национального
    // центра ISSN Российской Федерации. В своей работе Национальное
    // агентство руководствуется Международным стандартом ISO 3297-2007,
    // ГОСТ 7.56-2002 и Федеральным законом об обязательном
    // экземпляре документов (77-ФЗ).
    //
    // См. https://ru.wikipedia.org/wiki/%D0%9C%D0%B5%D0%B6%D0%B4%D1%83%D0%BD%D0%B0%D1%80%D0%BE%D0%B4%D0%BD%D1%8B%D0%B9_%D1%81%D1%82%D0%B0%D0%BD%D0%B4%D0%B0%D1%80%D1%82%D0%BD%D1%8B%D0%B9_%D1%81%D0%B5%D1%80%D0%B8%D0%B9%D0%BD%D1%8B%D0%B9_%D0%BD%D0%BE%D0%BC%D0%B5%D1%80
    // https://en.wikipedia.org/wiki/International_Standard_Serial_Number
    //
    // Пример расчёта контрольной суммы
    // ISSN 0033-765X (журнал — «Радио», 2-2006.)
    // 0x8 + 0x7 + 3x6 + 3x5 + 7x4 + 6x3 + 5x2 + 10x1 =
    // 0   + 0   + 18  + 15  + 28  + 18  + 10  + 10   = 99 = 9 * 11 + 0(остаток).
    //
    // Контрольная сумма(остаток) = 0 — номер правильный.
    //

    /// <summary>
    /// Всё, связанное с ISSN.
    /// </summary>
    [PublicAPI]
    public static class Issn
    {
        #region Public data

        /// <summary>
        /// Coefficients for control digit calculation.
        /// </summary>
        private static int[] Coefficients = { 8, 7, 6, 5, 4, 3, 2, 1 };

        #endregion

        #region Private members

        private static int ConvertDigit(char c) => c == 'X' || c == 'x' ? 10 : c - '0';

        private static char ConvertDigit(int n) => n == 10 ? 'X' : (char)('0' + n);

        private static bool IsDigitX(char c) => c >= '0' && c <= '9' || c == 'X';

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
                sum = sum + ConvertDigit(digits[i]) * Coefficients[i];
            }
            char result = ConvertDigit(11 - sum % 11);

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

            if (digits.Length != 8)
            {
                return false;
            }

            int sum = 0;
            for (int i = 0; i < 8; i++)
            {
                int n = ConvertDigit(digits[i]);
                sum = sum + n * Coefficients[i];
            }

            bool result = sum % 11 == 0;

            return result;
        }

        /// <summary>
        /// Check control digit.
        /// </summary>
        public static bool CheckControlDigit
            (
                [NotNull] string issn
            )
        {
            Sure.NotNullNorEmpty(issn, nameof(issn));

            List<char> digits = new List<char>(issn.Length);
            foreach (char c in issn)
            {
                if (IsDigitX(c))
                {
                    digits.Add(c);
                }
            }

            bool result = CheckControlDigit(digits.ToArray());

            return result;
        }

        #endregion
    }
}
