namespace Intersect.Utilities;

public delegate string ProvideNumericalFormatString(double timeInUnit);

public static class MeasurementExtensions
{
    public static readonly ProvideNumericalFormatString RoundAboveTen = timeInUnit => timeInUnit > 10 ? "0." : "0.#";

    public static string WithSuffix(
        this TimeSpan timeSpan,
        string? numericalFormat = default,
        ProvideNumericalFormatString? numericalFormatProvider = default,
        uint truncateDigits = 1
    )
    {
        double timeInUnit;
        string suffix;

        switch (timeSpan.Ticks)
        {
            case < TimeSpan.TicksPerMicrosecond:
                timeInUnit = timeSpan.Ticks;
                suffix = MeasurementFormattingResources.TimeNanosecondSuffix;
                break;

            case < TimeSpan.TicksPerMillisecond:
                timeInUnit = timeSpan.Ticks / (double)TimeSpan.TicksPerMicrosecond;
                suffix = MeasurementFormattingResources.TimeMicrosecondSuffix;
                break;

            case < TimeSpan.TicksPerSecond:
                timeInUnit = timeSpan.Ticks / (double)TimeSpan.TicksPerMillisecond;
                suffix = MeasurementFormattingResources.TimeMillisecondSuffix;
                break;

            case < TimeSpan.TicksPerMinute:
                timeInUnit = timeSpan.Ticks / (double)TimeSpan.TicksPerSecond;
                suffix = MeasurementFormattingResources.TimeSecondSuffix;
                break;

            case < TimeSpan.TicksPerHour:
                timeInUnit = timeSpan.Ticks / (double)TimeSpan.TicksPerMinute;
                suffix = MeasurementFormattingResources.TimeMinuteSuffix;
                break;

            case < TimeSpan.TicksPerDay:
                timeInUnit = timeSpan.Ticks / (double)TimeSpan.TicksPerHour;
                suffix = MeasurementFormattingResources.TimeHourSuffix;
                break;

            case < TimeSpan.TicksPerDay * 7:
                timeInUnit = timeSpan.Ticks / (double)TimeSpan.TicksPerDay;
                suffix = MeasurementFormattingResources.TimeDaySuffix;
                break;

            case < TimeSpan.TicksPerDay * 365:
                timeInUnit = timeSpan.Ticks / (double)(TimeSpan.TicksPerDay * 7);
                suffix = MeasurementFormattingResources.TimeWeekSuffix;
                break;

            default:
                timeInUnit = timeSpan.Ticks / (double)(TimeSpan.TicksPerDay * 7);
                suffix = MeasurementFormattingResources.TimeYearSuffix;
                break;
        }

        if (numericalFormat == default)
        {
            numericalFormatProvider ??= RoundAboveTen;
            numericalFormat = numericalFormatProvider(timeInUnit);
        }

        if (truncateDigits > 0)
        {
            var truncationScale = Math.Pow(10, truncateDigits);
            timeInUnit = Math.Truncate(timeInUnit * truncationScale) / truncationScale;
        }

        var formattedTimeSpan = timeInUnit.ToString($"{numericalFormat}'{suffix}");
        return formattedTimeSpan;
    }
}