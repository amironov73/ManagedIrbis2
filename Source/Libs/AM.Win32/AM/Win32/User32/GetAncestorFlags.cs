﻿/* GetAncestorFlags.cs --  
   Ars Magna project, http://library.istu.edu/am */

#region Using directives

using System;

#endregion

namespace AM.Win32
{
	/// <summary>
	/// Specifies the ancestor to be retrieved.
	/// </summary>
	public enum GetAncestorFlags : int
	{
		/// <summary>
		/// Retrieves the parent window. This does not include the 
		/// owner, as it does with the GetParent function.
		/// </summary>
		GA_PARENT = 1,

		/// <summary>
		/// Retrieves the root window by walking the chain of parent 
		/// windows.
		/// </summary>
		GA_ROOT = 2,

		/// <summary>
		/// Retrieves the owned root window by walking the chain 
		/// of parent and owner windows returned by GetParent.
		/// </summary>
		GA_ROOTOWNER = 3
	}
}
