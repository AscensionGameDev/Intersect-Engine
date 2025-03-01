using System.Diagnostics.CodeAnalysis;
using Intersect.Utilities;
using NUnit.Framework;

namespace Intersect.Plugins.Manifests.Types;

[TestFixture]
public partial class AuthorTests
{
    private const string AuthorEmail = "test@email.com";

    private const string AuthorHomepage = "http://example.com";

    private const string AuthorName = "Test Author";

    private const string AuthorStringNameEmail = $"{AuthorName} <{AuthorEmail}>";

    private const string AuthorStringNameEmailHomepage = $"{AuthorName} <{AuthorEmail}> ({AuthorHomepage})";

    private const string AuthorStringNameHomepage = $"{AuthorName} ({AuthorHomepage})";

    [Test]
    public void AreEqual_Operator()
    {
        var author = new Author(AuthorStringNameEmailHomepage);
        Assert.That(author, Is.EqualTo(new Author(AuthorStringNameEmailHomepage)));
    }

    [Test]
    public void AreNotEqual_Operator()
    {
        var author = new Author(AuthorStringNameEmailHomepage);
        Assert.That(author, Is.Not.EqualTo(new Author(AuthorName)));
    }

    [Test]
    public void AuthorFromParts()
    {
        var author = new Author(AuthorName, AuthorEmail, AuthorHomepage);
        Assert.Multiple(
            () =>
            {
                Assert.That(author.Name, Is.EqualTo(AuthorName));
                Assert.That(author.Email, Is.EqualTo(AuthorEmail));
                Assert.That(author.Homepage, Is.EqualTo(AuthorHomepage));
            }
        );
    }

    [Test]
    public void AuthorFromPartsWithoutHomepage()
    {
        var author = new Author(AuthorName, AuthorEmail);
        Assert.Multiple(
            () =>
            {
                Assert.That(author.Name, Is.EqualTo(AuthorName));
                Assert.That(author.Email, Is.EqualTo(AuthorEmail));
                Assert.That(author.Homepage, Is.Empty);
            }
        );
    }

    [Test]
    public void AuthorPaddedWhitespace()
    {
        var author = new Author($" \t {AuthorName} \t <{AuthorEmail}> \t ({AuthorHomepage}) \t ");
        Assert.Multiple(
            () =>
            {
                Assert.That(author.Name, Is.EqualTo(AuthorName));
                Assert.That(author.Email, Is.EqualTo(AuthorEmail));
                Assert.That(author.Homepage, Is.EqualTo(AuthorHomepage));
            }
        );
    }

    [Test]
    public void AuthorWithEmail()
    {
        var author = new Author(AuthorStringNameEmail);
        Assert.Multiple(
            () =>
            {
                Assert.That(author.Name, Is.EqualTo(AuthorName));
                Assert.That(author.Email, Is.EqualTo(AuthorEmail));
                Assert.That(author.Homepage, Is.Empty);
            }
        );
    }

    [Test]
    public void AuthorWithEmailAndHomepage()
    {
        var author = new Author(AuthorStringNameEmailHomepage);
        Assert.Multiple(
            () =>
            {
                Assert.That(author.Name, Is.EqualTo(AuthorName));
                Assert.That(author.Email, Is.EqualTo(AuthorEmail));
                Assert.That(author.Homepage, Is.EqualTo(AuthorHomepage));
            }
        );
    }

    [Test]
    public void AuthorWithHomepage()
    {
        var author = new Author(AuthorStringNameHomepage);
        Assert.Multiple(
            () =>
            {
                Assert.That(author.Name, Is.EqualTo(AuthorName));
                Assert.That(author.Email, Is.Empty);
                Assert.That(author.Homepage, Is.EqualTo(AuthorHomepage));
            }
        );
    }

    [Test]
    public void BasicAuthor()
    {
        var author = new Author(AuthorName);
        Assert.Multiple(
            () =>
            {
                Assert.That(author.Name, Is.EqualTo(AuthorName));
                Assert.That(author.Email, Is.Empty);
                Assert.That(author.Homepage, Is.Empty);
            }
        );
    }

    [Test]
    public void Clone()
    {
        var author = new Author(AuthorName, AuthorEmail, AuthorHomepage);
        var clone = author.Clone();
        Assert.Multiple(
            () =>
            {
                Assert.That(clone.Name, Is.EqualTo(AuthorName));
                Assert.That(clone.Email, Is.EqualTo(AuthorEmail));
                Assert.That(clone.Homepage, Is.EqualTo(AuthorHomepage));
            }
        );
    }

    [Test]
    public void CompareTo_Name_NameEmailHomepage()
    {
        var author = new Author(AuthorName);
        Assert.That(author.CompareTo(AuthorStringNameEmailHomepage), Is.Negative);
    }

    [Test]
    public void CompareTo_NameEmailHomepage_NameEmailHomepage()
    {
        var author = new Author(AuthorStringNameEmailHomepage);
        Assert.That(author.CompareTo(AuthorStringNameEmailHomepage), Is.Zero);
    }

