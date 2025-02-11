using System.Reflection;
using Intersect.Client.Core.Controls;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.Input;
using Intersect.Client.Interface.Shared;
using Intersect.Configuration;
using Intersect.Core;
using Intersect.Enums;
using Intersect.Framework.Reflection;
using Intersect.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Intersect.Client.Localization;


public static partial class Strings
{
    private const string StringsFileName = "client_strings.json";
    private static char[] mQuantityTrimChars = new char[] { '.', '0' };

    public static string FormatQuantityAbbreviated(long value)
    {
        if (value == 0)
        {
            return "";
        }
        else
        {
            double returnVal = 0;
            var postfix = string.Empty;

            // hundreds
            if (value <= 999)
            {
                returnVal = value;
            }

            // thousands
            else if (value >= 1000 && value <= 999999)
            {
                returnVal = value / 1000.0;
                postfix = Numbers.Thousands;
            }

            // millions
            else if (value >= 1000000 && value <= 999999999)
            {
                returnVal = value / 1000000.0;
                postfix = Numbers.Millions;
            }

            // billions
            else if (value >= 1000000000 && value <= 999999999999)
            {
                returnVal = value / 1000000000.0;
                postfix = Numbers.Billions;
            }
            else
            {
                return "OOB";
            }

            if (returnVal >= 10)
            {
                returnVal = Math.Floor(returnVal);

                return returnVal.ToString() + postfix;
            }
            else
            {
                returnVal = Math.Floor(returnVal * 10) / 10.0;

                return returnVal.ToString("F1")
                           .TrimEnd(mQuantityTrimChars)
                           .ToString()
                           .Replace(".", Numbers.Decimal) +
                       postfix;
            }
        }
    }

    private static void SynchronizeConfigurableStrings()
    {
        if (Options.Instance == default)
        {
            return;
        }

        var keyedRarityTiers = Options.Instance.Items.RarityTiers.Select((rarityName, rarity) => (rarity, rarityName));
        foreach (var (rarity, rarityName) in keyedRarityTiers)
        {
            if (!ItemDescription.Rarity.ContainsKey(rarityName))
            {
                ItemDescription.Rarity[rarityName] = $"{rarity}:{rarityName}";
            }
        }
    }

    private static void PostLoad()
    {

        Core.Program.OpenGLLink = Errors.OpenGlLink.ToString();
        Core.Program.OpenALLink = Errors.OpenAllLink.ToString();
    }

    private class OrdinalComparer : IComparer<string>
    {
        public int Compare(string? x, string? y) => string.CompareOrdinal(x, y);
    }

    private class OrdinalComparer<T> : IComparer<T>
    {
        public int Compare(T? x, T? y) => string.CompareOrdinal(x?.ToString(), y?.ToString());
    }

    public static void Load()
    {
        SynchronizeConfigurableStrings();

        try
        {
            var serialized = new Dictionary<string, Dictionary<string, object>>();
            serialized = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(
                File.ReadAllText(Path.Combine(ClientConfiguration.ResourcesDirectory, StringsFileName))
            );

            var rootType = typeof(Strings);
            var groupTypes = rootType.GetNestedTypes(BindingFlags.Static | BindingFlags.Public);
            List<string> missingStrings = [];
            List<string> argumentCountMismatch = [];
            foreach (var groupType in groupTypes.OrderBy(t => t.Name, new OrdinalComparer()))
            {
                if (!serialized.TryGetValue(groupType.Name, out var serializedGroup))
                {
                    missingStrings.Add($"{groupType.Name}");
                    serialized[groupType.Name] = SerializeGroup(groupType);
                    continue;
                }

                foreach (var fieldInfo in groupType.GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    var fieldValue = fieldInfo.GetValue(null);
                    if (!serializedGroup.TryGetValue(fieldInfo.Name, out var serializedValue))
                    {
                        var foundKey = serializedGroup.Keys.FirstOrDefault(key => string.Equals(fieldInfo.Name, key, StringComparison.OrdinalIgnoreCase));
                        if (foundKey != default)
                        {
                            _ = serializedGroup.TryGetValue(foundKey, out serializedValue);
                        }
                    }

                    switch (fieldValue)
                    {
                        case LocalizedString localizedString:
                            var jsonString = (string)serializedValue;
                            if (jsonString == default)
                            {
                                ApplicationContext.Context.Value?.Logger.LogWarning($"{groupType.Name}.{fieldInfo.Name} is null.");
                                missingStrings.Add($"{groupType.Name}.{fieldInfo.Name} (string)");
                                serializedGroup[fieldInfo.Name] = (string)localizedString;
                            }
                            else
                            {
                                LocalizedString? existingLocalizedString = null;
                                try
                                {
                                    existingLocalizedString = fieldInfo.GetValue(null) as LocalizedString;
                                }
                                catch
                                {
                                    // Ignore
                                }

                                LocalizedString newLocalizedString = new(jsonString);
                                if (existingLocalizedString?.ArgumentCount != newLocalizedString.ArgumentCount)
                                {
                                    argumentCountMismatch.Add(
                                        $"{groupType.Name}.{fieldInfo.Name} expected {existingLocalizedString?.ArgumentCount.ToString() ?? "ERROR"} argument(s) but the loaded string had {newLocalizedString.ArgumentCount}"
                                    );
                                }

                                fieldInfo.SetValue(null, newLocalizedString);
                            }
                            break;

                        case Dictionary<int, LocalizedString> intDictionary:
                            DeserializeDictionary(missingStrings, groupType, fieldInfo, fieldValue, serializedGroup, serializedValue, intDictionary);
                            break;

                        case Dictionary<string, LocalizedString> stringDictionary:
                            DeserializeDictionary(missingStrings, groupType, fieldInfo, fieldValue, serializedGroup, serializedValue, stringDictionary);
                            break;

                        default:
                            {
                                var fieldType = fieldInfo.FieldType;
                                if (!fieldType.IsGenericType || typeof(Dictionary<,>) != fieldType.GetGenericTypeDefinition())
                                {
                                    ApplicationContext.Context.Value?.Logger.LogError(
                                        new NotSupportedException(
                                            $"Unsupported localization type for {groupType.Name}.{fieldInfo.Name}: {fieldInfo.FieldType.FullName}"
                                        ),
                                        "Invalid field type {Type}",
                                        fieldType
                                    );
                                    break;
                                }

                                var parameters = fieldType.GenericTypeArguments;
                                var localizedParameterType = parameters.Last();
                                if (localizedParameterType != typeof(LocalizedString))
                                {
                                    ApplicationContext.Context.Value?.Logger.LogError(
                                        new NotSupportedException(
                                            $"Unsupported localization dictionary value type for {groupType.Name}.{fieldInfo.Name}: {localizedParameterType.FullName}"
                                        ),
                                        "Unsupported localization dictionary value type for {GroupName}.{FieldName}: {ParameterTypeName}",
                                        groupType.Name,
                                        fieldInfo.Name,
                                        localizedParameterType.FullName
                                    );
                                    break;
                                }

                                var genericDeserializeDictionary = _methodInfoDeserializeDictionary.MakeGenericMethod(parameters.First());
                                _ = genericDeserializeDictionary
                                    .Invoke(
                                        default,
                                        [
                                            missingStrings,
                                            groupType,
                                            fieldInfo,
                                            fieldValue,
                                            serializedGroup,
                                            serializedValue,
                                            fieldValue,
                                        ]
                                    );
                                break;
                            }
                    }
                }
            }

            if (missingStrings.Count > 0)
            {
                ApplicationContext.Context.Value?.Logger.LogWarning(
                    "Missing strings, overwriting strings file:\n\t{Strings}",
                    string.Join(",\n\t", missingStrings)
                );
                SaveSerialized(serialized);
            }

            if (argumentCountMismatch.Count > 0)
            {
                ApplicationContext.Context.Value?.Logger.LogWarning(
                    "Argument count mismatch on {MismatchCount} strings:\n\t{Strings}",
                    argumentCountMismatch.Count,
                    string.Join(",\n\t", argumentCountMismatch)
                );
            }
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogWarning(exception, "Error occurred while loading strings");
            Save();
        }

        PostLoad();
    }

