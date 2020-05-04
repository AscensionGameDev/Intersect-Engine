using Intersect.Utilities;

using NUnit.Framework;

using System;
using System.Diagnostics.CodeAnalysis;

namespace Intersect.Plugins.Manifests.Types
{
    [TestFixture]
    public class AuthorTests
    {
        #region Test Constants

        private const string AuthorEmail = "test@email.com";

        private const string AuthorHomepage = "http://example.com";

        private const string AuthorName = "Test Author";

        private static readonly string AuthorStringNameEmail = $"{AuthorName} <{AuthorEmail}>";

        private static readonly string AuthorStringNameEmailHomepage =
            $"{AuthorName} <{AuthorEmail}> ({AuthorHomepage})";

        private static readonly string AuthorStringNameHomepage = $"{AuthorName} ({AuthorHomepage})";

        #endregion Test Constants

        #region Tests

        [Test]
        public void AreEqual_Operator()
        {
            var author = new Author(AuthorStringNameEmailHomepage);
            Assert.IsTrue(author == new Author(AuthorStringNameEmailHomepage));
        }

        [Test]
        public void AreNotEqual_Operator()
        {
            var author = new Author(AuthorStringNameEmailHomepage);
            Assert.IsTrue(author != new Author(AuthorName));
        }

        [Test]
        public void AuthorFromParts()
        {
            var author = new Author(AuthorName, AuthorEmail, AuthorHomepage);
            Assert.AreEqual(AuthorName, author.Name);
            Assert.AreEqual(AuthorEmail, author.Email);
            Assert.AreEqual(AuthorHomepage, author.Homepage);
        }

        [Test]
        public void AuthorFromPartsWithoutHomepage()
        {
            var author = new Author(AuthorName, AuthorEmail);
            Assert.AreEqual(AuthorName, author.Name);
            Assert.AreEqual(AuthorEmail, author.Email);
            Assert.IsEmpty(author.Homepage);
        }

        [Test]
        public void AuthorPaddedWhitespace()
        {
            var author = new Author($" \t {AuthorName} \t <{AuthorEmail}> \t ({AuthorHomepage}) \t ");
            Assert.AreEqual(AuthorName, author.Name);
            Assert.AreEqual(AuthorEmail, author.Email);
            Assert.AreEqual(AuthorHomepage, author.Homepage);
        }

        [Test]
        public void AuthorWithEmail()
        {
            var author = new Author(AuthorStringNameEmail);
            Assert.AreEqual(AuthorName, author.Name);
            Assert.AreEqual(AuthorEmail, author.Email);
            Assert.IsEmpty(author.Homepage);
        }

        [Test]
        public void AuthorWithEmailAndHomepage()
        {
            var author = new Author(AuthorStringNameEmailHomepage);
            Assert.AreEqual(AuthorName, author.Name);
            Assert.AreEqual(AuthorEmail, author.Email);
            Assert.AreEqual(AuthorHomepage, author.Homepage);
        }

        [Test]
        public void AuthorWithHomepage()
        {
            var author = new Author(AuthorStringNameHomepage);
            Assert.AreEqual(AuthorName, author.Name);
            Assert.IsEmpty(author.Email);
            Assert.AreEqual(AuthorHomepage, author.Homepage);
        }

        [Test]
        public void BasicAuthor()
        {
            var author = new Author(AuthorName);
            Assert.AreEqual(AuthorName, author.Name);
            Assert.IsEmpty(author.Email);
            Assert.IsEmpty(author.Homepage);
        }

        [Test]
        public void Clone()
        {
            var author = new Author(AuthorName, AuthorEmail, AuthorHomepage);
            var clone = author.Clone();
            Assert.AreEqual(AuthorName, clone.Name);
            Assert.AreEqual(AuthorEmail, clone.Email);
            Assert.AreEqual(AuthorHomepage, clone.Homepage);
        }

        [Test]
        public void CompareTo_Name_NameEmailHomepage()
        {
            var author = new Author(AuthorName);
            Assert.AreEqual(-1, author.CompareTo(AuthorStringNameEmailHomepage));
        }

        [Test]
        public void CompareTo_NameEmailHomepage_NameEmailHomepage()
        {
            var author = new Author(AuthorStringNameEmailHomepage);
            Assert.AreEqual(0, author.CompareTo(AuthorStringNameEmailHomepage));
        }

        [Test]
        public void CompareTo_Null()
        {
            var author = new Author(AuthorName);
            Assert.AreEqual(1, author.CompareTo(null));
        }

        [Test]
        public void CompareTo_Object()
        {
            var author = new Author(AuthorName);

            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws(
                typeof(NotSupportedException), () => author.CompareTo(new object()),
                "Comparison only supported with Author."
            );
        }

        [Test]
        public void Equals_Author_Equal()
        {
            var author = new Author(AuthorStringNameEmailHomepage);
            Assert.IsTrue(author.Equals(new Author(AuthorStringNameEmailHomepage)));
        }

