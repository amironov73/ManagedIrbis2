using System;

using ManagedIrbis.Infrastructure;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.ManagedIrbis.Infrastructure
{
    [TestClass]
    public class IrbisNetworkExceptionTest
    {
        [TestMethod]
        public void IrbisNetworkException_Construction_1()
        {
            IrbisNetworkException exception = new IrbisNetworkException();
            Assert.IsNotNull(exception.Message);
        }

        [TestMethod]
        public void IrbisNetworkException_Construction_2()
        {
            string message = "message";
            IrbisNetworkException exception = new IrbisNetworkException(message);
            Assert.AreSame(message, exception.Message);
        }

        [TestMethod]
        public void IrbisNetworkException_Construction_3()
        {
            string message = "message";
            Exception innerException = new Exception();
            IrbisNetworkException exception = new IrbisNetworkException
                (
                    message,
                    innerException
                );
            Assert.AreSame(message, exception.Message);
            Assert.AreSame(innerException, exception.InnerException);
        }
    }
}
