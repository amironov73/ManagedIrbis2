﻿/* DLLVERSIONINFO.cs -- 
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
	public class DLLVERSIONINFO
	{
		/// <summary>
		/// 
		/// </summary>
		[CLSCompliant (false)]
		public uint cbSize;

		/// <summary>
		/// 
		/// </summary>
		[CLSCompliant (false)]
		public uint dwMajorVersion;
		
		/// <summary>
		/// 
		/// </summary>
		[CLSCompliant (false)]
		public uint dwMinorVersion;
		
		/// <summary>
		/// 
		/// </summary>
		[CLSCompliant (false)]
		public uint dwBuildNumber;
		
		/// <summary>
		/// 
		/// </summary>
		[CLSCompliant (false)]
		public uint dwPlatformID;
	}
}