    private static readonly MethodInfo _methodInfoDeserializeDictionary = typeof(Strings).GetMethod(
        nameof(DeserializeDictionary),
        BindingFlags.NonPublic | BindingFlags.Static
    ) ?? throw new InvalidOperationException();

    private static readonly MethodInfo _methodInfoSerializeDictionary = typeof(Strings).GetMethod(
        nameof(SerializeDictionary),
        BindingFlags.NonPublic | BindingFlags.Static
    ) ?? throw new InvalidOperationException();

    private static void DeserializeDictionary<TKey>(
        List<string> missingStrings,
        Type groupType,
        FieldInfo fieldInfo,
        object? fieldValue,
        Dictionary<string, object> serializedGroup,
        object? serializedValue,
        Dictionary<TKey, LocalizedString> dictionary
    )
    {
        switch (serializedValue)
        {
            case null:
                missingStrings.Add($"{groupType.Name}.{fieldInfo.Name} (string dictionary)");
                serializedGroup[fieldInfo.Name] = fieldValue is null ? JValue.CreateNull() : JToken.FromObject(fieldValue);
                break;

            case JObject serializedDictionary:
            {
                var keys = dictionary.Keys.ToList();
                foreach (var key in keys)
                {
                    var stringKey = key?.ToString() ??
                                    throw new InvalidOperationException(
                                        $"{key}.{nameof(key.ToString)}() returned null"
                                    );
                    if (!serializedDictionary.TryGetValue(stringKey, out var token) || token?.Type != JTokenType.String)
                    {
                        missingStrings.Add($"{groupType.Name}.{fieldInfo.Name}[{key}]");
                        serializedDictionary[stringKey] = (string)dictionary[key];
                        continue;
                    }

                    dictionary[key] = token.Value<string>();
                }
                break;
            }
        }
    }

    private static Dictionary<string, string> SerializeDictionary<TKey>(Dictionary<TKey, LocalizedString> localizedDictionary) where TKey : notnull
    {
        return localizedDictionary.OrderBy(pair => pair.Key, new OrdinalComparer<TKey>()).ToDictionary(
            pair => pair.Key.ToString() ?? throw new InvalidOperationException($"Failed to use {pair.Key} as a key"),
            pair => pair.Value.ToString()
        );
    }

    private static Dictionary<string, object> SerializeGroup(Type groupType)
    {
        var serializedGroup = new Dictionary<string, object>();
        foreach (var fieldInfo in groupType.GetFields(BindingFlags.Static | BindingFlags.Public).OrderBy(f => f.Name, new OrdinalComparer()))
        {
            var fieldValue = fieldInfo.GetValue(null);
            switch (fieldValue)
            {
                case LocalizedString localizedString:
                    serializedGroup.Add(fieldInfo.Name, localizedString.ToString());
                    break;

                case Dictionary<int, LocalizedString> localizedIntKeyDictionary:
                    serializedGroup.Add(fieldInfo.Name, SerializeDictionary(localizedIntKeyDictionary));
                    break;

                case Dictionary<string, LocalizedString> localizedStringKeyDictionary:
                    serializedGroup.Add(fieldInfo.Name, SerializeDictionary(localizedStringKeyDictionary));
                    break;

                case Dictionary<ChatboxTab, LocalizedString> localizedChatboxTabKeyDictionary:
                    serializedGroup.Add(fieldInfo.Name, SerializeDictionary(localizedChatboxTabKeyDictionary));
                    break;

                default:
                    if (fieldValue?.GetType() is {} fieldType && typeof(Dictionary<,>).ExtendedBy(fieldType))
                    {
                        var typeArguments = fieldType.GenericTypeArguments;
                        var keyType = typeArguments.First();
                        if (keyType == typeof(LocalizedString))
                        {
                            throw new InvalidOperationException(
                                $"Expected a type that is not a {typeof(LocalizedString).GetName(qualified: true)}"
                            );
                        }
                        var genericMethod = _methodInfoSerializeDictionary.MakeGenericMethod(keyType);
                        var serializationResult = genericMethod.Invoke(null, [fieldValue]);
                        if (serializationResult is Dictionary<string, string> serializedGenericDictionary)
                        {
                            serializedGroup.Add(fieldInfo.Name, serializedGenericDictionary);
                        }
                    }
                    break;
            }
        }
        return serializedGroup;
    }

    private static void SaveSerialized(Dictionary<string, Dictionary<string, object>> serialized)
    {
        var languageDirectory = Path.Combine(ClientConfiguration.ResourcesDirectory);
        if (Directory.Exists(languageDirectory))
        {
            File.WriteAllText(
                Path.Combine(languageDirectory, StringsFileName),
                JsonConvert.SerializeObject(serialized, Formatting.Indented)
            );
        }
    }

    public static void Save()
    {
        var serialized = new Dictionary<string, Dictionary<string, object>>();
        var rootType = typeof(Strings);
        var groupTypes = rootType.GetNestedTypes(BindingFlags.Static | BindingFlags.Public);

        foreach (var groupType in groupTypes.OrderBy(g => g.Name, new OrdinalComparer()))
        {
            var serializedGroup = SerializeGroup(groupType);
            serialized.Add(groupType.Name, serializedGroup);
        }

        SaveSerialized(serialized);
    }

    public partial struct Admin
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Access = @"Access:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Access0 = @"None";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Access1 = @"Moderator";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Access2 = @"Admin";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Ban = @"Ban";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString BanCaption = @"Ban {00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString BanPrompt = @"Banning {00} will not allow them to access this game for the duration you set!";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Chronological = @"123...";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ChronologicalTip = @"Order maps chronologically.";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Face = @"Face:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Kick = @"Kick";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Kill = @"Kill";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString MapList = @"Map List:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Mute = @"Mute";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString MuteCaption = @"Mute {00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString MutePrompt = @"Muting {00} will not allow them to chat in game for the duration you set!";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Name = @"Name:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString None = @"None";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString OverworldReturn = @"Leave Instance";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString SetFace = @"Set Face";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString SetPower = @"Set Power";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString SetSprite = @"Set Sprite";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Sprite = @"Sprite:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Title = @"Administration";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Unban = @"Unban";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString UnbanCaption = @"Unban {00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString UnbanPrompt = @"Are you sure that you want to unban {00}?";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Unmute = @"Unmute";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString UnmuteCaption = @"Unmute {00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString UnmutePrompt = @"Are you sure that you want to unmute {00}?";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Warp2Me = @"Warp To Me";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString WarpMe2 = @"Warp Me To";
    }

    public partial struct Bags
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString RetrieveItem = @"Retrieve Item";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString RetrieveItemPrompt = @"How many/much {00} would you like to retrieve?";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString StoreItem = @"Store Item";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString StoreItemPrompt = @"How many/much {00} would you like to store?";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Title = @"Bag";
    }

