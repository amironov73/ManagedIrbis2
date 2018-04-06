// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* CommandFactory.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Text;

using AM;
using AM.Logging;

using JetBrains.Annotations;

using ManagedIrbis.Infrastructure.Commands;

#endregion

namespace ManagedIrbis.Infrastructure
{
    /// <summary>
    /// Command factory.
    /// </summary>
    [PublicAPI]
    public class CommandFactory
    {
        #region Properties

        /// <summary>
        /// Connection.
        /// </summary>
        [NotNull]
        public IIrbisConnection Connection { get; protected internal set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public CommandFactory
            (
                [NotNull] IIrbisConnection connection
            )
        {
            Sure.NotNull(connection, nameof(connection));

            Log.Trace(nameof(CommandFactory) + "::Constructor");

            Connection = connection;
        }

        static CommandFactory()
        {
            _superFactory = connection => new CommandFactory(connection);
        }

        #endregion

        #region Private members

        private static Func<IIrbisConnection, CommandFactory> _superFactory;

        #endregion

        #region Public methods

        /// <summary>
        /// Create the command.
        /// </summary>
        [NotNull]
        public virtual T CreateCommand<T>()
            where T : AbstractCommand
        {
            T result = (T)Activator.CreateInstance(typeof(T), Connection);

            return result;
        }

        /// <summary>
        /// Get UniversalCommand.
        /// </summary>
        [NotNull]
        public virtual UniversalCommand CreateUniversalCommand
            (
                [NotNull] string commandCode,
                params object[] arguments
            )
        {
            return new UniversalCommand
                (
                    Connection,
                    commandCode,
                    arguments
                );
        }

        /// <summary>
        /// Get UniversalTextCommand.
        /// </summary>
        [NotNull]
        public virtual UniversalTextCommand CreateUniversalTextCommand
            (
                [NotNull] string commandCode,
                [NotNull] string[] lines,
                [NotNull] Encoding encoding
            )
        {
            return new UniversalTextCommand
                (
                    Connection,
                    commandCode,
                    lines,
                    encoding
                );
        }

        /// <summary>
        /// Get default <see cref="CommandFactory"/>.
        /// </summary>
        [NotNull]
        public static CommandFactory GetDefaultFactory
            (
                [NotNull] IIrbisConnection connection
            )
        {
            Sure.NotNull(connection, nameof(connection));

            CommandFactory result = _superFactory(connection);

            return result;
        }

        /// <summary>
        /// Set Super Factory.
        /// </summary>
        [NotNull]
        public static Func<IIrbisConnection, CommandFactory> SetSuperFactory
            (
                [NotNull] Func<IIrbisConnection, CommandFactory> superFactory
            )
        {
            Sure.NotNull(superFactory, nameof(superFactory));

            Func<IIrbisConnection, CommandFactory> result = _superFactory;
            _superFactory = superFactory;

            return result;
        }

        #endregion
    }
}
