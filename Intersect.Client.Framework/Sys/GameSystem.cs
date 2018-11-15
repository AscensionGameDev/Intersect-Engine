namespace Intersect.Client.Framework.Sys
{
    public abstract class GameSystem
    {
        public abstract long GetTimeMs();
        public abstract void Log(string msg);
        public abstract void LogError(string error);
    }
}