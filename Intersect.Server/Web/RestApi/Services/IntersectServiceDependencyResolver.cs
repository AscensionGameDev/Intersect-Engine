using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Dependencies;

using Intersect.Server.Web.RestApi.Configuration;

using JetBrains.Annotations;

namespace Intersect.Server.Web.RestApi.Services
{

    internal sealed class IntersectServiceDependencyResolver : IDependencyResolver
    {

        public IntersectServiceDependencyResolver(
            [NotNull] ApiConfiguration apiConfiguration,
            [NotNull] HttpConfiguration httpConfiguration
        )
        {
            ResolvedDependencies = new Dictionary<Type, object>();
            ApiConfiguration = apiConfiguration;
            HttpConfiguration = httpConfiguration;
        }

        [NotNull]
        private Dictionary<Type, object> ResolvedDependencies { get; }

        [NotNull]
        public HttpConfiguration HttpConfiguration { get; }

        [NotNull]
        public ApiConfiguration ApiConfiguration { get; }

        /// <inheritdoc />
        public void Dispose()
        {
            ResolvedDependencies.Clear();
        }

        /// <inheritdoc />
        public object GetService([NotNull] Type serviceType)
        {
            if (ResolvedDependencies.TryGetValue(serviceType, out var foundService))
            {
                return foundService;
            }

            var thisType = typeof(IntersectServiceDependencyResolver);
            var definitionType = serviceType;
            if (serviceType.IsAbstract || serviceType.IsInterface)
            {
                definitionType = Assembly.GetAssembly(thisType)
                    ?.DefinedTypes
                    ?.Where(typeInfo => typeInfo?.Namespace?.StartsWith(thisType.Namespace ?? "") ?? false)
                    .FirstOrDefault(typeInfo => serviceType.IsAssignableFrom(typeInfo.AsType()));
            }

            var constructor =
                definitionType?.GetConstructor(new[] {typeof(ApiConfiguration), typeof(HttpConfiguration)});

            var constructedService = constructor?.Invoke(new object[] {ApiConfiguration, HttpConfiguration});
            ResolvedDependencies[serviceType] = constructedService;

            return constructedService;
        }

        /// <inheritdoc />
        public IEnumerable<object> GetServices([NotNull] Type serviceType)
        {
            return new[] {GetService(serviceType)};
        }

        /// <inheritdoc />
        public IDependencyScope BeginScope()
        {
            return this;
        }

    }

}
