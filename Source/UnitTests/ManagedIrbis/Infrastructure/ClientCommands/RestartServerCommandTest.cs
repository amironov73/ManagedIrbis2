using System;
using System.IO;
using System.Text;

using AM.IO;
using AM.Runtime;
using AM.Text;

using ManagedIrbis;
using ManagedIrbis.Infrastructure;
using ManagedIrbis.Infrastructure.ClientCommands;
using ManagedIrbis.Infrastructure.Sockets;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace UnitTests.ManagedIrbis.Infrastructure.ClientCommands
{
    [TestClass]
    public class RestartServerCommandTest
        : CommandTest
    {
        //[TestMethod]
        //public void RestartServerCommand_Construciton_1()
        //{
        //    Mock<IIrbisConnection> mock = GetConnectionMock();
        //    IIrbisConnection connection = mock.Object;
        //    RestartServerCommand command = new RestartServerCommand();
        //    Assert.AreSame(connection, command.Connection);
        //}

        [TestMethod]
        public void RestartServerCommand_ExecuteRequest_1()
        {
            int returnCode = 0;
            Mock<IIrbisConnection> mock = GetConnectionMock();
            IIrbisConnection connection = mock.Object;
            RestartServerCommand command = new RestartServerCommand();
            ResponseBuilder builder = new ResponseBuilder()
                .StandardHeader(CommandCode.RestartServer, 123, 456)
                .NewLine()
                .Append(returnCode)
                .NewLine();
            TestingSocket socket = (TestingSocket) connection.Socket;
            socket.Response = builder.Encode();
            ClientContext context = new ClientContext(connection);
            ServerResponse response = command.Execute(context);
            Assert.AreEqual(returnCode, response.ReturnCode);
        }

        //[TestMethod]
        //public void RestartServerCommand_Verify_1()
        //{
        //    Mock<IIrbisConnection> mock = GetConnectionMock();
        //    IIrbisConnection connection = mock.Object;
        //    RestartServerCommand command = new RestartServerCommand();
        //    Assert.IsTrue(command.Verify(false));
        //}
    }
}
