using System;
using System.IO;
using System.Text;

using AM.IO;
using AM.Runtime;
using AM.Text;

using ManagedIrbis;
using ManagedIrbis.Infrastructure;
using ManagedIrbis.Infrastructure.ClientCommands;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace UnitTests.ManagedIrbis.Infrastructure.ClientCommands
{
    [TestClass]
    public class TruncateDatabaseCommandTest
        : CommandTest
    {
        [TestMethod]
        public void TruncateDatabaseCommand_Construciton_1()
        {
            Mock<IIrbisConnection> mock = GetConnectionMock();
            IIrbisConnection connection = mock.Object;
            TruncateDatabaseCommand command
                = new TruncateDatabaseCommand(connection);
            Assert.AreSame(connection, command.Connection);
        }

        [TestMethod]
        public void TruncateDatabaseCommand_Verify_1()
        {
            Mock<IIrbisConnection> mock = GetConnectionMock();
            IIrbisConnection connection = mock.Object;
            TruncateDatabaseCommand command
                = new TruncateDatabaseCommand(connection);
            Assert.IsFalse(command.Verify(false));
        }
    }
}
