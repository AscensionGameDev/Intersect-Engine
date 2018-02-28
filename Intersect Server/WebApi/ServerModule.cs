using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Nancy;
using Nancy.Responses;
using Nancy.Security;

namespace Intersect.Server.WebApi
{
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

        protected ServerModule(string modulePath) : base(modulePath)
        {
            Initialize();
        }

        private void Initialize()
        {
            if (Secured) this.RequiresAuthentication();
        }
    }
}
