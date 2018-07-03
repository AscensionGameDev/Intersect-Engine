using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Nancy;
using Nancy.Extensions;
using Nancy.Responses;
using Nancy.Security;

namespace Intersect.Server.WebApi
{
    using MethodPath = ValueTuple<string, string>;
    using MethodPathCode = ValueTuple<string, string, HttpStatusCode>;

    public abstract class ServerModule : NancyModule
    {
        private T QuerySelector<T>(string key) => Query(key ?? "");

        public dynamic Query([NotNull] string key) => (Request?.Query as DynamicDictionary)?[key];

        public T Query<T>([NotNull] string key, T defaultValue) => Query(key) ?? defaultValue;

        public T Query<T>(T defaultValue, params string[] keys)
        {
            var list = keys?.ToList();
            var notNull = list?.FindAll(key => key != null);
            var queryValues = notNull?.Select(QuerySelector<dynamic>);
            var notNullQueryValues = queryValues?.ToList().FindAll(value => value != null);
            var firstOrDefault = notNullQueryValues?.FirstOrDefault();
            return firstOrDefault ?? defaultValue;
        }

        public T Query<T>([NotNull] string key, [NotNull] Func<dynamic, T> processValue, T defaultValue)
            => processValue(Query(key, defaultValue));

        public T Query<T>([NotNull] Func<dynamic, T> processValue, T defaultValue, params string[] keys)
        {
            var list = keys?.ToList();
            var notNull = list?.FindAll(key => key != null);
            var queryValues = notNull?.Select(QuerySelector<dynamic>).Select(value => processValue(value));
            var notNullQueryValues = queryValues?.ToList().FindAll(value => value != null);
            var firstOrDefault = notNullQueryValues?.FirstOrDefault();
            return firstOrDefault ?? defaultValue;
        }

        protected virtual bool Secured => true;

        protected virtual IDictionary<MethodPath, bool> RouteSecurity => null;

        protected virtual IDictionary<MethodPathCode, object> DefaultResponse => null;

        protected ServerModule(string modulePath) : base(modulePath)
        {
            Secure();
        }

        protected virtual void Secure()
        {
            this.AddBeforeHookOrExecute(CheckAuthentication, "Authentication required.");
        }

        private Response CheckAuthentication([NotNull] NancyContext context)
        {
            var request = context.Request;
            if (request == null) return Forbid();

            var method = request.Method;
            if (method == null) return Forbid();

            var path = context.ResolvedRoute.Description.Path;
            if (path == null) return Forbid();

            var modulePath = ModulePath ?? "";
            if (path.StartsWith(modulePath))
            {
                path = path.Remove(0, modulePath.Length);
            }

            var routeSecured = Secured;
            RouteSecurity?.TryGetValue((method.ToUpperInvariant(), path), out routeSecured);

            return (context.CurrentUser?.IsAuthenticated() ?? false) || !routeSecured
                ? null : GetDefaultResponse(context, HttpStatusCode.Unauthorized);
        }

        protected Response GetDefaultResponse([NotNull] NancyContext context, HttpStatusCode httpStatusCode)
        {
            var request = context.Request;
            if (request == null) return Forbid();

            var method = request.Method;
            if (method == null) return Forbid();

            var path = request.Path;
            if (path == null) return Forbid();

            var modulePath = ModulePath ?? "";
            if (path.StartsWith(modulePath))
            {
                path = path.Remove(0, modulePath.Length);
            }

            /* Normally I'd go with more code outside the if-block, but
             * I'd rather not repeat the default response twice. */
            // ReSharper disable once InvertIf
            if (DefaultResponse != null && DefaultResponse.Count > 0)
            {
                if (DefaultResponse.TryGetValue((method, path, httpStatusCode), out object methodPathResponse))
                    return Response.AsJson(methodPathResponse, httpStatusCode);

                if (DefaultResponse.TryGetValue(("*", path, httpStatusCode), out object pathResponse))
                    return Response.AsJson(pathResponse, httpStatusCode);

                if (DefaultResponse.TryGetValue((method, "*", httpStatusCode), out object methodResponse))
                    return Response.AsJson(methodResponse, httpStatusCode);

                if (DefaultResponse.TryGetValue(("*", "*", httpStatusCode), out object wildcardResponse))
                    return Response.AsJson(wildcardResponse, httpStatusCode);
            }

            return Response.AsJson(new
            {
                status = httpStatusCode,
                message = "Either nothing is here or you are unauthorized to view this page."
            }, httpStatusCode);
        }

        protected static Response Forbid() => new Response { StatusCode = HttpStatusCode.Forbidden };
    }
}
