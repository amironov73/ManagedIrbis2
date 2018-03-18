// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* UserContext.cs -- user context
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Authentication
{
    /// <summary>
    /// User context.
    /// </summary>
    [PublicAPI]
    public sealed class UserContext
    {
        #region Properties

        /// <summary>
        /// User name.
        /// </summary>
        public string UserName { get; set; }

        #endregion
    }
}
