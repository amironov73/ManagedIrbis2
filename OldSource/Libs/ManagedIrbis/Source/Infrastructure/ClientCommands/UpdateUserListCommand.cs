// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* UpdateUserListCommand.cs -- update user list on the server
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using AM.Logging;

using JetBrains.Annotations;

using ManagedIrbis.Properties;

#endregion

namespace ManagedIrbis.Infrastructure.ClientCommands
{
    /// <summary>
    /// Update user list on the server.
    /// </summary>
    [PublicAPI]
    public sealed class UpdateUserListCommand
        : ClientCommand
    {
        #region Properties

        /// <summary>
        /// User list to update.
        /// </summary>
        [CanBeNull]
        public UserInfo[] UserList { get; set; }

        #endregion

        #region ClientCommand members

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext)" />
        public override void Execute
            (
                ClientContext context
            )
        {
            UserInfo[] userList = UserList;
            if (ReferenceEquals(userList, null))
            {
                Log.Error
                    (
                        nameof(UpdateUserListCommand) + "::" + nameof(Execute)
                        + ": " + Resources.UserListNotSet
                    );

                throw new IrbisException(Resources.UserListNotSet);
            }

            ClientQuery query = CreateQuery(context, CommandCode.SetUserList);
            foreach (UserInfo userInfo in userList)
            {
                string line = userInfo.Encode();
                query.AddAnsi(line);
            }

            ServerResponse response = BaseExecute(context);
            CheckResponse(response);
        }

        #endregion
    }
}
