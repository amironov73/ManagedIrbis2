﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using AM;

// ReSharper disable ConvertToLocalFunction
// ReSharper disable InvokeAsExtensionMethod

namespace UnitTests.AM
{
    [TestClass]
    public class StringUtilityTest
    {
        //[TestMethod]
        //public void StringUtility_CCat_1()
        //{
        //    Assert.AreEqual
        //        (
        //            "Directory\\",
        //            "Directory".CCat("\\")
        //        );
        //    Assert.AreEqual
        //        (
        //            "Directory\\",
        //            "Directory\\".CCat("\\")
        //        );
        //}

        //[TestMethod]
        //public void StringUtility_CCat_2()
        //{
        //    Assert.AreEqual
        //        (
        //            "Directory\\",
        //            "Directory".CCat("?", "\\")
        //        );
        //    Assert.AreEqual
        //        (
        //            "Directory?",
        //            "Directory?".CCat("?", "\\")
        //        );
        //}

        //[TestMethod]
        //public void StringUtility_ChangeEncoding_1()
        //{
        //    Assert.AreEqual
        //        (
        //            "РџСЂРёРІРµС‚",
        //            StringUtility.ChangeEncoding
        //            (
        //                Encoding.UTF8,
        //                Encoding.GetEncoding(1251),
        //                "Привет"
        //            )
        //        );

        //    Assert.AreEqual
        //        (
        //            "Привет",
        //            StringUtility.ChangeEncoding
        //            (
        //                Encoding.GetEncoding(1251),
        //                Encoding.UTF8,
        //                "РџСЂРёРІРµС‚"
        //            )
        //        );
        //}

        //[TestMethod]
        //public void StringUtility_ChangeEncoding_2()
        //{
        //    Assert.AreEqual
        //        (
        //            "Привет",
        //            StringUtility.ChangeEncoding
        //                (
        //                    Encoding.UTF8,
        //                    Encoding.UTF8,
        //                    "Привет"
        //                )
        //        );
        //}

        [TestMethod]
        public void StringUtility_CompareNoCase_1()
        {
            Assert.IsTrue(StringUtility.CompareNoCase('A', 'a'));
            Assert.IsTrue(StringUtility.CompareNoCase('A', 'A'));
            Assert.IsTrue(StringUtility.CompareNoCase(' ', ' '));
            Assert.IsFalse(StringUtility.CompareNoCase(' ', 'A'));
        }

        [TestMethod]
        public void StringUtility_CompareNoCase_2()
        {
            Assert.IsTrue(StringUtility.CompareNoCase(string.Empty, string.Empty));
            Assert.IsTrue(StringUtility.CompareNoCase(" ", " "));
            Assert.IsTrue(StringUtility.CompareNoCase("Hello", "HELLO"));
            Assert.IsFalse(StringUtility.CompareNoCase("Hello", "HELLO2"));
        }

        [TestMethod]
        public void StringUtility_ConsistOf_1()
        {
            Assert.IsFalse(string.Empty.ConsistOf('a'));
            Assert.IsTrue("aaa".ConsistOf('a'));
            Assert.IsFalse("aba".ConsistOf('a'));
        }

        [TestMethod]
        public void StringUtility_ConsistOf_2()
        {
            Assert.IsFalse(string.Empty.ConsistOf('a', 'b'));
            Assert.IsTrue("abc".ConsistOf('a', 'b', 'c'));
            Assert.IsFalse("abcd".ConsistOf('a', 'b', 'c'));
        }

        //[TestMethod]
        //public void StringUtility_ConsistOfDigits_1()
        //{
        //    Assert.IsFalse(StringUtility.ConsistOfDigits(string.Empty, 0, 5));
        //    Assert.IsFalse(StringUtility.ConsistOfDigits("     456", 0, 5));
        //    Assert.IsTrue(StringUtility.ConsistOfDigits("12345adfg", 0, 5));
        //}

        //[TestMethod]
        //public void StringUtility_ConsistOfDigits_2()
        //{
        //    Assert.IsFalse(StringUtility.ConsistOfDigits(string.Empty));
        //    Assert.IsFalse(StringUtility.ConsistOfDigits("     456"));
        //    Assert.IsTrue(StringUtility.ConsistOfDigits("123456"));
        //}

        //[TestMethod]
        //public void StringUtility_ContainsAnySymbol_1()
        //{
        //    Assert.IsTrue("Again".ContainsAnySymbol('a', 'b', 'c'));
        //    Assert.IsFalse(string.Empty.ContainsAnySymbol('a', 'b', 'c'));
        //    Assert.IsFalse("Other".ContainsAnySymbol('a', 'b', 'c'));
        //}

