namespace Intersect.Client.Framework.Gwen.Control;

public interface INumericInput
{
    double Maximum { get; set; }

    double Minimum { get; set; }

    double Value { get; set; }
}