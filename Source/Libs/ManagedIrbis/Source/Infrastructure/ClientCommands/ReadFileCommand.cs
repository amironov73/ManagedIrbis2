// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ReadFileCommand.cs -- read text file(s) from the server
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using AM;
using AM.Collections;

using JetBrains.Annotations;

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
        public NonNullCollection<FileSpecification> Files
        {
            get { return _files; }
        }

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
            //(
            //    [NotNull] IIrbisConnection connection
            //)
            //: base(connection)
        {
            _files = new NonNullCollection<FileSpecification>();
        }

        #endregion

        #region Private members

        private readonly NonNullCollection<FileSpecification> _files;

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

        /// <summary>
        /// Check the server response.
        /// </summary>
        public override void CheckResponse
            (
                ServerResponse response
            )
        {
            Sure.NotNull(response, nameof(response));

            // Don't check: there's no return code
            response.RefuseAnReturnCode();
        }

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext) "/>
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
                string item = fileName.ToString();
                query.AddAnsi(item);
            }

            ServerResponse result = Execute(connection, query);
            Result = GetFileText(result);

            return result;
        }

        /// <summary>
        /// Verify object state.
        /// </summary>
        public override bool Verify
            (
                bool throwOnError
            )
        {
            Verifier<ReadFileCommand> verifier
                = new Verifier<ReadFileCommand>
                    (
                        this,
                        throwOnError
                    );

            verifier
                .Assert(Files.Count != 0, "Files.Count")
                .Assert(base.Verify(throwOnError));

            return verifier.Result;
        }

        #endregion
    }
}
