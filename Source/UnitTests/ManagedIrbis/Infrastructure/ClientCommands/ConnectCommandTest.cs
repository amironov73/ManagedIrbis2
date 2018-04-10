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
    public class ConnectCommandTest
        : CommandTest
    {
        //[TestMethod]
        //public void ConnectCommand_Construciton_1()
        //{
        //    Mock<IIrbisConnection> mock = GetConnectionMock();
        //    IIrbisConnection connection = mock.Object;
        //    ConnectCommand command = new ConnectCommand();
        //    Assert.AreSame(connection, command.Connection);
        //    Assert.IsFalse(command.RequireConnection);
        //}

        [TestMethod]
        public void ConnectCommand_CreateQuery_3()
        {
            Mock<IIrbisConnection> mock = GetConnectionMock();
            IIrbisConnection connection = mock.Object;
            ConnectCommand command = new ConnectCommand
            {
                Username = "user",
                Password = "pass"
            };
            ClientContext context = new ClientContext(connection);
            ClientQuery query = command.CreateQuery(context, CommandCode.RegisterClient);
            Assert.IsNotNull(query);
        }

        //[TestMethod]
        //public void ConnectCommand_ExecuteRequest_1()
        //{
        //    int returnCode = 0;
        //    string configuration = "Some=Text";
        //    Mock<IIrbisConnection> mock = GetConnectionMock();
        //    IIrbisConnection connection = mock.Object;
        //    ConnectCommand command = new ConnectCommand(connection)
        //    {
        //        Username = "user",
        //        Password = "pass"
        //    };
        //    ResponseBuilder builder = new ResponseBuilder()
        //        .AppendAnsi(CommandCode.RegisterClient).NewLine()
        //        .AppendAnsi("12345678").NewLine()
        //        .AppendAnsi("1").NewLine()
        //        .AppendAnsi("123").NewLine()
        //        .AppendAnsi("64.2014").NewLine()
        //        .NewLine()
        //        .NewLine()
        //        .NewLine()
        //        .NewLine()
        //        .NewLine()
        //        .Append(returnCode).NewLine()
        //        .AppendAnsi("30").NewLine()
        //        .AppendAnsi(configuration);
        //    TestingSocket socket = (TestingSocket)connection.Socket;
        //    socket.Response = builder.Encode();
        //    ServerResponse response = command.Execute();
        //    Assert.AreEqual(returnCode, response.ReturnCode);
        //    Assert.AreEqual(configuration, command.Configuration);
        //    Assert.AreEqual(30, command.ConfirmationInterval);
        //    Assert.AreEqual("64.2014", command.ServerVersion);
        //}

        [TestMethod]
        [ExpectedException(typeof(IrbisException))]
        public void ConnectCommand_ExecuteRequest_2()
        {
            int returnCode = 0;
            string configuration = "Some=Text";
            Mock<IIrbisConnection> mock = GetConnectionMock();
            mock.SetupGet(c => c.Connected).Returns(true);
            IIrbisConnection connection = mock.Object;
            ConnectCommand command = new ConnectCommand
            {
                Username = "user",
                Password = "pass"
            };
            ResponseBuilder builder = new ResponseBuilder()
                .AppendAnsi(CommandCode.RegisterClient).NewLine()
                .AppendAnsi("12345678").NewLine()
                .AppendAnsi("1").NewLine()
                .AppendAnsi("123").NewLine()
                .AppendAnsi("64.2014").NewLine()
                .NewLine()
                .NewLine()
                .NewLine()
                .NewLine()
                .NewLine()
                .Append(returnCode).NewLine()
                .AppendAnsi("30").NewLine()
                .AppendAnsi(configuration);
            TestingSocket socket = (TestingSocket)connection.Socket;
            socket.Response = builder.Encode();
            ClientContext context = new ClientContext(connection);
            command.Execute(context);
        }

        //[TestMethod]
        //public void ConnectCommand_Verify_1()
        //{
        //    Mock<IIrbisConnection> mock = GetConnectionMock();
        //    IIrbisConnection connection = mock.Object;
        //    ConnectCommand command = new ConnectCommand();
        //    Assert.IsFalse(command.Verify(false));
        //}

        //[TestMethod]
        //public void ConnectCommand_Verify_2()
        //{
        //    Mock<IIrbisConnection> mock = GetConnectionMock();
        //    IIrbisConnection connection = mock.Object;
        //    ConnectCommand command = new ConnectCommand
        //    {
        //        Username = "user",
        //        Password = "pass"
        //    };
        //    Assert.IsTrue(command.Verify(false));
        //}
    }
}
