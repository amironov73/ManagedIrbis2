﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* AbstractEngine.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.IO;
using AM;
using AM.IOC;
using AM.Logging;
using AM.Threading;

using JetBrains.Annotations;

using ManagedIrbis.Infrastructure.Commands;

#endregion

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
        [NotNull]
        public ServiceRepository Services { get; private set; }

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
            Sure.NotNull(connection, "connection");

            Log.Trace("AbstractEngine::Constructor");

            Connection = connection;
            NestedEngine = nestedEngine;
            Services = new ServiceRepository();
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
            AbstractCommand command = context.Command
                .ThrowIfNull("Command");
            IrbisConnection connection = context.Connection
                .ThrowIfNull("Connection");

            if (command.RequireConnection && connection.Socket.RequireConnection)
            {
                if (!connection.Connected)
                {
                    Log.Error
                        (
                            "AbstractEngine::CheckConnection: "
                            + "not connected"
                        );

                    throw new IrbisException("Not connected");
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
            Log.Trace("AbstractEngine::OnAfterExecute");

            EventHandler<ExecutionEventArgs> handler
                = AfterExecution;

            if (!ReferenceEquals(handler, null))
            {
                ExecutionEventArgs args
                    = new ExecutionEventArgs(context);

                handler(this, args);
            }
        }

        /// <summary>
        /// Before command execution.
        /// </summary>
        protected void OnBeforeExecute
            (
                [NotNull] ExecutionContext context
            )
        {
            Log.Trace("AbstractEngine::OnBeforeExecute");

            EventHandler<ExecutionEventArgs> handler
                = BeforeExecution;

            if (!ReferenceEquals(handler, null))
            {
                ExecutionEventArgs args
                    = new ExecutionEventArgs(context);

                handler(this, args);
            }
        }

        /// <summary>
        /// Exception occurs.
        /// </summary>
        protected void OnException
            (
                [NotNull] ExecutionContext context
            )
        {
            Log.Trace("AbstractEngine::OnException");

            // TODO Implement properly!

            ArsMagnaException exception
                = context.Exception as ArsMagnaException;
            IrbisConnection connection = Connection as IrbisConnection;
            if (!ReferenceEquals(exception, null)
                && !ReferenceEquals(connection, null))
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

            EventHandler<ExecutionEventArgs> handler = ExceptionOccurs;

            if (!ReferenceEquals(handler, null))
            {
                ExecutionEventArgs args
                    = new ExecutionEventArgs(context);

                handler(this, args);
            }
        }

        /// <summary>
        /// Standard command execution.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected ServerResponse StandardExecution
            (
                [NotNull] ExecutionContext context
            )
        {
            Log.Trace("AbstractEngine::StandardExecution");

            AbstractCommand command = context.Command
                .ThrowIfNull("Command");
            IrbisConnection connection = context.Connection
                .ThrowIfNull("Connection");

            if (!command.Verify(ThrowOnVerify))
            {
                Log.Error
                    (
                        "AbstractEngine::StandardExecution: "
                        + "command.Verify() failed"
                    );
            }

            using (new BusyGuard(connection.Busy))
            {
                ServerResponse result
                    = ServerResponse.GetEmptyResponse(connection);
                connection.Interrupted = false;

                try
                {

                    ClientQuery query = command.CreateQuery();
                    if (!query.Verify(ThrowOnVerify))
                    {
                        Log.Error
                            (
                                "AbstractEngine::StandardExecution: "
                                + "query.Verify() failed"
                            );
                    }

                    result = command.Execute(query);
                    if (!result.Verify(ThrowOnVerify))
                    {
                        Log.Error
                            (
                                "AbstractEngine::StandardExecution: "
                                + "result.Verify() failed"
                            );
                    }

                    command.CheckResponse(result);
                }
                catch (Exception exception)
                {
                    Log.TraceException
                        (
                            "AbstractEngine::StandardExecution",
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
        public virtual MemoryStream GetMemoryStream
            (
                [NotNull] Type consumer
            )
        {
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
            Sure.NotNull(context, "context");

            Log.Trace("AbstractEngine::ExecuteCommand");

            context.Verify(true);

            OnBeforeExecute(context);

            var result = ReferenceEquals(NestedEngine, null)
                ? StandardExecution(context)
                : NestedEngine.ExecuteCommand(context);

            context.Response = result;

            OnAfterExecute(context);

            return result;
        }

        /// <summary>
        /// Report memory usage.
        /// </summary>
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
