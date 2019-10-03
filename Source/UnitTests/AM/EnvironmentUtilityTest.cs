using AM;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.AM
{
    [TestClass]
    public class EnvironmentUtilityTest
    {
        [TestMethod]
        public void EnvironmentUtility_NetCoreVersion_1()
        {
            var version = EnvironmentUtility.NetCoreVersion();
            Assert.IsNotNull(version);
        }

        [TestMethod]
        public void EnvironmentUtility_Uptime_1()
        {
            var uptime = EnvironmentUtility.Uptime;
            Assert.IsTrue(uptime.Ticks >= 0);
        }
    }
}