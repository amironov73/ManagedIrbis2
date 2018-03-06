﻿/* StdHandles.cs --  
   Ars Magna project, http://library.istu.edu/am */

#region Using directives

using System;

#endregion

namespace AM.Win32
{
	/// <summary>
	/// Standard handles.
	/// </summary>
    [CLSCompliant ( false )]
    public enum StdHandles : uint
	{
		/// <summary>
		/// Handle to the standard input device. 
		/// Initially, this is a handle to the console input buffer, 
		/// CONIN$.
		/// </summary>
		STD_INPUT_HANDLE    = unchecked ( (uint) -10 ),

		/// <summary>
		/// Handle to the standard output device. 
		/// Initially, this is a handle to the console screen 
		/// buffer, CONOUT$.
		/// </summary>
		STD_OUTPUT_HANDLE   = unchecked ( (uint) -11 ),

		/// <summary>
		/// Handle to the standard error device. 
		/// Initially, this is a handle to the console screen buffer, 
		/// CONOUT$.
		/// </summary>
		STD_ERROR_HANDLE    = unchecked ( (uint) -12 )
	}
}
