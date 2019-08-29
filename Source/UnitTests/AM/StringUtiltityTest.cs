using System.Collections.Generic;

using AM;

using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable ConvertToLocalFunction
// ReSharper disable InvokeAsExtensionMethod

namespace UnitTests.AM
{
    [TestClass]
    public class StringUtilityTest
    {
        [TestMethod]
        public void StringUtility_CompareNoCase_1()
        {
            Assert.IsTrue(StringUtility.CompareNoCase('A', 'a') == 0);
            Assert.IsTrue(StringUtility.CompareNoCase('A', 'A') == 0);
            Assert.IsTrue(StringUtility.CompareNoCase(' ', ' ') == 0);
            Assert.IsFalse(StringUtility.CompareNoCase(' ', 'A') == 0);
        }

        [TestMethod]
        public void StringUtility_CompareNoCase_2()
        {
            Assert.IsTrue(StringUtility.CompareNoCase(string.Empty, string.Empty) == 0);
            Assert.IsTrue(StringUtility.CompareNoCase(" ", " ") == 0);
            Assert.IsTrue(StringUtility.CompareNoCase("Hello", "HELLO") == 0);
            Assert.IsFalse(StringUtility.CompareNoCase("Hello", "HELLO2") == 0);
        }

//        [TestMethod]
//        public void StringUtility_ConsistOf_1()
//        {
//            Assert.IsFalse(StringUtility.ConsistOf(string.Empty, 'a'));
//            Assert.IsTrue(StringUtility.ConsistOf("aaa", 'a'));
//            Assert.IsFalse(StringUtility.ConsistOf("aba", 'a'));
//        }

//        [TestMethod]
//        public void StringUtility_ConsistOf_2()
//        {
//            Assert.IsFalse(StringUtility.ConsistOf(string.Empty, 'a', 'b'));
//            Assert.IsTrue(StringUtility.ConsistOf("abc", 'a', 'b', 'c'));
//            Assert.IsFalse(StringUtility.ConsistOf("abcd", 'a', 'b', 'c'));
//        }
//
//        [TestMethod]
//        public void StringUtility_ContainsAny_1()
//        {
//            Assert.IsTrue(StringUtility.ContainsAny("Again", 'a', 'b', 'c'));
//            Assert.IsFalse(StringUtility.ContainsAny(string.Empty, 'a', 'b', 'c'));
//            Assert.IsFalse(StringUtility.ContainsAny("Other", 'a', 'b', 'c'));
//        }
//
//        [TestMethod]
//        public void StringUtility_Contains_1()
//        {
//            Assert.IsTrue(StringUtility.Contains("Again", 'a'));
//            Assert.IsFalse(StringUtility.Contains(string.Empty, 'a'));
//            Assert.IsFalse(StringUtility.Contains("Other", 'a'));
//        }

//        [TestMethod]
//        public void StringUtility_CountSubstring_1()
//        {
//            Assert.AreEqual(0, StringUtility.CountSubstring(null, null));
//            Assert.AreEqual(0, StringUtility.CountSubstring(string.Empty, null));
//            Assert.AreEqual(0, StringUtility.CountSubstring(string.Empty, string.Empty));
//            Assert.AreEqual(0, StringUtility.CountSubstring("aga", string.Empty));
//            Assert.AreEqual(0, StringUtility.CountSubstring(string.Empty, "aga"));
//        }
//
//        [TestMethod]
//        public void StringUtility_CountSubstring_2()
//        {
//            Assert.AreEqual(2, StringUtility.CountSubstring("aga", "a"));
//            Assert.AreEqual(1, StringUtility.CountSubstring("aga", "ag"));
//            Assert.AreEqual(1, StringUtility.CountSubstring("aga", "aga"));
//            Assert.AreEqual(0, StringUtility.CountSubstring("aga", "aga2"));
//            Assert.AreEqual(0, StringUtility.CountSubstring("aga", string.Empty));
//        }

        [TestMethod]
        public void StringUtility_EmptyArray_1()
        {
            string[] array = StringUtility.EmptyArray;
            Assert.IsNotNull(array);
            Assert.AreEqual(0, array.Length);
        }

        [TestMethod]
        public void StringUtility_EmptyToNull_1()
        {
            Assert.AreEqual("Hello", StringUtility.EmptyToNull("Hello"));
            Assert.IsNull(null, StringUtility.EmptyToNull(string.Empty));
            Assert.IsNull(null, StringUtility.EmptyToNull(null));
        }