        //[TestMethod]
        //public void StringUtility_ContainsCharacter_1()
        //{
        //    Assert.IsTrue("Again".ContainsCharacter('a'));
        //    Assert.IsFalse(string.Empty.ContainsCharacter('a'));
        //    Assert.IsFalse("Other".ContainsCharacter('a'));
        //}

        //[TestMethod]
        //public void StringUtility_ContainsWhitespace_1()
        //{
        //    Assert.IsTrue("Again and again".ContainsWhitespace());
        //    Assert.IsFalse("Again_and_again".ContainsWhitespace());
        //    Assert.IsFalse("".ContainsWhitespace());
        //}

        //[TestMethod]
        //public void StringUtility_CountSubstrings_1()
        //{
        //    Assert.AreEqual(0, StringUtility.CountSubstrings(null, null));
        //    Assert.AreEqual(0, "".CountSubstrings(null));
        //    Assert.AreEqual(0, "".CountSubstrings(""));
        //    Assert.AreEqual(0, "aga".CountSubstrings(""));
        //    Assert.AreEqual(0, "".CountSubstrings("aga"));
        //}

        //[TestMethod]
        //public void StringUtility_CountSubstrings_2()
        //{
        //    Assert.AreEqual(2, "aga".CountSubstrings("a"));
        //    Assert.AreEqual(1, "aga".CountSubstrings("ag"));
        //    Assert.AreEqual(1, "aga".CountSubstrings("aga"));
        //    Assert.AreEqual(0, "aga".CountSubstrings("aga2"));
        //    Assert.AreEqual(0, "aga".CountSubstrings(""));
        //}

        [TestMethod]
        public void StringUtility_EmptyArray_1()
        {
            string[] array = StringUtility.EmptyArray;
            Assert.IsNotNull(array);
            Assert.AreEqual(0, array.Length);
        }

        //[TestMethod]
        //public void StringUtility_EmptyToNull_1()
        //{
        //    Assert.AreEqual("Hello", "Hello".EmptyToNull());
        //    Assert.AreEqual(null, "".EmptyToNull());
        //    Assert.AreEqual(null, ((string)null).EmptyToNull());
        //}

        //[TestMethod]
        //public void StringUtility_FirstChar_1()
        //{
        //    Assert.AreEqual('H', "Hello".FirstChar());
        //    Assert.AreEqual('\0', "".FirstChar());
        //    Assert.AreEqual('\0', ((string)null).FirstChar());
        //}

        //[TestMethod]
        //public void StringUtility_GetGroups_1()
        //{
        //    string[] result = StringUtility.GetGroups
        //        (
        //            "Слева=Справа",
        //            @"^(\w+)=(\w+)$"
        //        );
        //    Assert.AreEqual(2, result.Length);
        //    Assert.AreEqual("Слева", result[0]);
        //    Assert.AreEqual("Справа", result[1]);
        //}

        //[TestMethod]
        //public void StringUtility_GetGroups2()
        //{
        //    string[] result = StringUtility.GetGroups
        //        (
        //            "Слева=Справа",
        //            @"^(\d+)=(\d+)$"
        //        );
        //    Assert.AreEqual(0, result.Length);
        //}

        //private void _TestGetPositions
        //    (
        //        string text,
        //        char c,
        //        params int[] expected
        //    )
        //{
        //    int[] actual = text.GetPositions(c);
        //    Assert.AreEqual(expected.Length, actual.Length);
        //    for (int i = 0; i < expected.Length; i++)
        //    {
        //        Assert.AreEqual(expected[i], actual[i]);
        //    }
        //}

        //[TestMethod]
        //public void StringUtility_GetPositions_1()
        //{
        //    _TestGetPositions("", 'a');
        //    _TestGetPositions("a", 'a', 0);
        //    _TestGetPositions("aga", 'a', 0, 2);
        //    _TestGetPositions("aga", 'b');
        //}

        //[TestMethod]
        //public void StringUtility_GetWords_1()
        //{
        //    string[] words = StringUtility.GetWords("");
        //    Assert.AreEqual(0, words.Length);

        //    words = StringUtility.GetWords("Hello");
        //    Assert.AreEqual(1, words.Length);

        //    words = StringUtility.GetWords("Hello, world!");
        //    Assert.AreEqual(2, words.Length);
        //    Assert.AreEqual("Hello", words[0]);
        //    Assert.AreEqual("world", words[1]);

        //    words = StringUtility.GetWords("!!!");
        //    Assert.AreEqual(0, words.Length);
        //}

        //[TestMethod]
        //public void StringUtility_IfEmpty_1()
        //{
        //    Assert.AreEqual("Hello", "Hello".IfEmpty("Again"));
        //    Assert.AreEqual("Again", "".IfEmpty("Again"));
        //    Assert.AreEqual("Again", "".IfEmpty("", "Again"));
        //}

