using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

using Intersect.Server.Web.RestApi.Routes;

namespace Intersect.Server.Web.RestApi.RouteProviders
{

    internal sealed class VersionedRouteProvider : DefaultDirectRouteProvider
    {

        public VersionedRouteProvider() : this(DefaultRootNamespace)
        {
        }

        public VersionedRouteProvider(string rootNamespace, string prefix = "api")
        {
            RootNamespace = rootNamespace;
            Prefix = prefix;
        }

        public static string DefaultRootNamespace =>
            typeof(RootInfoController).Namespace ?? throw new InvalidOperationException();

        public string RootNamespace { get; }

        public string Prefix { get; }

        protected override IReadOnlyList<IDirectRouteFactory> GetActionRouteFactories(
            HttpActionDescriptor actionDescriptor
        )
        {
            return base.GetActionRouteFactories(actionDescriptor);
        }

        protected override IReadOnlyList<RouteEntry> GetActionDirectRoutes(
            HttpActionDescriptor actionDescriptor,
            IReadOnlyList<IDirectRouteFactory> factories,
            IInlineConstraintResolver constraintResolver
        )
        {
            return base.GetActionDirectRoutes(actionDescriptor, factories, constraintResolver);
        }

        protected override IReadOnlyList<IDirectRouteFactory> GetControllerRouteFactories(
            HttpControllerDescriptor controllerDescriptor
        )
        {
            return base.GetControllerRouteFactories(controllerDescriptor);
        }

        public override IReadOnlyList<RouteEntry> GetDirectRoutes(
            HttpControllerDescriptor controllerDescriptor,
            IReadOnlyList<HttpActionDescriptor> actionDescriptors,
            IInlineConstraintResolver constraintResolver
        )
        {
            return base.GetDirectRoutes(controllerDescriptor, actionDescriptors, constraintResolver);
        }

        protected override IReadOnlyList<RouteEntry> GetControllerDirectRoutes(
            HttpControllerDescriptor controllerDescriptor,
            IReadOnlyList<HttpActionDescriptor> actionDescriptors,
            IReadOnlyList<IDirectRouteFactory> factories,
            IInlineConstraintResolver constraintResolver
        )
        {
            return base.GetControllerDirectRoutes(
                controllerDescriptor, actionDescriptors, factories, constraintResolver
            );
        }

        protected override string GetRoutePrefix(HttpControllerDescriptor controllerDescriptor)
        {
            var prefixBuilder = new StringBuilder(Prefix);

            var controllerType = controllerDescriptor.ControllerType;
            var namespaceSegments =
                controllerType?.Namespace?.Replace(RootNamespace, "").Split('.') ?? new string[] { };

            namespaceSegments.Where(segment => !string.IsNullOrWhiteSpace(segment))
                .ToList()
                .ForEach(
                    segment =>
                    {
                        prefixBuilder.Append('/');
                        prefixBuilder.Append(segment?.ToLower());
                    }
                );

            var prefix = base.GetRoutePrefix(controllerDescriptor)?.Trim();

            // More readable with less return points
            // ReSharper disable once InvertIf
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                prefixBuilder.Append('/');
                prefixBuilder.Append(prefix);
            }

            return prefixBuilder.ToString();
        }

    }

}
