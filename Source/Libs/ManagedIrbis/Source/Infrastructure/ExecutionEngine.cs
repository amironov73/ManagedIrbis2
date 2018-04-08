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

using ManagedIrbis.Infrastructure.ClientCommands;
using ManagedIrbis.Properties;

#endregion

// ReSharper disable VirtualMemberNeverOverridden.Global

namespace ManagedIrbis.Infrastructure
{
    /// <summary>
    /// Abstract execution engine.
    /// </summary>
    [PublicAPI]
    public class ExecutionEngine
    {
        #region Events

        /// <summary>
        /// Raised after execution.
        /// </summary>
        public event EventHandler<ClientEventArgs> AfterExecution;

        /// <summary>
        /// Raised before execution.
        /// </summary>
        public event EventHandler<ClientEventArgs> BeforeExecution;

        /// <summary>
        /// Raised on exception.
        /// </summary>
        public event EventHandler<ClientEventArgs> ExceptionOccurs;

        #endregion

        #region Properties

        /// <summary>
        /// Connection.
        /// </summary>
        [NotNull]
        public IIrbisConnection Connection { get; private set; }

        /// <summary>
        /// Additional services.
        /// </summary>
        public IServiceProvider Services { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public ExecutionEngine
            (
                [NotNull] IIrbisConnection connection
            )
        {
            Sure.NotNull(connection, nameof(connection));

            Log.Trace(nameof(ExecutionEngine) + "::Constructor");

            Connection = connection;
            Services = new ServiceContainer();
        }

        #endregion

        #region Private members

        /// <summary>
        /// Check whether connection established.
        /// </summary>
        protected virtual void CheckConnection
            (
                [NotNull] ClientContext context
            )
        {
            ClientCommand command = context.Command.ThrowIfNull(nameof(context.Command));
            IIrbisConnection connection = context.Connection.ThrowIfNull(nameof(context.Connection));

            //if (command.RequireConnection && connection.Socket.RequireConnection)
            //{
            //    if (!connection.Connected)
            //    {
            //        Log.Error
            //            (
            //                nameof(ExecutionEngine) + "::" + nameof(CheckConnection)
            //                + Resources.AbstractEngine_NotConnected2
            //            );

            //        throw new IrbisException(Resources.AbstractEngine_NotConnected);
            //    }
            //}
        }

        /// <summary>
        /// After command execution.
        /// </summary>
        protected void OnAfterExecute
            (
                [NotNull] ClientContext context
            )
        {
            Sure.NotNull(context, nameof(context));

            Log.Trace(nameof(ExecutionEngine) + "::" + nameof(OnAfterExecute));

            AfterExecution?.Invoke(this, new ClientEventArgs(context));
        }

        /// <summary>
        /// Before command execution.
        /// </summary>
        protected void OnBeforeExecute
            (
                [NotNull] ClientContext context
            )
        {
            Sure.NotNull(context, nameof(context));
            Log.Trace(nameof(ExecutionEngine) + "::" + nameof(OnBeforeExecute));

            BeforeExecution?.Invoke(this, new ClientEventArgs(context));
        }

        /// <summary>
        /// Exception occurs.
        /// </summary>
        protected void OnException
            (
                [NotNull] ClientContext context
            )
        {
            Sure.NotNull(context, nameof(context));
            Log.Trace(nameof(ExecutionEngine) + "::" + nameof(OnException));

            // TODO Implement properly!

            //if (context.Exception is ArsMagnaException exception
            //    && context.Connection is IrbisConnection connection)
            //{
                //if (!ReferenceEquals(connection.RawClientRequest, null))
                //{
                //    BinaryAttachment request = new BinaryAttachment
                //        (
                //            "request",
                //            connection.RawClientRequest
                //        );
                //    exception.Attach(request);
                //}

                //if (!ReferenceEquals(connection.RawServerResponse, null))
                //{
                //    BinaryAttachment response = new BinaryAttachment
                //        (
                //            "response",
                //            connection.RawServerResponse
                //        );
                //    exception.Attach(response);
                //}
            //}

            ExceptionOccurs?.Invoke(this, new ClientEventArgs(context));
        }

        /// <summary>
        /// Standard command execution.
        /// </summary>
        protected ServerResponse StandardExecution
            (
                [NotNull] ClientContext context
            )
        {
            Sure.NotNull(context, nameof(context));

            Log.Trace(nameof(ExecutionEngine) + "::" + nameof(StandardExecution));

            CheckConnection(context);

            ClientCommand command = context.Command.ThrowIfNull(nameof(context.Command));
            IIrbisConnection connection = context.Connection.ThrowIfNull(nameof(context.Connection));

            using (new BusyGuard(connection.Busy))
            {
                ServerResponse result = ServerResponse.GetEmptyResponse(connection);
                connection.Interrupted = false;

                try
                {
                    ClientContext clientContext = new ClientContext(Connection);
                    result = command.Execute(clientContext);

                    command.CheckResponse(result);
                }
                catch (Exception exception)
                {
                    Log.TraceException
                        (
                            nameof(ExecutionEngine) + "::" + nameof(StandardExecution),
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
                [NotNull] ClientContext context
            )
        {
            Sure.NotNull(context, nameof(context));

            Log.Trace(nameof(ExecutionEngine) + "::" + nameof(ExecuteCommand));

            OnBeforeExecute(context);

            ServerResponse result = StandardExecution(context);
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
