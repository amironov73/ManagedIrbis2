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
    public class NopCommandTest
        : CommandTest
    {
        //[TestMethod]
        //public void NopCommand_Construciton_1()
        //{
        //    Mock<IIrbisConnection> mock = GetConnectionMock();
        //    IIrbisConnection connection = mock.Object;
        //    NopCommand command = new NopCommand();
        //    Assert.AreSame(connection, command.Connection);
        //}

        [TestMethod]
        public void NopCommand_ExecuteRequest_1()
        {
            Mock<IIrbisConnection> mock = GetConnectionMock();
            IIrbisConnection connection = mock.Object;
            NopCommand command = new NopCommand();
            ResponseBuilder builder = new ResponseBuilder()
                .StandardHeader(CommandCode.Nop, 123, 456)
                .NewLine()
                .Append(0)
                .NewLine();
            TestingSocket socket = (TestingSocket) connection.Socket;
            socket.Response = builder.Encode();
            ClientContext context = new ClientContext(connection);
            command.Execute(context);
            ServerResponse response = context.Response;
            Assert.AreEqual(0, response.ReturnCode);
        }

        //[TestMethod]
        //public void NopCommand_Verify_1()
        //{
        //    Mock<IIrbisConnection> mock = GetConnectionMock();
        //    IIrbisConnection connection = mock.Object;
        //    NopCommand command = new NopCommand();
        //    Assert.IsTrue(command.Verify(false));
        //}
    }
}
