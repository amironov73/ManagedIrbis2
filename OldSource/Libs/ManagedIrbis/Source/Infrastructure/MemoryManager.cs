﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* MemoryManager.cs -- memory manager for IRBIS client
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.IO;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Infrastructure
{
    /// <summary>
    /// Memory manager for IRBIS client.
    /// </summary>
    static class MemoryManager
    {
        #region Construction

        static MemoryManager()
        {
#if !WINMOBILE && !PocketPC

            MemoryManager.Manager
                = new Microsoft.IO.RecyclableMemoryStreamManager ();

#endif
        }

        #endregion

        #region Private members

        private const string Tag = "IRBIS";

        private static readonly Microsoft.IO.RecyclableMemoryStreamManager Manager;

        #endregion

        #region Public methods

        /// <summary>
        /// Get the memory stream.
        /// </summary>
        [NotNull]
        public static MemoryStream GetMemoryStream()
        {
            return Manager.GetStream(Tag);

        }

        /// <summary>
        /// Get the memory stream.
        /// </summary>
        [NotNull]
        public static MemoryStream GetMemoryStream
            (
                int initialSize
            )
        {
            return Manager.GetStream(Tag, initialSize);

        }

        #endregion
    }
}
