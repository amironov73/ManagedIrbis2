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
    public class GblVirtualCommandTest
        : CommandTest
    {
        //[TestMethod]
        //public void GblVirtualCommand_Construciton_1()
        //{
        //    Mock<IIrbisConnection> mock = GetConnectionMock();
        //    IIrbisConnection connection = mock.Object;
        //    GblVirtualCommand command = new GblVirtualCommand();
        //    Assert.AreSame(connection, command.Connection);
        //}

        //[TestMethod]
        //public void GblVirtualCommand_Verify_1()
        //{
        //    Mock<IIrbisConnection> mock = GetConnectionMock();
        //    IIrbisConnection connection = mock.Object;
        //    GblVirtualCommand command = new GblVirtualCommand();
        //    Assert.IsTrue(command.Verify(false));
        //}
    }
}
