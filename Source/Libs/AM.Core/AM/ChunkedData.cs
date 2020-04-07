// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ChunkedData.cs -- Data in chunks
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

#endregion

namespace AM
{
    /// <summary>
    /// Data in chunks.
    /// </summary>
    public class ChunkedData<T>
    {
        #region Properties

        /// <summary>
        /// List of chunks.
        /// </summary>
        public List<Memory<T>> Chunks { get; private set; } = new List<Memory<T>>();

        /// <summary>
        /// Total size.
        /// </summary>
        public int Size
        {
            get
            {
                int result = Chunks.Sum(chunk => chunk.Length);
                return result;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Append the chunk.
        /// </summary>
        public ChunkedData<T> Append
            (
                Memory<T> chunk
            )
        {
            Chunks.Add (chunk);
            return this;
        }


        #endregion
    }
}
