// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ListFilesCommand.cs -- list server files
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * State: poor
 */

#region Using directives

using System.Collections.Generic;
using System.Linq;

using AM;
using AM.Collections;
using AM.Logging;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Infrastructure.ClientCommands
{
    /// <summary>
    /// List server files.
    /// </summary>
    [PublicAPI]
    public class ListFilesCommand
        : ClientCommand
    {
        #region Properties

        /// <summary>
        /// File specification (can contain wildcards).
        /// </summary>
        [NotNull]
        public NonNullCollection<FileSpecification> Specifications { get; }

        /// <summary>
        /// List of found files.
        /// </summary>
        [CanBeNull]
        public string[] Files { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public ListFilesCommand()
            //(
            //    [NotNull] IIrbisConnection connection
            //)
            //: base(connection)
        {
            Specifications = new NonNullCollection<FileSpecification>();
        }

        #endregion

        #region ClientCommand members

        /// <inheritdoc cref="ClientCommand.CheckResponse"/>
        public override void CheckResponse
            (
                ServerResponse response
            )
        {
            Sure.NotNull(response, nameof(response));

            // Don't check: there's no return code
            response.RefuseAnReturnCode();
        }

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext)"/>
        public override ServerResponse Execute
            (
                ClientContext context
            )
        {
            IIrbisConnection connection = context.Connection;

            ClientQuery query = CreateQuery(connection);
            query.CommandCode = CommandCode.ListFiles;

            if (Specifications.Count == 0)
            {
                Log.Error
                    (
                        "ListFilesCommand::CreateQuery: "
                        + "specification list is empty"
                    );

                throw new IrbisException("specification list is empty");
            }

            foreach (FileSpecification specification in Specifications)
            {
                specification.Verify(true);
                query.Add(specification);
            }

            ServerResponse result = base.Execute(connection, query);

            List<string> files = result.RemainingAnsiStrings();
            Files = files.SelectMany
                (
                    line => IrbisText.IrbisToWindows(line)
                        .ThrowIfNull(nameof(line)).SplitLines()
                )
                .ToArray();

            return result;
        }

        #endregion

        #region IVerifiable members

        /// <inheritdoc cref="IVerifiable.Verify"/>
        public override bool Verify
            (
                bool throwOnError
            )
        {
            // ReSharper disable AssignNullToNotNullAttribute
            Verifier<ListFilesCommand> verifier
                = new Verifier<ListFilesCommand>
                    (
                        this,
                        throwOnError
                    );

            foreach (FileSpecification specification in Specifications)
            {
                verifier.VerifySubObject
                    (
                        specification,
                        "specification"
                    );
            }

            return verifier.Result;
            // ReSharper restore AssignNullToNotNullAttribute
        }

        #endregion

        #region Object members

        #endregion
    }
}
