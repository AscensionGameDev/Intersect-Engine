using NUnit.Framework;

using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Intersect.Utilities
{

    [TestFixture]
    public class TextUtilsTests
    {

        [SetUp]
        public void SaveState()
        {
            mNone = TextUtils.None;
        }

        [TearDown]
        public void ResetState()
        {
            TextUtils.None = mNone;
        }

        private string mNone;

        [Test]
        public void IsNoneTest()
        {
            Assert.IsTrue(TextUtils.IsNone(null));
            Assert.IsTrue(TextUtils.IsNone(""));
            Assert.IsTrue(TextUtils.IsNone("   "));
            Assert.IsTrue(TextUtils.IsNone("none"));
            Assert.IsTrue(TextUtils.IsNone("nOnE"));
            Assert.IsTrue(TextUtils.IsNone(" n O n E "));
            Assert.IsTrue(TextUtils.IsNone("None"));
            Assert.IsTrue(TextUtils.IsNone("NONE"));
            Assert.IsTrue(TextUtils.IsNone(" N ONE   "));
            Assert.IsTrue(TextUtils.IsNone(TextUtils.None));
            Assert.IsFalse(TextUtils.IsNone(" N e SSu nO "));
            Assert.IsFalse(TextUtils.IsNone("Nessuno"));
            Assert.IsFalse(TextUtils.IsNone("nessuno"));
            Assert.IsFalse(TextUtils.IsNone("NotNone"));

            TextUtils.None = "Nessuno";

            Assert.IsTrue(TextUtils.IsNone(" N e SSu nO "));
            Assert.IsTrue(TextUtils.IsNone(TextUtils.None));
            Assert.IsFalse(TextUtils.IsNone("NotNone"));
        }

        [Test]
        public void NullToNoneTest()
        {
            Assert.AreEqual("None", TextUtils.NullToNone(null));
            Assert.AreEqual(TextUtils.None, TextUtils.NullToNone(null));
            Assert.AreEqual(TextUtils.None, TextUtils.NullToNone(" n o n e "));
            Assert.AreNotEqual(TextUtils.None, TextUtils.NullToNone("Nessuno"));
            Assert.AreEqual("Nessuno", TextUtils.NullToNone("Nessuno"));
            Assert.AreEqual("Intersect", TextUtils.NullToNone("Intersect"));

            TextUtils.None = "Nessuno";

            Assert.AreEqual("Nessuno", TextUtils.NullToNone(null));
            Assert.AreEqual(TextUtils.None, TextUtils.NullToNone(null));
            Assert.AreEqual(TextUtils.None, TextUtils.NullToNone(" n o n e "));
            Assert.AreEqual(TextUtils.None, TextUtils.NullToNone("Nessuno"));
            Assert.AreEqual("Nessuno", TextUtils.NullToNone("Nessuno"));
            Assert.AreEqual("Intersect", TextUtils.NullToNone("Intersect"));
        }

        [Test]
        public void SanitizeNoneTest()
        {
            Assert.IsNull(TextUtils.SanitizeNone(null));
            Assert.IsNull(TextUtils.SanitizeNone(" n o n e "));
            Assert.IsNotNull(TextUtils.SanitizeNone("Nessuno"));
            Assert.AreEqual("Nessuno", TextUtils.SanitizeNone("Nessuno"));
            Assert.AreEqual("Intersect", TextUtils.SanitizeNone("Intersect"));

            TextUtils.None = "Nessuno";

            Assert.IsNull(TextUtils.SanitizeNone(null));
            Assert.IsNull(TextUtils.SanitizeNone(" n o n e "));
            Assert.IsNull(TextUtils.SanitizeNone("Nessuno"));
            Assert.IsNull(TextUtils.SanitizeNone("Nessuno"));
            Assert.AreEqual("Intersect", TextUtils.SanitizeNone("Intersect"));
        }

        [Test]
        public void StaticConstructor()
        {
            Assert.AreEqual("None", TextUtils.None);
        }

        [Test]
        public void StripToLowerTest()
        {
            Assert.AreEqual("test", TextUtils.StripToLower("test"));
            Assert.AreEqual("test", TextUtils.StripToLower("Test"));
            Assert.AreEqual("test", TextUtils.StripToLower("TeSt"));
            Assert.AreEqual("test", TextUtils.StripToLower("TEST"));
            Assert.AreEqual("test", TextUtils.StripToLower(" T e S t "));
        }

    }

}
