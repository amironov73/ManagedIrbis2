using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AM.IO;
using AM.Runtime;

namespace UnitTests.AM.IO
{
    [TestClass]
    public class FileUtilityTest
    {
        [TestMethod]
        public void FileUtility_FindFileInPath()
        {
            var shellName = RuntimeUtility.IsWindows
                ? "cmd.exe"
                : "sh";
            var delimiter = RuntimeUtility.IsWindows
                ? ';'
                : ':';
            var path = Environment.GetEnvironmentVariable("PATH");
            var found = FileUtility.FindFileInPath
                (
                    shellName,
                    path,
                    delimiter
                );
            Assert.IsTrue(!string.IsNullOrEmpty(found)
                          || found is null);
        }
    }
}
