using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

using Newtonsoft.Json;

namespace Intersect.Localization
{

    public abstract class LocaleCommandNamespace : LocaleNamespace
    {

        protected LocaleCommandNamespace()
        {
            var commands = GetType()
                .GetMembers(BindingFlags.Public | BindingFlags.Instance)
                .Select(
                    member =>
                    {
                        switch (member)
                        {
                            case FieldInfo fieldInfo:
                                return fieldInfo.GetValue(this) as LocaleCommand;

                            case PropertyInfo propertyInfo:
                                return propertyInfo.GetValue(this) as LocaleCommand;

                            default:
                                return null;
                        }
                    }
                )
                .Where(command => command != null)
                .ToList();

            CommandList = commands.ToImmutableList() ?? throw new InvalidOperationException();

            CommandLookup = commands.Select(
                                    command =>
                                    {
                                        if (command == null)
                                        {
                                            throw new InvalidOperationException();
                                        }

                                        return new KeyValuePair<string, LocaleCommand>(command.Name, command);
                                    }
                                )
                                .ToImmutableDictionary() ??
                            throw new InvalidOperationException();
        }

        [JsonIgnore]
        public ImmutableList<LocaleCommand> CommandList { get; }

        [JsonIgnore]
        public IDictionary<string, LocaleCommand> CommandLookup { get; }

    }

}
