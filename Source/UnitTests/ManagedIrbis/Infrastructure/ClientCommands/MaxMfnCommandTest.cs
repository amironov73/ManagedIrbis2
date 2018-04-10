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
    public class MaxMfnCommandTest
        : CommandTest
    {
        //[TestMethod]
        //public void MaxMfnCommand_Construciton_1()
        //{
        //    Mock<IIrbisConnection> mock = GetConnectionMock();
        //    IIrbisConnection connection = mock.Object;
        //    MaxMfnCommand command = new MaxMfnCommand();
        //    Assert.AreSame(connection, command.Connection);
        //}

        [TestMethod]
        public void MaxMfnCommand_ExecuteRequest_1()
        {
            int maxMfn = 123456;
            Mock<IIrbisConnection> mock = GetConnectionMock();
            IIrbisConnection connection = mock.Object;
            MaxMfnCommand command = new MaxMfnCommand
            {
                Database = "IBIS"
            };
            ResponseBuilder builder = new ResponseBuilder()
                .StandardHeader(CommandCode.GetMaxMfn, 123, 456)
                .NewLine()
                .Append(maxMfn)
                .NewLine();
            TestingSocket socket = (TestingSocket) connection.Socket;
            socket.Response = builder.Encode();
            ClientContext context = new ClientContext(connection);
            command.Execute(context);
            ServerResponse response = context.Response;
            Assert.AreEqual(maxMfn, response.ReturnCode);
        }

        //[TestMethod]
        //public void MaxMfnCommand_Verify_1()
        //{
        //    Mock<IIrbisConnection> mock = GetConnectionMock();
        //    IIrbisConnection connection = mock.Object;
        //    MaxMfnCommand command = new MaxMfnCommand();
        //    Assert.IsFalse(command.Verify(false));
        //}

        //[TestMethod]
        //public void MaxMfnCommand_Verify_2()
        //{
        //    Mock<IIrbisConnection> mock = GetConnectionMock();
        //    IIrbisConnection connection = mock.Object;
        //    MaxMfnCommand command = new MaxMfnCommand
        //    {
        //        Database = "IBIS"
        //    };
        //    Assert.IsTrue(command.Verify(false));
        //}
    }
}
