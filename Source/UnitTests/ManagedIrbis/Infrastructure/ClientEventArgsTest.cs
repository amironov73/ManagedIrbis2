using ManagedIrbis;
using ManagedIrbis.Infrastructure;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.ManagedIrbis.Infrastructure
{
    [TestClass]
    public class ClientEventArgsTest
    {
        [TestMethod]
        public void ExecutionEventArgs_Construction_1()
        {
            IIrbisConnection connection = new IrbisConnection();
            ClientContext context = new ClientContext(connection);
            ClientEventArgs args = new ClientEventArgs(context);
            Assert.AreSame(context, args.Context);
        }
    }
}
