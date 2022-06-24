namespace Intersect
{
    /// <summary>
    /// Collection of commonly used justifications for suppressing code analysis warnings.
    /// </summary>
    public static partial class SuppressionJustifications
    {
        /// <summary>
        /// Analyzer doesn't respect JetBrains NotNullAttribute which already asserts non-nullability.
        /// </summary>
        public const string NotNullJetBrains =
            "Analyzer doesn't respect JetBrains NotNullAttribute which already asserts non-nullability.";
    }
}
