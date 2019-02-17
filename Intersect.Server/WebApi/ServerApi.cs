using Intersect.Server.WebApi.Authentication;
using JetBrains.Annotations;
using Nancy.Hosting.Self;
using System;
using System.Net;

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

        public ServerApi(ushort port)
        {
            Instance = this;

            AuthorizationProvider = new AuthorizationProvider();

            mHost = new NancyHost(
                new Bootstrapper(AuthorizationProvider),
                new HostConfiguration
                {
                    UnhandledExceptionCallback = exception => Intersect.Server.Core.Bootstrapper.ProcessUnhandledException(this, exception),
                    UrlReservations = new UrlReservations { CreateAutomatically = true }
                },
                new Uri($"http://localhost:{port}")
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
			try
			{
				mHost.Start();
			}
			catch (Exception ex)
			{
				//API Port is in use. Ignoring for now but when we actually add in the api we should throw a warning or something.
			}
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
