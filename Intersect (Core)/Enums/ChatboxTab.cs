namespace Intersect.Enums
{
    /// <summary>
    /// Defines the chat tabs available within the engine, and is used to determine which one is currently in view.
    /// </summary>
    public enum ChatboxTab
    {
        All,

        Local,

        Party,

        Guild,

        Global,

        System,

        // Always keep this at the bottom, or you're going to have a very bad time!
        Count
    }
}
