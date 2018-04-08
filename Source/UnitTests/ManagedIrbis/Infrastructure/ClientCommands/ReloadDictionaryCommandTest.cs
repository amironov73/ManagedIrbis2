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
    public class ReloadDictionaryCommandTest
        : CommandTest
    {
        //[TestMethod]
        //public void ReloadDictionaryCommand_Construciton_1()
        //{
        //    Mock<IIrbisConnection> mock = GetConnectionMock();
        //    IIrbisConnection connection = mock.Object;
        //    ReloadDictionaryCommand command = new ReloadDictionaryCommand();
        //    Assert.AreSame(connection, command.Connection);
        //}

        [TestMethod]
        public void ReloadDictionaryCommand_ExecuteRequest_1()
        {
            int returnCode = 0;
            Mock<IIrbisConnection> mock = GetConnectionMock();
            IIrbisConnection connection = mock.Object;
            ReloadDictionaryCommand command = new ReloadDictionaryCommand
            {
                Database = "IBIS"
            };
            ResponseBuilder builder = new ResponseBuilder()
                .StandardHeader(CommandCode.ReloadDictionary, 123, 456)
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
        //public void ReloadDictionaryCommand_Verify_1()
        //{
        //    Mock<IIrbisConnection> mock = GetConnectionMock();
        //    IIrbisConnection connection = mock.Object;
        //    ReloadDictionaryCommand command = new ReloadDictionaryCommand();
        //    Assert.IsFalse(command.Verify(false));
        //}

        //[TestMethod]
        //public void ReloadDictionaryCommand_Verify_2()
        //{
        //    Mock<IIrbisConnection> mock = GetConnectionMock();
        //    IIrbisConnection connection = mock.Object;
        //    ReloadDictionaryCommand command = new ReloadDictionaryCommand
        //        {
        //            Database = "IBIS"
        //        };
        //    Assert.IsTrue(command.Verify(false));
        //}
    }
}
