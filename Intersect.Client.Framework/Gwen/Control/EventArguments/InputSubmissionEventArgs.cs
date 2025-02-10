using Intersect.Client.Framework.Gwen.Control.EventArguments.InputSubmissionEvent;

namespace Intersect.Client.Framework.Gwen.Control.EventArguments;

public class InputSubmissionEventArgs(SubmissionValue value) : EventArgs
{
    public SubmissionValue Value { get; } = value;
}