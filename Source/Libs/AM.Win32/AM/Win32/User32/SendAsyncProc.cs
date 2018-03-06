﻿/* SendAsyncProc.cs --  
   Ars Magna project, http://library.istu.edu/am */

#region Using directives

using System;

#endregion

namespace AM.Win32
{
	/// <summary>
	/// The SendAsyncProc function is an application-defined callback 
	/// function used with the SendMessageCallback function. The system 
	/// passes the message to the callback function after passing the 
	/// message to the destination window procedure. The SENDASYNCPROC 
	/// type defines a pointer to this callback function. SendAsyncProc 
	/// is a placeholder for the application-defined function name.
	/// </summary>
	public delegate void SendAsyncProc
	(      
		IntPtr hwnd,
		int uMsg,
		IntPtr dwData,
		int lResult
	);
}
