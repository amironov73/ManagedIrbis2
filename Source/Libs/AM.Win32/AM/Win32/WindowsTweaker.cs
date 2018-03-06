﻿/* WindowsTweaker.cs -- 
   Ars Magna project, http://library.istu.edu/am */

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Win32;

#endregion

namespace AM.Win32
{
	/// <summary>
	/// 
	/// </summary>
	public static class WindowsTweaker
	{
		#region Properties
		#endregion

		#region Private members
		#endregion

		#region Public methods

		/// <summary>
		/// Sets the legal notice at logon time.
		/// </summary>
		/// <param name="caption">The caption.</param>
		/// <param name="noticeText">The message text.</param>
		public static void SetLegalNotice
			(
				string caption,
				string noticeText
			)
		{
			using (RegistryKey key = Registry.LocalMachine.OpenSubKey
				(
					@"Software\Microsoft\WindowsNT\CurrentVersion\Winlogon",
					true
				))
			{
				key.SetValue 
					( 
						"LegalNoticeCaption",
						caption,
						RegistryValueKind.String
					);
				key.SetValue 
					( 
						"LegalNoticeText",
						noticeText,
						RegistryValueKind.String
					);
			}
		}

		#endregion
	}
}
