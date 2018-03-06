﻿/* BoundsRectFlags.cs -- 
   Ars Magna project, http://library.istu.edu/am */

#region Using directives

using System;

#endregion

namespace AM.Win32
{
	/// <summary>
	/// Flags for bounding rectangle.
	/// </summary>
	[Flags]
	public enum BoundsRectFlags
	{
		/// <summary>
		/// Error.
		/// </summary>
		ERROR = 0,

		/// <summary>
		/// Clears the bounding rectangle after returning it. 
		/// If this flag is not set, the bounding rectangle will 
		/// not be cleared.
		/// </summary>
		DCB_RESET = 0x0001,

		/// <summary>
		/// 
		/// </summary>
		DCB_ACCUMULATE = 0x0002,

		/// <summary>
		/// 
		/// </summary>
		DCB_DIRTY = DCB_ACCUMULATE,

		/// <summary>
		/// 
		/// </summary>
		DCB_SET = ( DCB_RESET | DCB_ACCUMULATE ),

		/// <summary>
		/// 
		/// </summary>
		DCB_ENABLE = 0x0004,

		/// <summary>
		/// 
		/// </summary>
		DCB_DISABLE = 0x0008
	}
}
