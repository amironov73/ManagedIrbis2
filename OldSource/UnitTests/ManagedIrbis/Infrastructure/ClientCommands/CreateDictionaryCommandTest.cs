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
    public class CreateDictionaryCommandTest
        : CommandTest
    {
        //[TestMethod]
        //public void CreateDictionaryCommand_Construciton_1()
        //{
        //    Mock<IIrbisConnection> mock = GetConnectionMock();
        //    IIrbisConnection connection = mock.Object;
        //    CreateDictionaryCommand command = new CreateDictionaryCommand();
        //    Assert.AreSame(connection, command.Connection);
        //}

        [TestMethod]
        public void CreateDictionaryCommand_CreateQuery_2()
        {
            Mock<IIrbisConnection> mock = GetConnectionMock();
            IIrbisConnection connection = mock.Object;
            CreateDictionaryCommand command = new CreateDictionaryCommand
            {
                Database = "IBIS"
            };
            ClientContext context = new ClientContext(connection);
            ClientQuery query = command.CreateQuery(context, CommandCode.CreateDictionary);
            Assert.IsNotNull(query);
        }

        [TestMethod]
        public void CreateDictionaryCommand_ExecuteRequest_1()
        {
            int returnCode = 0;
            Mock<IIrbisConnection> mock = GetConnectionMock();
            IIrbisConnection connection = mock.Object;
            CreateDictionaryCommand command = new CreateDictionaryCommand
            {
                Database = "IBIS"
            };
            ResponseBuilder builder = new ResponseBuilder()
                .StandardHeader(CommandCode.CreateDictionary, 123, 456)
                .NewLine()
                .Append(returnCode)
                .NewLine();
            TestingSocket socket = (TestingSocket) connection.Socket;
            socket.Response = builder.Encode();
            ClientContext context = new ClientContext(connection);
            command.Execute(context);
            ServerResponse response = context.Response;
            Assert.AreEqual(returnCode, response.ReturnCode);
        }

        //[TestMethod]
        //public void CreateDictionaryCommand_Verify_1()
        //{
        //    Mock<IIrbisConnection> mock = GetConnectionMock();
        //    IIrbisConnection connection = mock.Object;
        //    CreateDictionaryCommand command = new CreateDictionaryCommand();
        //    Assert.IsFalse(command.Verify(false));
        //}

        //[TestMethod]
        //public void CreateDictionaryCommand_Verify_2()
        //{
        //    Mock<IIrbisConnection> mock = GetConnectionMock();
        //    IIrbisConnection connection = mock.Object;
        //    CreateDictionaryCommand command = new CreateDictionaryCommand
        //    {
        //        Database = "IBIS"
        //    };
        //    Assert.IsTrue(command.Verify(false));
        //}
    }
}
