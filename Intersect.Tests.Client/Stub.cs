using Intersect.Client.Utilities;
using NUnit.Framework;

namespace Intersect.Tests.Client
{

    [TestFixture]
    public partial class Stub
    {

        [Test]
        public void TestStub()
        {
            // Needed so NUnit doesn't return -2
            Assert.That(0, Is.EqualTo(MathHelper.Lerp(0, 0, 0)));
        }

    }

}
