// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* WriteFileCommand.cs -- write text file(s) to the server
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: moderate
 */

#region Using directives

using AM.Collections;
using AM.Logging;
using AM.Text;

using JetBrains.Annotations;

using ManagedIrbis.Properties;

#endregion

namespace ManagedIrbis.Infrastructure.ClientCommands
{
    /// <summary>
    /// Write text file(s) to the server.
    /// </summary>
    [PublicAPI]
    public sealed class WriteFileCommand
        : ClientCommand
    {
        #region Properties

        /// <summary>
        /// File list.
        /// </summary>
        [NotNull]
        public NonNullCollection<FileSpecification> Files { get; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public WriteFileCommand()
        {
            Files = new NonNullCollection<FileSpecification>();
        }

        #endregion

        #region ClientCommand members

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext)" />
        public override void Execute
            (
                ClientContext context
            )
        {
            if (Files.Count == 0)
            {
                Log.Error
                    (
                        nameof(WriteFileCommand) + "::" + nameof(Execute)
                        + ": " + Resources.NoFilesSpecified
                    );

                throw new IrbisNetworkException(Resources.NoFilesSpecified);
            }

            ClientQuery query = CreateQuery(context, CommandCode.ReadDocument);
            foreach (FileSpecification fileName in Files)
            {
                TextWithEncoding text = new TextWithEncoding
                    (
                        fileName.ToString(),
                        IrbisEncoding.Ansi
                    );
                query.Arguments.Add(text);
            }

            ServerResponse response = BaseExecute(context);
            CheckResponse(response);
        }

        #endregion
    }
}
