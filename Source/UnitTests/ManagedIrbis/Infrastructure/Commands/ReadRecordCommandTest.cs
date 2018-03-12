﻿using System;
using System.IO;
using System.Text;

using AM.IO;
using AM.Runtime;
using AM.Text;

using ManagedIrbis;
using ManagedIrbis.Infrastructure;
using ManagedIrbis.Infrastructure.Commands;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace UnitTests.ManagedIrbis.Infrastructure.Commands
{
    [TestClass]
    public class ReadRecordCommandTest
        : CommandTest
    {
        [TestMethod]
        public void ReadRecordCommand_Construciton_1()
        {
            Mock<IIrbisConnection> mock = GetConnectionMock();
            IIrbisConnection connection = mock.Object;
            ReadRecordCommand command
                = new ReadRecordCommand(connection);
            Assert.AreSame(connection, command.Connection);
        }

        [TestMethod]
        public void ReadRecordCommand_Verify_1()
        {
            Mock<IIrbisConnection> mock = GetConnectionMock();
            IIrbisConnection connection = mock.Object;
            ReadRecordCommand command
                = new ReadRecordCommand(connection);
            Assert.IsFalse(command.Verify(false));
        }
    }
}
