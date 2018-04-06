using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using AM.Text;

using ManagedIrbis;
using ManagedIrbis.Infrastructure;
using ManagedIrbis.Search;

using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable ExpressionIsAlwaysNull
// ReSharper disable InvokeAsExtensionMethod

namespace UnitTests.ManagedIrbis.Infrastructure
{
    [TestClass]
    public class IrbisNetworkUtilityTest
    {
        [TestMethod]
        public void IrbisNetworkUtility_DumpBytes_1()
        {
            byte[] bytes =
            {
                0x48, 0x0D, 0x0A, 0x32, 0x32, 0x32, 0x38, 0x38, 0x34, 0x0D,
                0x0A, 0x35, 0x0D, 0x0A, 0x31, 0x30, 0x35, 0x36, 0x0D, 0x0A
            };
            string expected
                = "00000000  48 0D 0A 32 32 32 38 38  34 0D 0A 35 0D 0A 31 30  H  222884  5  10\n"
                + "00000010  35 36 0D 0A                                       56              \n\n";
            string actual = IrbisNetworkUtility.DumpBytes(bytes).DosToUnix();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IrbisNetworkUtility_EncodeBoolean_1()
        {
            MemoryStream stream = new MemoryStream();
            IrbisNetworkUtility.EncodeBoolean(stream, false);
            IrbisNetworkUtility.EncodeBoolean(stream, true);
            byte[] expected = { 48, 49 };
            byte[] actual = stream.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IrbisNetworkUtility_EncodeBytes_1()
        {
            MemoryStream stream = new MemoryStream();
            byte[] bytes = { 0x01, 0x02, 0x03 };
            IrbisNetworkUtility.EncodeBytes(stream, bytes);
            byte[] expected = { 1, 2, 3 };
            byte[] actual = stream.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IrbisNetworkUtility_EncodeDelimiter_1()
        {
            MemoryStream stream = new MemoryStream();
            IrbisNetworkUtility.EncodeBoolean(stream, false);
            IrbisNetworkUtility.EncodeDelimiter(stream);
            IrbisNetworkUtility.EncodeBoolean(stream, true);
            byte[] expected = { 48, 10, 49 };
            byte[] actual = stream.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IrbisNetworkUtility_EncodeFileSpecification_1()
        {
            MemoryStream stream = new MemoryStream();
            FileSpecification specification = new FileSpecification
                (
                    IrbisPath.MasterFile,
                    "IBIS",
                    "someFile.txt"
                );
            IrbisNetworkUtility.EncodeFileSpecification(stream, specification);
            byte[] expected =
            {
                50, 46, 73, 66, 73, 83, 46, 115, 111, 109, 101, 70,
                105, 108, 101, 46, 116, 120, 116
            };
            byte[] actual = stream.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IrbisNetworkUtility_EncodeInt32_1()
        {
            MemoryStream stream = new MemoryStream();
            IrbisNetworkUtility.EncodeInt32(stream, 123456);
            IrbisNetworkUtility.EncodeInt32(stream, 1890123456);
            byte[] expected =
            {
                49, 50, 51, 52, 53, 54, 49, 56, 57, 48, 49, 50, 51,
                52, 53, 54
            };
            byte[] actual = stream.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IrbisNetworkUtility_EncodeInt64_1()
        {
            MemoryStream stream = new MemoryStream();
            IrbisNetworkUtility.EncodeInt64(stream, 123456);
            IrbisNetworkUtility.EncodeInt64(stream, 78901234567890);
            byte[] expected =
            {
                49, 50, 51, 52, 53, 54, 55, 56, 57, 48, 49, 50, 51,
                52, 53, 54, 55, 56, 57, 48
            };
            byte[] actual = stream.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IrbisNetworkUtility_EncodeObject_1()
        {
            MemoryStream stream = new MemoryStream();
            TermInfo termInfo = new TermInfo
            {
                Count = 123,
                Text = "K=БЕТОН"
            };
            IrbisNetworkUtility.EncodeObject(stream, termInfo);
            IrbisNetworkUtility.EncodeObject(stream, null);
            byte[] expected = {49, 50, 51, 35, 75, 61, 193, 197, 210, 206, 205};
            byte[] actual = stream.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IrbisNetworkUtility_EncodeRecord_1()
        {
            MemoryStream stream = new MemoryStream();
            MarcRecord record = new MarcRecord
            {
                Database = "IBIS",
                Mfn = 123456
            };
            record.Fields.Add(new RecordField(903, "Index"));
            record.Fields.Add(new RecordField(200, new SubField('a', "Заглавие")));
            IrbisNetworkUtility.EncodeRecord(stream, record);
            byte[] expected =
            {
                49, 50, 51, 52, 53, 54, 35, 48, 31, 30, 48, 35, 48,
                31, 30, 57, 48, 51, 35, 73, 110, 100, 101, 120, 31,
                30, 50, 48, 48, 35, 94, 97, 208, 151, 208, 176, 208,
                179, 208, 187, 208, 176, 208, 178, 208, 184, 208,
                181, 31, 30
            };
            byte[] actual = stream.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IrbisNetworkUtility_EncodeRecordReference_1()
        {
            MemoryStream stream = new MemoryStream();
            MarcRecord record = new MarcRecord
            {
                Database = "IBIS",
                Mfn = 123456
            };
            record.Fields.Add(new RecordField(903, "Index"));
            RecordReference reference = new RecordReference(record);
            IrbisNetworkUtility.EncodeRecordReference(stream, reference);
            byte[] expected =
            {
                73, 66, 73, 83, 31, 30, 49, 50, 51, 52, 53, 54, 35,
                48, 31, 30, 48, 35, 48, 31, 30, 57, 48, 51, 35, 73,
                110, 100, 101, 120, 31, 30
            };
            byte[] actual = stream.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(IrbisException))]
        public void IrbisNetworkUtility_EncodeRecordReference_2()
        {
            MemoryStream stream = new MemoryStream();
            MarcRecord record = new MarcRecord
            {
                Mfn = 123456
            };
            record.Fields.Add(new RecordField(903, "Index"));
            RecordReference reference = new RecordReference(record);
            IrbisNetworkUtility.EncodeRecordReference(stream, reference);
        }

        [TestMethod]
        [ExpectedException(typeof(IrbisException))]
        public void IrbisNetworkUtility_EncodeRecordReference_3()
        {
            MemoryStream stream = new MemoryStream();
            RecordReference reference = new RecordReference
            {
                Database = "IBIS",
                Mfn = 123
            };
            IrbisNetworkUtility.EncodeRecordReference(stream, reference);
        }

        [TestMethod]
        public void IrbisNetworkUtility_EncodeString_1()
        {
            MemoryStream stream = new MemoryStream();
            string text = "Какой-то текст";
            IrbisNetworkUtility.EncodeString(stream, text);
            byte[] expected =
            {
                202, 224, 234, 238, 233, 45, 242, 238, 32, 242, 229,
                234, 241, 242
            };
            byte[] actual = stream.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IrbisNetworkUtility_EncodeTextWithEncoding_1()
        {
            MemoryStream stream = new MemoryStream();
            TextWithEncoding text = new TextWithEncoding("Какой-то текст", IrbisEncoding.Ansi);
            IrbisNetworkUtility.EncodeTextWithEncoding(stream, text);
            IrbisNetworkUtility.EncodeBoolean(stream, true);
            byte[] expected =
            {
                202, 224, 234, 238, 233, 45, 242, 238, 32, 242, 229,
                234, 241, 242, 49
            };
            byte[] actual = stream.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IrbisNetworkUtility_EncodeTextWithEncoding_2()
        {
            MemoryStream stream = new MemoryStream();
            TextWithEncoding text = new TextWithEncoding("Какой-то текст", IrbisEncoding.Oem);
            IrbisNetworkUtility.EncodeTextWithEncoding(stream, text);
            IrbisNetworkUtility.EncodeBoolean(stream, true);
            byte[] expected =
            {
                138, 160, 170, 174, 169, 45, 226, 174, 32, 226, 165,
                170, 225, 226, 49
            };
            byte[] actual = stream.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IrbisNetworkUtility_EncodeTextWithEncoding_3()
        {
            MemoryStream stream = new MemoryStream();
            TextWithEncoding text = new TextWithEncoding("Какой-то текст", IrbisEncoding.Utf8);
            IrbisNetworkUtility.EncodeTextWithEncoding(stream, text);
            IrbisNetworkUtility.EncodeBoolean(stream, true);
            byte[] expected =
            {
                208, 154, 208, 176, 208, 186, 208, 190, 208, 185, 45,
                209, 130, 208, 190, 32, 209, 130, 208, 181, 208, 186,
                209, 129, 209, 130, 49
            };
            byte[] actual = stream.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IrbisNetworkUtility_EncodeWorkstation_1()
        {
            MemoryStream stream = new MemoryStream();
            IrbisNetworkUtility.EncodeWorkstation(stream, IrbisWorkstation.Administrator);
            IrbisNetworkUtility.EncodeWorkstation(stream, IrbisWorkstation.Reader);
            byte[] expected = { 65, 82 };
            byte[] actual = stream.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IrbisNetworkUtility_EncodeAny_1()
        {
            MemoryStream stream = new MemoryStream();
            object anyObject = null;
            IrbisNetworkUtility.EncodeAny(stream, anyObject);
            byte[] expected = new byte[0];
            byte[] actual = stream.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IrbisNetworkUtility_EncodeAny_2()
        {
            MemoryStream stream = new MemoryStream();
            object anyObject = (byte)123;
            IrbisNetworkUtility.EncodeAny(stream, anyObject);
            byte[] expected = { 123 };
            byte[] actual = stream.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IrbisNetworkUtility_EncodeAny_3()
        {
            MemoryStream stream = new MemoryStream();
            object anyObject = new byte[] { 1, 2, 3 };
            IrbisNetworkUtility.EncodeAny(stream, anyObject);
            byte[] expected = { 1, 2, 3 };
            byte[] actual = stream.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IrbisNetworkUtility_EncodeAny_4()
        {
            MemoryStream stream = new MemoryStream();
            object anyObject = 123;
            IrbisNetworkUtility.EncodeAny(stream, anyObject);
            byte[] expected = { 49, 50, 51 };
            byte[] actual = stream.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IrbisNetworkUtility_EncodeAny_5()
        {
            MemoryStream stream = new MemoryStream();
            object anyObject = 1234567890123L;
            IrbisNetworkUtility.EncodeAny(stream, anyObject);
            byte[] expected =
            {
                49, 50, 51, 52, 53, 54, 55, 56, 57, 48, 49, 50, 51
            };
            byte[] actual = stream.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IrbisNetworkUtility_EncodeAny_6()
        {
            MemoryStream stream = new MemoryStream();
            object anyObject = "Какой-то текст";
            IrbisNetworkUtility.EncodeAny(stream, anyObject);
            byte[] expected =
            {
                202, 224, 234, 238, 233, 45, 242, 238, 32, 242, 229,
                234, 241, 242
            };
            byte[] actual = stream.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IrbisNetworkUtility_EncodeAny_7()
        {
            MemoryStream stream = new MemoryStream();
            object anyObject = new TextWithEncoding
                (
                    "Какой-то текст",
                    IrbisEncoding.Utf8
                );
            IrbisNetworkUtility.EncodeAny(stream, anyObject);
            byte[] expected =
            {
                208, 154, 208, 176, 208, 186, 208, 190, 208, 185, 45,
                209, 130, 208, 190, 32, 209, 130, 208, 181, 208, 186,
                209, 129, 209, 130
            };
            byte[] actual = stream.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IrbisNetworkUtility_EncodeAny_8()
        {
            MemoryStream stream = new MemoryStream();
            MarcRecord record = new MarcRecord
            {
                Database = "IBIS",
                Mfn = 123
            };
            record.AddField(903, "Index");
            object anyObject = new RecordReference(record);
            IrbisNetworkUtility.EncodeAny(stream, anyObject);
            byte[] expected =
            {
                73, 66, 73, 83, 31, 30, 49, 50, 51, 35, 48, 31, 30,
                48, 35, 48, 31, 30, 57, 48, 51, 35, 73, 110, 100,
                101, 120, 31, 30
            };
            byte[] actual = stream.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IrbisNetworkUtility_EncodeAny_9()
        {
            MemoryStream stream = new MemoryStream();
            object anyObject = new FileSpecification
                (
                    IrbisPath.MasterFile,
                    "IBIS",
                    "someFile.txt"
                );
            IrbisNetworkUtility.EncodeAny(stream, anyObject);
            byte[] expected =
            {
                50, 46, 73, 66, 73, 83, 46, 115, 111, 109, 101, 70,
                105, 108, 101, 46, 116, 120, 116
            };
            byte[] actual = stream.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IrbisNetworkUtility_EncodeAny_10()
        {
            MemoryStream stream = new MemoryStream();
            MarcRecord record = new MarcRecord
            {
                Database = "IBIS",
                Mfn = 123
            };
            record.AddField(903, "Index");
            IrbisNetworkUtility.EncodeAny(stream, record);
            byte[] expected =
            {
                49, 50, 51, 35, 48, 31, 30, 48, 35, 48, 31, 30, 57,
                48, 51, 35, 73, 110, 100, 101, 120, 31, 30
            };
            byte[] actual = stream.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IrbisNetworkUtility_EncodeAny_11()
        {
            MemoryStream stream = new MemoryStream();
            object anyObject = new TermInfo
            {
                Count = 123,
                Text = "К=БЕТОН"
            };
            IrbisNetworkUtility.EncodeAny(stream, anyObject);
            byte[] expected =
            {
                49, 50, 51, 35, 202, 61, 193, 197, 210, 206, 205
            };
            byte[] actual = stream.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IrbisNetworkUtility_ThrowIfEmptyRecord_1()
        {
            IrbisConnection connection = new IrbisConnection();
            ServerResponse response = ServerResponse.GetEmptyResponse(connection);
            MarcRecord record = new MarcRecord().AddField(903, "Index");
            IrbisNetworkUtility.ThrowIfEmptyRecord(record, response);
        }

        [TestMethod]
        [ExpectedException(typeof(IrbisNetworkException))]
        public void IrbisNetworkUtility_ThrowIfEmptyRecord_2()
        {
            IrbisConnection connection = new IrbisConnection();
            ServerResponse response = ServerResponse.GetEmptyResponse(connection);
            MarcRecord record = new MarcRecord();
            IrbisNetworkUtility.ThrowIfEmptyRecord(record, response);
        }
    }
}
