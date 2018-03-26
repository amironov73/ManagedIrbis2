﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
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

using ManagedIrbis.Gbl;
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
        public IIrbisConnection Connection
        {
            get;
            protected internal set;
        }

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
        /// Get <see cref="ActualizeRecordCommand"/>.
        /// </summary>
        [NotNull]
        public virtual ActualizeRecordCommand GetActualizeRecordCommand()
        {
            return new ActualizeRecordCommand(Connection);
        }

        /// <summary>
        /// Get <see cref="ConnectCommand"/>.
        /// </summary>
        [NotNull]
        public virtual ConnectCommand GetConnectCommand()
        {
            return new ConnectCommand(Connection);
        }

        /// <summary>
        /// Get <see cref="CreateDatabaseCommand"/>.
        /// </summary>
        [NotNull]
        public virtual CreateDatabaseCommand GetCreateDatabaseCommand()
        {
            return new CreateDatabaseCommand(Connection);
        }

        /// <summary>
        /// Get <see cref="CreateDictionaryCommand"/>.
        /// </summary>
        [NotNull]
        public virtual CreateDictionaryCommand GetCreateDictionaryCommand()
        {
            return new CreateDictionaryCommand(Connection);
        }

        /// <summary>
        /// Get <see cref="DatabaseInfoCommand"/>.
        /// </summary>
        [NotNull]
        public virtual DatabaseInfoCommand GetDatabaseInfoCommand()
        {
            return new DatabaseInfoCommand(Connection);
        }

        /// <summary>
        /// Get <see cref="DatabaseStatCommand"/>.
        /// </summary>
        [NotNull]
        public virtual DatabaseStatCommand GetDatabaseStatCommand()
        {
            return new DatabaseStatCommand(Connection);
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
        /// Get <see cref="DeleteDatabaseCommand"/>.
        /// </summary>
        [NotNull]
        public virtual DeleteDatabaseCommand GetDeleteDatabaseCommand()
        {
            return new DeleteDatabaseCommand(Connection);
        }

        /// <summary>
        /// Get <see cref="DisconnectCommand"/>.
        /// </summary>
        [NotNull]
        public virtual DisconnectCommand GetDisconnectCommand()
        {
            return new DisconnectCommand(Connection);
        }

        /// <summary>
        /// Get <see cref="FormatCommand"/>.
        /// </summary>
        [NotNull]
        public virtual FormatCommand GetFormatCommand()
        {
            return new FormatCommand(Connection);
        }

        /// <summary>
        /// Get GblCommand.
        /// </summary>
        [NotNull]
        public virtual GblCommand GetGblCommand
            (
                [NotNull] GblSettings settings
            )
        {
            return new GblCommand
                (
                    Connection,
                    settings
                );
        }

        /// <summary>
        /// Get GblVirtualCommand.
        /// </summary>
        [NotNull]
        public virtual GblVirtualCommand GetGblVirtualCommand()
        {
            return new GblVirtualCommand(Connection);
        }

        /// <summary>
        /// Get ListFilesCommand.
        /// </summary>
        [NotNull]
        public virtual ListFilesCommand GetListFilesCommand()
        {
            return new ListFilesCommand(Connection);
        }

        /// <summary>
        /// Get <see cref="ListProcessesCommand"/>.
        /// </summary>
        [NotNull]
        public virtual ListProcessesCommand GetListProcessCommand()
        {
            return new ListProcessesCommand(Connection);
        }

        /// <summary>
        /// Get <see cref="ListUsersCommand"/>.
        /// </summary>
        [NotNull]
        public virtual ListUsersCommand GetListUsersCommand()
        {
            return new ListUsersCommand(Connection);
        }

        /// <summary>
        /// Get <see cref="MaxMfnCommand"/>.
        /// </summary>
        [NotNull]
        public virtual MaxMfnCommand GetMaxMfnCommand()
        {
            return new MaxMfnCommand(Connection);
        }

        /// <summary>
        /// Get <see cref="NopCommand"/>.
        /// </summary>
        [NotNull]
        public virtual NopCommand GetNopCommand()
        {
            return new NopCommand(Connection);
        }

        /// <summary>
        /// Get PrintTableCommand.
        /// </summary>
        [NotNull]
        public virtual PrintTableCommand GetPrintTableCommand()
        {
            return new PrintTableCommand(Connection);
        }

        /// <summary>
        /// Get ReadBinaryFileCommand();
        /// </summary>
        [NotNull]
        public virtual ReadBinaryFileCommand GetReadBinaryFileCommand()
        {
            return new ReadBinaryFileCommand(Connection);
        }

        /// <summary>
        /// Get ReadFileCommand.
        /// </summary>
        [NotNull]
        public virtual ReadFileCommand GetReadFileCommand()
        {
            return new ReadFileCommand(Connection);
        }

        /// <summary>
        /// Get ReadPostingsCommand.
        /// </summary>
        [NotNull]
        public virtual ReadPostingsCommand GetReadPostingsCommand()
        {
            return new ReadPostingsCommand(Connection);
        }

        /// <summary>
        /// Get <see cref="ReadRawRecordCommand"/>.
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public virtual ReadRawRecordCommand GetReadRawRecordCommand()
        {
            return new ReadRawRecordCommand(Connection);
        }

        /// <summary>
        /// Get ReadRecordCommand.
        /// </summary>
        [NotNull]
        public virtual ReadRecordCommand GetReadRecordCommand()
        {
            return new ReadRecordCommand(Connection);
        }

        /// <summary>
        /// Get ReadTermsCommand.
        /// </summary>
        [NotNull]
        public virtual ReadTermsCommand GetReadTermsCommand()
        {
            return new ReadTermsCommand(Connection);
        }

        /// <summary>
        /// Get <see cref="ReloadDictionaryCommand"/>.
        /// </summary>
        [NotNull]
        public virtual ReloadDictionaryCommand GetReloadDictionaryCommand()
        {
            return new ReloadDictionaryCommand(Connection);
        }

        /// <summary>
        /// Get <see cref="ReloadMasterFileCommand"/>.
        /// </summary>
        [NotNull]
        public virtual ReloadMasterFileCommand GetReloadMasterFileCommand()
        {
            return new ReloadMasterFileCommand(Connection);
        }

        /// <summary>
        /// Get <see cref="RestartServerCommand"/>.
        /// </summary>
        [NotNull]
        public virtual RestartServerCommand GetRestartServerCommand()
        {
            return new RestartServerCommand(Connection);
        }

        /// <summary>
        /// Get <see cref="SearchCommand"/>.
        /// </summary>
        [NotNull]
        public virtual SearchCommand GetSearchCommand()
        {
            return new SearchCommand(Connection);
        }

        /// <summary>
        /// Get <see cref="SearchRawCommand"/>.
        /// </summary>
        [NotNull]
        public virtual SearchRawCommand GetSearchRawCommand()
        {
            return new SearchRawCommand(Connection);
        }

        /// <summary>
        /// Get SearchReadCommand.
        /// </summary>
        [NotNull]
        public virtual SearchReadCommand GetSearchReadCommand()
        {
            return new SearchReadCommand(Connection);
        }

        /// <summary>
        /// Get <see cref="ServerStatCommand"/>.
        /// </summary>
        [NotNull]
        public virtual ServerStatCommand GetServerStatCommand()
        {
            return new ServerStatCommand(Connection);
        }

        /// <summary>
        /// Get <see cref="ServerVersionCommand"/>.
        /// </summary>
        [NotNull]
        public virtual ServerVersionCommand GetServerVersionCommand()
        {
            return new ServerVersionCommand(Connection);
        }

        /// <summary>
        /// Get <see cref="TruncateDatabaseCommand"/>.
        /// </summary>
        [NotNull]
        public virtual TruncateDatabaseCommand GetTruncateDatabaseCommand()
        {
            return new TruncateDatabaseCommand(Connection);
        }

        /// <summary>
        /// Get UniversalCommand.
        /// </summary>
        [NotNull]
        public virtual UniversalCommand GetUniversalCommand
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
        public virtual UniversalTextCommand GetUniversalTextCommand
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
        /// Get <see cref="UnlockDatabaseCommand"/>.
        /// </summary>
        [NotNull]
        public virtual UnlockDatabaseCommand GetUnlockDatabaseCommand()
        {
            return new UnlockDatabaseCommand(Connection);
        }

        /// <summary>
        /// Get <see cref="UnlockRecordsCommand"/>.
        /// </summary>
        [NotNull]
        public virtual UnlockRecordsCommand GetUnlockRecordsCommand()
        {
            return new UnlockRecordsCommand(Connection);
        }

        /// <summary>
        /// Get <see cref="UpdateIniFileCommand"/>.
        /// </summary>
        [NotNull]
        public virtual UpdateIniFileCommand GetUpdateIniFileCommand()
        {
            return new UpdateIniFileCommand(Connection);
        }

        /// <summary>
        /// Get UpdateUserListCommand.
        /// </summary>
        [NotNull]
        public virtual UpdateUserListCommand GetUpdateUserListCommand()
        {
            return new UpdateUserListCommand(Connection);
        }

        /// <summary>
        /// Get WriteFileCommand.
        /// </summary>
        [NotNull]
        public virtual WriteFileCommand GetWriteFileCommand()
        {
            return new WriteFileCommand(Connection);
        }

        /// <summary>
        /// Get WriteRecordCommand.
        /// </summary>
        [NotNull]
        public virtual WriteRecordCommand GetWriteRecordCommand()
        {
            return new WriteRecordCommand(Connection);
        }

        /// <summary>
        /// Get WriteRecordsCommand.
        /// </summary>
        [NotNull]
        public virtual WriteRecordsCommand GetWriteRecordsCommand()
        {
            return new WriteRecordsCommand(Connection);
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
