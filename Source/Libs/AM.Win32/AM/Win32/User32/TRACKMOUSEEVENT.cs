﻿/* TRACKMOUSEEVENT.cs --  
   Ars Magna project, http://library.istu.edu/am */

#region Using directives

using System;
using System.Runtime.InteropServices;

#endregion

namespace AM.Win32
{
	/// <summary>
	/// The TRACKMOUSEEVENT structure is used by the TrackMouseEvent 
	/// function to track when the mouse pointer leaves a window or 
	/// hovers over a window for a specified amount of time.
	/// </summary>
	[Serializable]
	[StructLayout ( LayoutKind.Sequential )]
	public struct TRACKMOUSEEVENT
	{
		/// <summary>
		/// Specifies the size of the TRACKMOUSEEVENT structure.
		/// </summary>
		public int cbSize;

		/// <summary>
		/// Specifies the services requested.
		/// </summary>
		public TrackMouseEventFlags dwFlags;

		/// <summary>
		/// Specifies a handle to the window to track.
		/// </summary>
		public IntPtr hwndTrack;

		/// <summary>
		/// Specifies the hover time-out (if TME_HOVER was specified in 
		/// dwFlags), in milliseconds. Can be HOVER_DEFAULT, which means 
		/// to use the system default hover time-out.
		/// </summary>
		public int dwHoverTime;
	}
}
