﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* IOleControl.cs -- 
   Ars Magna project, http://arsmagna.ru */

#region Using directives

using System.Runtime.InteropServices;

#endregion

namespace AM.Win32
{
	/// <summary>
	/// 
	/// </summary>
	[ComImport]
	[Guid ( "B196B288-BAB4-101A-B69C-00AA00341D07" )]
	[InterfaceType ( ComInterfaceType.InterfaceIsIUnknown )]
	public interface IOleControl
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pCI"></param>
		/// <returns></returns>
		[PreserveSig]
		int GetControlInfo ( [Out] object pCI );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pMsg"></param>
		/// <returns></returns>
		[PreserveSig]
		int OnMnemonic ( [In] ref MSG pMsg );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dispID"></param>
		/// <returns></returns>
		[PreserveSig]
		int OnAmbientPropertyChange ( int dispID );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="bFreeze"></param>
		/// <returns></returns>
		[PreserveSig]
		int FreezeEvents ( int bFreeze );
	}
}