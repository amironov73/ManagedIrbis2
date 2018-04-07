using System;
using System.ComponentModel.Design;
using System.IO;

using AM;
using AM.Threading;

using JetBrains.Annotations;

using ManagedIrbis;
using ManagedIrbis.Infrastructure;
using ManagedIrbis.Infrastructure.ClientCommands;
using ManagedIrbis.Infrastructure.Sockets;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace UnitTests.ManagedIrbis.Infrastructure
{
    [TestClass]
    public class AbstractEngineTest
    {
        [NotNull]
        private AbstractEngine _GetEngine()
        {
            IServiceProvider services = new ServiceContainer();
            Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            IIrbisConnection connection = mock.Object;
            BusyState busyState = new BusyState();
            mock.SetupGet(c => c.Busy).Returns(busyState);
            mock.SetupGet(c => c.Workstation).Returns(IrbisWorkstation.Cataloger);
            mock.SetupGet(c => c.ClientID).Returns(123456);
            AbstractEngine result = new StandardEngine(connection, null);
            mock.SetupGet(c => c.Executive).Returns(result);
            TestingSocket socket = new TestingSocket(connection);
            ServerResponse response = ServerResponse.GetEmptyResponse(connection);
            socket.Response = response.GetDump();
            mock.SetupGet(c => c.Socket).Returns(socket);
            result.Services = new NonNullValue<IServiceProvider>(services);

            return result;
        }

        [NotNull]
        private AbstractEngine _GetEngine2()
        {
            IServiceProvider services = new ServiceContainer();
            Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            IIrbisConnection connection = mock.Object;
            BusyState busyState = new BusyState();
            mock.SetupGet(c => c.Busy).Returns(busyState);
            mock.SetupGet(c => c.Workstation).Returns(IrbisWorkstation.Cataloger);
            mock.SetupGet(c => c.ClientID).Returns(123456);
            AbstractEngine result = new StandardEngine(connection, null);
            mock.SetupGet(c => c.Executive).Returns(result);
            SimpleClientSocket socket = new SimpleClientSocket(connection);
            mock.SetupGet(c => c.Socket).Returns(socket);
            result.Services = new NonNullValue<IServiceProvider>(services);

            return result;
        }

        [TestMethod]
        public void AbstractEngine_Construction_1()
        {
            Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            IIrbisConnection connection = mock.Object;

            AbstractEngine inner = new StandardEngine(connection, null);
            Assert.AreSame(connection, inner.Connection);
            Assert.IsNotNull(inner.Services.Value);

            AbstractEngine outer = new StandardEngine(connection, inner);
            Assert.AreSame(connection, outer.Connection);
            Assert.AreSame(inner, outer.NestedEngine);
            Assert.IsNotNull(outer.Services.Value);
        }

        [TestMethod]
        public void AbstractEngine_ThrowOnVerify_1()
        {
            bool saved = AbstractEngine.ThrowOnVerify;
            AbstractEngine.ThrowOnVerify = false;
            Assert.IsFalse(AbstractEngine.ThrowOnVerify);
            AbstractEngine.ThrowOnVerify = true;
            Assert.IsTrue(AbstractEngine.ThrowOnVerify);
            AbstractEngine.ThrowOnVerify = saved;
        }

        [TestMethod]
        public void AbstractEngine_ExecuteCommand_1()
        {
            bool before = false, after = false;

            AbstractEngine engine = _GetEngine();
            engine.BeforeExecution += (sender, args) => { before = true; };
            engine.AfterExecution += (sender, args) => { after = true; };
            ClientCommand command = new NopCommand(engine.Connection);
            command.RelaxResponse = true;
            ExecutionContext context = new ExecutionContext(engine.Connection, command);

            engine.ExecuteCommand(context);

            Assert.IsTrue(before);
            Assert.IsTrue(after);
        }

        [TestMethod]
        public void AbstractEngine_ExecuteCommand_2()
        {
            bool exceptionSeen = false;

            AbstractEngine engine = _GetEngine();
            engine.ExceptionOccurs += (sender, args) => { exceptionSeen = true; };
            DynamicCommand command = new DynamicCommand(engine.Connection)
            {
                ExecuteHandler = dynamicCommand => throw new IrbisNetworkException()
            };
            ExecutionContext context = new ExecutionContext(engine.Connection, command);

            try
            {
                engine.ExecuteCommand(context);
            }
            catch
            {
                // Eat the exception
            }

            Assert.IsTrue(exceptionSeen);
        }

        [TestMethod]
        [ExpectedException(typeof(IrbisException))]
        public void AbstractEngine_ExecuteCommand_3()
        {
            AbstractEngine engine = _GetEngine2();
            ClientCommand command = new NopCommand(engine.Connection);
            ExecutionContext context = new ExecutionContext(engine.Connection, command);

            engine.ExecuteCommand(context);
        }

        [TestMethod]
        [ExpectedException(typeof(IrbisNetworkException))]
        public void AbstractEngine_ExecuteCommand_4()
        {
            bool saved = AbstractEngine.ThrowOnVerify;
            try
            {
                AbstractEngine.ThrowOnVerify = false;
                AbstractEngine engine = _GetEngine();
                DynamicCommand command = new DynamicCommand(engine.Connection);
                command.VerifyHandler += (dynamicCommand, b) => false;
                ExecutionContext context = new ExecutionContext(engine.Connection, command);

                engine.ExecuteCommand(context);
            }
            finally
            {
                AbstractEngine.ThrowOnVerify = saved;
            }
        }

        [TestMethod]
        public void AbstractEngine_GetMemoryStream_1()
        {
            AbstractEngine engine = _GetEngine();
            MemoryStream stream = engine.GetMemoryStream(typeof(AbstractEngineTest));
            Assert.IsNotNull(stream);
        }

        [TestMethod]
        public void AbstractEngine_ReportMemoryUsage_1()
        {
            AbstractEngine engine = _GetEngine();
            engine.ReportMemoryUsage(typeof(AbstractEngineTest), 100500);
        }
    }
}
