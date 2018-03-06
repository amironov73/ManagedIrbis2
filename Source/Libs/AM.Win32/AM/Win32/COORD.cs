﻿/* COORD.cs -- defines the coordinates of a character cell 
   Ars Magna project, http://library.istu.edu/am */

#region Using directives

using System;
using System.Runtime.InteropServices;

#endregion

namespace AM.Win32
{
	/// <summary>
	/// The COORD structure defines the coordinates of a character cell 
	/// in a console screen buffer. The origin of the coordinate system 
	/// (0,0) is at the top, left cell of the buffer.
	/// </summary>
	[Serializable]
	[StructLayout ( LayoutKind.Explicit, Size = 4 )]
	public struct COORD
	{
		/// <summary>
		/// Horizontal coordinate or column value.
		/// </summary>
		[FieldOffset ( 0 )]
		public short X;

		/// <summary>
		/// Vertical coordinate or row value.
		/// </summary>
		[FieldOffset ( 2 )]
		public short Y;
	}
}