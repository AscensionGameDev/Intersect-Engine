using Intersect.Client.Utilities;

using NUnit.Framework;

namespace Intersect.Tests.Client
{

    [TestFixture]
    public class Stub
    {

        [Test]
        public void TestStub()
        {
            // Needed so NUnit doesn't return -2
            Assert.AreEqual(0, MathHelper.Lerp(0, 0, 0));
        }

    }

}
