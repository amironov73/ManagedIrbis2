using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using AM.IO;

namespace UnitTests.AM.IO
{
    [TestClass]
    public class NonCloseableTextReaderTest
        : Common.CommonUnitTest
    {
        //[NotNull]
        //private string _GetFileName()
        //{
        //    string result = Path.Combine(TestDataPath, "record.txt");

        //    return result;
        //}

        [TestMethod]
        public void NonCloseableTextReader_Construction_1()
        {
            TextReader textReader = new StringReader("hello");
            NonCloseableTextReader reader = new NonCloseableTextReader(textReader);
            reader.Dispose();
        }
    }
}
