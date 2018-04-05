// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* AbstractEngine.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using AM;
using AM.Logging;
using AM.Threading;

using JetBrains.Annotations;

using ManagedIrbis.Infrastructure.Commands;
using ManagedIrbis.Properties;

#endregion

// ReSharper disable VirtualMemberNeverOverridden.Global

namespace ManagedIrbis.Infrastructure
{
    /// <summary>
    /// Abstract execution engine.
    /// </summary>
    [PublicAPI]
    public abstract class AbstractEngine
    {
        #region Events

        /// <summary>
        /// Raised after execution.
        /// </summary>
        public event EventHandler<ExecutionEventArgs> AfterExecution;

        /// <summary>
        /// Raised before execution.
        /// </summary>
        public event EventHandler<ExecutionEventArgs> BeforeExecution;

        /// <summary>
        /// Raised on exception.
        /// </summary>
        public event EventHandler<ExecutionEventArgs> ExceptionOccurs;

        #endregion

        #region Properties

        /// <summary>
        /// Connection.
        /// </summary>
        [NotNull]
        public IIrbisConnection Connection { get; private set; }

        /// <summary>
        /// Nested engine.
        /// </summary>
        [CanBeNull]
        public AbstractEngine NestedEngine { get; private set; }

        /// <summary>
        /// Additional services.
        /// </summary>
        public NonNullValue<IServiceProvider> Services { get; set; }

