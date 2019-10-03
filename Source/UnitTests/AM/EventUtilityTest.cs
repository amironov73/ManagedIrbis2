using System;
using System.ComponentModel;
using System.Threading.Tasks;

using AM;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.AM
{
    [TestClass]
    public class EventUtilityTest
    {
        public event EventHandler Handler1;

        public event EventHandler<CancelEventArgs> Handler2;

        [TestMethod]
        public void EventUtility_Raise_1()
        {
            var args = new EventArgs();

            Handler1.Raise(this, args);
        }

        [TestMethod]
        public void EventUtility_Raise_2()
        {
            var args = new CancelEventArgs();

            Handler2.Raise(this, args);
        }

        [TestMethod]
        public void EventUtility_Raise_3()
        {
            Handler1.Raise(this);
        }

        [TestMethod]
        public void EventUtility_Raise_4()
        {
            Handler2.Raise(this);
        }

        [TestMethod]
        public void EventUtility_Raise_5()
        {
            Handler2.Raise();
        }

        [TestMethod]
        public void EventUtility_RaiseAsync_1()
        {
            var args = new EventArgs();

            var task = Handler1.RaiseAsync(this, args);
            Assert.IsNotNull(task);
            task.Wait();
        }

        [TestMethod]
        public void EventUtility_RaiseAsync_2()
        {
            var task = Handler1.RaiseAsync(this);
            Assert.IsNotNull(task);
            task.Wait();
        }
    }
}
