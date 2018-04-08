// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* DynamicCommand.cs -- fully dynamic command.
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Infrastructure.ClientCommands
{
    /// <summary>
    /// Dynamic command. All actions configured
    /// during runtime.
    /// </summary>
    [PublicAPI]
    public sealed class DynamicCommand
        : ClientCommand
    {
        #region Properties

        ///// <summary>
        ///// Handle return codes.
        ///// </summary>
        //public Func<DynamicCommand, int[]> GoodReturnCodesHandler { get; set; }

        /// <summary>
        /// Check server response.
        /// </summary>
        public Action<DynamicCommand, ServerResponse> CheckResponseHandler { get; set; }

        /// <summary>
        /// Execute command.
        /// </summary>
        public Func<DynamicCommand, ServerResponse> ExecuteHandler { get; set; }

        /// <summary>
        /// Verify command settings.
        /// </summary>
        public Func<DynamicCommand, bool, bool> VerifyHandler { get; set; }

        #endregion

        #region Construction

        ///// <summary>
        ///// Constructor.
        ///// </summary>
        //public DynamicCommand
        //    (
        //        [NotNull] IIrbisConnection connection
        //    )
        //    : base(connection)
        //{
        //}

        #endregion

        #region Public methods

        ///// <summary>
        ///// Basic implementation of <see cref="GoodReturnCodes"/>.
        ///// </summary>
        //public int[] BaseGoodReturnCodes()
        //{
        //    int[] result = base.GoodReturnCodes;

        //    return result;
        //}

        /// <summary>
        /// Basic implementation of <see cref="CheckResponse"/>.
        /// </summary>
        public void BaseCheckResponse
            (
                ServerResponse response
            )
        {
            base.CheckResponse(response);
        }

        /// <summary>
        /// Basic implementation of <see cref="Execute"/>.
        /// </summary>
        public ServerResponse BaseExecute
            (
                IIrbisConnection connection,
                ClientQuery query
            )
        {
            ServerResponse result = base.Execute(connection, query);

            return result;
        }

        /// <summary>
        /// Basic implementation of <see cref="ClientCommand.Verify"/>.
        /// </summary>
        public bool BaseVerify
            (
                bool throwOnError
            )
        {
            bool result = base.Verify(throwOnError);

            return result;
        }

        #endregion

        #region ClientCommand members

        ///// <inheritdoc cref="ClientCommand.GoodReturnCodes" />
        //public override int[] GoodReturnCodes
        //{
        //    get
        //    {
        //        Func<DynamicCommand, int[]> handler = GoodReturnCodesHandler;

        //        int[] result = !ReferenceEquals(handler, null)
        //              ? handler(this)
        //              : BaseGoodReturnCodes();

        //        return result;
        //    }
        //}

        /// <inheritdoc cref="ClientCommand.CheckResponse" />
        public override void CheckResponse
            (
                ServerResponse response
            )
        {
            Action<DynamicCommand, ServerResponse> handler = CheckResponseHandler;

            if (!ReferenceEquals(handler, null))
            {
                handler(this, response);
            }
            else
            {
                BaseCheckResponse(response);
            }
        }

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext)" />
        public override ServerResponse Execute
            (
                ClientContext context
            )
        {
            // TODO Fix this

            IIrbisConnection connection = context.Connection;

            ClientQuery query = CreateQuery(connection);
            query.CommandCode = CommandCode.UnregisterClient;

            query.AddAnsi(context.Connection.Username);

            Func<DynamicCommand, ServerResponse> handler = ExecuteHandler;

            ServerResponse result = !ReferenceEquals(handler, null)
                ? handler(this)
                : BaseExecute(connection, query);

            return result;
        }

        #endregion

        #region IVerifiable members

        /// <inheritdoc cref="ClientCommand.Verify" />
        public override bool Verify
            (
                bool throwOnError
            )
        {
            Func<DynamicCommand, bool, bool> handler = VerifyHandler;

            bool result = handler?.Invoke(this, throwOnError) ?? BaseVerify(throwOnError);

            return result;
        }

        #endregion
    }
}
