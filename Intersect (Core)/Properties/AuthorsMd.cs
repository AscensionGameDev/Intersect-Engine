using System.Text.RegularExpressions;

namespace Intersect.Properties;

public partial class AuthorsMd
{
    public static readonly Regex EntryExpression =
        new(@"^(\[?)(?<name>[^\[\]\<\(]+)(?:\s+\((?<alias>[^\)]+)\))?(?:(?(1)(?: (?:\<(?<email>[^\>]+)\>|\((?<alias>[^\)]+)\)))*\])\((?<url>[^\)]+)\))?$");

    public struct Entry
    {
        private readonly string _stringified;

        public readonly string Name;

        public readonly string? Alias;

        public readonly string? Email;

        public readonly string? Url;

        public Entry(string entry)
        {
            var match = EntryExpression.Match(entry);
            if (match == default)
            {
                throw new ArgumentException("Entry is not in the correct format.", nameof(entry));
            }

            Name = match.Groups["name"].Value;
            Alias = match.Groups["alias"].Value;
            Email = match.Groups["email"].Value;
            Url = match.Groups["url"].Value;

            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new ArgumentException("Entry does not have a valid name.", nameof(entry));
            }

            var stringified = Name;

            if (!string.IsNullOrWhiteSpace(Alias))
            {
                stringified += $" ({Alias})";
            }

            if (!string.IsNullOrWhiteSpace(Email))
            {
                stringified += $" <{Email}>";
            }

            _stringified = stringified;
        }

        public override string ToString() => _stringified;

        public string ToString(bool includeUrl)
        {
            if (!includeUrl || string.IsNullOrWhiteSpace(Url))
            {
                return _stringified;
            }

            return $"{_stringified} ({Url})";
        }
    }

    public Entry[] Authors { get; set; }

    public Entry[] Maintainers { get; set; }

    public Entry[] Developers { get; set; }

    public Entry[] Contributors { get; set; }
}
