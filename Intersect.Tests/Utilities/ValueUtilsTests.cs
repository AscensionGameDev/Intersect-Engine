using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Intersect.Utilities
{
    [TestClass]
    public class ValueUtilsTests
    {
        [TestMethod]
        public void SwapTest()
        {
            string a = "Test", b = "Unit";
            Assert.AreEqual("Unit", b);
            Assert.AreNotEqual("Unit", a);
            Assert.AreEqual("Test", a);
            Assert.AreNotEqual("Test", b);
            ValueUtils.Swap(ref a, ref b);
            Assert.AreEqual("Unit", a);
            Assert.AreNotEqual("Unit", b);
            Assert.AreEqual("Test", b);
            Assert.AreNotEqual("Test", a);
        }
    }
}