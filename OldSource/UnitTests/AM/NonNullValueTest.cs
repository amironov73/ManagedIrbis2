using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using AM;

#pragma warning disable 219

namespace UnitTests.AM
{
    [TestClass]
    public class NonNullValueTest
    {
        [TestMethod]
        public void NonNullValue_Construction_1()
        {
            const string expected = "abc";

            NonNullValue<string> value
                = new NonNullValue<string>(expected);
            Assert.AreEqual(expected, value.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NonNullValue_Construction_2()
        {
            NonNullValue<string> value = new NonNullValue<string>("a");
            value.SetValue(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NonNullValue_Assignment_1()
        {
            NonNullValue<string> value = new NonNullValue<string>("a");
            string text = null;
            value = text;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NonNullValue_Assignment_2()
        {
            NonNullValue<string> value = null;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NonNullValue_Default_1()
        {
            NonNullValue<string> value = new NonNullValue<string>();
            Assert.IsNotNull(value.Value);
        }

        [TestMethod]
        public void NonNullValue_SetValue_1()
        {
            const string expected = "World";
            NonNullValue<string> value = "Hello";
            value.SetValue(expected);
            Assert.AreEqual(expected, value.Value);
        }

        [TestMethod]
        public void NonNullValue_ToString_1()
        {
            string expected = "Hello";
            NonNullValue<string> value = expected;
            Assert.AreEqual(expected, value.ToString());
        }

        [TestMethod]
        public void NonNullValue_Value_1()
        {
            const string expected = "World";
            NonNullValue<string> value = "Hello";
            value.Value = expected;
            Assert.AreEqual(expected, value.Value);
        }
    }
}
