using System;

using AM.Text;

using ManagedIrbis;
using ManagedIrbis.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.ManagedIrbis
{
    [TestClass]
    public class IrbisExceptionTest
    {
        [TestMethod]
        public void IrbisException_Constructor_1()
        {
            IrbisException exception = new IrbisException();
            Assert.AreEqual(0, exception.ErrorCode);
        }

        [TestMethod]
        public void IrbisException_Constructor_2()
        {
            IrbisException exception = new IrbisException(-2222);
            Assert.AreEqual(-2222, exception.ErrorCode);
        }

        [TestMethod]
        public void IrbisException_Constructor_3()
        {
            const string expected = "internal error";
            IrbisException exception = new IrbisException(expected);
            Assert.AreEqual(0, exception.ErrorCode);
            Assert.AreEqual(expected, exception.Message);
        }

        [TestMethod]
        public void IrbisException_Constructor_4()
        {
            const string expected = "internal error";
            Exception innerException = new Exception();
            IrbisException exception = new IrbisException(expected, innerException);
            Assert.AreEqual(0, exception.ErrorCode);
            Assert.AreEqual(expected, exception.Message);
            Assert.AreSame(innerException, exception.InnerException);
        }

        [TestMethod]
        public void IrbisException_GetErrorDescription_1()
        {
            Assert.AreEqual("No error", IrbisException.GetErrorDescription(100));
            Assert.AreEqual("Normal return", IrbisException.GetErrorDescription(0));
            Assert.AreEqual("Field is absent", IrbisException.GetErrorDescription(-200));
        }

        [TestMethod]
        public void IrbisException_GetErrorDescription_2()
        {
            Assert.AreEqual("Normal return", IrbisException.GetErrorDescription(IrbisReturnCode.NoError));
            Assert.AreEqual("File not exist", IrbisException.GetErrorDescription(IrbisReturnCode.FileNotExist));
        }

        [TestMethod]
        public void IrbisException_GetErrorDescription_3()
        {
            Assert.AreEqual("No error", IrbisException.GetErrorDescription(new IrbisException(100)));
            Assert.AreEqual("Field is absent", IrbisException.GetErrorDescription(new IrbisException(-200)));
        }

        [TestMethod]
        public void IrbisException_ToString_1()
        {
            Assert.AreEqual
                (
                    "ErrorCode: -200\nDescription: Field is absent\nManagedIrbis.IrbisException: Field is absent",
                    new IrbisException(-200).ToString().DosToUnix()
                );

            Assert.AreEqual
                (
                    "ErrorCode: 0\nDescription: network error\nManagedIrbis.IrbisException: network error",
                    new IrbisException("network error").ToString().DosToUnix()
                );
        }
    }
}
