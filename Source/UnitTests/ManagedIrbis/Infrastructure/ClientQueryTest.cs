using System.IO;

using JetBrains.Annotations;

using ManagedIrbis;
using ManagedIrbis.Infrastructure;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.ManagedIrbis.Infrastructure
{
    [TestClass]
    public class ClientQueryTest
    {
        [NotNull]
        private ClientQuery _GetClientQuery()
        {
            ClientQuery result = new ClientQuery
            {
                CommandCode = CommandCode.Nop,
                Workstation = IrbisWorkstation.Cataloger,
                ClientID = 123456,
                CommandNumber = 123,
                UserLogin = "логин",
                UserPassword = "пароль"
            };

            result
                .AddAnsi("Строка ANSI")
                .Add(null)
                .AddUtf8("Строка UTF8");

            return result;
        }

        [TestMethod]
        public void ClientQuery_Construction_1()
        {
            ClientQuery query = _GetClientQuery();

            Assert.AreEqual(3, query.Arguments.Count);
            Assert.IsNotNull(query.CommandCode);
            Assert.IsNotNull(query.UserLogin);
            Assert.IsNotNull(query.UserPassword);
        }

        [TestMethod]
        public void ClientQuery_Clear_1()
        {
            ClientQuery query = _GetClientQuery();

            query.Clear();
            Assert.AreEqual(0, query.Arguments.Count);
        }

        [TestMethod]
        public void ClientQuery_EncodePacket_1()
        {
            ClientQuery query = _GetClientQuery();
            byte[] packet = query.EncodePacket();

            Assert.IsNotNull(packet);
            Assert.IsTrue(packet.Length > 10);
        }

        [TestMethod]
        public void ClientQuery_Verify_1()
        {
            ClientQuery query = _GetClientQuery();

            Assert.IsTrue(query.Verify(true));
        }

        [TestMethod]
        public void ClientQuery_Dump_1()
        {
            ClientQuery query = _GetClientQuery();

            StringWriter writer = new StringWriter();
            query.Dump(writer);
            string text = writer.ToString();
            Assert.IsNotNull(text);
        }

        [TestMethod]
        public void ClientQuery_ToString_1()
        {
            ClientQuery query = _GetClientQuery();
            string expected = "CommandCode: N, Workstation: Cataloger, ClientID: 123456, CommandNumber: 123, UserLogin: логин, UserPassword: пароль, Arguments: 3";
            string actual = query.ToString();
            Assert.AreEqual(expected, actual);
        }
    }
}
