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

using ManagedIrbis.Properties;

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
        {
            Specifications = new NonNullCollection<FileSpecification>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ListFilesCommand
            (
                [NotNull] IEnumerable<FileSpecification> specifications
            )
        {
            Sure.NotNull(specifications, nameof(specifications));

            Specifications = new NonNullCollection<FileSpecification>();
            Specifications.AddRange(specifications);
        }

        #endregion

        #region ClientCommand members

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext)"/>
        public override void Execute
            (
                ClientContext context
            )
        {
            ClientQuery query = CreateQuery(context, CommandCode.ListFiles);

            if (Specifications.Count == 0)
            {
                Log.Error
                    (
                        nameof(ListFilesCommand) + "::" + nameof(Execute)
                        + ": " + Resources.ListFilesCommand_SpecificationListIsEmpty
                    );

                throw new IrbisException(Resources.ListFilesCommand_SpecificationListIsEmpty);
            }

            foreach (FileSpecification specification in Specifications)
            {
                specification.Verify(true);
                query.Add(specification);
            }

            ServerResponse result = BaseExecute(context);
            List<string> files = result.RemainingAnsiStrings();
            Files = files.SelectMany
                (
                    line => IrbisText.IrbisToWindows(line)
                        .ThrowIfNull(nameof(line)).SplitLines()
                )
                .ToArray();
        }

        #endregion
    }
}
