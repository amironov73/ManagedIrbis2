// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* UpdateIniFileCommand.cs -- unlock the database
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
    /// Unlock the database on the server.
    /// </summary>
    [PublicAPI]
    public class UpdateIniFileCommand
        : ClientCommand
    {
        #region Properties

        /// <summary>
        /// Text to update.
        /// </summary>
        [CanBeNull]
        public string[] Lines { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public UpdateIniFileCommand
            (
                [NotNull] IIrbisConnection connection
            )
            : base(connection)
        {
        }

        #endregion

        #region AbstractCommand members

        /// <inheritdoc cref="ClientCommand.Execute()" />
        public override ServerResponse Execute()
        {
            ClientQuery query = CreateQuery();
            query.CommandCode = CommandCode.UpdateIniFile;

            if (Lines != null)
            {
                foreach (string line in Lines)
                {
                    query.AddAnsi(line);
                }
            }

            ServerResponse result = base.Execute(query);
            result.GetReturnCode();

            return result;
        }

        #endregion
    }
}
