using System;
using System.IO;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using AM.IO;

using JetBrains.Annotations;

using Moq;

namespace UnitTests.AM.IO
{
    [TestClass]
    public class NonCloseableTextReaderTest
        : Common.CommonUnitTest
    {
        [NotNull]
        private string _GetFileName()
        {
            string result = Path.Combine(TestDataPath, "record.txt");

            return result;
        }

        [TestMethod]
        public void NonCloseableTextReader_Construction_1()
        {
            Stream stream = Stream.Null;
            TextReader textReader = new StringReader("hello");
            NonCloseableTextReader reader = new NonCloseableTextReader(textReader);
            reader.Dispose();
        }
    }
}
