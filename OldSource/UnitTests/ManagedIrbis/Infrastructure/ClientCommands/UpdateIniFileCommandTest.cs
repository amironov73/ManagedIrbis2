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
    public class UpdateIniFileCommandTest
        : CommandTest
    {
        //[TestMethod]
        //public void UpdateIniFileCommand_Construciton_1()
        //{
        //    Mock<IIrbisConnection> mock = GetConnectionMock();
        //    IIrbisConnection connection = mock.Object;
        //    UpdateIniFileCommand command = new UpdateIniFileCommand();
        //    Assert.AreSame(connection, command.Connection);
        //}

        //[TestMethod]
        //public void UpdateIniFileCommand_Verify_1()
        //{
        //    Mock<IIrbisConnection> mock = GetConnectionMock();
        //    IIrbisConnection connection = mock.Object;
        //    UpdateIniFileCommand command
        //        = new UpdateIniFileCommand(connection);
        //    Assert.IsTrue(command.Verify(false));
        //}
    }
}
