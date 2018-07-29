// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* BuiltinCultures.cs --
 * Ars Magna project, https://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Globalization;

using JetBrains.Annotations;

#endregion

namespace AM.Globalization
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public static class BuiltinCultures
    {
        #region Properties

        /// <summary>
        /// American English.
        /// </summary>
        [NotNull]
        public static CultureInfo AmericanEnglish => new CultureInfo(CultureCode.AmericanEnglish);

        /// <summary>
        /// Gets the russian culture.
        /// </summary>
        [NotNull]
        public static CultureInfo Russian => new CultureInfo(CultureCode.Russian);

        #endregion
    }
}
