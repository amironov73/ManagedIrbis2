﻿/* COPYDATASTRUCT.cs -- 
   Ars Magna project, http://library.istu.edu/am */

#region Using directives

using System;
using System.Runtime.InteropServices;

#endregion

namespace AM.Win32
{
	/// <summary>
	/// 
	/// </summary>
	[StructLayout ( LayoutKind.Sequential )]
	public class COPYDATASTRUCT
	{
		/// <summary>
		/// 
		/// </summary>
		public int dwData;
		
		/// <summary>
		/// 
		/// </summary>
		public int cbData;
		
		/// <summary>
		/// 
		/// </summary>
		public IntPtr lpData;
	}
}
