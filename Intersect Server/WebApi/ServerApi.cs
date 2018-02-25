using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Server.Classes;
using JetBrains.Annotations;
using Nancy.Hosting.Self;

namespace Intersect.Server.WebApi
{
    public sealed class ServerApi : IDisposable
    {
        private bool mDisposed;

        [NotNull]
        private readonly NancyHost mHost;

        public bool IsRunning { get; private set; }

        public ServerApi()
        {
            mHost = new NancyHost(new HostConfiguration
            {
                UnhandledExceptionCallback = exception => ServerStart.ProcessUnhandledException(this, exception)
            }, new Uri("http://localhost:80"));
        }

        public void Dispose()
        {
            if (mDisposed)
                return;

            if (IsRunning)
                Stop();

            mHost.Dispose();
            mDisposed = true;
        }

        public void Start()
        {
            if (IsRunning)
                return;

            mHost.Start();
        }

        public void Stop()
        {
            if (!IsRunning)
                return;

            if (mDisposed)
                return;

            mHost.Stop();
        }
    }
}
