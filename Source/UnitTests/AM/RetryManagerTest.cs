using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using AM;

namespace UnitTests.AM
{
    [TestClass]
    public class RetryManagerTest
    {
        [TestMethod]
        public void RetryManager_Try_1()
        {
            int counter = 0;
            Action action = () =>
            {
                counter++;
                if (counter < 2)
                {
                    throw new Exception();
                }
            };

            RetryManager retryManager = new RetryManager(3);
            retryManager.Try(action);

            Assert.IsTrue(counter == 2);
        }

        [TestMethod]
        public void RetryManager_Try_2()
        {
            int counter = 0;
            Action<int> action = argument =>
            {
                counter++;
                if (counter < 2)
                {
                    throw new Exception();
                }
            };

            RetryManager retryManager = new RetryManager(3);
            retryManager.Try(action, 1);

            Assert.IsTrue(counter == 2);
        }

        [TestMethod]
        public void RetryManager_Try_3()
        {
            int counter = 0;
            Action<int> action = argument =>
            {
                counter++;
                if (counter < 2)
                {
                    throw new Exception();
                }
            };
            Func<Exception, bool> resolver =
                ex => ex.GetType() == typeof (Exception);

            RetryManager retryManager = new RetryManager(3, resolver);
            retryManager.Try(action, 1);

            Assert.IsTrue(counter == 2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArsMagnaException))]
        public void RetryManager_Try_4()
        {
            Action<int> action = argument =>
            {
                throw new ArgumentException();
            };
            Func<Exception, bool> resolver =
                ex => ex.GetType() == typeof(Exception);

            RetryManager retryManager = new RetryManager(3, resolver);
            retryManager.Try(action, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RetryManager_Try_5()
        {
            Action<int> action = argument =>
            {
                throw new ArgumentException();
            };

            RetryManager retryManager = new RetryManager(3);
            retryManager.Try(action, 1);
        }

        [TestMethod]
        public void RetryManager_Try_6()
        {
            const int expected = 2;
            int counter = 0;
            Func<int> func = () =>
            {
                counter++;
                if (counter < expected)
                {
                    throw new Exception();
                }

                return counter;
            };

            RetryManager retryManager = new RetryManager(3);
            int actual = retryManager.Try(func);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RetryManager_Try_7()
        {
            const int expected = 2;
            int counter = 0;
            Func<int,int> func = argument =>
            {
                counter++;
                if (counter < expected)
                {
                    throw new Exception();
                }

                return counter;
            };

            RetryManager retryManager = new RetryManager(3);
            int actual = retryManager.Try(func, 1);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void RetryManager_Try_8()
        {
            int counter = 0;
            Action action = () =>
            {
                counter++;
                throw new Exception();
            };

            RetryManager retryManager = new RetryManager(3);
            retryManager.Try(action);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void RetryManager_Try_9()
        {
            int counter = 0;
            Action<int> action = arg =>
            {
                counter += arg;
                throw new Exception();
            };

            RetryManager retryManager = new RetryManager(3);
            retryManager.Try(action, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void RetryManager_Try_10()
        {
            int counter = 0;
            Func<int, int> func = arg =>
            {
                counter += arg;
                throw new Exception();
            };

            RetryManager retryManager = new RetryManager(3);
            retryManager.Try(func, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void RetryManager_Try_11()
        {
            int counter = 0;
            Func<int> func = () =>
            {
                counter++;
                throw new Exception();
            };

            RetryManager retryManager = new RetryManager(3);
            retryManager.Try(func);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void RetryManager_Try_12()
        {
            int counter = 0;
            Action<int, int> action = (arg1, arg2) =>
            {
                counter += (arg1 + arg2);
                throw new Exception();
            };

            RetryManager retryManager = new RetryManager(3);
            retryManager.Try(action, 1, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void RetryManager_Try_13()
        {
            int counter = 0;
            Action<int, int, int> action = (arg1, arg2, arg3) =>
            {
                counter += (arg1 + arg2 + arg3);
                throw new Exception();
            };

            RetryManager retryManager = new RetryManager(3);
            retryManager.Try(action, 1, 0, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void RetryManager_Try_14()
        {
            int counter = 0;
            Func<int, int, int> func = (arg1, arg2) =>
            {
                counter += (arg1 + arg2);
                throw new Exception();
            };

            RetryManager retryManager = new RetryManager(3);
            retryManager.Try(func, 1, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void RetryManager_Try_15()
        {
            int counter = 0;
            Func<int, int, int, int> func = (arg1, arg2, arg3) =>
            {
                counter += (arg1 + arg2 + arg3);
                throw new Exception();
            };

            RetryManager retryManager = new RetryManager(3);
            retryManager.Try(func, 1, 0, 0);
        }

        [TestMethod]
        public void RetryManager_Try_16()
        {
            int counter = 0;
            Action action = () =>
            {
                counter++;
                if (counter < 2)
                {
                    throw new Exception();
                }
            };
            Func<Exception, bool> resolver =
                ex => ex.GetType() == typeof (Exception);
            RetryManager retryManager = new RetryManager(3, resolver)
            {
                DelayInterval = 10,
            };

            retryManager.Try(action);

            Assert.IsTrue(counter == 2);
        }
    }
}
