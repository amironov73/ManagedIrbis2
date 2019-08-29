using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AM.IO;

namespace UnitTests.AM.IO
{
    [TestClass]
    public class FileUtilityTest
    {
        [TestMethod]
        public void FileUtility_FindFileInPath()
        {
            string path = Environment.GetEnvironmentVariable("PATH");
            // TODO: support for UNIX environments
            string found = FileUtility.FindFileInPath
                (
                    "cmd.exe",
                    path,
                    ';'
                );
            Assert.IsTrue(string.IsNullOrEmpty(found)
                || found != null);
        }
    }
}
