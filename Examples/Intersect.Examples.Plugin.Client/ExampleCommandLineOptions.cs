using CommandLine;

using Intersect.Utilities;

using System;

namespace Intersect.Examples.Plugin.Client
{
    /// <summary>
    /// Example immutable command line options structure.
    /// </summary>
    public struct ExampleCommandLineOptions : IEquatable<ExampleCommandLineOptions>
    {
        public ExampleCommandLineOptions(bool exampleFlag, int exampleVariable)
        {
            ExampleFlag = exampleFlag;
            ExampleVariable = exampleVariable;
        }

        /// <summary>
        /// Flag that is true if the application was started with --example-flag
        /// </summary>
        [Option("example-flag", Default = false, Required = false)]
        public bool ExampleFlag { get; }

        /// <summary>
        /// Integer argument that corresponds to --example-variable
        /// </summary>
        [Option("example-variable", Default = 100, Required = false)]
        public int ExampleVariable { get; }

        public override bool Equals(object obj) => obj is ExampleCommandLineOptions other && Equals(other);

        public override int GetHashCode() => ValueUtils.ComputeHashCode(ExampleFlag, ExampleVariable);

        public static bool operator ==(ExampleCommandLineOptions left, ExampleCommandLineOptions right) =>
            left.Equals(right);

        public static bool operator !=(ExampleCommandLineOptions left, ExampleCommandLineOptions right) =>
            !(left == right);

        public bool Equals(ExampleCommandLineOptions other) =>
            ExampleFlag == other.ExampleFlag && ExampleVariable == other.ExampleVariable;
    }
}