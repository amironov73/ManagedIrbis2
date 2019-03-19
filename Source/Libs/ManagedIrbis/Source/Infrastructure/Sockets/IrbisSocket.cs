// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* IrbisSocket.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace ManagedIrbis.Infrastructure.Sockets
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class IrbisSocket
    {
        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        public abstract Task<ServerResponse> Transact(ClientQuery query);

        #endregion
    }
}
