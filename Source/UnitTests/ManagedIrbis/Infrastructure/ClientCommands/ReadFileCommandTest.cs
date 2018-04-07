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
    public class ReadFileCommandTest
        : CommandTest
    {
        [TestMethod]
        public void ReadFileCommand_Construciton_1()
        {
            Mock<IIrbisConnection> mock = GetConnectionMock();
            IIrbisConnection connection = mock.Object;
            ReadFileCommand command
                = new ReadFileCommand(connection);
            Assert.AreSame(connection, command.Connection);
        }

        [TestMethod]
        public void ReadFileCommand_ExecuteRequest_1()
        {
            int returnCode = 0;
            Mock<IIrbisConnection> mock = GetConnectionMock();
            IIrbisConnection connection = mock.Object;
            ReadFileCommand command = new ReadFileCommand(connection)
            {
                Files =
                {
                    new FileSpecification(IrbisPath.MasterFile, "IBIS", "file.txt")
                }
            };
            ResponseBuilder builder = new ResponseBuilder()
                .StandardHeader(CommandCode.ReadDocument, 123, 456)
                .NewLine()
                .Append(returnCode)
                .NewLine();
            TestingSocket socket = (TestingSocket) connection.Socket;
            socket.Response = builder.Encode();
            ServerResponse response = command.Execute();
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void ReadFileCommand_Verify_1()
        {
            Mock<IIrbisConnection> mock = GetConnectionMock();
            IIrbisConnection connection = mock.Object;
            ReadFileCommand command
                = new ReadFileCommand(connection);
            Assert.IsFalse(command.Verify(false));
        }
    }
}
