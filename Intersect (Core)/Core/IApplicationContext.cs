using System;

using Intersect.Threading;

using JetBrains.Annotations;

namespace Intersect.Core
{

    public interface IApplicationContext : IDisposable
    {

        bool IsDisposed { get; }

        bool IsStarted { get; }

        bool IsRunning { get; }

        void Start(bool lockUntilShutdown = true);

        [NotNull]
        LockingActionQueue StartWithActionQueue();

    }

}
