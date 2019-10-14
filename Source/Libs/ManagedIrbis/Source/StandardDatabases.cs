// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* StandardDatabases.cs -- standard databases included in the IRBIS64 distribution
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: moderate
 */

#region Using directives

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis
{
    /// <summary>
    /// Standard databases included in the IRBIS64 distribution.
    /// </summary>
    [PublicAPI]
    public static class StandardDatabases
    {
        #region Constants

        /// <summary>
        /// Digital catalogue.
        /// </summary>
        public const string ElectronicCatalog = "IBIS";

        /// <summary>
        /// Picking.
        /// </summary>
        public const string Acquisition = "CMPL";

        /// <summary>
        /// Readers.
        /// </summary>
        public const string Readers = "RDR";

        /// <summary>
        /// Orders for books.
        /// </summary>
        public const string Requests = "RQST";

        #endregion
    }
}
