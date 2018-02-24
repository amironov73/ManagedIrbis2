using System.Collections.Generic;

using AM;

using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable InvokeAsExtensionMethod

namespace UnitTests.AM
{
    [TestClass]
    public class CharUtilityTest
    {
        [TestMethod]
        public void CharUtility_IsArabicDigit_1()
        {
            Assert.IsTrue('0'.IsArabicDigit());
            Assert.IsTrue('9'.IsArabicDigit());
            Assert.IsFalse('A'.IsArabicDigit());
            Assert.IsFalse('\x0661'.IsArabicDigit());
        }

        [TestMethod]
        public void CharUtility_IsLatinLetter_1()
        {
            Assert.IsTrue('A'.IsLatinLetter());
            Assert.IsTrue('z'.IsLatinLetter());
            Assert.IsFalse('А'.IsLatinLetter());
            Assert.IsFalse('Я'.IsLatinLetter());
        }

        [TestMethod]
        public void CharUtility_IsLatinLetterOrArabicDigit_1()
        {
            Assert.IsTrue('0'.IsLatinLetterOrArabicDigit());
            Assert.IsTrue('9'.IsLatinLetterOrArabicDigit());
            Assert.IsTrue('A'.IsLatinLetterOrArabicDigit());
            Assert.IsFalse('\x0661'.IsLatinLetterOrArabicDigit());
            Assert.IsTrue('A'.IsLatinLetterOrArabicDigit());
            Assert.IsTrue('z'.IsLatinLetterOrArabicDigit());
            Assert.IsFalse('А'.IsLatinLetterOrArabicDigit());
            Assert.IsFalse('Я'.IsLatinLetterOrArabicDigit());
        }

        [TestMethod]
        public void CharUtility_IsRussianLetter_1()
        {
            Assert.IsTrue('А'.IsRussianLetter());
            Assert.IsTrue('Я'.IsRussianLetter());
            Assert.IsFalse('A'.IsRussianLetter());
            Assert.IsFalse('0'.IsRussianLetter());
        }

        [TestMethod]
        public void CharUtility_IsUrlSafeChar_1()
        {
            Assert.IsTrue(CharUtility.IsUrlSafeChar('A'));
            Assert.IsTrue(CharUtility.IsUrlSafeChar('Z'));
            Assert.IsTrue(CharUtility.IsUrlSafeChar('a'));
            Assert.IsTrue(CharUtility.IsUrlSafeChar('z'));
            Assert.IsTrue(CharUtility.IsUrlSafeChar('_'));
            Assert.IsTrue(CharUtility.IsUrlSafeChar('-'));
            Assert.IsFalse(CharUtility.IsUrlSafeChar('%'));
            Assert.IsFalse(CharUtility.IsUrlSafeChar('/'));
            Assert.IsFalse(CharUtility.IsUrlSafeChar('\0'));
        }

        [TestMethod]
        public void CharUtility_OneOf_1()
        {
            List<char> list = new List<char>
            {
                'a',
                'b',
                'c'
            };
            Assert.IsTrue(CharUtility.OneOf('a', list));
            Assert.IsTrue(CharUtility.OneOf('b', list));
            Assert.IsTrue(CharUtility.OneOf('c', list));
            Assert.IsFalse(CharUtility.OneOf('d', list));
            Assert.IsFalse(CharUtility.OneOf(' ', list));
        }

        [TestMethod]
        public void CharUtility_OneOf_2()
        {
            Assert.IsTrue(CharUtility.OneOf('a', 'a', 'b', 'c'));
            Assert.IsTrue(CharUtility.OneOf('b', 'a', 'b', 'c'));
            Assert.IsTrue(CharUtility.OneOf('c', 'a', 'b', 'c'));
            Assert.IsFalse(CharUtility.OneOf('d', 'a', 'b', 'c'));
            Assert.IsFalse(CharUtility.OneOf(' ', 'a', 'b', 'c'));
        }

        [TestMethod]
        public void CharUtility_SameChar_1()
        {
            Assert.IsTrue(CharUtility.SameChar('a', 'a'));
            Assert.IsTrue(CharUtility.SameChar('a', 'A'));
            Assert.IsTrue(CharUtility.SameChar('A', 'A'));
            Assert.IsFalse(CharUtility.SameChar('a', 'B'));
        }
    }
}