        //[TestMethod]
        //public void StringUtility_IndexOfAny_1()
        //{
        //    string[] anyOf = { "--", "!!" };
        //    int which;
        //    Assert.AreEqual(-1, "".IndexOfAny(out which, anyOf));
        //    Assert.AreEqual(-1, which);

        //    Assert.AreEqual(-1, "Hello".IndexOfAny(out which, anyOf));
        //    Assert.AreEqual(-1, which);

        //    Assert.AreEqual(5, "Hello--world".IndexOfAny(out which, anyOf));
        //    Assert.AreEqual(0, which);

        //    Assert.AreEqual(5, "Hello--world!!".IndexOfAny(out which, anyOf));
        //    Assert.AreEqual(0, which);

        //    Assert.AreEqual(5, "Hello!!world--".IndexOfAny(out which, anyOf));
        //    Assert.AreEqual(1, which);
        //}

        //[TestMethod]
        //public void StringUtility_IsBlank_1()
        //{
        //    Assert.IsTrue(StringUtility.IsBlank(null));
        //    Assert.IsTrue("".IsBlank());
        //    Assert.IsTrue(" ".IsBlank());
        //    Assert.IsTrue("  ".IsBlank());
        //    Assert.IsFalse("1".IsBlank());
        //}

        //[TestMethod]
        //public void StringUtility_IsDecimal_1()
        //{
        //    Assert.IsFalse(StringUtility.IsDecimal(string.Empty));
        //    Assert.IsFalse(StringUtility.IsDecimal(" "));
        //    Assert.IsTrue(StringUtility.IsDecimal("12"));
        //    Assert.IsTrue(StringUtility.IsDecimal("12.34"));
        //    Assert.IsTrue(StringUtility.IsDecimal("-12.34"));
        //    Assert.IsFalse(StringUtility.IsDecimal("12 340"));
        //    Assert.IsFalse(StringUtility.IsDecimal("12.34E50"));
        //}

        //[TestMethod]
        //public void StringUtility_IsInteger_1()
        //{
        //    Assert.IsFalse(StringUtility.IsInteger(""));
        //    Assert.IsFalse(StringUtility.IsInteger("A"));
        //    Assert.IsTrue(StringUtility.IsInteger("0"));
        //    Assert.IsTrue(StringUtility.IsInteger("1"));
        //    Assert.IsTrue(StringUtility.IsInteger("-1"));
        //    Assert.IsFalse(StringUtility.IsInteger(new string('1', 1000)));
        //}

        //[TestMethod]
        //public void StringUtility_IsLongInteger_1()
        //{
        //    Assert.IsFalse(StringUtility.IsLongInteger(""));
        //    Assert.IsFalse(StringUtility.IsLongInteger("A"));
        //    Assert.IsTrue(StringUtility.IsLongInteger("0"));
        //    Assert.IsTrue(StringUtility.IsLongInteger("1"));
        //    Assert.IsTrue(StringUtility.IsLongInteger("-1"));
        //    Assert.IsFalse(StringUtility.IsLongInteger(new string('1', 1000)));
        //}

        //[TestMethod]
        //public void StringUtility_IsNumeric_1()
        //{
        //    Assert.IsFalse(StringUtility.IsNumeric(string.Empty));
        //    Assert.IsFalse(StringUtility.IsNumeric(" "));
        //    Assert.IsTrue(StringUtility.IsNumeric("12"));
        //    Assert.IsTrue(StringUtility.IsNumeric("12.34"));
        //    Assert.IsTrue(StringUtility.IsNumeric("-12.34"));
        //    Assert.IsFalse(StringUtility.IsNumeric("12 340"));
        //    Assert.IsTrue(StringUtility.IsNumeric("12.34E50"));
        //}
        //[TestMethod]
        //public void StringUtility_IsShortInteger_1()
        //{
        //    Assert.IsFalse(StringUtility.IsShortInteger(""));
        //    Assert.IsFalse(StringUtility.IsShortInteger("A"));
        //    Assert.IsTrue(StringUtility.IsShortInteger("0"));
        //    Assert.IsTrue(StringUtility.IsShortInteger("1"));
        //    Assert.IsTrue(StringUtility.IsShortInteger("-1"));
        //    Assert.IsFalse(StringUtility.IsShortInteger(new string('1', 1000)));
        //}

        //[TestMethod]
        //public void StringUtility_IsValidIdentifier_1()
        //{
        //    Assert.IsFalse(string.Empty.IsValidIdentifier());
        //    Assert.IsTrue("a".IsValidIdentifier());
        //    Assert.IsTrue("a1".IsValidIdentifier());
        //    Assert.IsTrue("_a1".IsValidIdentifier());
        //    Assert.IsFalse("1a".IsValidIdentifier());
        //    Assert.IsFalse("a1$".IsValidIdentifier());
        //}

