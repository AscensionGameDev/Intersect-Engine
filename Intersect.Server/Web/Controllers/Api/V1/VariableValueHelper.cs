using System.Globalization;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Variables;

namespace Intersect.Server.Web.Controllers.Api.V1;

internal static class VariableValueHelper
{
    private const string InvalidValueErrorFormat = "Invalid value for variable of type {0}. Received: {1}";

    public static bool Equals(VariableDataType dataType, VariableValue variableValue, object? value)
    {
        if (value == null)
        {
            return variableValue.Value == null;
        }

        return dataType switch
        {
            VariableDataType.Boolean => value is bool booleanValue && variableValue.Boolean == booleanValue,
            VariableDataType.Integer => value is long longValue && variableValue.Integer == longValue,
            VariableDataType.Number => value is double doubleValue && variableValue.Number.Equals(doubleValue),
            VariableDataType.String => value is string stringValue && string.Equals(variableValue.String ?? string.Empty, stringValue, StringComparison.Ordinal),
            _ => Equals(variableValue.Value, value),
        };
    }

    public static bool TryConvertValue(
        VariableDataType dataType,
        object? value,
        out object convertedValue,
        out string? error
    )
    {
        convertedValue = string.Empty;
        error = null;

        if (value == null)
        {
            if (dataType == VariableDataType.String)
            {
                return true;
            }

            error = FormatError(dataType, "null");
            return false;
        }

        switch (dataType)
        {
            case VariableDataType.Boolean:
                if (value is bool booleanValue)
                {
                    convertedValue = booleanValue;
                    return true;
                }

                error = FormatError(dataType, value?.GetType()?.Name ?? "null");
                return false;

            case VariableDataType.Integer:
                if (long.TryParse(value.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedInt))
                {
                    convertedValue = parsedInt;
                    return true;
                }

                error = FormatError(dataType, value?.GetType()?.Name ?? "null");
                return false;

            case VariableDataType.Number:
                if (double.TryParse(value.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedDouble))
                {
                    convertedValue = parsedDouble;
                    return true;
                }

                error = FormatError(dataType, value?.GetType()?.Name ?? "null");
                return false;

            case VariableDataType.String:
                if (value is string stringValue)
                {
                    convertedValue = stringValue;
                    return true;
                }

                error = FormatError(dataType, value?.GetType()?.Name ?? "null");
                return false;

            default:
                error = FormatError(dataType, value?.GetType()?.Name ?? "null");
                return false;
        }
    }

    private static string FormatError(VariableDataType dataType, string received)
    {
        return string.Format(CultureInfo.InvariantCulture, InvalidValueErrorFormat, dataType, received);
    }
}
