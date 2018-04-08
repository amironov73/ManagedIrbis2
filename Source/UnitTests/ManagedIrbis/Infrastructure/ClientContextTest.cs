using System;

using ManagedIrbis;
using ManagedIrbis.Infrastructure;
using ManagedIrbis.Infrastructure.ClientCommands;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.ManagedIrbis.Infrastructure
{
    [TestClass]
    public class ClientContextTest
    {
        [TestMethod]
        public void ClientContext_Construction_1()
        {
            IIrbisConnection connection = new IrbisConnection();
            ClientContext context = new ClientContext(connection);
            Assert.AreSame(connection, context.Connection);
            Assert.IsNull(context.Command);
            Assert.IsNull(context.Exception);
            Assert.IsFalse(context.ExceptionHandled);
            Assert.IsNull(context.Response);
            Assert.IsNull(context.UserData);
        }

        [TestMethod]
        public void ClientContext_Construction_2()
        {
            IIrbisConnection connection = new IrbisConnection();
            ClientCommand command = new NopCommand();
            ClientContext context = new ClientContext(connection)
            {
                Command = command
            };
            Assert.AreSame(command, context.Command);
            Assert.AreSame(connection, context.Connection);
            Assert.IsNull(context.Exception);
            Assert.IsFalse(context.ExceptionHandled);
            Assert.IsNull(context.Response);
            Assert.IsNull(context.UserData);
        }

        [TestMethod]
        public void ClientContext_Properties_1()
        {
            IIrbisConnection connection = new IrbisConnection();
            ClientCommand command = new NopCommand();
            ClientContext context = new ClientContext(connection);

            Exception exception = new Exception();
            context.Exception = exception;
            Assert.AreSame(exception, context.Exception);

            context.ExceptionHandled = true;
            Assert.IsTrue(context.ExceptionHandled);

            ServerResponse response = new ServerResponse(connection, new byte[0], new byte[0], true);
            context.Response = response;
            Assert.AreSame(response, context.Response);

            object userData = new object();
            context.UserData = userData;
            Assert.AreSame(userData, context.UserData);
        }
    }
}
