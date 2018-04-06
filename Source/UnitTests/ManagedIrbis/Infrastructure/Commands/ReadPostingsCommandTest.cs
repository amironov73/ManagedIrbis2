﻿using System;
using System.IO;
using System.Text;

using AM.IO;
using AM.Runtime;
using AM.Text;

using ManagedIrbis;
using ManagedIrbis.Infrastructure;
using ManagedIrbis.Infrastructure.Commands;
using ManagedIrbis.Infrastructure.Sockets;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace UnitTests.ManagedIrbis.Infrastructure.Commands
{
    [TestClass]
    public class ReadPostingsCommandTest
        : CommandTest
    {
        [TestMethod]
        public void ReadPostingsCommand_Construciton_1()
        {
            Mock<IIrbisConnection> mock = GetConnectionMock();
            IIrbisConnection connection = mock.Object;
            ReadPostingsCommand command
                = new ReadPostingsCommand(connection);
            Assert.AreSame(connection, command.Connection);
        }

        [TestMethod]
        public void ReadPostingsCommand_ExecuteRequest_1()
        {
            int returnCode = 0;
            Mock<IIrbisConnection> mock = GetConnectionMock();
            IIrbisConnection connection = mock.Object;
            ReadPostingsCommand command = new ReadPostingsCommand(connection)
            {
                Database = "IBIS",
                ListOfTerms = new []
                {
                    "FIRST",
                    "SECOND",
                    "THIRD"
                }
            };
            ResponseBuilder builder = new ResponseBuilder()
                .StandardHeader(CommandCode.ReadPostings, 123, 456)
                .NewLine()
                .Append(returnCode)
                .NewLine();
            TestingSocket socket = (TestingSocket) connection.Socket;
            socket.Response = builder.Encode();
            ServerResponse response = command.Execute();
            Assert.AreEqual(returnCode, response.ReturnCode);
        }

        [TestMethod]
        public void ReadPostingsCommand_Verify_1()
        {
            Mock<IIrbisConnection> mock = GetConnectionMock();
            IIrbisConnection connection = mock.Object;
            ReadPostingsCommand command
                = new ReadPostingsCommand(connection);
            Assert.IsTrue(command.Verify(false));
        }
    }
}
