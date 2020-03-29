namespace Intersect.Client.Framework.Sys
{

    public abstract class GameSystem
    {

        public abstract long GetTimeMs();

        public abstract long GetTimeMsExact();

        public abstract void Update();

        public abstract void Log(string msg);

        public abstract void LogError(string error);

    }

}
