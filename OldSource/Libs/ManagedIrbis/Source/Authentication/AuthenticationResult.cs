// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* AuthentificationResult.cs -- authentication result
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
    /// Authentication result.
    /// </summary>
    [PublicAPI]
    public sealed class AuthenticationResult
    {
        #region Properties

        /// <summary>
        /// Successfull?
        /// </summary>
        public bool Success { get; internal set; }

        /// <summary>
        /// Error message (if any).
        /// </summary>
        [CanBeNull]
        public string ErrorMessage { get; internal set; }

        #endregion
    }
}
