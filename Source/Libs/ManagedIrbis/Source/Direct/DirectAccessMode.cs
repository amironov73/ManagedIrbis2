// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* DirectAccessMode.cs -- modes for database file direct access
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Direct
{
    /// <summary>
    /// Modes for database file direct access.
    /// </summary>
    [PublicAPI]
    public enum DirectAccessMode
    {
        /// <summary>
        /// Exclusive access mode.
        /// </summary>
        Exclusive,

        /// <summary>
        /// Shared access mode.
        /// </summary>
        Shared,

        /// <summary>
        /// Read-only access mode.
        /// </summary>
        ReadOnly

    } // enum DirectAccessMode
}
