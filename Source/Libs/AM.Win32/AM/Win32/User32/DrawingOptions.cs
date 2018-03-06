﻿/* DrawingOptions.cs --  
   Ars Magna project, http://library.istu.edu/am */

#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

#endregion

namespace AM.Win32
{
	/// <summary>
	/// Опции, используемые с сообщениями WM_PRINT и
	/// WM_PRINTCLIENT
	/// </summary>
	[Flags]
	public enum DrawingOptions
	{
        /// <summary>
        /// 
        /// </summary>
		PRF_CHECKVISIBLE = 0x00000001,

        /// <summary>
        /// 
        /// </summary>
		PRF_NONCLIENT = 0x00000002,

        /// <summary>
        /// 
        /// </summary>
		PRF_CLIENT = 0x00000004,

        /// <summary>
        /// 
        /// </summary>
		PRF_ERASEBKGND = 0x00000008,

        /// <summary>
        /// 
        /// </summary>
		PRF_CHILDREN = 0x00000010,

        /// <summary>
        /// 
        /// </summary>
		PRF_OWNED = 0x00000020
	}
}
