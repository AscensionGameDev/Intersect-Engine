namespace Intersect.Client.Plugins.Interfaces;

public partial class TimedArgs : EventArgs
{
    /// <summary>
    /// Time since the last update.
    /// </summary>
    public TimeSpan Delta { get; }

    public TimedArgs(TimeSpan delta)
    {
        Delta = delta;
    }
}