        //private void _TestJoin
        //    (
        //        string expected,
        //        string separator,
        //        params object[] objects
        //    )
        //{
        //    string actual = StringUtility.Join
        //        (
        //            separator,
        //            objects
        //        );

        //    Assert.AreEqual(expected, actual);
        //}

        //[TestMethod]
        //public void StringUtility_IsUrlSafeChar_1()
        //{
        //    Assert.IsTrue(StringUtility.IsUrlSafeChar('a'));
        //    Assert.IsFalse(StringUtility.IsUrlSafeChar(' '));
        //    Assert.IsTrue(StringUtility.IsUrlSafeChar('-'));
        //}

        //[TestMethod]
        //public void StringUtility_Join_1()
        //{
        //    const string comma = ", ";
        //    _TestJoin(string.Empty, comma);
        //    _TestJoin("1", comma, 1);
        //    _TestJoin(string.Empty, comma, new object[] { null });
        //    _TestJoin("1", comma, 1, null);
        //    _TestJoin("1", comma, null, 1);
        //    _TestJoin("1, 2, 3", comma, 1, 2, 3);
        //    _TestJoin("12", null, 1, 2);
        //    _TestJoin("1", comma, 1, null, null);
        //    _TestJoin("1, 4", comma, 1, null, null, 4);
        //}

        //[TestMethod]
        //public void StringUtility_LastChar_1()
        //{
        //    Assert.AreEqual('\0', ((string)null).LastChar());
        //    Assert.AreEqual('\0', "".LastChar());
        //    Assert.AreEqual(' ', " ".LastChar());
        //    Assert.AreEqual('!', "Hello, world!".LastChar());
        //}

        //private void _TestMangle
        //    (
        //        string text,
        //        string expected
        //    )
        //{
        //    char[] badCharacters = { 'a', 'b', 'c' };
        //    string actual = StringUtility.Mangle
        //        (
        //            text,
        //            '\\',
        //            badCharacters
        //        );
        //    Assert.AreEqual(expected, actual);
        //}

        //[TestMethod]
        //public void StringUtility_Mangle_1()
        //{
        //    _TestMangle(null, null);
        //    _TestMangle(string.Empty, string.Empty);
        //    _TestMangle("Hello", "Hello");
        //    _TestMangle("Hello: a", "Hello: \\a");
        //    _TestMangle("abc", "\\a\\b\\c");
        //    _TestMangle("\\", "\\\\");
        //}

        //[TestMethod]
        //public void StringUtility_MergeLines_1()
        //{
        //    string[] lines = new string[0];
        //    string text = lines.MergeLines();
        //    Assert.AreEqual(string.Empty, text);

        //    lines = new[] { "Hello" };
        //    text = lines.MergeLines();
        //    Assert.AreEqual("Hello", text);
        //}

        //[TestMethod]
        //public void StringUtility_OneOf_1()
        //{
        //    List<string> list = new List<string>
        //    {
        //        "Hello",
        //        "world"
        //    };
        //    Assert.IsFalse(((string)null).OneOf(list));
        //    Assert.IsTrue("Hello".OneOf(list));
        //    Assert.IsTrue("hello".OneOf(list));
        //    Assert.IsTrue("WORLD".OneOf(list));
        //    Assert.IsFalse("".OneOf(list));
        //    Assert.IsFalse("Other".OneOf(list));
        //}

        //[TestMethod]
        //public void StringUtility_OneOf_2()
        //{
        //    Assert.IsFalse(((string)null).OneOf("Hello", "world"));
        //    Assert.IsTrue("Hello".OneOf("Hello", "world"));
        //    Assert.IsTrue("hello".OneOf("Hello", "world"));
        //    Assert.IsTrue("WORLD".OneOf("Hello", "world"));
        //    Assert.IsFalse("".OneOf("Hello", "world"));
        //    Assert.IsFalse("Other".OneOf("Hello", "world"));
        //}

        //[TestMethod]
        //public void StringUtility_OneOf_3()
        //{
        //    List<char> list = new List<char>
        //    {
        //        'a',
        //        'b',
        //        'c'
        //    };
        //    Assert.IsTrue('a'.OneOf(list));
        //    Assert.IsTrue('b'.OneOf(list));
        //    Assert.IsTrue('c'.OneOf(list));
        //    Assert.IsFalse('d'.OneOf(list));
        //    Assert.IsFalse(' '.OneOf(list));
        //}

        //[TestMethod]
        //public void StringUtility_OneOf_4()
        //{
        //    Assert.IsTrue('a'.OneOf('a', 'b', 'c'));
        //    Assert.IsTrue('b'.OneOf('a', 'b', 'c'));
        //    Assert.IsTrue('c'.OneOf('a', 'b', 'c'));
        //    Assert.IsFalse('d'.OneOf('a', 'b', 'c'));
        //    Assert.IsFalse(' '.OneOf('a', 'b', 'c'));
        //}

