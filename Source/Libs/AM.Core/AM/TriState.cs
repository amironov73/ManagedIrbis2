// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* TriState.cs -- three-state logic
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: good
 */

#region Using directives

using JetBrains.Annotations;

#endregion

namespace AM
{
    /// <summary>
    /// Three-state logic.
    /// </summary>
    /// <remarks>See
    /// https://en.wikipedia.org/wiki/Three-state_logic
    /// </remarks>
    [PublicAPI]
    public enum TriState
    {
        /// <summary>
        /// False.
        /// </summary>
        False,

        /// <summary>
        /// True.
        /// </summary>
        True,

        /// <summary>
        /// Undefined.
        /// </summary>
        Undefined
    }
}
