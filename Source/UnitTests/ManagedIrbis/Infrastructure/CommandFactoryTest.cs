using System;

using ManagedIrbis;
using ManagedIrbis.Infrastructure;
using ManagedIrbis.Infrastructure.ClientCommands;

using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable ConvertToLocalFunction

namespace UnitTests.ManagedIrbis.Infrastructure
{
    [TestClass]
    public class CommandFactoryTest
    {
        [TestMethod]
        public void CommandFactory_Construction_1()
        {
            IIrbisConnection connection = new IrbisConnection();
            CommandFactory factory = new CommandFactory(connection);
            Assert.AreSame(connection, factory.Connection);
        }

        [TestMethod]
        public void CommandFactory_CreateCommand_1()
        {
            IIrbisConnection connection = new IrbisConnection();
            CommandFactory factory = new CommandFactory(connection);
            NopCommand command = factory.CreateCommand<NopCommand>();
            Assert.IsNull(command);
        }

        [TestMethod]
        public void CommandFactory_CreateUniversalCommand_1()
        {
            IIrbisConnection connection = new IrbisConnection();
            CommandFactory factory = new CommandFactory(connection);
            string code = "CODE";
            object[] arguments = {"first", "second", "third"};
            UniversalCommand command = factory.CreateUniversalCommand(code, arguments);
            Assert.AreEqual(code, command.CommandCode);
            CollectionAssert.AreEqual(arguments, command.Arguments);
        }

        [TestMethod]
        public void CommandFactory_CreateUniversalTextCommand_1()
        {
            IIrbisConnection connection = new IrbisConnection();
            CommandFactory factory = new CommandFactory(connection);
            string code = "CODE";
            string[] arguments = {"first", "second", "third"};
            UniversalTextCommand command = factory.CreateUniversalTextCommand
                (
                    code,
                    arguments,
                    IrbisEncoding.Ansi
                );
            Assert.AreEqual(code, command.CommandCode);
        }

        [TestMethod]
        public void CommandFactory_SetSuperFactory_1()
        {
            Func<IIrbisConnection, CommandFactory> newFactory
                = connection => new CommandFactory(connection);
            Func<IIrbisConnection, CommandFactory> saved
                = CommandFactory.SetSuperFactory(newFactory);
            Func<IIrbisConnection, CommandFactory> previous
                = CommandFactory.SetSuperFactory(saved);
            Assert.AreSame(newFactory, previous);
        }
    }
}
