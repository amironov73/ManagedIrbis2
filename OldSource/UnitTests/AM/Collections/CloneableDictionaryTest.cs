﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

using AM.Collections;

namespace UnitTests.AM.Collections
{
    [TestClass]
    public class CloneableDictionaryTest
    {
        [TestMethod]
        public void CloneableDictionary_Clone()
        {
            var source = new CloneableDictionary<int, string>
                {
                    { 1, "one" },
                    { 2, "two" },
                    { 3, "three" }
                };

            var clone = (CloneableDictionary<int, string>) source.Clone();

            Assert.AreEqual(source.Count, clone.Count);
            var keys = source.Keys;
            foreach (int key in keys)
            {
                Assert.AreEqual(source[key], clone[key]);
            }
        }
    }
}
