using System;
using System.Collections;
using System.Diagnostics;

using Intersect.Utilities;

using Moq;

using NUnit.Framework;

namespace Intersect.Plugins.Manifests.Types
{
    [TestFixture]
    public class AuthorsTests
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
        public void AreEqual()
        {
            var authors = new Authors(new Author(AuthorName), new Author(AuthorStringNameEmail));
            var authorArray = new[] {new Author(AuthorName), new Author(AuthorStringNameEmail)};
            Assert.AreEqual(authors, new Authors(authorArray));
        }

        [Test]
        public void AreEqual_Operator()
        {
            var authors = new Authors(new Author(AuthorName), new Author(AuthorStringNameEmail));
            var authorArray = new[] {new Author(AuthorName), new Author(AuthorStringNameEmail)};
            Assert.IsTrue(authors == new Authors(authorArray));
        }

        [Test]
        public void AreEqual_Operator_MixedType()
        {
            var authors = new Authors(new Author(AuthorName), new Author(AuthorStringNameEmail));
            var authorArray = new[] {new Author(AuthorName), new Author(AuthorStringNameEmail)};
            Assert.IsTrue(authors == authorArray);
        }

        [Test]
        public void AreEqual_String()
        {
            var authors = new Authors(new Author(AuthorName), new Author(AuthorStringNameEmail));
            Assert.IsTrue(authors.Equals($"[\"{AuthorName}\", \"{AuthorStringNameEmail}\"]"));
        }

        [Test]
        public void AreEqual_StringArray()
        {
            var authors = new Authors(new Author(AuthorName), new Author(AuthorStringNameEmail));
            Assert.IsTrue(authors.Equals(new[] {AuthorName, AuthorStringNameEmail}));
        }

        [Test]
        public void AreNotEqual()
        {
            var authors = new Authors(new Author(AuthorName), new Author(AuthorStringNameEmail));
            var authorArray = new[] {new Author(AuthorName), new Author(AuthorStringNameEmailHomepage)};
            Assert.AreNotEqual(authors, new Authors(authorArray));
        }

        [Test]
        public void AreNotEqual_Operator()
        {
            var authors = new Authors(new Author(AuthorName), new Author(AuthorStringNameEmail));
            var authorArray = new[] {new Author(AuthorName), new Author(AuthorName)};
            Assert.IsTrue(authors != new Authors(authorArray));
        }

        [Test]
        public void AreNotEqual_Operator_MixedType()
        {
            var authors = new Authors(new Author(AuthorName), new Author(AuthorStringNameEmail));
            var authorArray = new[] {new Author(AuthorName), new Author(AuthorStringNameEmailHomepage)};
            Assert.IsTrue(authors != authorArray);
        }

        [Test]
        public void Clone()
        {
            ICloneable authors = new Authors(AuthorStringNameEmailHomepage, AuthorName, AuthorStringNameEmail);
            if (authors.Clone() is Authors clone)
            {
                Assert.IsTrue(AuthorStringNameEmailHomepage == clone[0]);
                Assert.IsTrue(AuthorName == clone[1]);
                Assert.IsTrue(AuthorStringNameEmail == clone[2]);
            }
            else
            {
                Assert.Fail($"{nameof(clone)} is not an instance of Authors.");
            }
        }

        [Test]
        public void CompareTo_IEnumerableAuthor()
        {
            var authors = new Authors(AuthorName, AuthorStringNameEmailHomepage);
            object objectAuthors = new Authors(AuthorName, AuthorStringNameEmailHomepage);
            object arrayAuthors = new Author[] {AuthorName, AuthorStringNameEmailHomepage};
            object arrayAuthorStrings = new[] {AuthorName, AuthorStringNameEmailHomepage};

            Assert.AreEqual(0, authors.CompareTo(objectAuthors));
            Assert.AreEqual(0, authors.CompareTo(arrayAuthors));
            Assert.AreEqual(0, authors.CompareTo(arrayAuthorStrings));
        }

        [Test]
        public void CompareTo_object()
        {
            var authors = new Authors(AuthorName, AuthorStringNameEmailHomepage);
            var objectBare = new object();

            Assert.Throws<NotSupportedException>(

                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                () => authors.CompareTo(objectBare), Authors.ExceptionComparisonNotSupported
            );

            Assert.Throws<NotSupportedException>(

                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                // ReSharper disable once ExpressionIsAlwaysNull
                () => authors.CompareTo((object) null), Authors.ExceptionComparisonNotSupported
            );
        }

        [Test]
        public void CompareTo_object_IComparer()
        {
            var authors = new Authors(AuthorName, AuthorStringNameEmailHomepage);
            object objectAuthors = new Authors(AuthorName, AuthorStringNameEmailHomepage);

            var mockComparer = new Mock<IComparer>();
            mockComparer.Setup(setupComparer => setupComparer.Compare(It.IsAny<object>(), It.IsAny<object>()))
                ?.Returns<object, object>((a, b) => (a?.GetHashCode() ?? 0) ^ (b?.GetHashCode() ?? 0));

            Debug.Assert(mockComparer.Object != null, "mockComparer.Object != null");
            Assert.AreEqual(0, authors.CompareTo(objectAuthors, mockComparer.Object));
        }

        [Test]
        public void Count()
        {
            var authors = new Authors(new Author(AuthorName), new Author(AuthorStringNameEmail));
            Assert.AreEqual(2, authors.Count);
        }

