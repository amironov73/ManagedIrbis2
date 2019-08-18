// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* IrbisConstants.cs -- common constants
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: moderate
 */

#region Using directives

using JetBrains.Annotations;

#endregion

// ReSharper disable CommentTypo

namespace ManagedIrbis
{
    /// <summary>
    /// Common constants.
    /// </summary>
    [PublicAPI]
    public static class IrbisConstants
    {
        #region Constants

        /// <summary>
        /// File name of the database list for administrator.
        /// </summary>
        public const string AdministratorDatabaseList = "dbnam1.mnu";

        /// <summary>
        /// File name of the database list for cataloger.
        /// </summary>
        public const string CatalogerDatabaseList = "dbnam2.mnu";

        /// <summary>
        /// Record length (shelf size) is the format limitation.
        /// </summary>
        public const int MaxRecord = 32000;

        /// <summary>
        /// Max postings in the packet.
        /// </summary>
        public const int MaxPostings = 32758;

        /// <summary>
        /// File name of the database list for reader.
        /// </summary>
        public const string ReaderDatabaseList = "dbnam3.mnu";

        #endregion
    }
}
