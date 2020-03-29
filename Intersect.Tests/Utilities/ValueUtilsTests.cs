using NUnit.Framework;

using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Intersect.Utilities
{

    [TestFixture]
    public class ValueUtilsTests
    {

        [Test]
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
