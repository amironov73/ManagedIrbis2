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
    public class ReadBinaryFileCommandTest
        : CommandTest
    {
        [TestMethod]
        public void ReadBinaryFileCommand_Construciton_1()
        {
            Mock<IIrbisConnection> mock = GetConnectionMock();
            IIrbisConnection connection = mock.Object;
            ReadBinaryFileCommand command
                = new ReadBinaryFileCommand(connection);
            Assert.AreSame(connection, command.Connection);
        }

        [TestMethod]
        public void ReadBinaryFileCommand_Verify_1()
        {
            Mock<IIrbisConnection> mock = GetConnectionMock();
            IIrbisConnection connection = mock.Object;
            ReadBinaryFileCommand command
                = new ReadBinaryFileCommand(connection);
            Assert.IsFalse(command.Verify(false));
        }
    }
}
