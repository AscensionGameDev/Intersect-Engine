using System;

namespace Intersect.Core
{
    public interface IApplicationContext : IDisposable
    {
        bool IsDisposed { get; }

        bool IsStarted { get; }

        bool IsRunning { get; }

        void Start();
    }
}
