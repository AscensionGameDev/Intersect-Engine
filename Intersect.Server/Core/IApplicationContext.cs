using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Server.Core
{
    public interface IApplicationContext : IDisposable
    {
        bool IsDisposed { get; }

        bool IsStarted { get; }

        bool IsRunning { get; }

        void Start();
    }
}
