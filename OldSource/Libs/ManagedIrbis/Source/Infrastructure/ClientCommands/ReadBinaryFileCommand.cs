// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ReadBinaryFileCommand.cs -- read binary file from the server
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: moderate
 */

#region Using directives

using System.Text;

using AM;
using AM.Logging;

using JetBrains.Annotations;

using ManagedIrbis.Properties;

#endregion

namespace ManagedIrbis.Infrastructure.ClientCommands
{
    /// <summary>
    /// Read binary file from the server.
    /// </summary>
    [PublicAPI]
    public sealed class ReadBinaryFileCommand
        : ClientCommand
    {
        #region Constants

        /// <summary>
        /// Preamble for binary data.
        /// </summary>
        public const string Preamble = "IRBIS_BINARY_DATA";

        #endregion

        #region Properties

        /// <summary>
        /// File to read.
        /// </summary>
        [CanBeNull]
        public FileSpecification File { get; set; }

        /// <summary>
        /// File content.
        /// </summary>
        [CanBeNull]
        public byte[] Content { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public ReadBinaryFileCommand()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ReadBinaryFileCommand
            (
                [NotNull] FileSpecification file
            )
        {
            Sure.NotNull(file, nameof(file));

            File = file;
        }

        #endregion

        #region Private members

        private static int _FindPreamble
            (
                byte[] buffer,
                byte[] preamble
            )
        {
            int bufferLength = buffer.Length;
            int preambleLength = preamble.Length;
            bufferLength -= preambleLength;

            for (int i = 0; i < bufferLength; i++)
            {
                bool found = true;
                for (int j = 0; j < preamble.Length; j++)
                {
                    if (buffer[i + j] != preamble[j])
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    return i;
                }
            }

            return -1;
        }

        #endregion

        #region ClientCommand members

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext)"/>
        public override void Execute
            (
                ClientContext context
            )
        {
            ClientQuery query = CreateQuery(context, CommandCode.ReadDocument);

            if (ReferenceEquals(File, null))
            {
                Log.Error
                    (
                        nameof(ReadBinaryFileCommand) + "::" + nameof(Execute)
                        + ": " + Resources.ReadBinaryFileCommand_FileNameNotSpecified
                    );

                throw new IrbisException(Resources.ReadBinaryFileCommand_FileNameNotSpecified);
            }
            File.BinaryFile = true;
            query.AddAnsi(File.ToString());

            ServerResponse result = BaseExecute(context);

            byte[] buffer = result.RawAnswer;
            Encoding encoding = IrbisEncoding.Ansi;
            byte[] preamble = encoding.GetBytes(Preamble);
            int offset = _FindPreamble(buffer, preamble);
            if (offset < 0)
            {
                Log.Error
                    (
                        nameof(ReadBinaryFileCommand) + "::" + nameof(Execute)
                        + ": " + Resources.ReadBinaryFileCommand_NoBinaryDataReceived
                    );

                throw new IrbisNetworkException(Resources.ReadBinaryFileCommand_NoBinaryDataReceived);
            }
            offset += preamble.Length;
            Content = result.RawAnswer.GetSpan(offset);
        }

        #endregion
    }
}
