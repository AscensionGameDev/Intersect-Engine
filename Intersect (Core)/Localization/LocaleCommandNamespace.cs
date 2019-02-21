using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Intersect.Localization
{
    public class LocaleCommandNamespace : LocaleNamespace
    {
        [JsonIgnore]
        [NotNull]
        public IList<LocaleCommand> Commands { get; }

        [JsonIgnore]
        [NotNull]
        public IDictionary<string, LocaleCommand> Lookup { get; }

        public LocaleCommandNamespace()
        {
            Commands = GetType()
                           .GetMembers(BindingFlags.Public | BindingFlags.Instance)
                           .Where(member => typeof(LocaleCommand).IsAssignableFrom(member.ReflectedType))
                           .Select(member =>
                           {
                               switch (member)
                               {
                                   case PropertyInfo property:
                                       return property.GetValue(this);

                                   case FieldInfo field:
                                       return field.GetValue(this);

                                   default:
                                       throw new ArgumentException(
                                           $@"Invalid command member type {member.MemberType}.",
                                           nameof(member)
                                       );
                               }
                           })
                           .Select(command => command as LocaleCommand)
                           .ToList()
                           .ToImmutableList() ?? throw new InvalidOperationException();

            Lookup = Commands
                         .Select(command =>
                         {
                             if (command == null)
                             {
                                 throw new InvalidOperationException();
                             }

                             return new KeyValuePair<string, LocaleCommand>(command.Name, command);
                         })
                         .ToImmutableDictionary() ?? throw new InvalidOperationException();
        }
    }
}