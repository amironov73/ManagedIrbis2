using AM;

using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable SuspiciousTypeConversion.Global

namespace UnitTests.AM
{
    [TestClass]
    public class StringFragmentTest
    {
        [TestMethod]
        public void StringFragment_Construction_1()
        {
            string original = "Hello, world";
            StringFragment fragment = new StringFragment(original, 7, 5);
            Assert.AreSame(original, fragment.Original);
            Assert.AreEqual(7, fragment.Offset);
            Assert.AreEqual(5, fragment.Length);
        }

        [TestMethod]
        public void StringFragment_OperatorEquals_1()
        {
            string original = "Hello, world";
            StringFragment left = new StringFragment(original, 7, 5);
            StringFragment right = new StringFragment(original, 7, 5);
            Assert.IsTrue(left == right);
        }

        [TestMethod]
        public void StringFragment_OperatorEquals_2()
        {
            string original = "Hello, world";
            StringFragment left = new StringFragment(original, 7, 5);
            StringFragment right = new StringFragment(original, 7, 3);
            Assert.IsFalse(left == right);
        }

        [TestMethod]
        public void StringFragment_OperatorEquals_3()
        {
            string original = "Hello, world";
            StringFragment left = new StringFragment(original, 7, 5);
            Assert.IsTrue(left == "world");
        }

        [TestMethod]
        public void StringFragment_OperatorEquals_4()
        {
            string original = "Hello, world";
            StringFragment right = new StringFragment(original, 7, 5);
            Assert.IsTrue("world" == right);
        }

        [TestMethod]
        public void StringFragment_OperatorNotEquals_1()
        {
            string original = "Hello, world";
            StringFragment left = new StringFragment(original, 7, 5);
            StringFragment right = new StringFragment(original, 7, 5);
            Assert.IsFalse(left != right);
        }

        [TestMethod]
        public void StringFragment_OperatorNotEquals_2()
        {
            string original = "Hello, world";
            StringFragment left = new StringFragment(original, 7, 5);
            StringFragment right = new StringFragment(original, 7, 3);
            Assert.IsTrue(left != right);
        }

        [TestMethod]
        public void StringFragment_OperatorNotEquals_3()
        {
            string original = "Hello, world";
            StringFragment left = new StringFragment(original, 7, 5);
            Assert.IsFalse(left != "world");
        }

        [TestMethod]
        public void StringFragment_OperatorNotEquals_4()
        {
            string original = "Hello, world";
            StringFragment right = new StringFragment(original, 7, 5);
            Assert.IsFalse("world" != right);
        }

        [TestMethod]
        public void StringFragment_Equals_1()
        {
            string original = "Hello, world";
            StringFragment left = new StringFragment(original, 7, 5);
            StringFragment right = new StringFragment(original, 7, 5);
            Assert.IsTrue(left.Equals(right));

            right = new StringFragment(original, 7, 3);
            Assert.IsFalse(left.Equals(right));

            Assert.IsFalse(left.Equals("world"));
        }

        [TestMethod]
        public void StringFragment_ToString_1()
        {
            string original = "Hello, world";
            StringFragment fragment = new StringFragment(original, 7, 5);
            Assert.AreEqual("world", fragment.ToString());
        }
    }
}
