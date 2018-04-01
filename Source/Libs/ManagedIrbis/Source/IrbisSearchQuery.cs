// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* IrbisSearchQuery.cs -- IRBIS search query
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis
{
    /// <summary>
    /// IRBIS search query.
    /// </summary>
    [PublicAPI]
    public sealed class IrbisSearchQuery
    {
        #region Properties

        /// <summary>
        /// Запрещенные символы.
        /// </summary>
        public static char[] ForbiddenCharacters =
        {
            '\r', '\n', '\t'
        };

        #endregion

        #region Public methods

        /// <summary>
        /// Подготавливает строку запроса,
        /// заменяя запрещённые символы на пробелы.
        /// </summary>
        [CanBeNull]
        public static string PrepareQuery
            (
                [CanBeNull] string text
            )
        {
            if (ReferenceEquals(text, null) || text.Length == 0)
            {
                return text;
            }

            return text
                .Replace('\r', ' ')
                .Replace('\n', ' ')
                .Replace('\t', ' ');
        }

        #endregion
    }
}