    public partial struct Bank
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString DepositItem = @"Deposit Item";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString DepositItemPrompt = @"How many/much {00} would you like to deposit?";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString NoSpace = @"There is no space left in your bank for that item!";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Title = @"Bank";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString WithdrawItem = @"Withdraw Item";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString WithdrawItemPrompt = @"How many/much {00} would you like to withdraw?";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString WithdrawItemNoSpace = @"There is no space left in your inventory for that item!";
    }

    public partial struct BanMute
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Cancel = @"Cancel";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Duration = @"Duration:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString FiveDays = @"5 days";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString FourDays = @"4 days";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Forever = @"Indefinitely";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString IncludeIp = @"Include IP:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString OneDay = @"1 day";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString OneMonth = @"1 month";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString OneWeek = @"1 week";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString OneYear = @"1 year";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Okay = @"Okay";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Reason = @"Reason:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString SixMonths = @"6 months";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ThreeDays = @"3 days";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString TwoDays = @"2 days";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString TwoMonths = @"2 months";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString TwoWeeks = @"2 weeks";
    }

    public partial struct Character
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString AttackSpeed = @"Attack Speed: {00}s";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString CooldownReduction = @"Cooldown Reduction: {00}%";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Equipment = @"Equipment:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ExtraBuffs = @"Extra Buffs";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ExtraExp = @"Bonus EXP: {00}%";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString HealthRegen = @"Health Regen: {00}%";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString LevelAndClass = @"Level: {00}, Class: {01}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Lifesteal = @"Lifesteal: {00}%";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Luck = @"Luck: {00}%";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ManaRegen = @"Mana Regen: {00}%";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Manasteal = @"Manasteal: {00}%";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Points = @"Points: {00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString StatLabelValue = @"{00}: {01}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Stats = @"Stats:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Tenacity = @"Tenacity: {00}%";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Title = @"Character Information";
    }

    public partial struct CharacterCreation
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Back = @"Back";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Class = @"Class:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Create = @"Create";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Female = @"Female";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Gender = @"Gender:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Hint = @"Customize";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Hint2 = @"Your Character";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString InvalidName = @"Character name is invalid. Please use alphanumeric characters with a length between 2 and 20.";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Male = @"Male";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Name = @"Name:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Title = @"Create a New Character";
    }

    public partial struct CharacterSelection
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Delete = @"Delete";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString DeletePrompt = @"Are you sure you want to delete {00}? This action is irreversible!";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString DeleteTitle = @"Delete {00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Empty = @"Empty Character Slot";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Info = @"Level: {00}, Class: {01}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Logout = @"Logout";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Name = @"{00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString New = @"New";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Play = @"Play";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Title = @"Select a Character to Play";
    }

    public partial struct Chatbox
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Channel = @"Channel:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ChannelAdmin = @"admin";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static Dictionary<int, LocalizedString> Channels = new Dictionary<int, LocalizedString>
        {
            {0, @"local"},
            {1, @"global"},
            {2, @"party"},
            {3, @"guild"}
        };

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ClearLogButtonToolTip = @"Clear chat log messages";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static Dictionary<ChatboxTab, LocalizedString> ChatTabButtons = new Dictionary<ChatboxTab, LocalizedString>() {
            { ChatboxTab.All, @"All" },
            { ChatboxTab.Local, @"Local" },
            { ChatboxTab.Party, @"Party" },
            { ChatboxTab.Global, @"Global" },
            { ChatboxTab.Guild, @"Guild" },
            { ChatboxTab.System, @"System" },
        };

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString EnterChat = @"Click here to chat.";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString EnterChat1 = @"Press '{00}' to chat.";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString EnterChat2 = @"Press '{00}' or '{01}' to chat.";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Send = @"Send";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Title = @"Chat";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ToggleLogButtonToolTip = @"Toggle chat log visibility";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString TooFast = @"You are chatting too fast!";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString UnableToCopy = @"It appears you are not able to copy/paste on this platform. Please make sure you have either the 'xclip' or 'wl-clipboard' packages installed if you are running Linux.";
    }

    public partial struct Combat
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString AttackWhileCastingDeny = @"You cannot attack while casting a spell.";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static Dictionary<Stat, LocalizedString> Stats = new() {
            { Stat.Attack, @"Attack" },
            { Stat.AbilityPower, @"Ability Power" },
            { Stat.Defense, @"Defense" },
            { Stat.MagicResist, @"Magic Resist" },
            { Stat.Speed, @"Speed" },
        };

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString WarningCharacterSelect = @"You are attempting to logout while in combat! Your character will remain in-game until combat has ended. Are you sure you want to logout now?";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString WarningExitDesktop = @"You are attempting to exit while in combat! Your character will remain in-game until combat has ended. Are you sure you want to quit now?";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString WarningLogout = @"You are attempting to logout while in combat! Your character will remain in-game until combat has ended. Are you sure you want to logout now?";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString WarningTitle = @"Combat Warning!";
    }

    public partial struct Content
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static Dictionary<ContentType, LocalizedString> Types = new()
        {
            { ContentType.Animation, @"Animation" },
            { ContentType.Entity, @"Entity" },
            { ContentType.Face, @"Face" },
            { ContentType.Fog, @"Fog" },
            { ContentType.Font, @"Font" },
            { ContentType.Image, @"Image" },
            { ContentType.Interface, @"Interface (gui)" },
            { ContentType.Item, @"Item" },
            { ContentType.Miscellaneous, @"Miscellaneous" },
            { ContentType.Music, @"Music" },
            { ContentType.Paperdoll, @"Paperdoll" },
            { ContentType.Resource, @"Resource" },
            { ContentType.Shader, @"Shader" },
            { ContentType.Sound, @"Sound" },
            { ContentType.Spell, @"Spell" },
            { ContentType.TextureAtlas, @"Texture Atlas" },
            { ContentType.Tileset, @"Tileset" },
        };
    }

    public partial struct Controls
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString HotkeyXLabel = @"Hot Key {00}:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static Dictionary<string, LocalizedString> KeyDictionary = new()
        {
            {"attackinteract", @"Attack/Interact:"},
            {"block", @"Block:"},
            {"autotarget", @"Auto Target:"},
            {"enter", @"Chat:"},
            {nameof(Control.HoldToSoftRetargetOnSelfCast).ToLowerInvariant(), @"Hold to Soft-Retarget on Self-Cast:"},
            {"hotkey1", @"Hot Key 1:"},
            {"hotkey2", @"Hot Key 2:"},
            {"hotkey3", @"Hot Key 3:"},
            {"hotkey4", @"Hot Key 4:"},
            {"hotkey5", @"Hot Key 5:"},
            {"hotkey6", @"Hot Key 6:"},
            {"hotkey7", @"Hot Key 7:"},
            {"hotkey8", @"Hot Key 8:"},
            {"hotkey9", @"Hot Key 9:"},
            {"hotkey10", @"Hot Key 10:"},
            {"movedown", @"Down:"},
            {"moveleft", @"Left:"},
            {"moveright", @"Right:"},
            {"moveup", @"Up:"},
            {"pickup", @"Pick Up:"},
            {"screenshot", @"Screenshot:"},
            {"openmenu", @"Open Menu:"},
            {"openinventory", @"Open Inventory:"},
            {"openquests", @"Open Quests:"},
            {"opencharacterinfo", @"Open Character Info:"},
            {"openparties", @"Open Parties:"},
            {"openspells", @"Open Spells:"},
            {"openfriends", @"Open Friends:"},
            {"openguild", @"Open Guild:"},
            {"opensettings", @"Open Settings:"},
            {"opendebugger", @"Open Debugger:"},
            {"openadminpanel", @"Open Admin Panel:"},
            {"togglegui", @"Toggle Interface:"},
            {"turnaround", @"Hold to turn around:"},
            {"togglezoomin", "Toggle Zoom In:"},
            {"holdtozoomin", "Hold to Zoom In:"},
            {"togglezoomout", "Toggle Zoom Out:"},
            {"holdtozoomout", "Hold to Zoom Out:"},
            {"togglefullscreen", "Toggle Fullscreen:"},
            {nameof(Control.ToggleAutoSoftRetargetOnSelfCast).ToLowerInvariant(), "Toggle Auto Soft-Retarget on Self-Cast:"},
        };

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Listening = @"Listening";

    }

    public partial struct Crafting
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Craft = @"Craft 1";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString CraftAll = @"Craft {00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString CraftChance = @"Chance of failure: {00}%";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString CraftingTime = "Crafting time: {00}s";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString CraftStop = "Stop";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString DestroyMaterialsChance = @"Chance to destroy materials: {00}%";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString IncorrectResources = @"You do not have the necessary resources to craft this item.";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Ingredients = @"Required:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Product = @"Crafting:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Recipes = @"Recipes:";
    }

    public partial struct Credits
    {

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Back = @"Back to Main Menu";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Title = @"Credits";

    }

    public partial struct Debug
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ControlUnderCursor = @"Control Under Cursor";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString DrawDebugOutlines = @"Draw Debug Outlines";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Draws = @"Draws";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString EnableLayoutHotReloading = @"Enable Experimental Layout Hot Reloading";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString IncludeTextNodesInHover = @"Include Text Nodes in Hover";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ViewClickedNodeInDebugger = @"View Clicked Node in Debugger";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ViewClickedNodeInDebuggerTooltip = @"Hold Alt to temporarily turn this off (so you can turn off this option)";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString EntitiesDrawn = @"Entities Drawn";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Fps = @"FPS";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString InterfaceObjects = @"Interface Objects";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString KnownEntities = @"Known Entities";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString KnownMaps = @"Known Maps";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString LightsDrawn = @"Lights Drawn";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Map = @"Map";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString MapsDrawn = @"Maps Drawn";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Ping = @"Ping";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ShutdownServer = @"Shutdown Server";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ShutdownServerAndExit = @"Shutdown Server and Exit";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString FormatTextureFromAtlas = @"{00} (from atlas)";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Time = @"Time";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Title = @"Debug";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString TabLabelInfo = @"Info";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString TabLabelAssets = @"Assets";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ReloadAsset = @"Reload Asset";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString AssetsSearchPlaceholder = @"Filter assets...";
    }

    public partial struct EntityBox
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Exp = @"EXP:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ExpValue = @"{00} / {01}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Level = @"Lv. {00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Map = @"{00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString MaxLevel = @"Max Level";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString NameAndLevel = @"{00}    {01}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Vital0 = @"HP:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Vital0Value = @"{00} / {01}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Vital1 = @"MP:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Vital1Value = @"{00} / {01}";
    }

    public partial struct Alerts
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString FallbackTitle = @"Alert";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static Dictionary<AlertType, LocalizedString> Titles = new()
        {
            { AlertType.Error, @"Error" },
            { AlertType.Warning, @"Warning" },
        };
    }

    public partial struct Errors
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString DisconnectionEvent = @"Please provide this message to your administrator: {0}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString DisplayNotSupported = @"Invalid Display Configuration!";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString DisplayNotSupportedError = @"Fullscreen {00} resolution is not supported on this device!";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString HostNotFound = @"DNS resolution error, please report this to the game administrator.";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString LoadFile = @"Failed to load a {00}. Please send the game administrator a copy of your errors log file in the logs directory.";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString LostConnection = @"Lost connection to the game server. Please make sure you're connected to the internet and try again!";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString NotConnected = @"Not connected to the game server. Is it online?";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString OpenAllLink = @"https://goo.gl/Nbx6hx";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString OpenGlLink = @"https://goo.gl/RSP3ts";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString PasswordInvalid = @"Password is invalid. Please use alphanumeric characters with a length between 4 and 20.";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ResourcesNotFound = @"The resources directory could not be found! Intersect will now close.";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Title = @"Error!";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString UsernameInvalid = @"Username is invalid. Please use alphanumeric characters with a length between 2 and 20.";
    }

    public partial struct Words
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString LcaseSound = @"sound";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString LcaseSprite = @"sprite";
    }

    public partial struct EventWindow
    {

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Continue = @"Continue";

    }

    public partial struct ForgotPass
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Back = @"Back";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Hint = @"If your account exists, we will send you a temporary password reset code.";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Label = @"Enter your username or email below:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Submit = @"Submit";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Title = @"Password Reset";
    }

    public partial struct Friends
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString AddFriend = @"Add Friend";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString AddFriendPrompt = @"Who would you like to add as a friend?";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString InFight = @"You are currently in a fight!";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Offline = @"Offline";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString RemoveFriend = @"Remove Friend";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString RemoveFriendPrompt = @"Do you wish to remove {00} from your friends list?";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Request = @"Friend Request";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString RequestPrompt = @"{00} has sent you a friend request. Do you accept?";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Title = @"Friends";
    }

    public partial struct GameMenu
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Character = @"Character Information";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Friends = @"Friends";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Items = @"Inventory";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Menu = @"Open Menu";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Party = @"Party";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Quest = @"Quest Log";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Spells = @"Spell Book";
    }

    public partial struct General
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString MapItemStackable = @"{01} {00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString None = @"None";
    }

    public partial struct Guilds
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Add = @"+";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Bank = @"{00} Guild Bank";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Demote = @"Demote to {00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString DemotePrompt = @"Are you sure you want to demote {00} to rank {01}?";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString DemoteTitle = @"Demote Guild Member";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Guild = @"Guild";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString GuildTip = @"Send {00} an invite to your guild.";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Invite = @"Invite";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString InviteAlreadyInGuild = @"The player you're trying to invite is already in a guild or has a pending invite.";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString InviteMemberPrompt = @"Who would you like to invite to {00}?";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString InviteMemberTitle = @"Invite Player";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString InviteRequestPrompt = @"{00} would like to invite you to join their guild, {01}. Do you accept?";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString InviteRequestPromptMissingGuild = @"{00} would like to invite you to join their guild. Do you accept?";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString InviteRequestTitle = @"Guild Invite";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Kick = @"Kick";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString KickPrompt = @"Are you sure you want to kick {00}?";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString KickTitle = @"Kick Guild Member";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Leave = "Leave";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString LeavePrompt = @"Are you sure you want to leave your guild?";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString LeaveTitle = @"Leave Guild";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString NotAllowedDeposit = @"You do not have permission to deposit items into {00}'s guild bank.";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString NotAllowedSwap = @"You do not have permission to swap items within {00}'s guild bank.";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString NotAllowedWithdraw = @"You do not have permission to withdraw from {00}'s guild bank.";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString NotInGuild = @"You are not currently in a guild!";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString OfflineListEntry = @"[{00}] {01} - {02}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString OnlineListEntry = @"[{00}] {01} - {02}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString PM = @"PM";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Promote = @"Promote to {00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString PromotePrompt = @"Are you sure you want to promote {00} to rank {01}?";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString PromoteTitle = @"Promote Guild Member";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Tooltip = @"Lv. {00} {01}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Transfer = @"Transfer";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString TransferPrompt = @"This action will completely transfer all ownership of your guild to {00} and you will lose your rank of {01}. If you are sure you want to hand over your guild enter '{02}' below.";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString TransferTitle = @"Transfer Guild";
    }

    public partial struct InputBox
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Cancel = @"Cancel";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString No = @"No";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Okay = @"Okay";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Yes = @"Yes";
    }

    public partial struct MapItemWindow
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString LootButton = @"Loot All";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Title = @"Loot";
    }

    public partial struct Internals
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Bounds = @"Bounds";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Color = @"Color";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ColorOverride = @"Color Override";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString CoordinateX = @"X";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString CoordinateY = @"Y";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString CoordinateZ = @"Z";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ExperimentalFeatureTooltip = @"This feature is experimental and may cause issues when enabled.";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString GlobalItem = @"Global {00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString LocalItem = @"Local {00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Name = @"Name";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString NotApplicable = @"N/A";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ResolutionXByY = @"{0}x{1}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Type = @"Type";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString InnerBounds = @"Inner Bounds";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Margin = @"Margin";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Padding = @"Padding";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Dock = @"Dock";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Alignment = @"Alignment";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString TextAlign = @"Text Align";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString OuterBounds = @"Outer Bounds";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Font = @"Font";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString AutoSizeToContents = @"Auto Size-to-Conents";
    }

    public partial struct Inventory
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString DropItemPrompt = @"How many units of {00} would you like to discard?";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString DropItemTitle = @"Drop Item";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString DropPrompt = @"Do you wish to drop the item: {00}?";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString EquippedSymbol = "E";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Title = @"Inventory";
    }

    public partial struct EntityContextMenu
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString AddFriend = @"Add Friend";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString InviteToGuild = @"Invite to Guild";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString InviteToParty = @"Invite to Party";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString PrivateMessage = @"Private Message";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Trade = @"Trade";
    }

    public partial struct ItemContextMenu
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Bag = @"Bag {00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Bank = @"Bank {00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Cast = @"Cast {00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Drop = @"Drop {00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Equip = @"Equip {00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Learn = @"Learn {00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Open = @"Open {00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Sell = @"Sell {00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Trade = @"Offer {00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Unequip = @"Unequip {00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Use = @"Use {00}";
    }

    public partial struct SpellContextMenu
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Cast = @"Cast {00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Forget = @"Forget {00}";
    }

    public partial struct BankContextMenu
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Withdraw = @"Withdraw {00}";
    }

    public partial struct BagContextMenu
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Withdraw = @"Withdraw {00}";
    }

    public partial struct TradeContextMenu
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Withdraw = @"Revoke {00}";
    }

    public partial struct ChatContextMenu
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString FriendInvite = @"Send Friend request to {00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString GuildInvite = @"Invite {00} to Guild";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString PartyInvite = @"Invite {00} to Party";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString PM = @"Send Private Message to {00}";
    }

    public partial struct ShopContextMenu
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Buy = @"Buy {00}";
    }

    public partial struct ItemDescription
    {
        // String Properties (A - Z):

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Amount = @"Amount:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString AttackSpeed = @"Attack Speed:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString BagSlots = @"Bag Slots:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Bagged = @"Bagged";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Banked = @"Banked";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString BaseDamage = @"Base Damage:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString BaseDamageType = @"Damage Type:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString BlockAbsorption = @"Block Absorption:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString BlockAmount = @"Block Amount:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString BlockChance = @"Block Chance:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString CastSpell = @"Casts Spell: {00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString CritChance = @"Critical Chance:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString CritMultiplier = @"Critical Multiplier:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Description = @"{00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString DropOnDeath = @"Drop chance on death:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Dropped = @"Dropped";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString GuildBanked = @"Guild Banked";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ItemLimits = @"Cannot be {00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Multiplier = @"{00}x";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Percentage = @"{00}%";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString RegularAndPercentage = @"{00} + {01}%";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ScalingPercentage = @"Scaling Percentage:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ScalingStat = @"Scaling Stat:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString SingleUse = @"Single use";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Sold = @"Sold";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString StatGrowthRange = @"{00} to {01}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString TeachSpell = @"Teaches Spell: {00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Traded = @"Traded";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString TwoHand = @"2H";

        // Integer Dictionaries (A - Z):

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static Dictionary<int, LocalizedString> BonusEffects = new Dictionary<int, LocalizedString>
        {
            {0, @""},
            {1, @"Cooldown Reduction:"},
            {2, @"Lifesteal:"},
            {3, @"Tenacity:"},
            {4, @"Luck:"},
            {5, @"Bonus Experience:"},
            {6, @"Manasteal:"},
        };

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static Dictionary<int, LocalizedString> ConsumableTypes = new Dictionary<int, LocalizedString>()
        {
            {0, "Restores HP:"},
            {1, "Restores MP:"},
            {2, "Grants Experience:"},
        };

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static Dictionary<int, LocalizedString> DamageTypes = new Dictionary<int, LocalizedString>()
        {
            {0, @"Physical"},
            {1, @"Magic"},
            {2, @"True"},
        };

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static Dictionary<int, LocalizedString> ItemTypes = new Dictionary<int, LocalizedString>
        {
            {0, @"None"},
            {1, @"Equipment"},
            {2, @"Consumable"},
            {3, @"Currency"},
            {4, @"Spell"},
            {5, @"Special"},
            {6, @"Bag"},
        };

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static Dictionary<int, LocalizedString> StatCounts = new Dictionary<int, LocalizedString>
        {
            {0, @"Attack:"},
            {1, @"Ability Power:"},
            {2, @"Defense:"},
            {3, @"Magic Resist:"},
            {4, @"Speed:"},
        };

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static Dictionary<int, LocalizedString> Stats = new Dictionary<int, LocalizedString>
        {
            {0, @"Attack"},
            {1, @"Ability Power"},
            {2, @"Defense"},
            {3, @"Magic Resist"},
            {4, @"Speed"},
        };

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static Dictionary<int, LocalizedString> Vitals = new Dictionary<int, LocalizedString>
        {
            {0, @"HP:"},
            {1, @"MP:"},
        };

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static Dictionary<int, LocalizedString> VitalsRegen = new Dictionary<int, LocalizedString>
        {
            {0, @"HP Regen:"},
            {1, @"MP Regen:"},
        };

        // String Dictionaries (A - Z):

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static Dictionary<string, LocalizedString> Rarity = new Dictionary<string, LocalizedString>
        {
            {"None", @"None"},
            {"Common", @"Common"},
            {"Uncommon", @"Uncommon"},
            {"Rare", @"Rare"},
            {"Epic", @"Epic"},
            {"Legendary", @"Legendary"},
        };
    }

    public partial struct Keys
    {
        public static string FormatKeyName(Framework.GenericClasses.Keys modifier, Framework.GenericClasses.Keys key)
        {
            var formatted = KeyDictionary[Enum.GetName(typeof(Framework.GenericClasses.Keys), key).ToLower()];

            if (modifier != Framework.GenericClasses.Keys.None)
            {
                var modifierName = KeyDictionary[Enum.GetName(typeof(Framework.GenericClasses.Keys), modifier).ToLower()];
                formatted = KeyNameWithModifier.ToString(modifierName, formatted);
            }

            return formatted;
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static Dictionary<string, LocalizedString> KeyDictionary = new Dictionary<string, LocalizedString>()
        {
            {"a", @"A"},
            {"add", @"Add"},
            {"alt", @"Alt"},
            {"apps", @"Apps"},
            {"attn", @"Attn"},
            {"b", @"B"},
            {"back", @"Back"},
            {"browserback", @"BrowserBack"},
            {"browserfavorites", @"BrowserFavorites"},
            {"browserforward", @"BrowserForward"},
            {"browserhome", @"BrowserHome"},
            {"browserrefresh", @"BrowserRefresh"},
            {"browsersearch", @"BrowserSearch"},
            {"browserstop", @"BrowserStop"},
            {"c", @"C"},
            {"cancel", @"Cancel"},
            {"capital", @"Capital"},
            {"capslock", @"CapsLock"},
            {"clear", @"Clear"},
            {"control", @"Control"},
            {"controlkey", @"ControlKey"},
            {"crsel", @"Crsel"},
            {"d", @"D"},
            {"d0", @"0"},
            {"d1", @"1"},
            {"d2", @"2"},
            {"d3", @"3"},
            {"d4", @"4"},
            {"d5", @"5"},
            {"d6", @"6"},
            {"d7", @"7"},
            {"d8", @"8"},
            {"d9", @"9"},
            {"decimal", @"Decimal"},
            {"delete", @"Delete"},
            {"divide", @"Divide"},
            {"down", @"Down"},
            {"e", @"E"},
            {"end", @"End"},
            {"enter", @"Enter"},
            {"eraseeof", @"EraseEof"},
            {"escape", @"Escape"},
            {"execute", @"Execute"},
            {"exsel", @"Exsel"},
            {"f", @"F"},
            {"f1", @"F1"},
            {"f10", @"F10"},
            {"f11", @"F11"},
            {"f12", @"F12"},
            {"f13", @"F13"},
            {"f14", @"F14"},
            {"f15", @"F15"},
            {"f16", @"F16"},
            {"f17", @"F17"},
            {"f18", @"F18"},
            {"f19", @"F19"},
            {"f2", @"F2"},
            {"f20", @"F20"},
            {"f21", @"F21"},
            {"f22", @"F22"},
            {"f23", @"F23"},
            {"f24", @"F24"},
            {"f3", @"F3"},
            {"f4", @"F4"},
            {"f5", @"F5"},
            {"f6", @"F6"},
            {"f7", @"F7"},
            {"f8", @"F8"},
            {"f9", @"F9"},
            {"finalmode", @"FinalMode"},
            {"g", @"G"},
            {"h", @"H"},
            {"hanguelmode", @"HanguelMode"},
            {"hangulmode", @"HangulMode"},
            {"hanjamode", @"HanjaMode"},
            {"help", @"Help"},
            {"home", @"Home"},
            {"i", @"I"},
            {"imeaccept", @"IMEAccept"},
            {"imeaceept", @"IMEAceept"},
            {"imeconvert", @"IMEConvert"},
            {"imemodechange", @"IMEModeChange"},
            {"imenonconvert", @"IMENonconvert"},
            {"insert", @"Insert"},
            {"j", @"J"},
            {"junjamode", @"JunjaMode"},
            {"k", @"K"},
            {"kanamode", @"KanaMode"},
            {"kanjimode", @"KanjiMode"},
            {"keycode", @"KeyCode"},
            {"l", @"L"},
            {"launchapplication1", @"LaunchApplication1"},
            {"launchapplication2", @"LaunchApplication2"},
            {"launchmail", @"LaunchMail"},
            {"lbutton", @"Left Mouse"},
            {"lcontrolkey", @"L Control"},
            {"left", @"Left"},
            {"linefeed", @"LineFeed"},
            {"lmenu", @"L Alt"},
            {"lshiftkey", @"L Shift"},
            {"lwin", @"LWin"},
            {"m", @"M"},
            {"mbutton", @"Middle Mouse"},
            {"medianexttrack", @"MediaNextTrack"},
            {"mediaplaypause", @"MediaPlayPause"},
            {"mediaprevioustrack", @"MediaPreviousTrack"},
            {"mediastop", @"MediaStop"},
            {"menu", @"Menu"},
            {"modifiers", @"Modifiers"},
            {"multiply", @"Multiply"},
            {"n", @"N"},
            {"next", @"Next"},
            {"noname", @"NoName"},
            {"none", @"None"},
            {"numlock", @"NumLock"},
            {"numpad0", @"NumPad 0"},
            {"numpad1", @"NumPad 1"},
            {"numpad2", @"NumPad 2"},
            {"numpad3", @"NumPad 3"},
            {"numpad4", @"NumPad 4"},
            {"numpad5", @"NumPad 5"},
            {"numpad6", @"NumPad 6"},
            {"numpad7", @"NumPad 7"},
            {"numpad8", @"NumPad 8"},
            {"numpad9", @"NumPad 9"},
            {"o", @"O"},
            {"oem1", @"1"},
            {"oem102", @"0"},
            {"oem2", @"2"},
            {"oem3", @"3"},
            {"oem4", @"4"},
            {"oem5", @"5"},
            {"oem6", @"6"},
            {"oem7", @"7"},
            {"oem8", @"8"},
            {"oembackslash", @"\"},
            {"oemclear", @"OemClear"},
            {"oemclosebrackets", @"]"},
            {"oemcomma", @","},
            {"oemminus", @"-"},
            {"oemopenbrackets", @"["},
            {"oemperiod", @"."},
            {"oempipe", @"|"},
            {"oemplus", @"+"},
            {"oemquestion", @"?"},
            {"oemquotes", @"'"},
            {"oemsemicolon", @"OemSemicolon"},
            {"oemtilde", @"`"},
            {"p", @"P"},
            {"pa1", @"Pa1"},
            {"packet", @"Packet"},
            {"pagedown", @"Page Down"},
            {"pageup", @"Page Up"},
            {"pause", @"Pause"},
            {"play", @"Play"},
            {"print", @"Print"},
            {"printscreen", @"PrintScreen"},
            {"prior", @"Prior"},
            {"processkey", @"ProcessKey"},
            {"q", @"Q"},
            {"r", @"R"},
            {"rbutton", @"Right Mouse"},
            {"rcontrolkey", @"R Control"},
            {"return", @"Return"},
            {"right", @"Right"},
            {"rmenu", @"R Alt"},
            {"rshiftkey", @"R Shift"},
            {"rwin", @"RWin"},
            {"s", @"S"},
            {"scroll", @"Scroll"},
            {"select", @"Select"},
            {"selectmedia", @"SelectMedia"},
            {"separator", @"Separator"},
            {"shift", @"Shift"},
            {"shiftkey", @"ShiftKey"},
            {"sleep", @"Sleep"},
            {"snapshot", @"Snapshot"},
            {"space", @"Space"},
            {"subtract", @"Subtract"},
            {"t", @"T"},
            {"tab", @"Tab"},
            {"u", @"U"},
            {"up", @"Up"},
            {"v", @"V"},
            {"volumedown", @"VolumeDown"},
            {"volumemute", @"VolumeMute"},
            {"volumeup", @"VolumeUp"},
            {"w", @"W"},
            {"x", @"X"},
            {"xbutton1", @"XButton1"},
            {"xbutton2", @"XButton2"},
            {"y", @"Y"},
            {"z", @"Z"},
            {"zoom", @"Zoom"},
        };

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString KeyNameWithModifier = @"{00} + {01}";

    }

    public partial struct LoginWindow
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Back = @"Back";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ForgotPassword = @"Forgot Password?";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Login = @"Login";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Password = @"Password";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString SavePassword = @"Save Password";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Title = @"Login";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Username = @"Username";
    }

    public partial struct Main
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString GameName = @"Intersect";
    }

    public partial struct MainMenu
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Credits = @"Credits";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Exit = @"Exit";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Login = @"Login";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Register = @"Register";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Settings = @"Settings";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString SettingsTooltip = @"";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Start = @"Start";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Title = @"Main Menu";
    }

    public partial struct Settings
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Apply = @"Apply";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString AudioSettingsTab = @"Audio";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString AutoCloseWindows = @"Auto-close Windows";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString AutoToggleChatLog = @"Auto-toggle chat log visibility";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Cancel = @"Cancel";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString EnableLighting = @"Enable Light Effects";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString FormatResolution = @"{00}x{01}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString FormatZoom = @"{00}x";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Fps120 = @"120";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Fps30 = @"30";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Fps60 = @"60";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Fps90 = @"90";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Fullscreen = @"Fullscreen";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString GameSettingsTab = @"Game";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString InformationSettings = @"Information";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString InterfaceSettings = @"Interface";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ControlsTab = @"Controls";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Language = @"Language";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString MusicVolume = @"Music Volume: {00}%";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString SectionVolume = @"Volume";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString VolumeMusic = @"Music";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString VolumeSoundEffects = @"Sound Effects";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Resolution = @"Resolution";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ResolutionCustom = @"Custom Resolution";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Restore = @"Restore Defaults";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString SoundVolume = @"Sound Volume: {00}%";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ShowExperienceAsPercentage = @"Show experience as percentage";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ShowFriendOverheadHpBar = @"Show friends overhead HP bar";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ShowFriendOverheadInformation = @"Show friends overhead information";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ShowGuildOverheadHpBar = @"Show guild member overhead HP bar";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ShowGuildOverheadInformation = @"Show guild member overhead information";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ShowHealthAsPercentage = @"Show health as percentage";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ShowNpcOverheadHpBar = @"Show NPC overhead HP bar";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ShowNpcOverheadInformation = @"Show NPC overhead information";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ShowManaAsPercentage = @"Show mana as percentage";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ShowMyOverheadHpBar = @"Show my overhead HP bar";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ShowMyOverheadInformation = @"Show my overhead information";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ShowPartyOverheadHpBar = @"Show party member overhead HP bar";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ShowPartyOverheadInformation = @"Show party member overhead information";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ShowPlayerOverheadHpBar = @"Show players overhead HP bar";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ShowPlayerOverheadInformation = @"Show players overhead information";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString SimplifiedEscapeMenu = @"Simplified escape menu";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString StickyTarget = @"Sticky Target";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString FPS = @"FPS";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString TargetingSettings = @"Targeting";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString AutoTurnToTarget = @"Auto-turn to target";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString AutoSoftRetargetOnSelfCast = @"Auto Soft-Retarget on Self-Cast";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString AutoSoftRetargetOnSelfCastTooltip =
            @"When this is enabled and an enemy is targeted and a single-target friendly spell is used, the spell will be self-cast without removing the target on the enemy.";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Title = @"Settings";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString TypewriterText = @"Typewriter Text";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString UIScale = @"UI Scale";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString UnlimitedFps = @"No Limit";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString VideoSettingsTab = @"Video";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Vsync = @"V-Sync";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString WorldScale = @"World Scale";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString WorldScaleTooltip =
            @"World Scale is only available after connecting to the server.";
    }

    public partial struct Parties
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString InFight = @"You are currently fighting!";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString InvitePrompt = @"{00} has invited you to their party. Do you accept?";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Kick = @"Kick {00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString KickLabel = @"Kick";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Leader = @"Leader";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString LeaderTip = @"Party Leader";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Leave = @"Leave Party";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString LeaveTip = @"Leave Tip";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Name = @"{00} - Lv. {01}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString PartyInvite = @"Party Invite";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Title = @"Party";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Vital0 = @"HP:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Vital0Value = @"{00} / {01}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Vital1 = @"MP:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Vital1Value = @"{00} / {01}";
    }

    public partial struct QuestLog
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Abandon = @"Abandon";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString AbandonPrompt = @"Are you sure that you want to quit the quest ""{00}""?";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString AbandonTitle = @"Abandon Quest: {00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Back = @"Back";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Completed = @"Quest Completed";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString CurrentTask = @"Current Task:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString InProgress = @"Quest In Progress";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString NotStarted = @"Quest Not Started";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString TaskItem = @"{00}/{01} {02}(s) gathered.";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString TaskNpc = @"{00}/{01} {02}(s) slain.";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Title = @"Quest Log";
    }

    public partial struct QuestOffer
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Accept = @"Accept";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Decline = @"Decline";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Title = @"Quest Offer";
    }

    public partial struct Regex
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Email =
            @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Password = @"^[-_=\+`~!@#\$%\^&\*()\[\]{}\\|;\:'"",<\.>/\?a-zA-Z0-9]{4,64}$";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Username = @"^[a-zA-Z0-9]{2,20}$";
    }

    public partial struct Registration
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Back = @"Back";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ConfirmPassword = @"Confirm Password:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Email = @"Email:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString EmailInvalid = @"The email is invalid.";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Password = @"Password:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString PasswordMismatch = @"The passwords do not match.";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Register = @"Register";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Title = @"Register";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Username = @"Username:";
    }

    public partial struct ResetPass
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Back = @"Cancel";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Code = @"Enter the reset code that was sent to you:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ConfirmPassword = @"Confirm Password:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Error = @"Error!";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ErrorMessage =
            @"The reset code was not valid, has expired, or the account does not exist!";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString InputCode = @"Please enter your password reset code.";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString NewPassword = @"New Password:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Submit = @"Submit";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Success = @"Success!";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString SuccessMessage = @"Your password has been reset!";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Title = @"Password Reset";
    }

    public partial struct Server
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Connecting = @"Connecting...";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Failed = @"Network Error";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString HandshakeFailure = @"Handshake Error";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Offline = @"Offline";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Online = @"Online";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ServerFull = @"Server is Full";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString StatusLabel = @"Server Status: {00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Unknown = @"Unknown";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString VersionMismatch = @"Version Mismatch";
    }

    public partial struct Shop
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString BuyItem = @"Buy Item";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString BuyItemPrompt = @"How many units of {00} would you like to buy?";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString CannotSell = @"This shop does not accept that item!";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Costs = @"Costs {00} {01}(s)";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString SellItem = @"Sell Item";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString SellItemPrompt = @"How many units of {00} would you like to sell?";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString SellPrompt = @"Do you wish to sell the item: {00}?";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString SellsFor = @"Sells for {00} {01}(s)";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString WontBuy = @"Shop Will Not Buy This Item";
    }

    public partial struct SpellDescription
    {
        // String Properties (A - Z):

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Bound = @"Cannot be unlearned.";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString CastTime = @"Cast Time:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Cooldown = @"Cooldown:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString CooldownGroup = @"Cooldown Group:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString CritChance = @"Critical Chance:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString CritMultiplier = @"Critical Multiplier:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString DamageType = @"Damage Type:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Distance = @"Distance:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString DoT = @"Damages over Time";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Duration = @"Duration:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Effect = @"Effect:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Friendly = @"Support Spell";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString HoT = @"Heals over Time";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString IgnoreConsumedResourceBlock = @"Ignores Consumed Resources";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString IgnoreCooldownReduction = @"Ignores Cooldown Reduction";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString IgnoreGlobalCooldown = @"Ignores Global Cooldown";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString IgnoreMapBlock = @"Ignores Map Blocks";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString IgnoreResourceBlock = @"Ignores Active Resources";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString IgnoreZDimension = @"Ignores Height Differences";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Instant = @"Instant";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Multiplier = @"{00}x";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Percentage = @"{00}%";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString RegularAndPercentage = @"{00} + {01}%";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ScalingPercentage = @"Scaling Percentage:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ScalingStat = @"Scaling Stat:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString StatBuff = @"Stat Buff";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Tick = @"Ticks every:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Tiles = @"{00} Tiles";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Unfriendly = @"Damaging Spell";


        // Integer Dictionaries (A - Z):

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static Dictionary<int, LocalizedString> DamageTypes = new Dictionary<int, LocalizedString>
        {
            {0, @"Physical"},
            {1, @"Magic"},
            {2, @"True"},
        };

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static Dictionary<int, LocalizedString> Effects = new Dictionary<int, LocalizedString>
        {
            {0, @""},
            {1, @"Silence"},
            {2, @"Stun"},
            {3, @"Snare"},
            {4, @"Blind"},
            {5, @"Stealth"},
            {6, @"Transforms"},
            {7, @"Cleanse"},
            {8, @"Invulnerable"},
            {9, @"Shield"},
            {10, @"Sleep"},
            {11, @"On-Hit"},
            {12, @"Taunt"},
        };

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static Dictionary<int, LocalizedString> SpellTypes = new Dictionary<int, LocalizedString>
        {
            {0, @"Combat Spell"},
            {1, @"Warp to Map"},
            {2, @"Warp to Target"},
            {3, @"Dash"},
            {4, @"Special"},
        };

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static Dictionary<int, LocalizedString> Stats = new Dictionary<int, LocalizedString>
        {
            {0, @"Attack"},
            {1, @"Ability Power"},
            {2, @"Defense"},
            {3, @"Magic Resist"},
            {4, @"Speed"},
        };

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static Dictionary<int, LocalizedString> StatCounts = new Dictionary<int, LocalizedString>
        {
            {0, @"Attack:"},
            {1, @"Ability Power:"},
            {2, @"Defense:"},
            {3, @"Magic Resist:"},
            {4, @"Speed:"},
        };

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static Dictionary<int, LocalizedString> TargetTypes = new Dictionary<int, LocalizedString>
        {
            {0, @"Self Cast"},
            {1, @"Targetted - Range: {00} Tiles"},
            {2, @"AOE"},
            {3, @"Projectile - Range: {00} Tiles"},
            {4, @"On Hit"},
            {5, @"Trap"},
        };

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static Dictionary<int, LocalizedString> VitalCosts = new Dictionary<int, LocalizedString>
        {
            {0, @"HP Cost:"},
            {1, @"MP Cost:"},
        };

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static Dictionary<int, LocalizedString> VitalDamage = new Dictionary<int, LocalizedString>
        {
            {0, @"HP Damage:"},
            {1, @"MP Damage:"},
        };

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static Dictionary<int, LocalizedString> VitalRecovery = new Dictionary<int, LocalizedString>
        {
            {0, @"HP Recovery:"},
            {1, @"MP Recovery:"},
        };
    }

    public partial struct Spells
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ForgetSpell = @"Forget Spell";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ForgetSpellPrompt = @"Are you sure you want to forget {00}?";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Title = @"Spells";
    }

    public partial struct Trading
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Accept = @"Accept";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString InFight = @"You are currently fighting!";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString OfferItem = @"Offer Item";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString OfferItemPrompt = @"How many units of {00} would you like to offer?";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Pending = @"Pending";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString RequestPrompt =
            @"{00} has invited you to trade items with them. Do you accept?";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString RevokeItem = @"Revoke Item";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString RevokeItemPrompt = @"How many units of {00} would you like to revoke?";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString TheirOffer = @"Their Offer:";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Title = @"Trading with {00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString TradeRequest = @"Trading Invite";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Value = @"Value: {00}";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString YourOffer = @"Your Offer:";
    }

    public partial struct EscapeMenu
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString CharacterSelect = @"Characters";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Close = @"Close";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString ExitToDesktop = @"Desktop";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Logout = @"Logout";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Settings = @"Settings";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Title = @"Menu";
    }

    public partial struct Numbers
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Billions = "b";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Comma = ",";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Decimal = ".";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Millions = "m";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Thousands = "k";
    }

    public partial struct Update
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Checking = @"Checking for updates, please wait.";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Done = @"Update complete! Launching game.";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Error = @"Update Error! Check logs for more information.";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString FilesRemaining = @"{00} Files Remaining";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString PercentComplete = @"{00}%";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString RemainingSize = @"{00} Left";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Restart = @"Update complete! Relaunch {00} to play.";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString Updating = @"Downloading updates, please wait.";
    }

    public partial struct GameWindow
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public static LocalizedString EntityNameAndLevel = @"{00} [Lv. {01}]";
    }

}
