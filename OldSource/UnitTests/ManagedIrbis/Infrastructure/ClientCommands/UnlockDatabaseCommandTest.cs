﻿using System;
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
    public class UnlockDatabaseCommandTest
        : CommandTest
    {
        //[TestMethod]
        //public void UnlockDatabaseCommand_Construciton_1()
        //{
        //    Mock<IIrbisConnection> mock = GetConnectionMock();
        //    IIrbisConnection connection = mock.Object;
        //    UnlockDatabaseCommand command = new UnlockDatabaseCommand();
        //    Assert.AreSame(connection, command.Connection);
        //}

        //[TestMethod]
        //public void UnlockDatabaseCommand_Verify_1()
        //{
        //    Mock<IIrbisConnection> mock = GetConnectionMock();
        //    IIrbisConnection connection = mock.Object;
        //    UnlockDatabaseCommand command = new UnlockDatabaseCommand();
        //    Assert.IsFalse(command.Verify(false));
        //}
    }
}
