﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* FORMATRANGE.cs -- format output for a particular device
   Ars Magna project, http://arsmagna.ru */

#region Using directives

using System;
using System.Drawing;
using System.Runtime.InteropServices;

using JetBrains.Annotations;

#endregion

// ReSharper disable InconsistentNaming

namespace AM.Win32
{
    /// <summary>
    /// The FORMATRANGE structure contains information that a rich
    /// edit control uses to format its output for a particular device.
    /// This structure is used with the EM_FORMATRANGE message.
    /// </summary>
    [PublicAPI]
    [StructLayout(LayoutKind.Sequential)]
    public struct FORMATRANGE
    {
        /// <summary>
        /// Device to render to.
        /// </summary>
        public IntPtr hdc;

        /// <summary>
        /// Target device to format for.
        /// </summary>
        public IntPtr hdcTarget;

        /// <summary>
        /// Area to render to. Units are measured in twips.
        /// </summary>
        public Rectangle rc;

        /// <summary>
        /// Entire area of rendering device. Units are measured in twips.
        /// </summary>
        public Rectangle rcPage;

        /// <summary>
        /// CHARRANGE structure that specifies the range of text to format.
        /// </summary>
        public CHARRANGE chrg;
    }
}
