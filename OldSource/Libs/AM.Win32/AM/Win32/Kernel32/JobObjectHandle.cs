// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* JobObjectHandle.cs -- 
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

using JetBrains.Annotations;

#endregion

namespace AM.Win32
{
    /// <summary>
    /// Contains handle for <see cref="WindowsJob"/>.
    /// </summary>
    [PublicAPI]
    public sealed class JobObjectHandle
        : SafeHandle
    {
        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public JobObjectHandle()
            : base(IntPtr.Zero, true)
        {
        }

        #endregion

        #region SafeHandle members

        /// <inheritdoc cref="SafeHandle.IsInvalid" />
        public override bool IsInvalid
        {
            [PrePrepareMethod]
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            get { return (handle == IntPtr.Zero); }
        }

        /// <inheritdoc cref="SafeHandle.ReleaseHandle" />
        [PrePrepareMethod]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle()
        {
            return Kernel32.CloseHandle(handle);
        }

        #endregion
    }
}
