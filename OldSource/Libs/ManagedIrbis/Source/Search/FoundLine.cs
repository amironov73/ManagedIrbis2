﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* FoundLine.cs -- line in list of found documents
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Search
{
    /// <summary>
    /// Line in list of found documents.
    /// </summary>
    [PublicAPI]
    public sealed class FoundLine
    {
        #region Properties

        /// <summary>
        /// Whether the line is materialized?
        /// </summary>
        public bool Materialized { get; set; }

        /// <summary>
        /// Serial number.
        /// </summary>
        public int SerialNumber { get; set; }

        /// <summary>
        /// MFN.
        /// </summary>
        public int Mfn { get; set; }

        /// <summary>
        /// Icon.
        /// </summary>
        [CanBeNull]
        public object Icon { get; set; }

        /// <summary>
        /// Selected by user.
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// Description.
        /// </summary>
        [CanBeNull]
        public string Description { get; set; }

        /// <summary>
        /// For list sorting.
        /// </summary>
        [CanBeNull]
        public string Sort { get; set; }

        /// <summary>
        /// Arbitrary user data.
        /// </summary>
        [CanBeNull]
        public object UserData { get; set; }

        #endregion
    }
}
