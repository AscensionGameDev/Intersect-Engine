using System.Collections.Generic;

using Intersect.Localization;

using Newtonsoft.Json;

namespace Intersect.Server.Core.CommandParsing
{

    public sealed class CommandParsingNamespace : LocaleNamespace
    {

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]        public readonly CommandParserErrorsNamespace Errors = new CommandParserErrorsNamespace();

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]        public readonly CommandParserFormattingNamespace Formatting = new CommandParserFormattingNamespace();

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]        public readonly LocaleDictionary<string, LocalizedString> TypeNames =
            new LocaleDictionary<string, LocalizedString>(
                new Dictionary<string, LocalizedString>
                {
                    {typeof(bool).Name, "bool"},
                    {typeof(byte).Name, "byte"},
                    {typeof(sbyte).Name, "sbyte"},
                    {typeof(short).Name, "short"},
                    {typeof(ushort).Name, "ushort"},
                    {typeof(int).Name, "int"},
                    {typeof(uint).Name, "uint"},
                    {typeof(long).Name, "long"},
                    {typeof(ulong).Name, "ulong"},
                    {typeof(float).Name, "float"},
                    {typeof(double).Name, "double"},
                    {typeof(decimal).Name, "decimal"},
                    {typeof(object).Name, "object"},
                    {typeof(char).Name, "char"},
                    {typeof(string).Name, "string"}
                }
            );

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]        public readonly LocalizedString TypeUnknown = @"unknown";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]        public readonly LocalizedString ValueFalse = @"false";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]        public readonly LocalizedString ValueTrue = @"true";

    }

}
