﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ACPowerState.cs -- AC power status.
   Ars Magna project, http://arsmagna.ru */

namespace AM.Win32
{
	/// <summary>
	/// AC power status.
	/// </summary>
	public enum ACPowerStatus : byte
	{
		/// <summary>
		/// Offline.
		/// </summary>
		Offline = 0,

		/// <summary>
		/// Online.
		/// </summary>
		Online = 1,

		/// <summary>
		/// Unknown.
		/// </summary>
		Unknown = 255
	}
}