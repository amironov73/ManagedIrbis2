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
    public class ActualizeRecordCommandTest
        : CommandTest
    {
        //[TestMethod]
        //public void ActualizeRecordCommand_Construciton_1()
        //{
        //    Mock<IIrbisConnection> mock = GetConnectionMock();
        //    IIrbisConnection connection = mock.Object;
        //    ActualizeRecordCommand command = new ActualizeRecordCommand(connection);
        //    Assert.AreSame(connection, command.Connection);
        //}

        [TestMethod]
        public void ActualizeRecordCommand_CreateQuery_2()
        {
            Mock<IIrbisConnection> mock = GetConnectionMock();
            IIrbisConnection connection = mock.Object;
            ActualizeRecordCommand command = new ActualizeRecordCommand
            {
                Database = "IBIS",
                Mfn = 123
            };
            ClientQuery query = command.CreateQuery(connection, CommandCode.ActualizeRecord);
            Assert.IsNotNull(query);
        }

        //[TestMethod]
        //public void ActualizeRecordCommand_ExecuteRequest_1()
        //{
        //    int returnCode = 0;
        //    Mock<IIrbisConnection> mock = GetConnectionMock();
        //    IIrbisConnection connection = mock.Object;
        //    ActualizeRecordCommand command = new ActualizeRecordCommand(connection)
        //    {
        //        Database = "IBIS",
        //        Mfn = 123
        //    };
        //    ResponseBuilder builder = new ResponseBuilder()
        //        .StandardHeader(CommandCode.ActualizeRecord, 123, 456)
        //        .NewLine()
        //        .Append(returnCode)
        //        .NewLine();
        //    TestingSocket socket = (TestingSocket) connection.Socket;
        //    socket.Response = builder.Encode();
        //    ServerResponse response = command.Execute();
        //    Assert.AreEqual(returnCode, response.ReturnCode);
        //}
    }
}
