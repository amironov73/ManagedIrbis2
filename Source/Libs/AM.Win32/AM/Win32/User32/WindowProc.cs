﻿/* WindowProc.cs --  
   Ars Magna project, http://library.istu.edu/am */

#region Using directives

using System;

#endregion

namespace AM.Win32
{
	/// <summary>
	/// The WindowProc function is an application-defined function that 
	/// processes messages sent to a window. The WNDPROC type defines a 
	/// pointer to this callback function. WindowProc is a placeholder for 
	/// the application-defined function name.
	/// </summary>
	public delegate int WindowProc
	(          
		IntPtr hwnd,
		int uMsg,
		int wParam,
		int lParam
	);
}
