﻿using NUnit.Framework;

namespace Intersect.Tests.Network
{

    [TestFixture]
    public partial class Stub
    {

        [Test]
        public void TestStub()
        {
            // Needed so NUnit doesn't return -2
            // TODO: Remove this when there are actual tests in this assembly
            Assert.AreEqual(0, 0);
        }

    }

}