        //[TestMethod]
        //public void StringUtility_Random_1()
        //{
        //    string text = StringUtility.Random(1);
        //    Assert.AreEqual(1, text.Length);

        //    text = StringUtility.Random(2);
        //    Assert.AreEqual(2, text.Length);

        //    text = StringUtility.Random(10);
        //    Assert.AreEqual(10, text.Length);
        //}

        //[TestMethod]
        //public void StringUtility_ReplaceControlCharacters_1()
        //{
        //    Assert.IsNull(StringUtility.ReplaceControlCharacters(null, '!'));
        //    Assert.AreEqual(string.Empty, StringUtility.ReplaceControlCharacters(string.Empty, '!'));
        //    Assert.AreEqual("Hello, world", StringUtility.ReplaceControlCharacters("Hello, world", '!'));
        //    Assert.AreEqual("Hello,!world", StringUtility.ReplaceControlCharacters("Hello,\nworld", '!'));
        //}

        //[TestMethod]
        //public void StringUtility_ReplaceControlCharacters_2()
        //{
        //    Assert.IsNull(StringUtility.ReplaceControlCharacters(null));
        //    Assert.AreEqual(string.Empty, StringUtility.ReplaceControlCharacters(string.Empty));
        //    Assert.AreEqual("Hello, world", StringUtility.ReplaceControlCharacters("Hello, world"));
        //    Assert.AreEqual("Hello, world", StringUtility.ReplaceControlCharacters("Hello,\nworld"));
        //}

        //[TestMethod]
        //public void StringUtility_Replicate_1()
        //{
        //    string text = StringUtility.Replicate("Hello", 0);
        //    Assert.AreEqual("", text);

        //    text = StringUtility.Replicate("Hello", 1);
        //    Assert.AreEqual("Hello", text);

        //    text = StringUtility.Replicate("Hello", 2);
        //    Assert.AreEqual("HelloHello", text);

        //    text = StringUtility.Replicate("Hello", -1);
        //    Assert.AreEqual("", text);

        //    text = StringUtility.Replicate(null, 3);
        //    Assert.AreEqual(null, text);
        //}

        //[TestMethod]
        //public void StringUtility_SafeCompare_1()
        //{
        //    Assert.IsTrue("".SafeCompare("") == 0);
        //    Assert.IsTrue("".SafeCompare(" ") < 0);
        //    Assert.IsTrue("A".SafeCompare(" ") > 0);
        //    Assert.IsTrue("A".SafeCompare(null) > 0);
        //    Assert.IsTrue(((string)null).SafeCompare(null) == 0);
        //    Assert.IsTrue(((string)null).SafeCompare("") < 0);
        //}

        //[TestMethod]
        //public void StringUtility_SafeCompare_2()
        //{
        //    Assert.IsFalse(StringUtility.SafeCompare(null, "Hello", "World"));
        //    Assert.IsTrue(StringUtility.SafeCompare("Hello", "Hello", "World"));
        //    Assert.IsFalse(StringUtility.SafeCompare("Other", "Hello", "World"));
        //}

        //[TestMethod]
        //public void StringUtility_SafeContains_1()
        //{
        //    Assert.IsFalse("".SafeContains("aga"));
        //    Assert.IsFalse("Again".SafeContains((string)null));
        //    Assert.IsFalse(((string)null).SafeContains((string)null));
        //    Assert.IsTrue("Again".SafeContains("aga"));
        //}

        //[TestMethod]
        //public void StringUtility_SafeContains_2()
        //{
        //    Assert.IsFalse("".SafeContains("aga", "gain"));
        //    Assert.IsFalse("Again".SafeContains());
        //    Assert.IsTrue("Again".SafeContains("aga", "GAIN"));
        //    Assert.IsTrue("Again".SafeContains("oga", "GAIN"));
        //}

        //[TestMethod]
        //public void StringUtility_SafeStarts_1()
        //{
        //    Assert.IsFalse("".SafeStarts("aga"));
        //    Assert.IsFalse("Again".SafeStarts(""));
        //    Assert.IsTrue("Again".SafeStarts("aga"));
        //}

        //[TestMethod]
        //public void StringUtility_SafeSubstring_1()
        //{
        //    Assert.AreEqual("Aga", "Again".SafeSubstring(0, 3));
        //    Assert.AreEqual("Again", "Again".SafeSubstring(0, 300));
        //    Assert.AreEqual(null, ((string)null).SafeSubstring(0, 300));
        //    Assert.AreEqual("", "".SafeSubstring(0, 300));
        //    Assert.AreEqual("", "Again".SafeSubstring(0, -1));
        //    Assert.AreEqual("", "Again".SafeSubstring(500, 300));
        //}