        [TestMethod]
        public void StringUtility_FirstChar_1()
        {
            Assert.AreEqual('H', StringUtility.FirstChar("Hello"));
            Assert.AreEqual('\0', StringUtility.FirstChar(string.Empty));
            Assert.AreEqual('\0', StringUtility.FirstChar(null));
        }

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

//        [TestMethod]
//        public void StringUtility_IfEmpty_1()
//        {
//            Assert.AreEqual("Hello", StringUtility.IfEmpty("Hello", "Again"));
//            Assert.AreEqual("Again", StringUtility.IfEmpty(string.Empty, "Again"));
//            Assert.AreEqual("Again", StringUtility.IfEmpty(string.Empty, string.Empty, "Again"));
//        }
//
//        [TestMethod]
//        public void StringUtility_IfEmpty_2()
//        {
//            Assert.IsNull(StringUtility.IfEmpty(string.Empty));
//            Assert.IsNull(StringUtility.IfEmpty(string.Empty, string.Empty));
//            Assert.IsNull(StringUtility.IfEmpty(string.Empty, string.Empty, string.Empty));
//        }
//
//        [TestMethod]
//        public void StringUtility_IfEmpty_3()
//        {
//            Assert.IsNull(StringUtility.IfEmpty(null));
//            Assert.IsNull(StringUtility.IfEmpty(string.Empty, (string)null));
//            Assert.IsNull(StringUtility.IfEmpty(null, string.Empty, null));
//        }
//
//        [TestMethod]
//        public void StringUtility_IndexOfAny_1()
//        {
//            string[] anyOf = { "--", "!!" };
//
//            Assert.AreEqual(-1, StringUtility.IndexOfAny(string.Empty, out int which, anyOf));
//            Assert.AreEqual(-1, which);
//
//            Assert.AreEqual(-1, StringUtility.IndexOfAny("Hello", out which, anyOf));
//            Assert.AreEqual(-1, which);
//
//            Assert.AreEqual(5, StringUtility.IndexOfAny("Hello--world", out which, anyOf));
//            Assert.AreEqual(0, which);
//
//            Assert.AreEqual(5, StringUtility.IndexOfAny("Hello--world!!", out which, anyOf));
//            Assert.AreEqual(0, which);
//
//            Assert.AreEqual(5, StringUtility.IndexOfAny("Hello!!world--", out which, anyOf));
//            Assert.AreEqual(1, which);
//        }

//        private void _TestJoin
//            (
//                string expected,
//                string separator,
//                params object[] objects
//            )
//        {
//            string actual = StringUtility.Join
//                (
//                    separator,
//                    objects
//                );
//
//            Assert.AreEqual(expected, actual);
//        }

//        [TestMethod]
//        public void StringUtility_Join_1()
//        {
//            const string comma = ", ";
//            _TestJoin(string.Empty, comma);
//            _TestJoin("1", comma, 1);
//            _TestJoin(string.Empty, comma, new object[] { null });
//            _TestJoin("1", comma, 1, null);
//            _TestJoin("1", comma, null, 1);
//            _TestJoin("1, 2, 3", comma, 1, 2, 3);
//            _TestJoin("12", null, 1, 2);
//            _TestJoin("1", comma, 1, null, null);
//            _TestJoin("1, 4", comma, 1, null, null, 4);
//        }

        [TestMethod]
        public void StringUtility_LastChar_1()
        {
            Assert.AreEqual('\0', StringUtility.LastChar(null));
            Assert.AreEqual('\0', StringUtility.LastChar(string.Empty));
            Assert.AreEqual(' ', StringUtility.LastChar(" "));
            Assert.AreEqual('!', StringUtility.LastChar("Hello, world!"));
        }

//        private void _TestMangle
//            (
//                string text,
//                string expected
//            )
//        {
//            char[] badCharacters = { 'a', 'b', 'c' };
//            string actual = StringUtility.Mangle
//                (
//                    text,
//                    '\\',
//                    badCharacters
//                );
//            Assert.AreEqual(expected, actual);
//        }
//
//        [TestMethod]
//        public void StringUtility_Mangle_1()
//        {
//            _TestMangle(null, null);
//            _TestMangle(string.Empty, string.Empty);
//            _TestMangle("Hello", "Hello");
//            _TestMangle("Hello: a", "Hello: \\a");
//            _TestMangle("abc", "\\a\\b\\c");
//            _TestMangle("\\", "\\\\");
//        }

//        [TestMethod]
//        public void StringUtility_MergeLines_1()
//        {
//            string[] lines = new string[0];
//            string text = StringUtility.MergeLines(lines);
//            Assert.AreEqual(string.Empty, text);
//
//            lines = new[] { "Hello" };
//            text = StringUtility.MergeLines(lines);
//            Assert.AreEqual("Hello", text);
//        }

        [TestMethod]
        public void StringUtility_OneOf_1()
        {
            List<string> list = new List<string>
            {
                "Hello",
                "world"
            };
            Assert.IsFalse(StringUtility.OneOf(null, list));
            Assert.IsTrue(StringUtility.OneOf("Hello", list));
            Assert.IsTrue(StringUtility.OneOf("hello", list));
            Assert.IsTrue(StringUtility.OneOf("WORLD", list));
            Assert.IsFalse(StringUtility.OneOf("", list));
            Assert.IsFalse(StringUtility.OneOf("Other", list));
        }

