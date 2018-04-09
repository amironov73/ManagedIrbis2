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
    public class DisconnectCommandTest
        : CommandTest
    {
        //[TestMethod]
        //public void DisconnectCommand_Construciton_1()
        //{
        //    Mock<IIrbisConnection> mock = GetConnectionMock();
        //    IIrbisConnection connection = mock.Object;
        //    DisconnectCommand command = new DisconnectCommand();
        //    Assert.AreSame(connection, command.Connection);
        //}

        //[TestMethod]
        //public void DisconnectCommand_CheckResponse_1()
        //{
        //    Mock<IIrbisConnection> mock = GetConnectionMock();
        //    IIrbisConnection connection = mock.Object;
        //    DisconnectCommand command = new DisconnectCommand();
        //    ResponseBuilder builder = new ResponseBuilder();
        //    builder.Append(0).NewLine();
        //    byte[] bytes = builder.Encode();
        //    ServerResponse response = new ServerResponse(connection, bytes, bytes, true);
        //    command.CheckResponse(response);
        //}

        [TestMethod]
        public void DisconnectCommand_ExecuteRequest_1()
        {
            int returnCode = 0;
            Mock<IIrbisConnection> mock = GetConnectionMock();
            IIrbisConnection connection = mock.Object;
            DisconnectCommand command = new DisconnectCommand();
            ResponseBuilder builder = new ResponseBuilder()
                .StandardHeader(CommandCode.UnregisterClient, 123, 456)
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
        //public void DisconnectCommand_Verify_1()
        //{
        //    Mock<IIrbisConnection> mock = GetConnectionMock();
        //    IIrbisConnection connection = mock.Object;
        //    DisconnectCommand command = new DisconnectCommand();
        //    Assert.IsTrue(command.Verify(false));
        //}
    }
}
