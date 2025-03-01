using CommandLine;

namespace Intersect.Examples.Plugin.Client;

/// <summary>
///     Example immutable command line options structure.
/// </summary>
public readonly struct ExampleCommandLineOptions(bool exampleFlag, int exampleVariable)
    : IEquatable<ExampleCommandLineOptions>
{
    /// <summary>
    ///     Flag that is true if the application was started with --example-flag
    /// </summary>
    [Option("example-flag", Default = false, Required = false)]
    public bool ExampleFlag { get; } = exampleFlag;

    /// <summary>
    ///     Integer argument that corresponds to --example-variable
    /// </summary>
    [Option("example-variable", Default = 100, Required = false)]
    public int ExampleVariable { get; } = exampleVariable;

    public override bool Equals(object? obj)
    {
        return obj is ExampleCommandLineOptions other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ExampleFlag, ExampleVariable);
    }

    public static bool operator ==(ExampleCommandLineOptions left, ExampleCommandLineOptions right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ExampleCommandLineOptions left, ExampleCommandLineOptions right)
    {
        return !(left == right);
    }

    public bool Equals(ExampleCommandLineOptions other)
    {
        return ExampleFlag == other.ExampleFlag && ExampleVariable == other.ExampleVariable;
    }
}