// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ClientSocket.cs -- abstract client socket.
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Threading.Tasks;

using JetBrains.Annotations;

using AM;

#endregion

// ReSharper disable CommentTypo

namespace ManagedIrbis.Infrastructure.Sockets
{
    /// <summary>
    /// Абстрактный клиентский сокет.
    /// Занимается общением с сервером в самом широком смысле.
    /// Чаще всего - обычный BSD-сокет для TCP v4.
    /// </summary>
    public abstract class ClientSocket
    {
        #region Properties

        /// <summary>
        /// Используемое подключение (для нотификаций).
        /// </summary>
        protected IrbisConnection Connection { get; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        protected ClientSocket
            (
                IrbisConnection connection
            )
        {
            Connection = connection;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Собственно общение с сервером -- в асинхронном режиме.
        /// </summary>
        public abstract Task<ServerResponse?> TransactAsync
            (
                ClientQuery query
            );

        /// <summary>
        /// Собственно общение с сервером -- в синхронном режиме.
        /// </summary>
        public abstract ServerResponse? Transact
            (
                ClientQuery query
            );

        #endregion
    }
}
