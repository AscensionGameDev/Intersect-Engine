using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using Intersect.Extensions;
using Intersect.Logging;
using Intersect.Utilities;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace Intersect.Core.ExperimentalFeatures
{
    public abstract partial class CommonExperiments<TExperiments> where TExperiments : CommonExperiments<TExperiments>
    {
        private static readonly Guid NamespaceId = Guid.Parse("c68012b3-d666-4204-84eb-4976f2b570ab");

        [NotNull] private readonly IDictionary<Guid, PropertyInfo> mFlagsById;

        [NotNull] private readonly IDictionary<string, PropertyInfo> mFlagsByName;

        protected CommonExperiments()
        {
            mFlagsById = new Dictionary<Guid, PropertyInfo>();
            mFlagsByName = new Dictionary<string, PropertyInfo>();

            RegisterProperties();
        }

        private void RegisterProperties()
        {
            var experimentsType = typeof(TExperiments);
            var properties = experimentsType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var flagProperties = properties.Where(property => property.PropertyType == typeof(ExperimentalFlag));
            flagProperties.ToList()
                .ForEach(
                    property =>
                    {
                        if (property?.DeclaringType == null)
                        {
                            throw new InvalidOperationException();
                        }

                        var namespaceId = GetNamespaceIdFor(property.DeclaringType);
                        var flagId = GuidUtils.CreateNamed(namespaceId, property.Name);
                        mFlagsById.Add(flagId, property);
                        mFlagsByName.Add(property.Name.ToLowerInvariant(), property);
                    }
                );
        }

        public bool Disable(ExperimentalFlag flag) => TrySet(flag, false);

        public bool Disable(Guid flagId) => TrySet(flagId, false);

        public bool Disable([NotNull] string flagName) => TrySet(flagName, false);

        public bool Enable(ExperimentalFlag flag) => TrySet(flag, true);

        public bool Enable(Guid flagId) => TrySet(flagId, true);

        public bool Enable([NotNull] string flagName) => TrySet(flagName, true);

        public bool IsEnabled(Guid flagId) =>
            mFlagsById.TryGetValue(flagId, out var property) &&
            property.GetValue(this) is ExperimentalFlag flag &&
            flag.Enabled;

        public bool IsEnabled([NotNull] string flagName) =>
            mFlagsByName.TryGetValue(flagName, out var property) &&
            property.GetValue(this) is ExperimentalFlag flag &&
            flag.Enabled;

        public bool TryGet(Guid flagId, out ExperimentalFlag flag) =>
            ValueUtils.SetDefault(TryGetProperty(flagId, out var property), out flag) &&
            property.TryGetValue(this, out flag);

        public bool TryGet([NotNull] string flagName, out ExperimentalFlag flag) =>
            ValueUtils.SetDefault(TryGetProperty(flagName, out var property), out flag) &&
            property.TryGetValue(this, out flag);

        protected bool TryGetProperty(ExperimentalFlag flag, out PropertyInfo flagPropertyInfo) =>
            mFlagsById.TryGetValue(flag.Guid, out flagPropertyInfo);

        protected bool TryGetProperty(Guid flagId, out PropertyInfo flagPropertyInfo) =>
            mFlagsById.TryGetValue(flagId, out flagPropertyInfo);

        protected bool TryGetProperty([NotNull] string flagName, out PropertyInfo flagPropertyInfo) =>
            ValueUtils.SetDefault(!string.IsNullOrWhiteSpace(flagName), out flagPropertyInfo) &&
            mFlagsByName.TryGetValue(flagName.ToLowerInvariant(), out flagPropertyInfo);

        public bool TrySet(ExperimentalFlag flag, bool enabled)
        {
            // ReSharper disable once InvertIf
            if (TryGetProperty(flag, out var property))
            {
                property.SetValue(this, flag.With(enabled));
                Save();
                return true;
            }

            return false;
        }

        public bool TrySet(Guid flagId, bool enabled)
        {
            // ReSharper disable once InvertIf
            if (TryGetProperty(flagId, out var property) && property.GetValue(this) is ExperimentalFlag flag)
            {
                property.SetValue(this, flag.With(enabled));
                Save();
                return true;
            }

            return false;
        }

        public bool TrySet([NotNull] string flagName, bool enabled)
        {
            // ReSharper disable once InvertIf
            if (TryGetProperty(flagName, out var property) && property.GetValue(this) is ExperimentalFlag flag)
            {
                property.SetValue(this, flag.With(enabled));
                Save();
                return true;
            }

            return false;
        }

        protected virtual bool Load()
        {
            if (!File.Exists(CONFIG_PATH))
            {
                return false;
            }

            try
            {
                var json = File.ReadAllText(CONFIG_PATH, Encoding.UTF8);
                JsonConvert.PopulateObject(json, Instance, new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter>
                    {
                        new ExperimentalFlagConverter()
                    }
                });
                return true;
            }
            catch (Exception exception)
            {
                Log.Error(exception);
                return false;
            }
        }

        protected virtual void Save()
        {
            try
            {
                var directory = Path.GetDirectoryName(CONFIG_PATH) ?? "resources/config";
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonConvert.SerializeObject(Instance, Formatting.Indented);
                File.WriteAllText(CONFIG_PATH, json, Encoding.UTF8);
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }
        }
    }
}
