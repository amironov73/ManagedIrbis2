﻿/* IMAGEINFO.cs -- 
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
	public class IMAGEINFO
	{
		/// <summary>
		/// 
		/// </summary>
		public IntPtr hbmImage;
		
		/// <summary>
		/// 
		/// </summary>
		public IntPtr hbmMask;
		
		/// <summary>
		/// 
		/// </summary>
		public int Unused1;
		
		/// <summary>
		/// 
		/// </summary>
		public int Unused2;
		
		/// <summary>
		/// 
		/// </summary>
		public int rcImage_left;
		
		/// <summary>
		/// 
		/// </summary>
		public int rcImage_top;
		
		/// <summary>
		/// 
		/// </summary>
		public int rcImage_right;
		
		/// <summary>
		/// 
		/// </summary>
		public int rcImage_bottom;
	}

}
