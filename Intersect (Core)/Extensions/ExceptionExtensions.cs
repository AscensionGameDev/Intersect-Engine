using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Text;

namespace Intersect.Extensions;

public static class ExceptionExtensions
{
    [return: NotNull]
    [Pure]
    public static string AsFullStackString(this Exception exception)
    {
        StringBuilder messageBuilder = new();

        var currentException = exception;
        while (currentException != default)
        {
            if (currentException != exception)
            {
                messageBuilder.Append("Caused by: ");
            }

            messageBuilder.AppendLine(currentException.ToString());
            currentException = currentException.InnerException;
        }

        return messageBuilder.ToString();
    }
}