// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* BuiltinCultures.cs -- well-known built-in cultures
 * Ars Magna project, https://arsmagna.ru
 * -------------------------------------------------------
 * Status: moderate
 */

#region Using directives

using System.Globalization;

using JetBrains.Annotations;

#endregion

namespace AM.Globalization
{
    /// <summary>
    /// Well-known built-in cultures.
    /// </summary>
    [PublicAPI]
    public static class BuiltinCultures
    {
        #region Properties

        /// <summary>
        /// American English.
        /// </summary>
        public static CultureInfo AmericanEnglish => new CultureInfo(CultureCode.AmericanEnglish);

        /// <summary>
        /// Russian culture (just russian, not ru-RU).
        /// </summary>
        public static CultureInfo Russian => new CultureInfo(CultureCode.Russian);

        #endregion
    }
}
