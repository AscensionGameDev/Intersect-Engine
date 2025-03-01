using System.Collections;
using Newtonsoft.Json;

namespace Intersect.Plugins.Manifests.Types;

/// <summary>
///     Immutable container structure for multiple <see cref="Author" />s.
/// </summary>
public partial struct Authors : IComparable<IEnumerable<Author>>,
    IComparable<IEnumerable<string>>,
    IEquatable<IEnumerable<Author>>,
    IEquatable<IEnumerable<string>>,
    IEquatable<string>,
    IReadOnlyList<Author>,
    ICloneable,
    IComparable,
    IStructuralComparable,
    IStructuralEquatable
{
    #region Constants

    /// <summary>
    ///     The exception message that is used when comparison is attempted on invalid types or null.
    /// </summary>
    public const string ExceptionComparisonNotSupported =
        "Comparison only supported with IEnumerable<Author> or IEnumerable<string>.";

    #endregion Constants

    #region Fields

    /// <summary>
    ///     Backing enumerable of authors.
    /// </summary>
    private readonly Author[] _authors;

    /// <summary>
    ///     Precomputed hash code.
    /// </summary>
    private readonly int _hashCode;

    #endregion Fields

    #region Constructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="Authors" /> structure.
    /// </summary>
    /// <param name="authors">the <see cref="Author" />s in this list</param>
    public Authors(params Author[] authors) : this(authors as IEnumerable<Author>) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Authors" /> structure.
    /// </summary>
    /// <param name="authors">the <see cref="Author" />s in this list</param>
    public Authors(IEnumerable<Author> authors)
    {
        _authors = authors.ToArray();

        HashCode hashCode = new();
        foreach (var author in _authors)
        {
            hashCode.Add(author);
        }

        _hashCode = hashCode.ToHashCode();
    }

    #endregion Constructors

    #region Properties

    /// <inheritdoc />
    public int Count => _authors.Length;

    #endregion Properties

    #region Indexers

    /// <inheritdoc />
    public Author this[int index] => _authors[index];

    #endregion Indexers

    #region Methods

    /// <inheritdoc cref="ICloneable.Clone" />
    public Authors Clone()
    {
        return new Authors(_authors.Select(author => author.Clone()));
    }

    object ICloneable.Clone()
    {
        return Clone();
    }

    /// <inheritdoc />
    public int CompareTo(object other, IComparer comparer)
    {
        return comparer.Compare(this, other);
    }

    /// <inheritdoc />
    public int CompareTo(IEnumerable<Author> other)
    {
        return other?.Select(CompareElement).FirstOrDefault(comparison => comparison != 0) ?? 0;
    }

    /// <inheritdoc />
    public int CompareTo(IEnumerable<string> other)
    {
        return other?.Select(CompareElement).FirstOrDefault(comparison => comparison != 0) ?? 0;
    }

    /// <inheritdoc />
    public int CompareTo(object obj)
    {
        switch (obj)
        {
            case IEnumerable<Author> authors:
                return CompareTo(authors);

            case IEnumerable<string> authorStrings:
                return CompareTo(authorStrings);

            default:
                throw new NotSupportedException(ExceptionComparisonNotSupported);
        }
    }

    /// <inheritdoc />
    public bool Equals(object other, IEqualityComparer comparer)
    {
        switch (other)
        {
            case IEnumerable<Author> authors:
                return comparer.Equals(_authors, authors);

            case IEnumerable<string> authorStrings:
                return comparer.Equals(this, authorStrings);

            default:
                throw new NotSupportedException(ExceptionComparisonNotSupported);
        }
    }

    /// <inheritdoc />
    public bool Equals(IEnumerable<Author> other)
    {
        return Equals(other, EqualsElement);
    }

    /// <inheritdoc />
    public bool Equals(IEnumerable<string> other)
    {
        return Equals(other, EqualsElement);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        switch (obj)
        {
            case IEnumerable<Author> authors:
                return Equals(authors);

            case IEnumerable<string> authorStrings:
                return Equals(authorStrings);

            default:
                throw new NotSupportedException(ExceptionComparisonNotSupported);
        }
    }

    /// <inheritdoc />
    public bool Equals(string other)
    {
        return this == other;
    }

    /// <inheritdoc />
    public IEnumerator<Author> GetEnumerator()
    {
        return ((IEnumerable<Author>)_authors).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _authors.GetEnumerator();
    }

    /// <inheritdoc />
    public int GetHashCode(IEqualityComparer comparer)
    {
        return comparer.GetHashCode(this);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return _hashCode;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return JsonConvert.SerializeObject(_authors);
    }

    /// <summary>
    ///     Compares equality against the other <see cref="IEnumerable{T}" />.
    /// </summary>
    /// <typeparam name="TEnumerableValue"></typeparam>
    /// <param name="other">another enumerable</param>
    /// <param name="comparisonFunc">indexed equality comparison function</param>
    /// <returns>equality as determined by <paramref name="comparisonFunc" /></returns>
    private static bool Equals<TEnumerableValue>(
        IEnumerable<TEnumerableValue> other,
        Func<TEnumerableValue, int, bool> comparisonFunc
    )
    {
        return other?.Select(comparisonFunc).All(equality => equality) ?? false;
    }

    /// <summary>
    ///     Compares <paramref name="author" /> to the <see cref="Author" /> located at <paramref name="index" />.
    ///     Shorter arrays are considered "less than".
    /// </summary>
    /// <param name="author">the <see cref="Author" /> to compare</param>
    /// <param name="index">the index to compare at</param>
    /// <returns>the result of <see cref="Author.CompareTo(Author)" /> or -1 <paramref name="index" /> is out of bounds</returns>
    private int CompareElement(Author author, int index)
    {
        return index < _authors.Length ? _authors[index].CompareTo(author) : -1;
    }

    /// <summary>
    ///     Compares <paramref name="authorString" /> to the <see cref="Author" /> located at <paramref name="index" />.
    ///     Shorter arrays are considered "less than".
    /// </summary>
    /// <param name="authorString">the <see cref="Author" /> <see cref="string" /> to compare</param>
    /// <param name="index">the index to compare at</param>
    /// <returns>the result of <see cref="Author.CompareTo(Author)" /> or -1 <paramref name="index" /> is out of bounds</returns>
    private int CompareElement(string authorString, int index)
    {
        return index < _authors.Length ? _authors[index].CompareTo(authorString) : -1;
    }

    /// <summary>
    ///     Compares equality between <paramref name="author" /> and the <see cref="Author" /> located at
    ///     <paramref name="index" />.
    /// </summary>
    /// <param name="author">the <see cref="Author" /> to compare</param>
    /// <param name="index">the index to compare at</param>
    /// <returns>the result of <see cref="Author.Equals(Author)" /> or false if <paramref name="index" /> is out of bounds</returns>
    private bool EqualsElement(Author author, int index)
    {
        return index < _authors.Length && _authors[index].Equals(author);
    }

    /// <summary>
    ///     Compares equality between <paramref name="authorString" /> and the <see cref="Author" /> located at
    ///     <paramref name="index" />.
    /// </summary>
    /// <param name="authorString">the <see cref="Author" /><see cref="string" />  to compare</param>
    /// <param name="index">the index to compare at</param>
    /// <returns>the result of <see cref="Author.Equals(Author)" /> or false if <paramref name="index" /> is out of bounds</returns>
    private bool EqualsElement(string authorString, int index)
    {
        return index < _authors.Length && _authors[index].Equals(authorString);
    }

    #endregion Methods

    #region Operators

    /// <summary>
    ///     Converts a JSON <see cref="string" /> of <see cref="Author" />s into an instance of <see cref="Authors" />.
    /// </summary>
    /// <param name="authorsString">a JSON <see cref="string" /> of <see cref="Author" />s</param>
    /// <returns>an <see cref="Authors" /> instance</returns>
    /// <seealso cref="Author(string)" />
    /// <seealso cref="JsonConvert.DeserializeObject{T}(string)" />
    public static implicit operator Authors(string authorsString)
    {
        try
        {
            return JsonConvert.DeserializeObject<Authors>(authorsString);
        }
        catch
        {
            return new Author(authorsString);
        }
    }

    /// <summary>
    ///     Converts a single <see cref="Author" /> into a single-element instance of <see cref="Authors" />.
    /// </summary>
    /// <param name="author">an <see cref="Author" /></param>
    /// <returns>an <see cref="Authors" /> instance</returns>
    public static implicit operator Authors(Author author)
    {
        return new Authors(author);
    }

    /// <summary>
    ///     Converts an array of <see cref="Author" /> <see cref="string" />s to an instance of <see cref="Authors" />.
    /// </summary>
    /// <param name="authorStrings">an array of <see cref="Author" /> <see cref="string" />s</param>
    /// <returns>an <see cref="Authors" /> instance</returns>
    /// <seealso cref="Author(string)" />
    public static implicit operator Authors(string[] authorStrings)
    {
        return new Authors(authorStrings.Select(authorString => new Author(authorString)));
    }

    /// <summary>
    ///     Converts an array of <see cref="Author" />s to an instance of <see cref="Authors" />.
    /// </summary>
    /// <param name="authors">an array of <see cref="Author" />s</param>
    /// <returns>an <see cref="Author" /> array</returns>
    public static implicit operator Authors(Author[] authors)
    {
        return new Authors(authors ?? Array.Empty<Author>());
    }

    /// <summary>
    ///     Converts <see cref="Authors" /> to <see cref="string" /> using <see cref="ToString" />.
    /// </summary>
    /// <param name="authors">an <see cref="Authors" /> instance</param>
    public static implicit operator string(Authors authors)
    {
        return authors.ToString();
    }

    /// <summary>
    ///     Checks if two <see cref="Authors" /> are not equal.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns>if any of the <see cref="Author" />s are not equal</returns>
    public static bool operator !=(Authors a, Authors b)
    {
        return !a.Equals(b as IEnumerable<Author>);
    }

    /// <summary>
    ///     Checks if two <see cref="Authors" /> are equal.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns>if all of the <see cref="Author" />s are equal</returns>
    public static bool operator ==(Authors a, Authors b)
    {
        return a.Equals(b as IEnumerable<Author>);
    }

    #endregion Operators
}