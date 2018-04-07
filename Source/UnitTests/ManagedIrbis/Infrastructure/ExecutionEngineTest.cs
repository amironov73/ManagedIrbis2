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
    public class ExecutionEngineTest
    {
        [NotNull]
        private ExecutionEngine _GetEngine()
        {
            IServiceProvider services = new ServiceContainer();
            Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            IIrbisConnection connection = mock.Object;
            BusyState busyState = new BusyState();
            mock.SetupGet(c => c.Busy).Returns(busyState);
            mock.SetupGet(c => c.Workstation).Returns(IrbisWorkstation.Cataloger);
            mock.SetupGet(c => c.ClientID).Returns(123456);
            ExecutionEngine result = new ExecutionEngine(connection);
            mock.SetupGet(c => c.Executive).Returns(result);
            TestingSocket socket = new TestingSocket(connection);
            ServerResponse response = ServerResponse.GetEmptyResponse(connection);
            socket.Response = response.GetDump();
            mock.SetupGet(c => c.Socket).Returns(socket);
            result.Services = new NonNullValue<IServiceProvider>(services);

            return result;
        }

        [NotNull]
        private ExecutionEngine _GetEngine2()
        {
            IServiceProvider services = new ServiceContainer();
            Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            IIrbisConnection connection = mock.Object;
            BusyState busyState = new BusyState();
            mock.SetupGet(c => c.Busy).Returns(busyState);
            mock.SetupGet(c => c.Workstation).Returns(IrbisWorkstation.Cataloger);
            mock.SetupGet(c => c.ClientID).Returns(123456);
            ExecutionEngine result = new ExecutionEngine(connection);
            mock.SetupGet(c => c.Executive).Returns(result);
            SimpleClientSocket socket = new SimpleClientSocket(connection);
            mock.SetupGet(c => c.Socket).Returns(socket);
            result.Services = new NonNullValue<IServiceProvider>(services);

            return result;
        }

        [TestMethod]
        public void ExecutionEngine_Construction_1()
        {
            Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            IIrbisConnection connection = mock.Object;

            ExecutionEngine inner = new ExecutionEngine(connection);
            Assert.AreSame(connection, inner.Connection);
            Assert.IsNotNull(inner.Services.Value);

            ExecutionEngine outer = new ExecutionEngine(connection, inner);
            Assert.AreSame(connection, outer.Connection);
            Assert.AreSame(inner, outer.NestedEngine);
            Assert.IsNotNull(outer.Services.Value);
        }

        [TestMethod]
        public void ExecutionEngine_ThrowOnVerify_1()
        {
            bool saved = ExecutionEngine.ThrowOnVerify;
            ExecutionEngine.ThrowOnVerify = false;
            Assert.IsFalse(ExecutionEngine.ThrowOnVerify);
            ExecutionEngine.ThrowOnVerify = true;
            Assert.IsTrue(ExecutionEngine.ThrowOnVerify);
            ExecutionEngine.ThrowOnVerify = saved;
        }

        [TestMethod]
        public void ExecutionEngine_ExecuteCommand_1()
        {
            bool before = false, after = false;

            ExecutionEngine engine = _GetEngine();
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
        public void ExecutionEngine_ExecuteCommand_2()
        {
            bool exceptionSeen = false;

            ExecutionEngine engine = _GetEngine();
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
        public void ExecutionEngine_ExecuteCommand_3()
        {
            ExecutionEngine engine = _GetEngine2();
            ClientCommand command = new NopCommand(engine.Connection);
            ExecutionContext context = new ExecutionContext(engine.Connection, command);

            engine.ExecuteCommand(context);
        }

        [TestMethod]
        [ExpectedException(typeof(IrbisNetworkException))]
        public void ExecutionEngine_ExecuteCommand_4()
        {
            bool saved = ExecutionEngine.ThrowOnVerify;
            try
            {
                ExecutionEngine.ThrowOnVerify = false;
                ExecutionEngine engine = _GetEngine();
                DynamicCommand command = new DynamicCommand(engine.Connection);
                command.VerifyHandler += (dynamicCommand, b) => false;
                ExecutionContext context = new ExecutionContext(engine.Connection, command);

                engine.ExecuteCommand(context);
            }
            finally
            {
                ExecutionEngine.ThrowOnVerify = saved;
            }
        }

        [TestMethod]
        public void ExecutionEngine_GetMemoryStream_1()
        {
            ExecutionEngine engine = _GetEngine();
            MemoryStream stream = engine.GetMemoryStream(typeof(ExecutionEngineTest));
            Assert.IsNotNull(stream);
        }

        [TestMethod]
        public void ExecutionEngine_ReportMemoryUsage_1()
        {
            ExecutionEngine engine = _GetEngine();
            engine.ReportMemoryUsage(typeof(ExecutionEngineTest), 100500);
        }
    }
}
