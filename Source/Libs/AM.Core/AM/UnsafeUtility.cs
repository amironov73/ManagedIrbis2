// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* StreamUtility.cs -- stream manipulation routines.
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using AM.Logging;

using JetBrains.Annotations;

using static System.Runtime.InteropServices.MemoryMarshal;

#endregion

namespace AM
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public static class UnsafeUtility
    {
        #region Public methods

        /// <summary>
        ///
        /// </summary>
        public static Span<byte> AsSpan<T>
            (
                ref T value
            )
            where T: struct
        {
            int size = Unsafe.SizeOf<T>();

            return AsBytes(CreateSpan(ref value, size));
        }

        #endregion
    }
}
