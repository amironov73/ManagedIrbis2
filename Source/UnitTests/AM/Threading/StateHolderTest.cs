using System;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using AM.Threading;

// ReSharper disable ObjectCreationAsStatement

namespace UnitTests.AM.Threading
{
    [TestClass]
    public class StateHolderTest
    {
        [TestMethod]
        public void StateHolder_Event_1()
        {
            StateHolder<int> holder = 10;
            bool flag = false;
            holder.ValueChanged += (sender, args) => { flag = true; };
            holder.Value = 100;
            Assert.IsTrue(flag);
        }

        [TestMethod]
        public void StateHolder_Event_2()
        {
            StateHolder<string> holder = new StateHolder<string>();
            bool flag = false;
            holder.ValueChanged += (sender, args) => { flag = true; };
            holder.Value = "World";
            Assert.IsTrue(flag);
        }

        [TestMethod]
        public void StateHolder_Operator_1()
        {
            int value1 = 10;
            StateHolder<int> holder = value1;
            int value2 = holder;
            Assert.AreEqual(value1, value2);
        }

        [TestMethod]
        public void StateHolder_ToString_1()
        {
            StateHolder<string> holder = new StateHolder<string>();
            Assert.AreEqual("(null)", holder.ToString());

            holder = "Hello";
            Assert.AreEqual("Hello", holder.ToString());
        }

        [TestMethod]
        public void StateHolder_Value_1()
        {
            StateHolder<int> holder = 10;
            Assert.AreEqual(10, holder.Value);

            holder.Value = 100;
            Assert.AreEqual(100, holder.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StateHolder_Value_2()
        {
            new StateHolder<string>
            {
                AllowNull = false,
                Value = null
            };

        }

        [TestMethod]
        public void StateHolder_WaitHandle_1()
        {
            StateHolder<int> holder = 10;
            bool flag = false;
            Task task = Task.Factory.StartNew
                (
                    () =>
                    {
                        holder.WaitHandle.WaitOne();
                        flag = true;
                    }
                );
            holder.Value = 100;
            task.Wait();
            Assert.IsTrue(flag);
        }
    }
}
