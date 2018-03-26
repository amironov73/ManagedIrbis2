using System;
using System.Collections.Generic;
using System.Text;

using AM.IO;

using ManagedIrbis;
using ManagedIrbis.Infrastructure;
using ManagedIrbis.Infrastructure.Commands;
using ManagedIrbis.Menus;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

// ReSharper disable InvokeAsExtensionMethod

namespace UnitTests.ManagedIrbis
{
    [TestClass]
    public class IrbisConnectionUtilityTest
    {
        [TestMethod]
        public void IrbisConnectionUtility_DefaultConnectionString_1()
        {
            string expected = "qqq";
            string save = IrbisConnectionUtility.DefaultConnectionString;
            try
            {
                IrbisConnectionUtility.DefaultConnectionString = expected;
                Assert.AreSame(expected, IrbisConnectionUtility.DefaultConnectionString);
            }
            finally
            {
                IrbisConnectionUtility.DefaultConnectionString = save;
            }
        }

        [TestMethod]
        public void IrbisConnectionUtility_ActualizeDatabase_1()
        {
            Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            mock.Setup(c => c.ActualizeRecord(It.IsAny<string>(), It.IsAny<int>()));

            IIrbisConnection connection = mock.Object;
            IrbisConnectionUtility.ActualizeDatabase(connection, "IBIS");

            mock.Verify(c => c.ActualizeRecord(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void IrbisConnectionUtility_DeleteAnyFile_1()
        {
            Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            mock.Setup(c => c.FormatRecord(It.IsAny<string>(), It.IsAny<int>()));
            mock.Setup(c => c.GetServerVersion()).Returns(new IrbisVersion() { Version = "64.2017.1" });

            IIrbisConnection connection = mock.Object;
            IrbisConnectionUtility.DeleteAnyFile(connection, "any.file");

            mock.Verify(c => c.FormatRecord(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            mock.Verify(c => c.GetServerVersion(), Times.Once);
        }

        [TestMethod]
        public void IrbisConnectionUtility_DeleteRecord_1()
        {
            Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            mock.Setup(c => c.ReadRecord(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new MarcRecord());
            mock.Setup(c => c.WriteRecord(It.IsAny<MarcRecord>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()));

            IIrbisConnection connnection = mock.Object;
            IrbisConnectionUtility.DeleteRecord(connnection, 1);

            mock.Verify(c => c.ReadRecord(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>()),
                Times.Once);
            mock.Verify(c => c.WriteRecord(It.IsAny<MarcRecord>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()),
                Times.Once);
        }

        [TestMethod]
        public void IrbisConnectionUtility_DeleteRecord_2()
        {
            Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            mock.Setup(c => c.ReadRecord(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new MarcRecord());
            mock.Setup(c => c.WriteRecord(It.IsAny<MarcRecord>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()));

            IIrbisConnection connnection = mock.Object;
            IrbisConnectionUtility.DeleteRecord(connnection, 1, true);

            mock.Verify(c => c.ReadRecord(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>()),
                Times.Once);
            mock.Verify(c => c.WriteRecord(It.IsAny<MarcRecord>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()),
                Times.Once);
        }

        //[TestMethod]
        //public void IrbisConnectionUtility_DeleteRecords_1()
        //{
        //}

        [TestMethod]
        public void IrbisConnectionUtility_ExecuteArbitraryCommand_1()
        {
            Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            IIrbisConnection connection = mock.Object;
            CommandFactory factory = new CommandFactory(connection);
            mock.SetupGet(c => c.CommandFactory).Returns(factory);
            ServerResponse response = ServerResponse.GetEmptyResponse(connection);
            mock.Setup(c => c.ExecuteCommand(It.IsAny<AbstractCommand>()))
                .Returns(response);

            IrbisConnectionUtility.ExecuteArbitraryCommand
                (
                    connection,
                    "command",
                    "argument1",
                    "argument2"
                );

            mock.VerifyGet(c => c.CommandFactory, Times.Once);
            mock.Verify(c => c.ExecuteCommand(It.IsAny<AbstractCommand>()), Times.Once);

        }

        [TestMethod]
        public void IrbisConnectionUtility_ExtendedSearch_1()
        {
        }

        [TestMethod]
        public void IrbisConnectionUtility_FormatRecord_1()
        {
            //Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            //IIrbisConnection connection = mock.Object;
            //Mock<FormatCommand> formatMock = new Mock<FormatCommand>(connection);
            //formatMock.Setup()
            //FormatCommand command = formatMock.Object;
            //Mock<CommandFactory> factoryMock = new Mock<CommandFactory>(connection);
            //factoryMock.Setup(c => c.GetFormatCommand()).Returns(command);
            //CommandFactory factory = factoryMock.Object;
            //mock.SetupGet(c => c.CommandFactory).Returns(factory);
            //ServerResponse response = ServerResponse.GetEmptyResponse(connection);
            //mock.Setup(c => c.ExecuteCommand(It.IsAny<AbstractCommand>()))
            //    .Returns(response);

            //IrbisConnectionUtility.FormatRecord()
        }

        [TestMethod]
        public void IrbisConnectionUtility_FormatUtf8_1()
        {

        }

        [TestMethod]
        public void IrbisConnectionUtility_FormatUtf8_2()
        {

        }

        [TestMethod]
        public void IrbisConnectionUtility_FormatUtf8_3()
        {

        }

        [TestMethod]
        public void IrbisConnectionUtility_ListDatabases_1()
        {
            Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            string text = "IBIS\nIBIS\nISTU\nISTU\n*****";
            mock.Setup(c => c.ReadTextFile(It.IsAny<FileSpecification>()))
                .Returns(text);
            IIrbisConnection connection = mock.Object;

            DatabaseInfo[] databases = IrbisConnectionUtility.ListDatabases(connection, "list.mnu");
            Assert.AreEqual(2, databases.Length);

            mock.Verify(c => c.ReadTextFile(It.IsAny<FileSpecification>()), Times.Once);
        }

        [TestMethod]
        public void IrbisConnectionUtility_ListDatabases_2()
        {
            Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            string text = "IBIS\nIBIS\nISTU\nISTU\n*****";
            mock.Setup(c => c.ReadTextFile(It.IsAny<FileSpecification>()))
                .Returns(text);
            IIrbisConnection connection = mock.Object;

            DatabaseInfo[] databases = IrbisConnectionUtility.ListDatabases(connection);
            Assert.AreEqual(2, databases.Length);

            mock.Verify(c => c.ReadTextFile(It.IsAny<FileSpecification>()), Times.Once);
        }

        [TestMethod]
        public void IrbisConnectionUtility_ListStandardConnectionStrings_1()
        {
            Assert.IsNotNull(IrbisConnectionUtility.ListStandardConnectionStrings());
        }

        [TestMethod]
        public void IrbisConnectionUtility_LockRecord_1()
        {
            Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            IIrbisConnection connection = mock.Object;
            CommandFactory factory = CommandFactory.GetDefaultFactory(connection);
            mock.SetupGet(c => c.CommandFactory).Returns(factory);
            mock.Setup(c => c.ExecuteCommand(It.IsAny<AbstractCommand>()));

            IrbisConnectionUtility.LockRecord(connection, "IBIS", 1);

            mock.VerifyGet(c => c.CommandFactory, Times.Once);
            mock.Verify(c => c.ExecuteCommand(It.IsAny<AbstractCommand>()), Times.Once);
        }

        [TestMethod]
        public void IrbisConnectionUtility_LockRecords_1()
        {
            Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            IIrbisConnection connection = mock.Object;
            CommandFactory factory = CommandFactory.GetDefaultFactory(connection);
            mock.SetupGet(c => c.CommandFactory).Returns(factory);
            mock.Setup(c => c.ExecuteCommand(It.IsAny<AbstractCommand>()));

            int[] records = { 1, 2 };
            IrbisConnectionUtility.LockRecords(connection, "IBIS", records);

            Times twice = Times.Exactly(2);
            mock.VerifyGet(c => c.CommandFactory, twice);
            mock.Verify(c => c.ExecuteCommand(It.IsAny<AbstractCommand>()), twice);
        }

        [TestMethod]
        public void IrbisConnectionUtility_ReadAnyBinaryFile_1()
        {
            Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            mock.SetupGet(c => c.ServerVersion).Returns(new IrbisVersion { Version = "64.2017.1" });
            mock.Setup(c => c.FormatRecord(It.IsAny<string>(), It.IsAny<int>()))
                .Returns("^aqqq.bin^b%01%02%03");
            IIrbisConnection connection = mock.Object;

            byte[] expected = { 0x01, 0x02, 0x03 };
            byte[] actual = IrbisConnectionUtility.ReadAnyBinaryFile(connection, "qqq.bin");
            CollectionAssert.AreEqual(expected, actual);

            mock.VerifyGet(c => c.ServerVersion, Times.Once);
            mock.Verify(c => c.FormatRecord(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void IrbisConnectionUtility_ReadAnyBinaryFile_2()
        {
            Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            mock.SetupGet(c => c.ServerVersion).Returns(new IrbisVersion { Version = "64.2017.1" });
            mock.Setup(c => c.FormatRecord(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(string.Empty);
            IIrbisConnection connection = mock.Object;

            byte[] actual = IrbisConnectionUtility.ReadAnyBinaryFile(connection, "qqq.bin");
            Assert.IsNull(actual);

            mock.VerifyGet(c => c.ServerVersion, Times.Once);
            mock.Verify(c => c.FormatRecord(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void IrbisConnectionUtility_ReadAnyTextFile_1()
        {
            Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            mock.SetupGet(c => c.ServerVersion).Returns(new IrbisVersion { Version = "64.2017.1" });
            mock.Setup(c => c.FormatRecord(It.IsAny<string>(), It.IsAny<int>()))
                .Returns("^aqqq.txt^b" + IrbisUtility.EncodePercentString(IrbisEncoding.Ansi.GetBytes("Hello, world")));
            IIrbisConnection connection = mock.Object;

            string expected = "Hello, world";
            string actual = IrbisConnectionUtility.ReadAnyTextFile(connection, "qqq.txt");
            Assert.AreEqual(expected, actual);

            mock.VerifyGet(c => c.ServerVersion, Times.Once);
            mock.Verify(c => c.FormatRecord(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void IrbisConnectionUtility_ReadAnyTextFile_2()
        {
            Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            mock.SetupGet(c => c.ServerVersion).Returns(new IrbisVersion { Version = "64.2017.1" });
            mock.Setup(c => c.FormatRecord(It.IsAny<string>(), It.IsAny<int>()))
                .Returns("^aqqq.txt^b" + IrbisUtility.EncodePercentString(IrbisEncoding.Ansi.GetBytes("Hello, world")));
            IIrbisConnection connection = mock.Object;

            string expected = "Hello, world";
            string actual = IrbisConnectionUtility.ReadAnyTextFile(connection, "qqq.txt", IrbisEncoding.Ansi);
            Assert.AreEqual(expected, actual);

            mock.VerifyGet(c => c.ServerVersion, Times.Once);
            mock.Verify(c => c.FormatRecord(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void IrbisConnectionUtility_ReadIniFile_1()
        {
            Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            mock.SetupGet(c => c.Database).Returns("IBIS");
            string text = "[Main]\nFirst=Second\n";
            mock.Setup(c => c.ReadTextFile(It.IsAny<FileSpecification>()))
                .Returns(text);
            IIrbisConnection connection = mock.Object;

            IniFile iniFile = IrbisConnectionUtility.ReadIniFile(connection, "any.ini");
            Assert.AreEqual("Second", iniFile["Main"]?["First"]);

            mock.VerifyGet(c => c.Database, Times.Once);
            mock.Verify(c => c.ReadTextFile(It.IsAny<FileSpecification>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void IrbisConnectionUtility_ReadIniFile_2()
        {
            Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            mock.SetupGet(c => c.Database).Returns("IBIS");
            string text = "[Main\nFirst=Second\n";
            mock.Setup(c => c.ReadTextFile(It.IsAny<FileSpecification>()))
                .Returns(text);
            IIrbisConnection connection = mock.Object;

            IrbisConnectionUtility.ReadIniFile(connection, "any.ini");
        }

        [TestMethod]
        public void IrbisConnectionUtility_ReadMenu_1()
        {
            Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            string text = "First\nSecond\n*****";
            mock.Setup(c => c.ReadTextFile(It.IsAny<FileSpecification>()))
                .Returns(text);
            IIrbisConnection connection = mock.Object;

            MenuFile menu = IrbisConnectionUtility.ReadMenu(connection, "any.mnu");
            Assert.AreEqual("Second", menu.GetString("First"));

            mock.Verify(c => c.ReadTextFile(It.IsAny<FileSpecification>()), Times.Once);
        }

        [TestMethod]
        public void IrbisConnectionUtility_ReadMenu_2()
        {
            Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            string text = "First\nSecond\n*****";
            mock.Setup(c => c.ReadTextFile(It.IsAny<FileSpecification>()))
                .Returns(text);
            IIrbisConnection connection = mock.Object;

            FileSpecification specification = new FileSpecification(IrbisPath.MasterFile, "IBIS", "any.mnu");
            MenuFile menu = IrbisConnectionUtility.ReadMenu(connection, specification);
            Assert.AreEqual("Second", menu.GetString("First"));

            mock.Verify(c => c.ReadTextFile(It.IsAny<FileSpecification>()), Times.Once);
        }

        [TestMethod]
        public void IrbisConnectionUtility_ReadRawRecord_1()
        {
            Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            IIrbisConnection connection = mock.Object;
            mock.SetupGet(c => c.CommandFactory).Returns(CommandFactory.GetDefaultFactory(connection));
            RawRecord expected = new RawRecord
            {
                Database = "IBIS",
                Mfn = 1,
                Status = RecordStatus.Last
            };
            mock.Setup(c => c.ExecuteCommand(It.IsAny<ReadRawRecordCommand>()))
                .Returns((ReadRawRecordCommand command) => { command.RawRecord = expected; return ServerResponse.GetEmptyResponse(connection); });

            RawRecord actual = IrbisConnectionUtility.ReadRawRecord(connection, "IBIS", 1);
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Database, actual.Database);
            Assert.AreEqual(expected.Mfn, actual.Mfn);
            Assert.AreEqual(expected.Status, actual.Status);

            mock.VerifyGet(c => c.CommandFactory, Times.Once);
            mock.Verify(c=> c.ExecuteCommand(It.IsAny<ReadRawRecordCommand>()), Times.Once);
        }

        [TestMethod]
        public void IrbisConnectionUtility_ReadRawRecord_2()
        {
            Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            IIrbisConnection connection = mock.Object;
            mock.SetupGet(c => c.CommandFactory).Returns(CommandFactory.GetDefaultFactory(connection));
            RawRecord expected = new RawRecord
            {
                Database = "IBIS",
                Mfn = 1,
                Status = RecordStatus.Last
            };
            mock.Setup(c => c.ExecuteCommand(It.IsAny<ReadRawRecordCommand>()))
                .Returns((ReadRawRecordCommand command) => { command.RawRecord = expected; return ServerResponse.GetEmptyResponse(connection); });

            RawRecord actual = IrbisConnectionUtility.ReadRawRecord(connection, "IBIS", 1, false, "@brief");
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Database, actual.Database);
            Assert.AreEqual(expected.Mfn, actual.Mfn);
            Assert.AreEqual(expected.Status, actual.Status);

            mock.VerifyGet(c => c.CommandFactory, Times.Once);
            mock.Verify(c=> c.ExecuteCommand(It.IsAny<ReadRawRecordCommand>()), Times.Once);
        }

        [TestMethod]
        public void IrbisConnectionUtility_ReadRawRecord_3()
        {
            Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            IIrbisConnection connection = mock.Object;
            mock.SetupGet(c => c.CommandFactory).Returns(CommandFactory.GetDefaultFactory(connection));
            RawRecord expected = new RawRecord
            {
                Database = "IBIS",
                Mfn = 1,
                Status = RecordStatus.Last
            };
            mock.Setup(c => c.ExecuteCommand(It.IsAny<ReadRawRecordCommand>()))
                .Returns((ReadRawRecordCommand command) => { command.RawRecord = expected; return ServerResponse.GetEmptyResponse(connection); });

            RawRecord actual = IrbisConnectionUtility.ReadRawRecord(connection, "IBIS", 1, 1, "@brief");
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Database, actual.Database);
            Assert.AreEqual(expected.Mfn, actual.Mfn);
            Assert.AreEqual(expected.Status, actual.Status);

            mock.VerifyGet(c => c.CommandFactory, Times.Once);
            mock.Verify(c=> c.ExecuteCommand(It.IsAny<ReadRawRecordCommand>()), Times.Once);
        }

        [TestMethod]
        public void IrbisConnectionUtility_ReadRawRecords_1()
        {

        }

        [TestMethod]
        public void IrbisConnectionUtility_ReadRecords_1()
        {

        }

        [TestMethod]
        public void IrbisConnectionUtility_ReadSearchScenario_1()
        {

        }

        [TestMethod]
        public void IrbisConnectionUtility_RemoveLogging_1()
        {

        }

        [TestMethod]
        public void IrbisConnectionUtility_RecordHistory_1()
        {

        }

        [TestMethod]
        public void IrbisConnectionUtility_ReadRecord_1()
        {
            Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            MarcRecord expected = new MarcRecord
            {
                Database = "IBIS",
                Mfn = 1,
                Status = RecordStatus.Last
            };
            mock.Setup(c => c.ReadRecord(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(expected);
            IIrbisConnection connection = mock.Object;

            MarcRecord actual = IrbisConnectionUtility.ReadRecord(connection, 1);
            Assert.AreEqual(expected.Database, actual.Database);
            Assert.AreEqual(expected.Mfn, actual.Mfn);
            Assert.AreEqual(expected.Status, actual.Status);

            mock.Verify(c => c.ReadRecord(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void IrbisConnectionUtility_ReadTextFile_1()
        {
            Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            mock.SetupGet(c => c.Database).Returns("IBIS");
            string expected = "Some text";
            mock.Setup(c => c.ReadTextFile(It.IsAny<FileSpecification>())).Returns(expected);
            IIrbisConnection connection = mock.Object;

            string actual = IrbisConnectionUtility.ReadTextFile(connection, IrbisPath.System, "some.txt");
            Assert.AreEqual(expected, actual);

            mock.VerifyGet(c => c.Database, Times.Once);
            mock.Verify(c => c.ReadTextFile(It.IsAny<FileSpecification>()), Times.Once);
        }

        [TestMethod]
        public void IrbisConnectionUtility_RequireClientVersion_1()
        {

        }

        [TestMethod]
        public void IrbisConnectionUtility_RequireServerVersion_1()
        {

        }

        [TestMethod]
        public void IrbisConnectionUtility_Search_1()
        {

        }

        [TestMethod]
        public void IrbisConnectionUtility_SearchCount_1()
        {

        }

        [TestMethod]
        public void IrbisConnectionUtility_SearchFormat_1()
        {

        }

        [TestMethod]
        public void IrbisConnectionUtility_SearchFormatUtf8_1()
        {

        }

        [TestMethod]
        public void IrbisConnectionUtility_SearchRaw_1()
        {

        }

        [TestMethod]
        public void IrbisConnectionUtility_SearchRead_1()
        {

        }

        [TestMethod]
        public void IrbisConnectionUtility_SearchReadOneRecord_1()
        {

        }

        [TestMethod]
        public void IrbisConnectionUtility_SequentialSearchRaw_1()
        {

        }

        [TestMethod]
        public void IrbisConnectionUtility_UndeleteRecord_1()
        {
            Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            mock.Setup(c => c.ReadRecord(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new MarcRecord { Deleted = true });
            mock.Setup(c => c.WriteRecord(It.IsAny<MarcRecord>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()));
            mock.SetupGet(c => c.Database).Returns("IBIS");

            IIrbisConnection connnection = mock.Object;
            IrbisConnectionUtility.UndeleteRecord(connnection, 1);

            mock.Verify(c => c.ReadRecord(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>()),
                Times.Once);
            mock.Verify(c => c.WriteRecord(It.IsAny<MarcRecord>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()),
                Times.Once);
            mock.VerifyGet(c => c.Database, Times.Once);
        }

        //[TestMethod]
        //public void IrbisConnectionUtility_UndeleteRecords_1()
        //{
        //}

        [TestMethod]
        public void IrbisConnectionUtility_UnlockRecordAlternative_1()
        {
            Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            IIrbisConnection connection = mock.Object;
            CommandFactory factory = new CommandFactory(connection);
            mock.SetupGet(c => c.CommandFactory).Returns(factory);
            ServerResponse response = ServerResponse.GetEmptyResponse(connection);
            mock.Setup(c => c.ExecuteCommand(It.IsAny<AbstractCommand>()))
                .Returns(response);

            IrbisConnectionUtility.UnlockRecordAlternative(connection, "IBIS", 1);

            mock.VerifyGet(c => c.CommandFactory, Times.Once);
            mock.Verify(c => c.ExecuteCommand(It.IsAny<AbstractCommand>()), Times.Once);
        }

        [TestMethod]
        public void IrbisConnectionUtility_WriteRawRecord_1()
        {
            Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            IIrbisConnection connection = mock.Object;
            CommandFactory factory = new CommandFactory(connection);
            mock.SetupGet(c => c.CommandFactory).Returns(factory);
            ServerResponse response = ServerResponse.GetEmptyResponse(connection);
            mock.Setup(c => c.ExecuteCommand(It.IsAny<AbstractCommand>()))
                .Returns(response);

            IrbisConnectionUtility.WriteRawRecord
                (
                    connection,
                    "IBIS",
                    "record",
                    false,
                    true
                );

            mock.VerifyGet(c => c.CommandFactory, Times.Once);
            mock.Verify(c => c.ExecuteCommand(It.IsAny<AbstractCommand>()), Times.Once);
        }

        [TestMethod]
        public void IrbisConnectionUtility_WriteRawRecords_1()
        {
            Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            IIrbisConnection connection = mock.Object;
            CommandFactory factory = new CommandFactory(connection);
            mock.SetupGet(c => c.CommandFactory).Returns(factory);
            AbstractEngine engine = new StandardEngine(connection, null);
            mock.SetupGet(c => c.Executive).Returns(engine);
            ServerResponse response = ServerResponse.GetEmptyResponse(connection);
            mock.Setup(c => c.ExecuteCommand(It.IsAny<AbstractCommand>()))
                .Returns(response);

            IrbisConnectionUtility.WriteRawRecords
                (
                    connection,
                    "IBIS",
                    new string[2],
                    false,
                    true
                );

            mock.VerifyGet(c => c.CommandFactory, Times.Once);
            mock.VerifyGet(c => c.Executive, Times.Once);
            mock.Verify(c => c.ExecuteCommand(It.IsAny<AbstractCommand>()), Times.Once);
        }

        [TestMethod]
        public void IrbisConnectionUtility_WriteRecord_1()
        {
            Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            mock.Setup(c => c.WriteRecord(It.IsAny<MarcRecord>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()));

            IIrbisConnection connnection = mock.Object;
            MarcRecord record = new MarcRecord();
            IrbisConnectionUtility.WriteRecord(connnection, record);

            mock.Verify(c => c.WriteRecord(It.IsAny<MarcRecord>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()),
                Times.Once);

        }

        [TestMethod]
        public void IrbisConnectionUtility_WriteRecord_2()
        {
            Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            mock.Setup(c => c.WriteRecord(It.IsAny<MarcRecord>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()));

            IIrbisConnection connnection = mock.Object;
            MarcRecord record = new MarcRecord();
            IrbisConnectionUtility.WriteRecord(connnection, record, false, true);

            mock.Verify(c => c.WriteRecord(It.IsAny<MarcRecord>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()),
                Times.Once);
        }

        [TestMethod]
        public void IrbisConnectionUtility_WriteRecord_3()
        {
            Mock<IIrbisConnection> mock = new Mock<IIrbisConnection>();
            mock.Setup(c => c.WriteRecord(It.IsAny<MarcRecord>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()));

            IIrbisConnection connnection = mock.Object;
            MarcRecord record = new MarcRecord();
            IrbisConnectionUtility.WriteRecord(connnection, record, true);

            mock.Verify(c => c.WriteRecord(It.IsAny<MarcRecord>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()),
                Times.Once);
        }

    }
}
