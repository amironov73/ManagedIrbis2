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
    public class DatabaseStatCommandTest
        : CommandTest
    {
        [TestMethod]
        public void DatabaseStatCommand_Construciton_1()
        {
            Mock<IIrbisConnection> mock = GetConnectionMock();
            IIrbisConnection connection = mock.Object;
            DatabaseStatCommand command
                = new DatabaseStatCommand(connection);
            Assert.AreSame(connection, command.Connection);
        }

        [TestMethod]
        public void DatabaseStatCommand_Verify_1()
        {
            Mock<IIrbisConnection> mock = GetConnectionMock();
            IIrbisConnection connection = mock.Object;
            DatabaseStatCommand command
                = new DatabaseStatCommand(connection);
            Assert.IsTrue(command.Verify(false));
        }
    }
}