        /// <summary>
        /// Throw on <see cref="IVerifiable.Verify"/> calling.
        /// </summary>
        public static bool ThrowOnVerify { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AbstractEngine
            (
                [NotNull] IIrbisConnection connection,
                [CanBeNull] AbstractEngine nestedEngine
            )
        {
            Sure.NotNull(connection, nameof(connection));

            Log.Trace(nameof(AbstractEngine) + "::Constructor");

            Connection = connection;
            NestedEngine = nestedEngine;
            Services = new ServiceContainer();
        }

        static AbstractEngine()
        {
            ThrowOnVerify = true;
        }

        #endregion

        #region Private members

        /// <summary>
        /// Check whether connection established.
        /// </summary>
        protected virtual void CheckConnection
            (
                [NotNull] ExecutionContext context
            )
        {
            AbstractCommand command = context.Command.ThrowIfNull(nameof(context.Command));
            IIrbisConnection connection = context.Connection.ThrowIfNull(nameof(context.Connection));

            if (command.RequireConnection && connection.Socket.RequireConnection)
            {
                if (!connection.Connected)
                {
                    Log.Error
                        (
                            nameof(AbstractEngine) + "::" + nameof(CheckConnection)
                            + Resources.AbstractEngine_NotConnected2
                        );

                    throw new IrbisException(Resources.AbstractEngine_NotConnected);
                }
            }
        }

        /// <summary>
        /// After command execution.
        /// </summary>
        protected void OnAfterExecute
            (
                [NotNull] ExecutionContext context
            )
        {
            Sure.NotNull(context, nameof(context));
            Log.Trace(nameof(AbstractEngine) + "::" + nameof(OnAfterExecute));

            AfterExecution?.Invoke(this, new ExecutionEventArgs(context));
        }

        /// <summary>
        /// Before command execution.
        /// </summary>
        protected void OnBeforeExecute
            (
                [NotNull] ExecutionContext context
            )
        {
            Sure.NotNull(context, nameof(context));
            Log.Trace(nameof(AbstractEngine) + "::" + nameof(OnBeforeExecute));

            BeforeExecution?.Invoke(this, new ExecutionEventArgs(context));
        }

        /// <summary>
        /// Exception occurs.
        /// </summary>
        protected void OnException
            (
                [NotNull] ExecutionContext context
            )
        {
            Sure.NotNull(context, nameof(context));
            Log.Trace(nameof(AbstractEngine) + "::" + nameof(OnException));

            // TODO Implement properly!

            if (context.Exception is ArsMagnaException exception
                && context.Connection is IrbisConnection connection)
            {
                if (!ReferenceEquals(connection.RawClientRequest, null))
                {
                    BinaryAttachment request = new BinaryAttachment
                        (
                            "request",
                            connection.RawClientRequest
                        );
                    exception.Attach(request);
                }

                if (!ReferenceEquals(connection.RawServerResponse, null))
                {
                    BinaryAttachment response = new BinaryAttachment
                        (
                            "response",
                            connection.RawServerResponse
                        );
                    exception.Attach(response);
                }
            }

            ExceptionOccurs?.Invoke(this, new ExecutionEventArgs(context));
        }

        /// <summary>
        /// Standard command execution.
        /// </summary>
        protected ServerResponse StandardExecution
            (
                [NotNull] ExecutionContext context
            )
        {
            Sure.NotNull(context, nameof(context));

            Log.Trace(nameof(AbstractEngine) + "::" + nameof(StandardExecution));

            CheckConnection(context);

            AbstractCommand command = context.Command.ThrowIfNull(nameof(context.Command));
            IIrbisConnection connection = context.Connection.ThrowIfNull(nameof(context.Connection));

            if (!command.Verify(ThrowOnVerify))
            {
                Log.Error
                    (
                        nameof(AbstractEngine) + "::" + nameof(StandardExecution)
                        + ": " + nameof(command) + "." + nameof(command.Verify)
                        + Resources.AbstractEngine_StandardExecution_Failed
                    );
            }

            using (new BusyGuard(connection.Busy))
            {
                ServerResponse result = ServerResponse.GetEmptyResponse(connection);
                connection.Interrupted = false;

                try
                {
                    ClientQuery query = command.CreateQuery();
                    if (!query.Verify(ThrowOnVerify))
                    {
                        Log.Error
                            (
                                nameof(AbstractEngine) + "::" + nameof(StandardExecution)
                                + ": " + nameof(query) + "." + nameof(query.Verify)
                                + Resources.AbstractEngine_StandardExecution_Failed
                            );
                    }

                    result = command.Execute(query);
                    if (!result.Verify(ThrowOnVerify))
                    {
                        Log.Error
                            (
                                nameof(AbstractEngine) + "::" + nameof(StandardExecution)
                                + ": " + nameof(result) + "." + nameof(result.Verify)
                                + Resources.AbstractEngine_StandardExecution_Failed
                            );
                    }

                    command.CheckResponse(result);
                }
                catch (Exception exception)
                {
                    Log.TraceException
                        (
                            nameof(AbstractEngine) + "::" + nameof(StandardExecution),
                            exception
                        );

                    context.Exception = exception;

                    OnException(context);

                    if (!context.ExceptionHandled)
                    {
                        throw;
                    }
                }

                context.Response = result;

                return result;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Get <see cref="MemoryStream"/>.
        /// </summary>
        [NotNull]
        [ExcludeFromCodeCoverage]
        public virtual MemoryStream GetMemoryStream
            (
                [NotNull] Type consumer
            )
        {
            // TODO use ObjectPool?

            return new MemoryStream();
        }

        /// <summary>
        /// Execute specified command.
        /// </summary>
        [NotNull]
        public virtual ServerResponse ExecuteCommand
            (
                [NotNull] ExecutionContext context
            )
        {
            Sure.NotNull(context, nameof(context));
            Log.Trace(nameof(AbstractEngine) + "::" + nameof(ExecuteCommand));

            context.Verify(true);

            OnBeforeExecute(context);
            ServerResponse result = NestedEngine?.ExecuteCommand(context)
                                    ?? StandardExecution(context);
            context.Response = result;
            OnAfterExecute(context);

            return result;
        }

        /// <summary>
        /// Report memory usage.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public virtual void ReportMemoryUsage
            (
                [NotNull] Type consumer,
                int memoryUsage
            )
        {
            // Nothing to do here
        }

        #endregion
    }
}