        [TestMethod]
        public void StringUtility_OneOf_2()
        {
            Assert.IsFalse(StringUtility.OneOf(null, "Hello", "world"));
            Assert.IsTrue(StringUtility.OneOf("Hello", "Hello", "world"));
            Assert.IsTrue(StringUtility.OneOf("hello", "Hello", "world"));
            Assert.IsTrue(StringUtility.OneOf("WORLD", "Hello", "world"));
            Assert.IsFalse(StringUtility.OneOf(string.Empty, "Hello", "world"));
            Assert.IsFalse(StringUtility.OneOf("Other", "Hello", "world"));
        }

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

//        [TestMethod]
//        public void StringUtility_Replicate_1()
//        {
//            string text = StringUtility.Replicate("Hello", 0);
//            Assert.AreEqual(string.Empty, text);
//
//            text = StringUtility.Replicate("Hello", 1);
//            Assert.AreEqual("Hello", text);
//
//            text = StringUtility.Replicate("Hello", 2);
//            Assert.AreEqual("HelloHello", text);
//
//            text = StringUtility.Replicate("Hello", -1);
//            Assert.AreEqual(string.Empty, text);
//
//            text = StringUtility.Replicate(null, 3);
//            Assert.AreEqual(null, text);
//        }
//
//        [TestMethod]
//        public void StringUtility_SafeCompare_1()
//        {
//            Assert.IsTrue(StringUtility.SafeCompare(string.Empty, string.Empty) == 0);
//            Assert.IsTrue(StringUtility.SafeCompare(string.Empty, " ") < 0);
//            Assert.IsTrue(StringUtility.SafeCompare("A", " ") > 0);
//            Assert.IsTrue(StringUtility.SafeCompare("A", null) > 0);
//            Assert.IsTrue(StringUtility.SafeCompare(null, null) == 0);
//            Assert.IsTrue(StringUtility.SafeCompare(null, string.Empty) < 0);
//        }
//
//        [TestMethod]
//        public void StringUtility_SafeContains_1()
//        {
//            Assert.IsFalse(StringUtility.SafeContains(string.Empty, "aga"));
//            Assert.IsFalse(StringUtility.SafeContains("Again", (string)null));
//            Assert.IsFalse(StringUtility.SafeContains(null, (string)null));
//            Assert.IsTrue(StringUtility.SafeContains("Again", "aga"));
//        }
//
//        [TestMethod]
//        public void StringUtility_SafeContains_2()
//        {
//            Assert.IsFalse(StringUtility.SafeContains(string.Empty, "aga", "gain"));
//            Assert.IsFalse(StringUtility.SafeContains("Again"));
//            Assert.IsTrue(StringUtility.SafeContains("Again", "aga", "GAIN"));
//            Assert.IsTrue(StringUtility.SafeContains("Again", "oga", "GAIN"));
//        }

        [TestMethod]
        public void StringUtility_SafeStarts_1()
        {
            Assert.IsFalse(StringUtility.SafeStarts(string.Empty, "aga"));
            Assert.IsFalse(StringUtility.SafeStarts("Again", string.Empty));
            Assert.IsTrue(StringUtility.SafeStarts("Again", "aga"));
        }

        [TestMethod]
        public void StringUtility_SafeSubstring_1()
        {
            Assert.AreEqual("Aga", StringUtility.SafeSubstring("Again", 0, 3));
            Assert.AreEqual("Again", StringUtility.SafeSubstring("Again", 0, 300));
            Assert.AreEqual(null, StringUtility.SafeSubstring(null, 0, 300));
            Assert.AreEqual(string.Empty, StringUtility.SafeSubstring("", 0, 300));
            Assert.AreEqual(string.Empty, StringUtility.SafeSubstring("Again", 0, -1));
            Assert.AreEqual(string.Empty, StringUtility.SafeSubstring("Again", 500, 300));
        }

        [TestMethod]
        public void StringUtility_SameString_1()
        {
            Assert.IsTrue(StringUtility.SameString(string.Empty, string.Empty));
            Assert.IsTrue(StringUtility.SameString(" ", " "));
            Assert.IsTrue(StringUtility.SameString("Hello", "HELLO"));
            Assert.IsFalse(StringUtility.SameString("Hello", "HELLO2"));
        }

        [TestMethod]
        public void StringUtility_SameStringSensitive_1()
        {
            Assert.IsTrue(StringUtility.SameStringSensitive(string.Empty, string.Empty));
            Assert.IsTrue(StringUtility.SameStringSensitive(" ", " "));
            Assert.IsTrue(StringUtility.SameStringSensitive("Hello", "Hello"));
            Assert.IsFalse(StringUtility.SameStringSensitive("Hello", "HELLO"));
            Assert.IsFalse(StringUtility.SameStringSensitive("Hello", "HELLO2"));
        }

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
            Assert.AreEqual("a", StringUtility.ToVisibleString("a"));
            Assert.AreEqual("(null)", StringUtility.ToVisibleString(null));
            Assert.AreEqual("(empty)", StringUtility.ToVisibleString(string.Empty));
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