        [Test]
        public void Equals_Author_NotEqual()
        {
            var author = new Author(AuthorStringNameEmailHomepage);
            Assert.IsFalse(author.Equals(new Author(AuthorName)));
        }

        [Test]
        public void Equals_Null()
        {
            var author = new Author(AuthorStringNameEmailHomepage);
            Assert.IsFalse(author.Equals((object) null));
        }

        [Test]
        public void Equals_Null_Operator()
        {
            var author = new Author(AuthorStringNameEmailHomepage);
            Assert.IsFalse(author == null);
        }

        [Test]
        public void Equals_Object()
        {
            var author = new Author(AuthorStringNameEmailHomepage);
            Assert.IsFalse(author.Equals(new object()));
        }

        [Test]
        public void HashCode_AuthorString_Name()
        {
            var author = new Author(AuthorName);
            var hashCode = ValueUtils.ComputeHashCode(AuthorName, string.Empty, string.Empty);
            Assert.AreEqual(hashCode, author.GetHashCode());
        }

        [Test]
        public void HashCode_AuthorString_NameEmail()
        {
            var author = new Author(AuthorStringNameEmail);
            var hashCode = ValueUtils.ComputeHashCode(AuthorName, AuthorEmail, string.Empty);
            Assert.AreEqual(hashCode, author.GetHashCode());
        }

        [Test]
        public void HashCode_AuthorString_NameEmailHomepage()
        {
            var author = new Author(AuthorStringNameEmailHomepage);
            var hashCode = ValueUtils.ComputeHashCode(AuthorName, AuthorEmail, AuthorHomepage);
            Assert.AreEqual(hashCode, author.GetHashCode());
        }

        [Test]
        public void HashCode_AuthorString_NameHomepage()
        {
            var author = new Author(AuthorStringNameHomepage);
            var hashCode = ValueUtils.ComputeHashCode(AuthorName, string.Empty, AuthorHomepage);
            Assert.AreEqual(hashCode, author.GetHashCode());
        }

        [Test]
        public void HashCode_Parts()
        {
            var author = new Author(AuthorName, AuthorEmail, AuthorHomepage);
            var hashCode = ValueUtils.ComputeHashCode(AuthorName, AuthorEmail, AuthorHomepage);
            Assert.AreEqual(hashCode, author.GetHashCode());
        }

        [Test]
        public void HashCode_Parts_WithoutHomepage()
        {
            var author = new Author(AuthorName, AuthorEmail);
            var hashCode = ValueUtils.ComputeHashCode(AuthorName, AuthorEmail, string.Empty);
            Assert.AreEqual(hashCode, author.GetHashCode());
        }

        [Test]
        public void ICloneable_Clone()
        {
            ICloneable cloneable = new Author(AuthorName, AuthorEmail, AuthorHomepage);
            var clone = cloneable.Clone();
            if (clone is Author author)
            {
                Assert.AreEqual(AuthorName, author.Name);
                Assert.AreEqual(AuthorEmail, author.Email);
                Assert.AreEqual(AuthorHomepage, author.Homepage);
            }
            else
            {
                Assert.Fail("Clone is not an author.");
            }
        }

        [Test]
        [SuppressMessage(
            "Style", "IDE0022:Use expression body for methods",
            Justification = "Tests should have a proper method body."
        )]
        public void InvalidAuthorEmpty()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws(typeof(ArgumentNullException), () => new Author(""), "Author string is null or whitespace.");
        }

        [Test]
        [SuppressMessage(
            "Style", "IDE0022:Use expression body for methods",
            Justification = "Tests should have a proper method body."
        )]
        public void InvalidAuthorNull()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws(
                typeof(ArgumentNullException), () => new Author(null), "Author string is null or whitespace."
            );
        }

        [Test]
        public void Operator_ImplicitFromString()
        {
            Author author = AuthorStringNameEmailHomepage;
            Assert.AreEqual(AuthorName, author.Name);
            Assert.AreEqual(AuthorEmail, author.Email);
            Assert.AreEqual(AuthorHomepage, author.Homepage);
        }

        [Test]
        public void Operator_ImplicitToString()
        {
            string authorString = new Author(AuthorName, AuthorEmail, AuthorHomepage);
            Assert.AreEqual(AuthorStringNameEmailHomepage, authorString);
        }

        [Test]
        public void ToString_Name()
        {
            var author = new Author(AuthorName);
            Assert.AreEqual(AuthorName, author.ToString());
        }

        [Test]
        public void ToString_NameEmail()
        {
            var author = new Author(AuthorName, AuthorEmail);
            Assert.AreEqual(AuthorStringNameEmail, author.ToString());
        }

        [Test]
        public void ToString_NameEmailHomepage()
        {
            var author = new Author(AuthorName, AuthorEmail, AuthorHomepage);
            Assert.AreEqual(AuthorStringNameEmailHomepage, author.ToString());
        }

        [Test]
        public void ToString_NameHomepage()
        {
            var author = new Author(AuthorName, string.Empty, AuthorHomepage);
            Assert.AreEqual(AuthorStringNameHomepage, author.ToString());
        }

        #endregion Tests
    }
}
