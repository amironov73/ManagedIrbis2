// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* CultureCode.cs --
 * Ars Magna project, https://arsmagna.ru
 * -------------------------------------------------------
 * Status: moderate
 */

#region Using directives

using JetBrains.Annotations;

#endregion

namespace AM.Globalization
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public static class CultureCode
    {
        /// <summary>
        /// American English.
        /// </summary>
        public const string AmericanEnglish = "en-US";

        /// <summary>
        /// Russian in Russia.
        /// </summary>
        public const string Russian = "ru-RU";
    }
}
