using AM;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.AM
{
    [TestClass]
    public class UnsafeUtilityTest
    {
        [TestMethod]
        public void UnsafeUtility_AsSpan_1()
        {
            var value = 0;
            var span = UnsafeUtility.AsSpan(ref value);
            //Assert.AreEqual(Marshal.SizeOf(typeof(int)), span.Length);
            Assert.AreEqual((byte)0, span[0]);
        }
    }
}
