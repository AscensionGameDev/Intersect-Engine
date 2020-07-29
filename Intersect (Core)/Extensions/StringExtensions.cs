
using JetBrains.Annotations;

namespace Intersect.Extensions
{
    public static class StringExtensions
    {
        [NotNull]
        public static string TerminateWith(this string self, char terminateWith, bool condition = true) =>
            TerminateWith(self, new[] {terminateWith}, condition);

        [NotNull]
        public static string TerminateWith(this string self, char[] terminateWith, bool condition = true) =>
            TerminateWith(self, new string(terminateWith), condition);

        [NotNull]
        public static string TerminateWith(this string self, [NotNull] string terminateWith, bool condition = true)
        {
            if (!condition)
            {
                return self;
            }

            if (string.IsNullOrWhiteSpace(self))
            {
                return terminateWith;
            }

            if (self.EndsWith(terminateWith))
            {
                return self;
            }

            return self + terminateWith;
        }

    }
}