        [Test]
        public void Equals_object()
        {
            var authors = new Authors(AuthorName, AuthorStringNameEmailHomepage);
            object objectAuthors = new Authors(AuthorName, AuthorStringNameEmailHomepage);
            object arrayAuthors = new Author[] {AuthorName, AuthorStringNameEmailHomepage};
            object arrayAuthorStrings = new[] {AuthorName, AuthorStringNameEmailHomepage};
            var objectBare = new object();

            Assert.IsTrue(authors.Equals(objectAuthors));
            Assert.IsTrue(authors.Equals(arrayAuthors));
            Assert.IsTrue(authors.Equals(arrayAuthorStrings));

            Assert.Throws<NotSupportedException>(

                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                () => authors.Equals(objectBare), Authors.ExceptionComparisonNotSupported
            );

            Assert.Throws<NotSupportedException>(

                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                // ReSharper disable once ExpressionIsAlwaysNull
                () => authors.Equals((object) null), Authors.ExceptionComparisonNotSupported
            );
        }

        [Test]
        public void Equals_object_IEqualityComparer()
        {
            var authors = new Authors(AuthorName, AuthorStringNameEmailHomepage);
            object objectAuthors = new Authors(AuthorName, AuthorStringNameEmailHomepage);

            var mockComparer = new Mock<IEqualityComparer>();
            mockComparer.Setup(setupComparer => setupComparer.Equals(It.IsAny<object>(), It.IsAny<object>()))
                ?.Returns<object, object>((a, b) => a?.Equals(b) ?? false);

            Debug.Assert(mockComparer.Object != null, "mockComparer.Object != null");
            Assert.IsTrue(authors.Equals(objectAuthors, mockComparer.Object));
        }

        [Test]
        public void GetEnumerator()
        {
            IEnumerable enumerable = new Authors(AuthorName, AuthorStringNameEmail);
            var enumerator = enumerable.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(AuthorName, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(AuthorStringNameEmail, enumerator.Current);
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void GetEnumerator_Author()
        {
            using (var enumerator = new Authors(AuthorName, AuthorStringNameEmail).GetEnumerator())
            {
                Assert.IsTrue(enumerator.MoveNext());
                Assert.AreEqual(AuthorName, enumerator.Current);
                Assert.IsTrue(enumerator.MoveNext());
                Assert.AreEqual(AuthorStringNameEmail, enumerator.Current);
                Assert.IsFalse(enumerator.MoveNext());
            }
        }

        [Test]
        public void GetHashCode_default()
        {
            var authors = new Authors(AuthorName, AuthorStringNameEmail);
            var hashCode = ValueUtils.ComputeHashCode(new Author[] {AuthorName, AuthorStringNameEmail});
            Assert.AreEqual(hashCode, authors.GetHashCode());
        }

        [Test]
        public void GetHashCode_IEqualityComparer()
        {
            var mockComparer = new Mock<IEqualityComparer>();
            mockComparer.Setup(setupComparer => setupComparer.GetHashCode(It.IsAny<object>()))
                ?.Returns<object>(a => a is Authors authors ? authors.GetHashCode() : a?.GetHashCode() ?? 0);

            var comparer = mockComparer.Object;
            Debug.Assert(comparer != null, "comparer != null");

            Assert.AreEqual(
                ValueUtils.ComputeHashCode(new Author[] {AuthorName, AuthorStringNameEmail}),
                new Authors(AuthorName, AuthorStringNameEmail).GetHashCode(comparer)
            );
        }

        [Test]
        public void Item()
        {
            var authors = new Authors(new Author(AuthorName), new Author(AuthorStringNameHomepage));
            Assert.AreEqual((Author) AuthorName, authors[0]);
            Assert.AreEqual((Author) AuthorStringNameHomepage, authors[1]);
        }

        [Test]
        public void Operator_ImplicitFromString()
        {
            Author author = AuthorName;
            Authors authors = AuthorName;

            Assert.IsTrue(AuthorName == author);

            Assert.IsTrue(authors == AuthorName);

            Assert.IsTrue(authors == author);
        }

        [Test]
        public void Operator_ImplicitFromStringArray()
        {
            var authors = (Authors) new[] {AuthorStringNameEmailHomepage, AuthorName, AuthorStringNameEmail};

            Assert.AreEqual(3, authors.Count);
            Assert.IsTrue(AuthorStringNameEmailHomepage == authors[0]);
            Assert.IsTrue(AuthorName == authors[1]);
            Assert.IsTrue(AuthorStringNameEmail == authors[2]);
        }

        [Test]
        public void Operator_ImplicitToString()
        {
            Authors authors = new[] {AuthorStringNameEmailHomepage, AuthorName, AuthorStringNameEmail};
            Assert.AreEqual(
                $"[\"{AuthorStringNameEmailHomepage}\",\"{AuthorName}\",\"{AuthorStringNameEmail}\"]", (string) authors
            );
        }

        [Test]
        public void ToString_Array()
        {
            Authors authors = new[] {AuthorStringNameEmailHomepage, AuthorName, AuthorStringNameEmail};
            Assert.AreEqual(
                $"[\"{AuthorStringNameEmailHomepage}\",\"{AuthorName}\",\"{AuthorStringNameEmail}\"]",
                authors.ToString()
            );
        }

        #endregion Tests
    }
}