        //[TestMethod]
        //public void StringUtility_SameChar_1()
        //{
        //    Assert.IsTrue('a'.SameChar('a'));
        //    Assert.IsTrue('a'.SameChar('A'));
        //    Assert.IsTrue('A'.SameChar('A'));
        //    Assert.IsFalse('a'.SameChar('B'));
        //}

        //[TestMethod]
        //public void StringUtility_SameString_1()
        //{
        //    Assert.IsTrue("".SameString(""));
        //    Assert.IsTrue(" ".SameString(" "));
        //    Assert.IsTrue("Hello".SameString("HELLO"));
        //    Assert.IsFalse("Hello".SameString("HELLO2"));
        //}

        //[TestMethod]
        //public void StringUtility_SameStringSensitive_1()
        //{
        //    Assert.IsTrue("".SameStringSensitive(""));
        //    Assert.IsTrue(" ".SameStringSensitive(" "));
        //    Assert.IsTrue("Hello".SameStringSensitive("Hello"));
        //    Assert.IsFalse("Hello".SameStringSensitive("HELLO"));
        //    Assert.IsFalse("Hello".SameStringSensitive("HELLO2"));
        //}

        //private void _TestSparse
        //    (
        //        string text,
        //        string expected
        //    )
        //{
        //    string actual = StringUtility.Sparse(text);
        //    Assert.AreEqual(expected, actual);
        //}

        //[TestMethod]
        //public void StringUtility_Sparse()
        //{
        //    _TestSparse(null, null);
        //    _TestSparse(string.Empty, string.Empty);
        //    _TestSparse("1", "1");
        //    _TestSparse(" ", string.Empty);
        //    _TestSparse("Hello,world!", "Hello, world!");
        //    _TestSparse("Hello,  world!", "Hello, world!");
        //    _TestSparse("Hello ,world!", "Hello, world!");
        //}

        //[TestMethod]
        //public void StringUtility_SplitFirst_1()
        //{
        //    string[] parts = "".SplitFirst('!');
        //    Assert.AreEqual(1, parts.Length);
        //    Assert.AreEqual("", parts[0]);

        //    parts = "Hello!Again".SplitFirst('!');
        //    Assert.AreEqual(2, parts.Length);
        //    Assert.AreEqual("Hello", parts[0]);
        //    Assert.AreEqual("Again", parts[1]);

        //    parts = "Hello!Again!And again".SplitFirst('!');
        //    Assert.AreEqual(2, parts.Length);
        //    Assert.AreEqual("Hello", parts[0]);
        //    Assert.AreEqual("Again!And again", parts[1]);
        //}

        //[TestMethod]
        //public void StringUtility_SplitLines_1()
        //{
        //    string[] lines = "".SplitLines();
        //    Assert.AreEqual(1, lines.Length);

        //    lines = " ".SplitLines();
        //    Assert.AreEqual(1, lines.Length);

        //    lines = "first\r\n".SplitLines();
        //    Assert.AreEqual(2, lines.Length);

        //    lines = "first\r\nsecond".SplitLines();
        //    Assert.AreEqual(2, lines.Length);
        //}

        //[TestMethod]
        //public void StringUtility_SplitString_1()
        //{
        //    Func<char, bool> func = c => c == '!';
        //    string[] result = "!!aaa!bbb!!c".SplitString(func).ToArray();
        //    Assert.AreEqual(3, result.Length);
        //    Assert.AreEqual("aaa", result[0]);
        //    Assert.AreEqual("bbb", result[1]);
        //    Assert.AreEqual("c", result[2]);

        //    result = string.Empty.SplitString(func).ToArray();
        //    Assert.AreEqual(0, result.Length);

        //    result = string.Empty.SplitString(func).ToArray();
        //    Assert.AreEqual(0, result.Length);

        //    result = "!".SplitString(func).ToArray();
        //    Assert.AreEqual(0, result.Length);

        //    result = "a".SplitString(func).ToArray();
        //    Assert.AreEqual(1, result.Length);
        //    Assert.AreEqual("a", result[0]);

        //    result = "!!!".SplitString(func).ToArray();
        //    Assert.AreEqual(0, result.Length);
        //}

        //[TestMethod]
        //public void StringUtility_SplitString_2()
        //{
        //    const string separator = "--";
        //    string[] actual = StringUtility.SplitString("", separator);
        //    Assert.AreEqual(1, actual.Length);
        //    Assert.AreEqual("", actual[0]);

        //    actual = StringUtility.SplitString("Hello, world", separator);
        //    Assert.AreEqual(1, actual.Length);
        //    Assert.AreEqual("Hello, world", actual[0]);

        //    actual = StringUtility.SplitString("Hello--world", separator);
        //    Assert.AreEqual(2, actual.Length);
        //    Assert.AreEqual("Hello", actual[0]);
        //    Assert.AreEqual("world", actual[1]);

