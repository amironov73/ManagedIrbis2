﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ListProcessesCommand.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Infrastructure.ClientCommands
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public class ListProcessesCommand
        : ClientCommand
    {
        #region Properties

        /// <summary>
        /// Result.
        /// </summary>
        [CanBeNull]
        public IrbisProcessInfo[] Result { get; set; }

        #endregion

        #region ClientCommand members

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext)" />
        public override void Execute
            (
                ClientContext context
            )
        {
            CreateQuery(context, CommandCode.GetProcessList);
            ServerResponse response = BaseExecute(context);
            CheckResponse(response);
            Result = IrbisProcessInfo.Parse(response);
        }

        #endregion
    }
}
