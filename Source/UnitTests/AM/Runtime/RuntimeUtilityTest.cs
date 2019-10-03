using AM.Runtime;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.AM.Runtime
{
    [TestClass]
    public class RuntimeUtilityTest
    {
        [TestMethod]
        public void RuntimeUtility_FrameworkLocation_1()
        {
            var value = RuntimeUtility.FrameworkLocation;
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void RuntimeUtility_ExecutableFileName_1()
        {
            var value = RuntimeUtility.ExecutableFileName;
            Assert.IsNotNull(value);
        }
    }
}