        //    actual = StringUtility.SplitString("--Hello--world", separator);
        //    Assert.AreEqual(3, actual.Length);
        //    Assert.AreEqual("", actual[0]);
        //    Assert.AreEqual("Hello", actual[1]);
        //    Assert.AreEqual("world", actual[2]);

        //    actual = StringUtility.SplitString("Hello--world--", separator);
        //    Assert.AreEqual(3, actual.Length);
        //    Assert.AreEqual("Hello", actual[0]);
        //    Assert.AreEqual("world", actual[1]);
        //    Assert.AreEqual("", actual[2]);

        //    actual = StringUtility.SplitString("Hello----world", separator);
        //    Assert.AreEqual(3, actual.Length);
        //    Assert.AreEqual("Hello", actual[0]);
        //    Assert.AreEqual("", actual[1]);
        //    Assert.AreEqual("world", actual[2]);
        //}

        //[TestMethod]
        //public void StringUtility_SplitString_3()
        //{
        //    string[] separators = { "--", "!!" };
        //    string[] actual = StringUtility.SplitString("", separators);
        //    Assert.AreEqual(1, actual.Length);
        //    Assert.AreEqual("", actual[0]);

        //    actual = StringUtility.SplitString("Hello, world", separators);
        //    Assert.AreEqual(1, actual.Length);
        //    Assert.AreEqual("Hello, world", actual[0]);

        //    actual = StringUtility.SplitString("Hello--world", separators);
        //    Assert.AreEqual(2, actual.Length);
        //    Assert.AreEqual("Hello", actual[0]);
        //    Assert.AreEqual("world", actual[1]);

        //    actual = StringUtility.SplitString("--Hello!!world", separators);
        //    Assert.AreEqual(3, actual.Length);
        //    Assert.AreEqual("", actual[0]);
        //    Assert.AreEqual("Hello", actual[1]);
        //    Assert.AreEqual("world", actual[2]);

        //    actual = StringUtility.SplitString("Hello!!world--", separators);
        //    Assert.AreEqual(3, actual.Length);
        //    Assert.AreEqual("Hello", actual[0]);
        //    Assert.AreEqual("world", actual[1]);
        //    Assert.AreEqual("", actual[2]);

        //    actual = StringUtility.SplitString("Hello--!!world", separators);
        //    Assert.AreEqual(3, actual.Length);
        //    Assert.AreEqual("Hello", actual[0]);
        //    Assert.AreEqual("", actual[1]);
        //    Assert.AreEqual("world", actual[2]);
        //}

        //[TestMethod]
        //public void StringUtility_SplitString_4()
        //{
        //    char[] separators = { '#', '!' };

        //    string[] actual = StringUtility.SplitString(string.Empty, separators, 2);
        //    Assert.AreEqual(0, actual.Length);

        //    actual = StringUtility.SplitString("abc", separators, 2);
        //    Assert.AreEqual(1, actual.Length);
        //    Assert.AreEqual("abc", actual[0]);

        //    actual = StringUtility.SplitString("a!b#c", separators, 2);
        //    Assert.AreEqual(2, actual.Length);
        //    Assert.AreEqual("a", actual[0]);
        //    Assert.AreEqual("b#c", actual[1]);

        //    actual = StringUtility.SplitString("a!#c", separators, 2);
        //    Assert.AreEqual(2, actual.Length);
        //    Assert.AreEqual("a", actual[0]);
        //    Assert.AreEqual("#c", actual[1]);

        //    actual = StringUtility.SplitString("a!b#c", separators, 1);
        //    Assert.AreEqual(1, actual.Length);
        //    Assert.AreEqual("a!b#c", actual[0]);
        //}

        //[TestMethod]
        //public void StringUtility_ToLowerInvariant_1()
        //{
        //    Assert.AreEqual("", StringUtility.ToLowerInvariant(""));
        //    Assert.AreEqual("!", StringUtility.ToLowerInvariant("!"));
        //    Assert.AreEqual("a", StringUtility.ToLowerInvariant("A"));
        //}

        //[TestMethod]
        //public void StringUtility_ToUpperInvariant_1()
        //{
        //    Assert.AreEqual("", StringUtility.ToUpperInvariant(""));
        //    Assert.AreEqual("!", StringUtility.ToUpperInvariant("!"));
        //    Assert.AreEqual("A", StringUtility.ToUpperInvariant("a"));
        //}

        [TestMethod]
        public void StringUtility_ToVisibleString_1()
        {
            Assert.AreEqual("a", "a".ToVisibleString());
            Assert.AreEqual("(null)", StringUtility.ToVisibleString(null));
            Assert.AreEqual("(empty)", string.Empty.ToVisibleString());
        }

