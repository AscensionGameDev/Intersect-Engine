using Intersect.Utilities;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Intersect.Plugins.Manifests.Types
{
    /// <summary>
    /// Structure that defines an <see cref="Author"/> that can be represented as an <see cref="Author"/> <see cref="string"/>.
    /// </summary>
    [JsonConverter(typeof(AuthorStringConverter))]
    public struct Author : IComparable<Author>, IEquatable<Author>, IEquatable<string>, ICloneable, IComparable
    {
        #region Constants

        /// <summary>
        /// Expression that defines what qualifies as an <see cref="Author"/> <see cref="string"/>.
        ///
        /// Can be summarized as <code>Name[ &lt;Email&gt;][ (Homepage)]</code>
        /// </summary>
        public static readonly Regex AuthorStringExpression =
            new Regex(@"^\s*(.+?)(?:\s+<([^>]+)>)?(?:\s+\(([^\\)]+)\))?\s*$");

        /// <summary>
        /// The empty <see cref="Author"/> created using the default constructor.
        /// </summary>
        public static Author Empty => new Author();

        #endregion Constants

        #region Fields

        /// <summary>
        /// Email address of the <see cref="Author"/> (not validated).
        /// </summary>
        public readonly string Email;

        /// <summary>
        /// Homepage (URL) of the <see cref="Author"/> (not validated).
        /// </summary>
        public readonly string Homepage;

        /// <summary>
        /// Name of the <see cref="Author"/>.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Precomputed hash code.
        /// </summary>
        private readonly int mHashCode;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Constructs an <see cref="Author"/> from a <see cref="string"/>.
        /// </summary>
        /// <param name="authorString">an <see cref="Author"/> <see cref="string"/></param>
        /// <exception cref="ArgumentNullException">thrown if <paramref name="authorString"/> is null, empty, or whitespace</exception>
        /// <exception cref="ArgumentException">thrown if <paramref name="authorString"/> does not match the format defined by <see cref="AuthorStringExpression" /></exception>
        /// <seealso cref="AuthorStringExpression"/>
        public Author(string authorString)
        {
            if (string.IsNullOrWhiteSpace(authorString))
            {
                throw new ArgumentNullException(nameof(authorString), @"Author string is null or whitespace.");
            }

            var match = AuthorStringExpression.Match(authorString);
            var name = match.Groups[1];
            var email = match.Groups[2];
            var homepage = match.Groups[3];

            Name = name.Value;
            Email = email.Value;
            Homepage = homepage.Value;

            mHashCode = ValueUtils.ComputeHashCode(Name, Email, Homepage);
        }

        /// <summary>
        /// Constructs an <see cref="Author"/> from an explicitly defined name, email, and homepage.
        /// </summary>
        /// <param name="name">the name of the <see cref="Author"/></param>
        /// <param name="email">the email address of the <see cref="Author"/></param>
        /// <param name="homepage">the homepage of the <see cref="Author"/></param>
        [JsonConstructor]
        public Author(string name, string email, string homepage = null)
        {
            Name = name;
            Email = email;
            Homepage = homepage ?? string.Empty;

            mHashCode = ValueUtils.ComputeHashCode(Name, Email, Homepage);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the parts as an enumerable for <see cref="CompareTo(Author)"/>.
        /// </summary>
        private IEnumerable<string> Parts => new[] {Name, Email, Homepage};

        #endregion Properties

        #region Methods

        /// <inheritdoc cref="ICloneable.Clone"/>
        public Author Clone() => new Author(Name, Email, Homepage);

        object ICloneable.Clone() => Clone();

        /// <inheritdoc />
        public int CompareTo(object obj) =>
            obj is Author other
                ? CompareTo(other)
                : throw new NotSupportedException("Comparison only supported with Author.");

        /// <inheritdoc />
        public int CompareTo(Author other) => ValueUtils.Compare(Parts, other.Parts);

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Author author && Equals(author);

        /// <inheritdoc />
        public bool Equals(Author other) => this == other;

        /// <inheritdoc />
        public bool Equals(string other) => this == other;

        /// <inheritdoc />
        public override int GetHashCode() => mHashCode;

        /// <inheritdoc />
        public override string ToString() =>
            $"{Name}{(string.IsNullOrEmpty(Email) ? "" : $" <{Email}>")}{(string.IsNullOrEmpty(Homepage) ? "" : $" ({Homepage})")}";

        #endregion Methods

        #region Operators

        /// <summary>
        /// Converts a <see cref="string"/> to an <see cref="Author"/>.
        /// </summary>
        /// <param name="authorString">an <see cref="Author"/> <see cref="string"/></param>
        public static implicit operator Author(string authorString) =>
            authorString == null ? Empty : new Author(authorString);

        /// <summary>
        /// Converts an <see cref="Author"/> to a <see cref="string"/>.
        /// </summary>
        /// <param name="author">an <see cref="Author"/></param>
        public static implicit operator string(Author author) => author.ToString();

        /// <summary>
        /// Checks if two <see cref="Author"/>s are not equal.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>if any of the parts are not equal</returns>
        public static bool operator !=(Author a, Author b) =>
            !string.Equals(a.Name, b.Name, StringComparison.Ordinal) ||
            !string.Equals(a.Email, b.Email, StringComparison.Ordinal) ||
            !string.Equals(a.Homepage, b.Homepage, StringComparison.Ordinal);

        /// <summary>
        /// Checks if two <see cref="Author"/>s are equal.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>if all of the parts are equal</returns>
        public static bool operator ==(Author a, Author b) =>
            string.Equals(a.Name, b.Name, StringComparison.Ordinal) &&
            string.Equals(a.Email, b.Email, StringComparison.Ordinal) &&
            string.Equals(a.Homepage, b.Homepage, StringComparison.Ordinal);

        #endregion Operators

        #region Classes

        private class AuthorStringConverter : JsonConverter<Author>
        {
            #region Properties

            public override bool CanRead => false;

            #endregion Properties

            #region Methods

            /// <inheritdoc />
            [ExcludeFromCodeCoverage] // Will not be used because CanRead is false
            public override Author ReadJson(
                JsonReader reader,
                Type objectType,
                Author existingValue,
                bool hasExistingValue,
                JsonSerializer serializer
            ) =>
                throw new NotImplementedException();

            /// <inheritdoc />
            public override void WriteJson(JsonWriter writer, Author value, JsonSerializer serializer) =>
                writer?.WriteValue(value.ToString());

            #endregion Methods
        }

        #endregion Classes
    }
}
