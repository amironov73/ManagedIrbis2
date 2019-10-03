// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* Delimiters.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: good
 */

#region Using directives

using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

#endregion

namespace AM
{
    /// <summary>
    /// Common delimiters.
    /// </summary>
    [PublicAPI]
    [ExcludeFromCodeCoverage]
    public static class Delimiters
    {
        #region Public members

        /// <summary>
        /// Colon.
        /// </summary>
        public static readonly char[] Colon = { ':' };

        /// <summary>
        /// Comma.
        /// </summary>
        public static readonly char[] Comma = { ',' };

        /// <summary>
        /// Dot.
        /// </summary>
        public static readonly char[] Dot = { '.' };

        /// <summary>
        /// Semicolon.
        /// </summary>
        public static readonly char[] Semicolon = { ';' };

        /// <summary>
        /// Space.
        /// </summary>
        public static readonly char[] Space = { ' ' };

        /// <summary>
        /// Tab.
        /// </summary>
        public static readonly char[] Tab = { '\t' };

        /// <summary>
        /// Pipe sign.
        /// </summary>
        public static readonly char[] Pipe = { '|' };

        #endregion
    }
}
