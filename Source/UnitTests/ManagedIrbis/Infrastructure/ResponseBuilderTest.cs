using System;
using System.Collections.Generic;
using System.Text;

using ManagedIrbis;
using ManagedIrbis.Infrastructure;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.ManagedIrbis.Infrastructure
{
    [TestClass]
    public class ResponseBuilderTest
    {
        [TestMethod]
        public void ResponseBuilder_Construction_1()
        {
            using (ResponseBuilder builder = new ResponseBuilder())
            {
                Assert.IsNotNull(builder.Memory);
            }
        }

        [TestMethod]
        public void ResponseBuilder_Append_1()
        {
            using (ResponseBuilder builder = new ResponseBuilder())
            {
                builder.Append(123).NewLine();
                byte[] expected = { 49, 50, 51, 13, 10 };
                byte[] actual = builder.Encode();
                CollectionAssert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void ResponseBuilder_AppendAnsi_1()
        {
            using (ResponseBuilder builder = new ResponseBuilder())
            {
                builder.AppendAnsi("Какой-то текст").NewLine();
                byte[] expected =
                {
                    202, 224, 234, 238, 233, 45, 242, 238, 32, 242,
                    229, 234, 241, 242, 13, 10
                };
                byte[] actual = builder.Encode();
                CollectionAssert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void ResponseBuilder_AppendOem_1()
        {
            using (ResponseBuilder builder = new ResponseBuilder())
            {
                builder.AppendOem("Какой-то текст").NewLine();
                byte[] expected =
                {
                    138, 160, 170, 174, 169, 45, 226, 174, 32, 226,
                    165, 170, 225, 226, 13, 10
                };
                byte[] actual = builder.Encode();
                CollectionAssert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void ResponseBuilder_AppendUtf_1()
        {
            using (ResponseBuilder builder = new ResponseBuilder())
            {
                builder.AppendUtf("Какой-то текст").NewLine();
                byte[] expected =
                {
                    208, 154, 208, 176, 208, 186, 208, 190, 208, 185,
                    45, 209, 130, 208, 190, 32, 209, 130, 208, 181,
                    208, 186, 209, 129, 209, 130, 13, 10
                };
                byte[] actual = builder.Encode();
                CollectionAssert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void ResponseBuilder_Delimiter_1()
        {
            using (ResponseBuilder builder = new ResponseBuilder())
            {
                builder.Delimiter().NewLine();
                byte[] expected = { 30, 13, 10 };
                byte[] actual = builder.Encode();
                CollectionAssert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void ResponseBuilder_StandardHeader_1()
        {
            using (ResponseBuilder builder = new ResponseBuilder())
            {
                builder.StandardHeader(CommandCode.Nop, 123456, 234);
                byte[] expected =
                {
                    78, 13, 10, 49, 50, 51, 52, 53, 54, 13, 10, 50,
                    51, 52, 13, 10, 48, 13, 10, 13, 10, 13, 10, 13,
                    10, 13, 10, 13, 10
                };
                byte[] actual = builder.Encode();
                CollectionAssert.AreEqual(expected, actual);
            }
        }
    }
}
