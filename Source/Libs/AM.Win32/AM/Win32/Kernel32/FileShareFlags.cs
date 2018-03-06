﻿/* FileShareFlags.cs -- file share flags for functions like CreateFile. 
   Ars Magna project, http://library.istu.edu/am */

#region Using directives

using System;

#endregion

namespace AM.Win32
{
	/// <summary>
	/// File share flags for functions like CreateFile.
	/// </summary>
	[Flags]
	public enum FileShareFlags
	{
		/// <summary>
		/// No file sharing.
		/// </summary>
		NONE = 0,

		/// <summary>
		/// <para>Enables subsequent open operations on the object to 
		/// request read access. Otherwise, other processes cannot open 
		/// the object if they request read access.</para>
		/// <para>If the object has already been opened with read access,
		/// the sharing mode must include this flag.</para>
		/// </summary>
		FILE_SHARE_READ = 0x00000001,

		/// <summary>
		/// <para>Enables subsequent open operations on the object to 
		/// request write access. Otherwise, other processes cannot open 
		/// the object if they request write access.</para>
		/// <para>If the object has already been opened with write 
		/// access, the sharing mode must include this flag.</para>
		/// </summary>
		FILE_SHARE_WRITE = 0x00000002,

		/// <summary>
		/// <para>Enables subsequent open operations on the object to 
		/// request delete access. Otherwise, other processes cannot 
		/// open the object if they request delete access.</para>
		/// <para>If the object has already been opened with delete 
		/// access, the sharing mode must include this flag.</para>
		/// <para>Windows Me/98/95:  This flag is not supported.
		/// </para></summary>
		FILE_SHARE_DELETE = 0x00000004
	}
}
