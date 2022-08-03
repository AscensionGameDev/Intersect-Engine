using System.Reflection;

using Intersect.Reflection;

namespace Intersect.Properties;

public static class Markdown
{
    private static readonly Assembly CurrentAssembly = typeof(Markdown).Assembly;

    public static AuthorsMd ParseAuthorsMd(Assembly assembly = default)
    {
        var targetAssembly = assembly ?? CurrentAssembly;

        if (!targetAssembly.TryFindResource("AUTHORS.md", out var manifestResourceName))
        {
            return default;
        }

        using var stream = targetAssembly.GetManifestResourceStream(manifestResourceName);
        using var reader = new StreamReader(stream);
        var contents = reader.ReadToEnd();

        var lines = contents
            .Split('\n')
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrWhiteSpace(line));

        var authors = new List<AuthorsMd.Entry>();
        var contributors = new List<AuthorsMd.Entry>();
        var developers = new List<AuthorsMd.Entry>();
        var maintainers = new List<AuthorsMd.Entry>();

        var currentGroup = authors;
        foreach (var line in lines)
        {
            if (line.StartsWith('#'))
            {
                var sectionName = line[1..].Trim();
                currentGroup = sectionName switch
                {
                    "Authors" => authors,
                    "Contributors" => contributors,
                    "Developers" => developers,
                    "Maintainers" => maintainers,
                    _ => throw new InvalidOperationException($"Invalid section name '{sectionName}'"),
                };
            }
            else
            {
                var entry = new AuthorsMd.Entry(line);
                currentGroup.Add(entry);
            }
        }

        return new AuthorsMd
        {
            Authors = authors.ToArray(),
            Contributors = contributors.ToArray(),
            Developers = developers.ToArray(),
            Maintainers = maintainers.ToArray(),
        };
    }
}
