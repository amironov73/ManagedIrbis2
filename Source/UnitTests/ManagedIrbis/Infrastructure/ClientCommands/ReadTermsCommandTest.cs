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
    public class ReadTermsCommandTest
        : CommandTest
    {
        //[TestMethod]
        //public void ReadTermsCommandCommand_Construciton_1()
        //{
        //    Mock<IIrbisConnection> mock = GetConnectionMock();
        //    IIrbisConnection connection = mock.Object;
        //    ReadTermsCommand command = new ReadTermsCommand();
        //    Assert.AreSame(connection, command.Connection);
        //}

        [TestMethod]
        public void ReadTermsCommand_ExecuteRequest_1()
        {
            int returnCode = 0;
            Mock<IIrbisConnection> mock = GetConnectionMock();
            IIrbisConnection connection = mock.Object;
            ReadTermsCommand command = new ReadTermsCommand
            {
                Database = "IBIS"
            };
            ResponseBuilder builder = new ResponseBuilder()
                .StandardHeader(CommandCode.ReadTerms, 123, 456)
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
        //public void ReadTermsCommand_Verify_1()
        //{
        //    Mock<IIrbisConnection> mock = GetConnectionMock();
        //    IIrbisConnection connection = mock.Object;
        //    ReadTermsCommand command = new ReadTermsCommand();
        //    Assert.IsTrue(command.Verify(false));
        //}
    }
}
