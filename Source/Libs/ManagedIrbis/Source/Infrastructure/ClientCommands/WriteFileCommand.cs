// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* WriteFileCommand.cs -- write text file(s) to the server
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: moderate
 */

#region Using directives

using AM;
using AM.Collections;
using AM.Text;

using JetBrains.Annotations;

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
        public NonNullCollection<FileSpecification> Files
        {
            get; private set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public WriteFileCommand()
            //(
            //    [NotNull] IIrbisConnection connection
            //)
            //: base(connection)
        {
            Files = new NonNullCollection<FileSpecification>();
        }

        #endregion

        #region ClientCommand members

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext)" />
        public override ServerResponse Execute
            (
                ClientContext context
            )
        {
            IIrbisConnection connection = context.Connection;

            ClientQuery query = CreateQuery(connection);
            query.CommandCode = CommandCode.ReadDocument;

            foreach (FileSpecification fileName in Files)
            {
                TextWithEncoding text = new TextWithEncoding
                (
                    fileName.ToString(),
                    IrbisEncoding.Ansi
                );
                query.Arguments.Add(text);
            }

            ServerResponse result = Execute(connection, query);

            return result;
        }

        #endregion
    }
}
