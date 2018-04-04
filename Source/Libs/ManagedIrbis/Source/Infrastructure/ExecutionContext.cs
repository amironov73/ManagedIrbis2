﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ExecutionContext.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;

using AM;

using JetBrains.Annotations;

using ManagedIrbis.Infrastructure.Commands;

#endregion

namespace ManagedIrbis.Infrastructure
{
    /// <summary>
    /// Command execution context.
    /// </summary>
    [PublicAPI]
    public sealed class ExecutionContext
        : IVerifiable
    {
        #region Properties

        /// <summary>
        /// Command to execute.
        /// </summary>
        [CanBeNull]
        public AbstractCommand Command { get; set; }

        /// <summary>
        /// Connection.
        /// </summary>
        [CanBeNull]
        public IIrbisConnection Connection { get; set; }

        /// <summary>
        /// Exception.
        /// </summary>
        [CanBeNull]
        public Exception Exception { get; set; }

        /// <summary>
        /// Exception handled?
        /// </summary>
        public bool ExceptionHandled { get; set; }

        /// <summary>
        /// Server response.
        /// </summary>
        [CanBeNull]
        public ServerResponse Response { get; set; }

        /// <summary>
        /// Arbitrary user data.
        /// </summary>
        [CanBeNull]
        public object UserData { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public ExecutionContext()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ExecutionContext
            (
                [NotNull] IIrbisConnection connection,
                [NotNull] AbstractCommand command
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNull(command, nameof(command));

            Command = command;
            Connection = connection;
        }

        #endregion

        #region IVerifiable members

        /// <inheritdoc cref="IVerifiable.Verify" />
        public bool Verify
            (
                bool throwOnError
            )
        {
            Verifier<ExecutionContext> verifier
                = new Verifier<ExecutionContext>(this, throwOnError);

            verifier
                .NotNull(Command, nameof(Command))
                .NotNull(Connection, nameof(Connection));

            return verifier.Result;
        }

        #endregion
    }
}