        //[TestMethod]
        //public void StringUtility_TrimLines_1()
        //{
        //    string[] lines = new string[0];
        //    string[] actual = StringUtility.TrimLines(lines).ToArray();
        //    Assert.AreEqual(0, actual.Length);

        //    lines = new[] { "Hello", "World" };
        //    actual = StringUtility.TrimLines(lines).ToArray();
        //    Assert.AreEqual(2, actual.Length);
        //    Assert.AreEqual("Hello", actual[0]);
        //    Assert.AreEqual("World", actual[1]);

        //    lines = new[] { "Hello ", " World" };
        //    actual = StringUtility.TrimLines(lines).ToArray();
        //    Assert.AreEqual(2, actual.Length);
        //    Assert.AreEqual("Hello", actual[0]);
        //    Assert.AreEqual("World", actual[1]);
        //}

        //[TestMethod]
        //public void StringUtility_TrimLines_2()
        //{
        //    char[] characters = { '!' };
        //    string[] lines = new string[0];
        //    string[] actual = StringUtility.TrimLines(lines, characters).ToArray();
        //    Assert.AreEqual(0, actual.Length);

        //    lines = new[] { "Hello", "World" };
        //    actual = StringUtility.TrimLines(lines, characters).ToArray();
        //    Assert.AreEqual(2, actual.Length);
        //    Assert.AreEqual("Hello", actual[0]);
        //    Assert.AreEqual("World", actual[1]);

        //    lines = new[] { "Hello!", "!World" };
        //    actual = StringUtility.TrimLines(lines, characters).ToArray();
        //    Assert.AreEqual(2, actual.Length);
        //    Assert.AreEqual("Hello", actual[0]);
        //    Assert.AreEqual("World", actual[1]);
        //}

        //[TestMethod]
        //public void StringUtility_Unquote_1()
        //{
        //    Assert.AreEqual("", "\"\"".Unquote());
        //    Assert.AreEqual("\"\"1", "\"\"1".Unquote());
        //    Assert.AreEqual("text", "\"text\"".Unquote());
        //}

        //[TestMethod]
        //public void StringUtility_Unquote_2()
        //{
        //    Assert.AreEqual("", "()".Unquote('(', ')'));
        //    Assert.AreEqual("()1", "()1".Unquote('(', ')'));
        //    Assert.AreEqual("text", "\"text\"".Unquote('"'));
        //}

        //[TestMethod]
        //public void StringUtility_UrlDecode_1()
        //{
        //    Assert.AreEqual
        //        (
        //            "Тили-тили, трали-вали",
        //            StringUtility.UrlDecode
        //            (
        //                "%D0%A2%D0%B8%D0%BB%D0%B8-%D1%82%D0%B8%D0%BB%D0%B8%2C+%D1%82%D1%80%D0%B0%D0%BB%D0%B8-%D0%B2%D0%B0%D0%BB%D0%B8",
        //                Encoding.UTF8
        //            )
        //        );
        //}

        //[TestMethod]
        //public void StringUtility_UrlDecode_2()
        //{
        //    Assert.AreEqual
        //        (
        //            null,
        //            StringUtility.UrlDecode
        //            (
        //                null,
        //                Encoding.UTF8
        //            )
        //        );
        //}

        //[TestMethod]
        //public void StringUtility_UrlEncode_1()
        //{
        //    Assert.AreEqual
        //        (
        //            "%D0%A2%D0%B8%D0%BB%D0%B8-%D1%82%D0%B8%D0%BB%D0%B8%2C+%D1%82%D1%80%D0%B0%D0%BB%D0%B8-%D0%B2%D0%B0%D0%BB%D0%B8",
        //            StringUtility.UrlEncode
        //            (
        //                "Тили-тили, трали-вали",
        //                Encoding.UTF8
        //            )
        //        );
        //}

        //[TestMethod]
        //public void StringUtility_UrlEncode_2()
        //{
        //    Assert.AreEqual
        //        (
        //            null,
        //            StringUtility.UrlEncode
        //            (
        //                null,
        //                Encoding.UTF8
        //            )
        //        );
        //}

        //[TestMethod]
        //public void StringUtility_Wrap_1()
        //{
        //    const string nullString = null;

        //    Assert.AreEqual
        //        (
        //            nullString,
        //            nullString.Wrap("(", ")")
        //        );
        //    Assert.AreEqual
        //        (
        //            "()",
        //            string.Empty.Wrap("(", ")")
        //        );
        //    Assert.AreEqual
        //        (
        //            string.Empty,
        //            string.Empty.Wrap(nullString, nullString)
        //        );
        //    Assert.AreEqual
        //        (
        //            "[ArsMagna]",
        //            "ArsMagna".Wrap("[", "]")
        //        );
        //}
    }
}
