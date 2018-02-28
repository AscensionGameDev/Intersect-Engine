using Intersect.Server.Classes;
using Intersect.Server.WebApi.Authentication;
using JetBrains.Annotations;
using Nancy.Hosting.Self;
using System;
using Intersect.Server.Database;

namespace Intersect.Server.WebApi
{
    public sealed class ServerApi : IDisposable
    {
        [NotNull]
        public static ServerApi Instance { get; private set; }

        private bool mDisposed;

        [NotNull]
        private readonly NancyHost mHost;

        public bool IsRunning { get; private set; }

        [NotNull]
        public IAuthorizationProvider AuthorizationProvider { get; }

        public ServerApi()
        {
            Instance = this;

            AuthorizationProvider = new AuthorizationProvider();

            mHost = new NancyHost(
                new Bootstrapper(AuthorizationProvider),
                new HostConfiguration
                {
                    UnhandledExceptionCallback = exception => ServerStart.ProcessUnhandledException(this, exception),
                    UrlReservations = new UrlReservations { CreateAutomatically = true }
                },
                new Uri("http://localhost:80")
                );
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