    [Test]
    public void CompareTo_Null()
    {
        var author = new Author(AuthorName);
        Assert.That(author.CompareTo(null), Is.Positive);
    }

    [Test]
    public void CompareTo_Object()
    {
        var author = new Author(AuthorName);

        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
        Assert.Throws(
            typeof(NotSupportedException),
            () => author.CompareTo(new object()),
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
        Assert.That(author, Is.Not.EqualTo(null));
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
        var hashCode = HashCode.Combine(AuthorName, string.Empty, string.Empty);
        Assert.That(author.GetHashCode(), Is.EqualTo(hashCode));
    }

    [Test]
    public void HashCode_AuthorString_NameEmail()
    {
        var author = new Author(AuthorStringNameEmail);
        var hashCode = HashCode.Combine(AuthorName, AuthorEmail, string.Empty);
        Assert.That(author.GetHashCode(), Is.EqualTo(hashCode));
    }

    [Test]
    public void HashCode_AuthorString_NameEmailHomepage()
    {
        var author = new Author(AuthorStringNameEmailHomepage);
        var hashCode = HashCode.Combine(AuthorName, AuthorEmail, AuthorHomepage);
        Assert.That(author.GetHashCode(), Is.EqualTo(hashCode));
    }

    [Test]
    public void HashCode_AuthorString_NameHomepage()
    {
        var author = new Author(AuthorStringNameHomepage);
        var hashCode = HashCode.Combine(AuthorName, string.Empty, AuthorHomepage);
        Assert.That(author.GetHashCode(), Is.EqualTo(hashCode));
    }

    [Test]
    public void HashCode_Parts()
    {
        var author = new Author(AuthorName, AuthorEmail, AuthorHomepage);
        var hashCode = HashCode.Combine(AuthorName, AuthorEmail, AuthorHomepage);
        Assert.That(author.GetHashCode(), Is.EqualTo(hashCode));
    }

    [Test]
    public void HashCode_Parts_WithoutHomepage()
    {
        var author = new Author(AuthorName, AuthorEmail);
        var hashCode = HashCode.Combine(AuthorName, AuthorEmail, string.Empty);
        Assert.That(author.GetHashCode(), Is.EqualTo(hashCode));
    }

    [Test]
    public void ICloneable_Clone()
    {
        ICloneable cloneable = new Author(AuthorName, AuthorEmail, AuthorHomepage);
        var clone = cloneable.Clone();
        if (clone is Author author)
        {
            Assert.Multiple(
                () =>
                {
                    Assert.That(author.Name, Is.EqualTo(AuthorName));
                    Assert.That(author.Email, Is.EqualTo(AuthorEmail));
                    Assert.That(author.Homepage, Is.EqualTo(AuthorHomepage));
                }
            );
        }
        else
        {
            Assert.Fail("Clone is not an author.");
        }
    }

    [Test]
    [SuppressMessage(
        "Style",
        "IDE0022:Use expression body for methods",
        Justification = "Tests should have a proper method body."
    )]
    public void InvalidAuthorEmpty()
    {
        // ReSharper disable once ObjectCreationAsStatement
        Assert.Throws(typeof(ArgumentNullException), () => new Author(""), "Author string is null or whitespace.");
    }

    [Test]
    [SuppressMessage(
        "Style",
        "IDE0022:Use expression body for methods",
        Justification = "Tests should have a proper method body."
    )]
    public void InvalidAuthorNull()
    {
        // ReSharper disable once ObjectCreationAsStatement
        Assert.Throws(
            typeof(ArgumentNullException),
            () => new Author(null),
            "Author string is null or whitespace."
        );
    }

    [Test]
    public void Operator_ImplicitFromString()
    {
        Author author = AuthorStringNameEmailHomepage;
        Assert.Multiple(
            () =>
            {
                Assert.That(author.Name, Is.EqualTo(AuthorName));
                Assert.That(author.Email, Is.EqualTo(AuthorEmail));
                Assert.That(author.Homepage, Is.EqualTo(AuthorHomepage));
            }
        );
    }

    [Test]
    public void Operator_ImplicitToString()
    {
        string authorString = new Author(AuthorName, AuthorEmail, AuthorHomepage);
        Assert.That(authorString, Is.EqualTo(AuthorStringNameEmailHomepage));
    }

    [Test]
    public void ToString_Name()
    {
        var author = new Author(AuthorName);
        Assert.That(author.ToString(), Is.EqualTo(AuthorName));
    }

    [Test]
    public void ToString_NameEmail()
    {
        var author = new Author(AuthorName, AuthorEmail);
        Assert.That(author.ToString(), Is.EqualTo(AuthorStringNameEmail));
    }

    [Test]
    public void ToString_NameEmailHomepage()
    {
        var author = new Author(AuthorName, AuthorEmail, AuthorHomepage);
        Assert.That(author.ToString(), Is.EqualTo(AuthorStringNameEmailHomepage));
    }

    [Test]
    public void ToString_NameHomepage()
    {
        var author = new Author(AuthorName, string.Empty, AuthorHomepage);
        Assert.That(author.ToString(), Is.EqualTo(AuthorStringNameHomepage));
    }
}