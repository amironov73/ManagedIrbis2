// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ReadFileCommand.cs -- read text file(s) from the server
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Collections.Generic;

using AM;
using AM.Collections;
using AM.Logging;

using JetBrains.Annotations;

using ManagedIrbis.Properties;

#endregion

namespace ManagedIrbis.Infrastructure.ClientCommands
{
    /// <summary>
    /// Read text file(s) from the server
    /// </summary>
    [PublicAPI]
    public sealed class ReadFileCommand
        : ClientCommand
    {
        #region Properties

        /// <summary>
        /// File list.
        /// </summary>
        [NotNull]
        public NonNullCollection<FileSpecification> Files { get; }

        /// <summary>
        /// Retrieved text files.
        /// </summary>
        [CanBeNull]
        public string[] Result { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public ReadFileCommand()
        {
            Files = new NonNullCollection<FileSpecification>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ReadFileCommand
            (
                [NotNull] FileSpecification file
            )
        {
            Sure.NotNull(file, nameof(file));

            Files = new NonNullCollection<FileSpecification>
            {
                file
            };
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ReadFileCommand
            (
                [NotNull][ItemNotNull] IEnumerable<FileSpecification> files
            )
        {
            Sure.NotNull(files, nameof(files));

            Files = new NonNullCollection<FileSpecification>();
            Files.AddRange(files);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Get file text.
        /// </summary>
        [NotNull]
        public string[] GetFileText
            (
                [NotNull] ServerResponse response
            )
        {
            Sure.NotNull(response, nameof(response));

            int count = Files.Count;
            string[] result = new string[count];

            for (int i = 0; i < count; i++)
            {
                string text = response.GetAnsiString();
                text = IrbisText.IrbisToWindows(text);
                result[i] = text;
            }

            return result;
        }

        #endregion

        #region ClientCommand members

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext) "/>
        public override void Execute
            (
                ClientContext context
            )
        {
            ClientQuery query = CreateQuery(context, CommandCode.ReadDocument);

            if (Files.Count == 0)
            {
                Log.Error
                    (
                        nameof(ReadFileCommand) + "::" + nameof(Execute)
                        + ": " + Resources.NoFilesSpecified
                    );

                throw new IrbisNetworkException(Resources.NoFilesSpecified);
            }

            foreach (FileSpecification fileName in Files)
            {
                string item = fileName.ToString();
                query.AddAnsi(item);
            }

            ServerResponse result = BaseExecute(context);
            Result = GetFileText(result);
        }

        #endregion
    }
}
