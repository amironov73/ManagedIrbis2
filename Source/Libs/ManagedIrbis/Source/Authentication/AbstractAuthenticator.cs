// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* AbstractAuthenticator.cs -- abstract authenticator
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Authentication
{
    //
    // Аутентификация (authentification) aka проверка подлинности
    // — это процесс проверки учётных данных пользователя
    // (обычно имени и пароля). Другими словами введённые
    // учётные данные сверяются с данными, хранящимися в базе данных.
    //

    /// <summary>
    /// Abstract authenticator.
    /// </summary>
    [PublicAPI]
    public abstract class AbstractAuthenticator
    {
        #region Public methods

        /// <summary>
        /// Authenticate given user according provided role.
        /// </summary>
        [NotNull]
        public abstract AuthenticationResult Authenticate
            (
                [NotNull] IrbisCredentials credentials
            );

        /// <summary>
        /// Get or create context for authenticated user.
        /// </summary>
        [NotNull]
        public abstract UserContext GetContext
            (
                [NotNull] AuthenticationResult authentication
            );

        #endregion
    }
}
