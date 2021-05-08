using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intersect.Enums;
using Intersect.Localization;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Intersect.Client.Localization
{

    public static partial class Strings
    {

        private static char[] mQuantityTrimChars = new char[] {'.', '0'};

        public static string FormatQuantityAbbreviated(long value)
        {
            if (value == 0)
            {
                return "";
            }
            else
            {
                double returnVal = 0;
                var postfix = "";

                // hundreds
                if (value <= 999)
                {
                    returnVal = value;
                }

                // thousands
                else if (value >= 1000 && value <= 999999)
                {
                    returnVal = value / 1000.0;
                    postfix = Strings.Numbers.thousands;
                }

                // millions
                else if (value >= 1000000 && value <= 999999999)
                {
                    returnVal = value / 1000000.0;
                    postfix = Strings.Numbers.millions;
                }

                // billions
                else if (value >= 1000000000 && value <= 999999999999)
                {
                    returnVal = value / 1000000000.0;
                    postfix = Strings.Numbers.billions;
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
                               .Replace(".", Strings.Numbers.dec) +
                           postfix;
                }
            }
        }

        public static void Load()
        {
            if (File.Exists(Path.Combine("resources", "client_strings.json")))
            {
                var strings = new Dictionary<string, Dictionary<string, object>>();
                strings = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(
                    File.ReadAllText(Path.Combine("resources", "client_strings.json"))
                );

                var type = typeof(Strings);

                var fields = new List<Type>();
                fields.AddRange(
                    type.GetNestedTypes(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                );

                foreach (var p in fields)
                {
                    if (!strings.ContainsKey(p.Name))
                    {
                        continue;
                    }

                    var dict = strings[p.Name];
                    foreach (var fieldInfo in p.GetFields(
                        System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public
                    ))
                    {
                        var fieldValue = fieldInfo.GetValue(null);
                        if (!dict.ContainsKey(fieldInfo.Name.ToLower()))
                        {
                            continue;
                        }

                        if (fieldValue is LocalizedString)
                        {
                            fieldInfo.SetValue(null, new LocalizedString((string) dict[fieldInfo.Name.ToLower()]));
                        }
                        else if (fieldValue is Dictionary<int, LocalizedString>)
                        {
                            var existingDict = (Dictionary<int, LocalizedString>) fieldInfo.GetValue(null);
                            var values = ((JObject) dict[fieldInfo.Name]).ToObject<Dictionary<int, string>>();
                            var dic = values.ToDictionary<KeyValuePair<int, string>, int, LocalizedString>(
                                val => val.Key, val => val.Value
                            );

                            foreach (var val in dic)
                            {
                                existingDict[val.Key] = val.Value;
                            }
                        }
                        else if (fieldValue is Dictionary<string, LocalizedString>)
                        {
                            var existingDict = (Dictionary<string, LocalizedString>) fieldInfo.GetValue(null);
                            var pairs = ((JObject) dict[fieldInfo.Name])?.ToObject<Dictionary<string, string>>() ??
                                        new Dictionary<string, string>();

                            foreach (var pair in pairs)
                            {
                                if (pair.Key == null)
                                {
                                    continue;
                                }

                                existingDict[pair.Key.ToLower()] = pair.Value;
                            }
                        }
                    }
                }
            }

            Program.OpenGLLink = Errors.opengllink.ToString();
            Program.OpenALLink = Errors.openallink.ToString();

            Save();
        }

        public static void Save()
        {
            var strings = new Dictionary<string, Dictionary<string, object>>();
            var type = typeof(Strings);
            var fields = type.GetNestedTypes(
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public
            );

            foreach (var p in fields)
            {
                var dict = new Dictionary<string, object>();
                foreach (var p1 in p.GetFields(
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public
                ))
                {
                    if (p1.GetValue(null).GetType() == typeof(LocalizedString))
                    {
                        dict.Add(p1.Name.ToLower(), ((LocalizedString) p1.GetValue(null)).ToString());
                    }
                    else if (p1.GetValue(null).GetType() == typeof(Dictionary<int, LocalizedString>))
                    {
                        var dic = new Dictionary<int, string>();
                        foreach (var val in (Dictionary<int, LocalizedString>) p1.GetValue(null))
                        {
                            dic.Add(val.Key, val.Value.ToString());
                        }

                        dict.Add(p1.Name, dic);
                    }
                    else if (p1.GetValue(null).GetType() == typeof(Dictionary<string, LocalizedString>))
                    {
                        var dic = new Dictionary<string, string>();
                        foreach (var val in (Dictionary<string, LocalizedString>) p1.GetValue(null))
                        {
                            dic.Add(val.Key.ToLower(), val.Value.ToString());
                        }

                        dict.Add(p1.Name, dic);
                    }
                }

                strings.Add(p.Name, dict);
            }

            var languageDirectory = Path.Combine("resources");
            if (Directory.Exists(languageDirectory))
            {
                File.WriteAllText(
                    Path.Combine(languageDirectory, "client_strings.json"),
                    JsonConvert.SerializeObject(strings, Formatting.Indented)
                );
            }
        }

        public struct Admin
        {

            public static LocalizedString access = @"Access:";

            public static LocalizedString access0 = @"None";

            public static LocalizedString access1 = @"Moderator";

            public static LocalizedString access2 = @"Admin";

            public static LocalizedString ban = @"Ban";

            public static LocalizedString bancaption = @"Ban {00}";

            public static LocalizedString banprompt =
                @"Banning {00} will not allow them to access this game for the duration you set!";

            public static LocalizedString chronological = @"123...";

            public static LocalizedString chronologicaltip = @"Order maps chronologically.";

            public static LocalizedString face = @"Face:";

            public static LocalizedString kick = @"Kick";

            public static LocalizedString kill = @"Kill";

            public static LocalizedString maplist = @"Map List:";

            public static LocalizedString mute = @"Mute";

            public static LocalizedString mutecaption = @"Mute {00}";

            public static LocalizedString muteprompt =
                @"Muting {00} will not allow them to chat in game for the duration you set!";

            public static LocalizedString name = @"Name:";

            public static LocalizedString noclip = @"No Clip:";

            public static LocalizedString nocliptip = @"Check to walk through obstacles.";

            public static LocalizedString none = @"None";

            public static LocalizedString setface = @"Set Face";

            public static LocalizedString setpower = @"Set Power";

            public static LocalizedString setsprite = @"Set Sprite";

            public static LocalizedString sprite = @"Sprite:";

            public static LocalizedString title = @"Administration";

            public static LocalizedString unban = @"Unban";

            public static LocalizedString unbancaption = @"Unban {00}";

            public static LocalizedString unbanprompt = @"Are you sure that you want to unban {00}?";

            public static LocalizedString unmute = @"Unmute";

            public static LocalizedString unmutecaption = @"Unute {00}";

            public static LocalizedString unmuteprompt = @"Are you sure that you want to unmute {00}?";

            public static LocalizedString warp2me = @"Warp To Me";

            public static LocalizedString warpme2 = @"Warp Me To";

        }

        public struct Bags
        {

            public static LocalizedString retreiveitem = @"Retreive Item";

            public static LocalizedString retreiveitemprompt = @"How many/much {00} would you like to retreive?";

            public static LocalizedString storeitem = @"Store Item";

            public static LocalizedString storeitemprompt = @"How many/much {00} would you like to store?";

            public static LocalizedString title = @"Bag";

        }

        public struct Bank
        {

            public static LocalizedString deposititem = @"Deposit Item";

            public static LocalizedString deposititemprompt = @"How many/much {00} would you like to deposit?";

            public static LocalizedString title = @"Bank";

            public static LocalizedString withdrawitem = @"Withdraw Item";

            public static LocalizedString withdrawitemprompt = @"How many/much {00} would you like to withdraw?";

        }

        public struct BanMute
        {

            public static LocalizedString oneday = @"1 day";

            public static LocalizedString onemonth = @"1 month";

            public static LocalizedString oneweek = @"1 week";

            public static LocalizedString oneyear = @"1 year";

            public static LocalizedString twodays = @"2 days";

            public static LocalizedString twomonths = @"2 months";

            public static LocalizedString twoweeks = @"2 weeks";

            public static LocalizedString threedays = @"3 days";

            public static LocalizedString fourdays = @"4 days";

            public static LocalizedString fivedays = @"5 days";

            public static LocalizedString sixmonths = @"6 months";

            public static LocalizedString cancel = @"Cancel:";

            public static LocalizedString duration = @"Duration:";

            public static LocalizedString forever = @"Indefinitely";

            public static LocalizedString ip = @"Include IP:";

            public static LocalizedString ok = @"Okay:";

            public static LocalizedString reason = @"Reason:";

        }

        public struct Character
        {

            public static LocalizedString equipment = @"Equipment:";

            public static LocalizedString levelandclass = @"Level: {00} {01}";

            public static LocalizedString name = @"{00}";

            public static LocalizedString points = @"Points: {00}";

            public static LocalizedString stat0 = @"{00}: {01}";

            public static LocalizedString stat1 = @"{00}: {01}";

            public static LocalizedString stat2 = @"{00}: {01}";

            public static LocalizedString stat3 = @"{00}: {01}";

            public static LocalizedString stat4 = @"{00}: {01}";

            public static LocalizedString stats = @"Stats:";

            public static LocalizedString title = @"Character";

        }

        public struct CharacterCreation
        {

            public static LocalizedString back = @"Back";

            public static LocalizedString Class = @"Class:";

            public static LocalizedString create = @"Create";

            public static LocalizedString female = @"Female";

            public static LocalizedString gender = @"Gender:";

            public static LocalizedString hint = @"Customize";

            public static LocalizedString hint2 = @"Your Character";

            public static LocalizedString invalidname =
                @"Character name is invalid. Please use alphanumeric characters with a length between 2 and 20.";

            public static LocalizedString male = @"Male";

            public static LocalizedString name = @"Char Name:";

            public static LocalizedString title = @"Create Character";

        }

        public struct CharacterSelection
        {

            public static LocalizedString delete = @"Delete";

            public static LocalizedString deleteprompt =
                @"Are you sure you want to delete {00}? This action is irreversible!";

            public static LocalizedString deletetitle = @"Delete {00}";

            public static LocalizedString empty = @"Empty Character Slot";

            public static LocalizedString info = @"Level {00} {01}";

            public static LocalizedString logout = @"Logout";

            public static LocalizedString name = @"{00}";

            public static LocalizedString New = @"New";

            public static LocalizedString play = @"Use";

            public static LocalizedString title = @"Select a Character";

        }

        public struct Chatbox
        {

            public static LocalizedString channel = @"Channel:";

            public static Dictionary<int, LocalizedString> channels = new Dictionary<int, LocalizedString>
            {
                {0, @"local"},
                {1, @"global"},
                {2, @"party"},
                {3, @"guild"}
            };

            public static LocalizedString channeladmin = @"admin";

            public static LocalizedString enterchat = @"Click here to chat.";

            public static LocalizedString enterchat1 = @"Press {00} to chat.";

            public static LocalizedString enterchat2 = @"Press {00} or {01} to chat.";

            public static LocalizedString send = @"Send";

            public static LocalizedString title = @"Chat";

            public static LocalizedString toofast = @"You are chatting too fast!";

            public static Dictionary<ChatboxTab, LocalizedString> ChatTabButtons = new Dictionary<Enums.ChatboxTab, LocalizedString>() {
                { ChatboxTab.All, @"All" },
                { ChatboxTab.Local, @"Local" },
                { ChatboxTab.Party, @"Party" },
                { ChatboxTab.Global, @"Global" },
                { ChatboxTab.Guild, @"Guild" },
                { ChatboxTab.System, @"System" },
            };

            public static LocalizedString UnableToCopy = @"It appears you are not able to copy/paste on this platform. Please make sure you have either the 'xclip' or 'wl-clipboard' packages installed if you are running Linux.";

        }

        public struct Colors
        {

            public static Dictionary<int, LocalizedString> presets = new Dictionary<int, LocalizedString>
            {
                {0, @"Black"},
                {1, @"White"},
                {2, @"Pink"},
                {3, @"Blue"},
                {4, @"Red"},
                {5, @"Green"},
                {6, @"Yellow"},
                {7, @"Orange"},
                {8, @"Purple"},
                {9, @"Gray"},
                {10, @"Cyan"}
            };

        }

        public struct Combat
        {

            public static LocalizedString exp = @"Experience";

            public static LocalizedString stat0 = @"Attack";

            public static LocalizedString stat1 = @"Ability Power";

            public static LocalizedString stat2 = @"Defense";

            public static LocalizedString stat3 = @"Magic Resist";

            public static LocalizedString stat4 = @"Speed";

            public static LocalizedString targetoutsiderange = @"Target too far away!";

            public static LocalizedString vital0 = @"Health";

            public static LocalizedString vital1 = @"Mana";

            public static LocalizedString warningtitle = @"Combat Warning!";

            public static LocalizedString warningforceclose =
                @"Game was closed while in combat! Your character will remain logged in until combat has concluded!";

            public static LocalizedString warninglogout =
                @"You are about to logout while in combat! Your character will remain in-game until combat has ended! Are you sure you want to logout now?";

            public static LocalizedString warningcharacterselect =
                @"You are about to logout while in combat! Your character will remain in-game until combat has ended! Are you sure you want to logout now?";

            public static LocalizedString warningexitdesktop =
                @"You are about to exit while in combat! Your character will remain in-game until combat has ended! Are you sure you want to quit now?";

        }

        public struct Controls
        {

            public static Dictionary<string, LocalizedString> controldict = new Dictionary<string, LocalizedString>
            {
                {"attackinteract", @"Attack/Interact:"},
                {"block", @"Block:"},
                {"autotarget", @"Auto Target:"},
                {"enter", @"Enter:"},
                {"hotkey0", @"Hot Key 0:"},
                {"hotkey1", @"Hot Key 1:"},
                {"hotkey2", @"Hot Key 2:"},
                {"hotkey3", @"Hot Key 3:"},
                {"hotkey4", @"Hot Key 4:"},
                {"hotkey5", @"Hot Key 5:"},
                {"hotkey6", @"Hot Key 6:"},
                {"hotkey7", @"Hot Key 7:"},
                {"hotkey8", @"Hot Key 8:"},
                {"hotkey9", @"Hot Key 9:"},
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
                {"opensettings", @"Open Settings:"},
                {"opendebugger", @"Open Debugger:"},
                {"openadminpanel", @"Open Admin Panel:"},
                {"togglegui", @"Toggle Interface:"}
            };

            public static LocalizedString edit = @"Edit Controls";

            public static LocalizedString listening = @"Listening";

            public static LocalizedString title = @"Controls";

        }

        public struct Crafting
        {

            public static LocalizedString craft = @"Craft";

            public static LocalizedString incorrectresources =
                @"You do not have the correct resources to craft this item.";

            public static LocalizedString ingredients = @"Requires:";

            public static LocalizedString product = @"Crafting:";

            public static LocalizedString recipes = @"Recipes:";

            public static LocalizedString title = @"Crafting Table";

        }

        public struct Credits
        {

            public static LocalizedString back = @"Main Menu";

            public static LocalizedString title = @"Credits";

        }

        public struct Debug
        {

            public static LocalizedString draws = @"Draws: {00}";

            public static LocalizedString entitiesdrawn = @"Entities Drawn: {00}";

            public static LocalizedString fps = @"FPS: {00}";

            public static LocalizedString knownentities = @"Known Entities: {00}";

            public static LocalizedString knownmaps = @"Known Maps: {00}";

            public static LocalizedString lightsdrawn = @"Lights Drawn: {00}";

            public static LocalizedString map = @"Map: {00}";

            public static LocalizedString mapsdrawn = @"Maps Drawn: {00}";

            public static LocalizedString ping = @"Ping: {00}";

            public static LocalizedString time = @"Time: {00}";

            public static LocalizedString title = @"Debug";

            public static LocalizedString x = @"X: {00}";

            public static LocalizedString y = @"Y: {00}";

            public static LocalizedString z = @"Z: {00}";

            public static LocalizedString interfaceobjects = @"Interface Objects: {00}";

        }

        public struct EntityBox
        {

            public static LocalizedString NameAndLevel = @"{00}    {01}";

            public static LocalizedString cooldown = "{00}s";

            public static LocalizedString exp = @"EXP:";

            public static LocalizedString expval = @"{00} / {01}";

            public static LocalizedString friend = "Befriend";

            public static LocalizedString friendtip = "Send {00} a friend request.";

            public static LocalizedString level = @"Lv. {00}";

            public static LocalizedString map = @"{00}";

            public static LocalizedString maxlevel = @"Max Level";

            public static LocalizedString party = @"Party";

            public static LocalizedString partytip = @"Invite {00} to your party.";

            public static LocalizedString trade = @"Trade";

            public static LocalizedString tradetip = @"Request to trade with {00}.";

            public static LocalizedString vital0 = @"HP:";

            public static LocalizedString vital0val = @"{00} / {01}";

            public static LocalizedString vital1 = @"MP:";

            public static LocalizedString vital1val = @"{00} / {01}";

        }

        public struct Errors
        {

            public static LocalizedString displaynotsupported = @"Invalid Display Configuration!";

            public static LocalizedString displaynotsupportederror =
                @"Fullscreen {00} resolution is not supported on this device!";

            public static LocalizedString errorencountered =
                @"The Intersect Client has encountered an error and must close. Error information can be found in logs/errors.log";

            public static LocalizedString notconnected = @"Not connected to the game server. Is it online?";

            public static LocalizedString notsupported = @"Not Supported!";

            public static LocalizedString openallink = @"https://goo.gl/Nbx6hx";

            public static LocalizedString opengllink = @"https://goo.gl/RSP3ts";

            public static LocalizedString passwordinvalid =
                @"Password is invalid. Please use alphanumeric characters with a length between 4 and 20.";

            public static LocalizedString resourcesnotfound =
                @"The resources directory could not be found! Intersect will now close.";

            public static LocalizedString title = @"Error!";

            public static LocalizedString usernameinvalid =
                @"Username is invalid. Please use alphanumeric characters with a length between 2 and 20.";

            public static LocalizedString LoadFile =
                @"Failed to load a {00}. Please send the game administrator a copy of your errors log file in the logs directory.";

            public static LocalizedString lostconnection =
                @"Lost connection to the game server. Please make sure you're connected to the internet and try again!";

        }

        public struct Words
        {

            public static LocalizedString lcase_sound = @"sound";

            public static LocalizedString lcase_music = @"soundtrack";

            public static LocalizedString lcase_sprite = @"sprite";

            public static LocalizedString lcase_animation = @"animation";

        }

        public struct EventWindow
        {

            public static LocalizedString Continue = @"Continue";

        }

        public struct ForgotPass
        {

            public static LocalizedString back = @"Back";

            public static LocalizedString hint =
                @"If your account exists we will send you a temporary password reset code.";

            public static LocalizedString label = @"Enter your username or email below:";

            public static LocalizedString submit = @"Submit";

            public static LocalizedString title = @"Password Reset";

        }

        public struct Friends
        {

            public static LocalizedString addfriend = @"Add Friend";

            public static LocalizedString addfriendtitle = @"Add Friend";

            public static LocalizedString addfriendprompt = @"Who would you like to add as a friend?";

            public static LocalizedString infight = @"You are currently fighting!";

            public static LocalizedString removefriend = @"Remove Friend";

            public static LocalizedString removefriendprompt = @"Do you wish to remove {00} from your friends list?";

            public static LocalizedString request = @"Friend Request";

            public static LocalizedString requestprompt = @"{00} has sent you a friend request. Do you accept?";

            public static LocalizedString title = @"Friends";

        }

        public struct GameMenu
        {

            public static LocalizedString character = @"Character Info";

            public static LocalizedString Menu = @"Open Menu";

            public static LocalizedString friends = @"Friends";

            public static LocalizedString items = @"Inventory";

            public static LocalizedString party = @"Party";

            public static LocalizedString quest = @"Quest Log";

            public static LocalizedString spells = @"Spell Book";

        }

        public struct General
        {

            public static LocalizedString none = @"None";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString MapItemStackable = @"{01} {00}";

        }

        public struct Guilds
        {
            public static LocalizedString Guild = @"Guild";

            public static LocalizedString guildtip = "Send {00} an invite to your guild.";

            public static LocalizedString Invite = @"Invite";

            public static LocalizedString NotInGuild = @"You are not currently in a guild!";

            public static LocalizedString InviteMemberTitle = @"Invite Player";

            public static LocalizedString InviteMemberPrompt = @"Who would you like to invite to {00}?";

            public static LocalizedString InviteRequestTitle = @"Guild Invite";

            public static LocalizedString InviteRequestPrompt = @"{00} would like to invite you to join their guild, {01}. Do you accept?";

            public static LocalizedString Leave = "Leave";

            public static LocalizedString LeaveTitle = @"Leave Guild";

            public static LocalizedString LeavePrompt = @"Are you sure you would like to leave your guild?";

            public static LocalizedString Promote = @"Promote to {00}";

            public static LocalizedString Demote = @"Demote to {00}";

            public static LocalizedString Kick = @"Kick";

            public static LocalizedString PM = @"PM";

            public static LocalizedString Transfer = @"Transfer";

            public static LocalizedString OnlineListEntry = @"[{00}] {01} - {02}";

            public static LocalizedString OfflineListEntry = @"[{00}] {01} - {02}";

            public static LocalizedString Tooltip = @"Lv. {00} {01}";

            public static LocalizedString KickTitle = @"Kick Guild Member";

            public static LocalizedString KickPrompt = @"Are you sure you would like to kick {00}?";

            public static LocalizedString PromoteTitle = @"Promote Guild Member";

            public static LocalizedString PromotePrompt = @"Are you sure you would like promote {00} to rank {01}?";

            public static LocalizedString DemoteTitle = @"Demote Guild Member";

            public static LocalizedString DemotePrompt = @"Are you sure you would like to demote {00} to rank {01}?";

            public static LocalizedString TransferTitle = @"Transfer Guild";

            public static LocalizedString TransferPrompt = @"This action will completely transfer all ownership of your guild to {00} and you will lose your rank of {01}. If you are sure you want to hand over your guild enter '{02}' below.";

            public static LocalizedString Bank = @"{00} Guild Bank";

            public static LocalizedString NotAllowedWithdraw = @"You do not have permission to withdraw from {00}'s guild bank!";

            public static LocalizedString NotAllowedDeposit = @"You do not have permission to deposit items into {00}'s guild bank!";

            public static LocalizedString NotAllowedSwap = @"You do not have permission to swap items around within {00}'s guild bank!";
        }

        public struct InputBox
        {

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString no = @"No";

            public static LocalizedString okay = @"Okay";

            public static LocalizedString yes = @"Yes";

        }

        public struct MapItemWindow
        {
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString Title = @"Loot";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString LootButton = @"Loot All";

        }

        public struct Inventory
        {

            public static LocalizedString cooldown = "{00}s";

            public static LocalizedString dropitem = @"Drop Item";

            public static LocalizedString dropitemprompt = @"How many/much {00} do you want to drop?";

            public static LocalizedString dropprompt = @"Do you wish to drop the item: {00}?";

            public static LocalizedString equippedicon = "E";

            public static LocalizedString title = @"Inventory";

        }

        public struct ItemDesc
        {

            public static LocalizedString bonuses = @"Stat Bonuses:";

            public static LocalizedString damage = @"Base Damage: {00}";

            public static LocalizedString desc = @"{00}";

            public static LocalizedString effect = @"Bonus Effect: {00}% {01}";

            public static Dictionary<int, LocalizedString> effects = new Dictionary<int, LocalizedString>
            {
                {0, @"Cooldown Reduction"},
                {1, @"Lifesteal"},
                {2, @"Tenacity"},
                {3, @"Luck"},
                {4, @"Exp Increase"},
            };

            public static Dictionary<int, LocalizedString> itemtypes = new Dictionary<int, LocalizedString>
            {
                {0, @"None"},
                {1, @"Equipment"},
                {2, @"Consumable"},
                {3, @"Currency"},
                {4, @"Spell"},
                {5, @"Special"},
                {6, @"Bag"},
            };

            public static LocalizedString prereq = @"Prerequisites:";

            public static Dictionary<int, LocalizedString> stats = new Dictionary<int, LocalizedString>
            {
                {0, @"Attack: {00}"},
                {1, @"Ability Power: {00}"},
                {2, @"Defense: {00}"},
                {3, @"Magic Resist: {00}"},
                {4, @"Speed: {00}"}
            };

            public static LocalizedString twohand = @"2H";

            public static Dictionary<int, LocalizedString> rarity = new Dictionary<int, LocalizedString>
            {
                {0, @"None"},
                {1, @"Common"},
                {2, @"Uncommon"},
                {3, @"Rare"},
                {4, @"Epic"},
                {5, @"Legendary"},
            };

            public static Dictionary<int, LocalizedString> vitals = new Dictionary<int, LocalizedString>
            {
                {0, @"HP: {00}"},
                {1, @"MP: {00}"}
            };

        }

        public struct Keys
        {

            public static Dictionary<string, LocalizedString> keydict = new Dictionary<string, LocalizedString>()
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
                {"lmenu", @"L Menu"},
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
                {"rmenu", @"R Menu"},
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

        }

        public struct Login
        {

            public static LocalizedString back = @"Back";

            public static LocalizedString forgot = @"Forgot Password?";

            public static LocalizedString login = @"Login";

            public static LocalizedString password = @"Password:";

            public static LocalizedString savepass = @"Save Password";

            public static LocalizedString title = @"Login";

            public static LocalizedString username = @"Username:";

        }

        public struct Main
        {

            public static LocalizedString gamename = @"Intersect Client";

        }

        public struct MainMenu
        {

            public static LocalizedString credits = @"Credits";

            public static LocalizedString exit = @"Exit";

            public static LocalizedString login = @"Login";

            public static LocalizedString options = @"Settings";

            public static LocalizedString optionstooltip = @"";

            public static LocalizedString register = @"Register";

            public static LocalizedString title = @"Main Menu";

        }

        public struct Options
        {

            public static LocalizedString fps30 = @"30";

            public static LocalizedString fps60 = @"60";

            public static LocalizedString fps90 = @"90";

            public static LocalizedString fps120 = @"120";

            public static LocalizedString apply = @"Apply";

            public static LocalizedString back = @"Back";

            public static LocalizedString cancel = @"Cancel";

            public static LocalizedString fullscreen = @"Fullscreen";

            public static LocalizedString AutocloseWindows = @"Auto-close Windows";

            public static LocalizedString musicvolume = @"Music Volume: {00}%";

            public static LocalizedString resolution = @"Resolution:";

            public static LocalizedString ResolutionCustom = @"Custom Resolution";

            public static LocalizedString restore = @"Restore Defaults";

            public static LocalizedString soundvolume = @"Sound Volume: {00}%";

            public static LocalizedString targetfps = @"Target FPS:";

            public static LocalizedString title = @"Options";

            public static LocalizedString unlimitedfps = @"No Limit";

            public static LocalizedString vsync = @"V-Sync";

        }

        public struct Parties
        {
            public static LocalizedString infight = @"You are currently fighting!";

            public static LocalizedString inviteprompt = @"{00} has invited you to their party. Do you accept?";

            public static LocalizedString kick = @"Kick {00}";

            public static LocalizedString kicklbl = @"Kick";

            public static LocalizedString leader = @"Leader";

            public static LocalizedString leadertip = @"Party Leader";

            public static LocalizedString leave = @"Leave Party";

            public static LocalizedString leavetip = @"Leave Tip";

            public static LocalizedString name = @"{00} - Lv. {01}";

            public static LocalizedString partyinvite = @"Party Invite";

            public static LocalizedString title = @"Party";

            public static LocalizedString vital0 = @"HP:";

            public static LocalizedString vital0val = @"{00} / {01}";

            public static LocalizedString vital1 = @"MP:";

            public static LocalizedString vital1val = @"{00} / {01}";

        }

        public struct QuestLog
        {

            public static LocalizedString abandon = @"Abandon";

            public static LocalizedString abandonprompt = @"Are you sure that you want to quit the quest ""{00}""?";

            public static LocalizedString abandontitle = @"Abandon Quest: {00}";

            public static LocalizedString back = @"Back";

            public static LocalizedString completed = @"Quest Completed";

            public static LocalizedString currenttask = @"Current Task:";

            public static LocalizedString inprogress = @"Quest In Progress";

            public static LocalizedString notstarted = @"Quest Not Started";

            public static LocalizedString taskitem = @"{00}/{01} {02}(s) gathered.";

            public static LocalizedString tasknpc = @"{00}/{01} {02}(s) slain.";

            public static LocalizedString title = @"Quest Log";

        }

        public struct QuestOffer
        {

            public static LocalizedString accept = @"Accept";

            public static LocalizedString decline = @"Decline";

            public static LocalizedString title = @"Quest Offer";

        }

        public struct Regex
        {

            public static LocalizedString email =
                @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

            public static LocalizedString password = @"^[-_=\+`~!@#\$%\^&\*()\[\]{}\\|;\:'"",<\.>/\?a-zA-Z0-9]{4,64}$";

            public static LocalizedString username = @"^[a-zA-Z0-9]{2,20}$";

        }

        public struct Registration
        {

            public static LocalizedString back = @"Back";

            public static LocalizedString confirmpass = @"Confirm Password:";

            public static LocalizedString email = @"Email:";

            public static LocalizedString emailinvalid = @"Email is invalid.";

            public static LocalizedString password = @"Password:";

            public static LocalizedString passwordmatch = @"Passwords didn't match!";

            public static LocalizedString register = @"Register";

            public static LocalizedString title = @"Register";

            public static LocalizedString username = @"Username:";

        }

        public struct ResetPass
        {

            public static LocalizedString back = @"Cancel";

            public static LocalizedString code = @"Enter the reset code that was sent to you:";

            public static LocalizedString fail = @"Error!";

            public static LocalizedString failmsg =
                @"The reset code was not valid, has expired, or the account does not exist!";

            public static LocalizedString inputcode = @"Please enter your password reset code.";

            public static LocalizedString password = @"New Password:";

            public static LocalizedString password2 = @"Confirm Password:";

            public static LocalizedString submit = @"Submit";

            public static LocalizedString success = @"Success!";

            public static LocalizedString successmsg = @"Your password has been reset!";

            public static LocalizedString title = @"Password Reset";

        }

        public struct Resources
        {

            public static LocalizedString cancelled = @"Download was Cancelled!";

            public static LocalizedString failedtoload = @"Failed to load Resources!";

            public static LocalizedString resourceexception =
                @"Failed to download client resources.\n\nException Info: {00}\n\nWould you like to try again?";

            public static LocalizedString resourcesfatal =
                @"Failed to load resources from client directory and Ascension Game Dev server. Cannot launch game!";

        }

        public struct Server
        {

            public static LocalizedString StatusLabel = @"Server Status: {00}";

            public static LocalizedString Online = @"Online";

            public static LocalizedString Offline = @"Offline";

            public static LocalizedString Failed = @"Network Error";

            public static LocalizedString Connecting = @"Connecting...";

            public static LocalizedString Unknown = @"Unknown";

            public static LocalizedString VersionMismatch = @"Bad Version";

            public static LocalizedString ServerFull = @"Full";

            public static LocalizedString HandshakeFailure = @"Handshake Error";

        }

        public struct Shop
        {

            public static LocalizedString buyitem = @"Buy Item";

            public static LocalizedString buyitemprompt = @"How many/much {00} would you like to buy?";

            public static LocalizedString cannotsell = @"This shop does not accept that item!";

            public static LocalizedString costs = @"Costs {00} {01}(s)";

            public static LocalizedString sellitem = @"Sell Item";

            public static LocalizedString sellitemprompt = @"How many/much {00} would you like to sell?";

            public static LocalizedString sellprompt = @"Do you wish to sell the item: {00}?";

            public static LocalizedString sellsfor = @"Sells for {00} {01}(s)";

            public static LocalizedString wontbuy = @"Shop Will Not Buy This Item";

        }

        public struct SpellDesc
        {

            public static LocalizedString addsymbol = @"+";

            public static LocalizedString casttime = @"Cast Time: {00} Seconds";

            public static LocalizedString cooldowntime = @"Cooldown: {00} Seconds";

            public static LocalizedString desc = @"{00}";

            public static LocalizedString duration = @"Duration: {00}s";

            public static Dictionary<int, LocalizedString> effectlist = new Dictionary<int, LocalizedString>
            {
                {0, @""},
                {1, @"Silences Target"},
                {2, @"Stuns Target"},
                {3, @"Snares Target"},
                {4, @"Blinds Target"},
                {5, @"Stealths Target"},
                {6, @"Transforms Target"},
                {7, @"Cleanses Target"},
                {8, @"Target becomes Invulnerable"},
                {9, @"Shields Target"},
                {10, @"Makes the target fall asleep"},
                {11, @"Applies an On Hit effect to the target"},
                {12, @"Taunts Target"},
            };

            public static LocalizedString effects = @"Effects:";

            public static LocalizedString prereqs = @"Prerequisites:";

            public static LocalizedString shield = @"Shielding: {00}";

            public static LocalizedString radius = @"Hit Radius: {00}";

            public static LocalizedString removesymbol = @"-";

            public static Dictionary<int, LocalizedString> spelltypes = new Dictionary<int, LocalizedString>
            {
                {0, @"Combat Spell"},
                {1, @"Warp to Map"},
                {2, @"Warp to Target"},
                {3, @"Dash"},
                {4, @"Special"},
            };

            public static Dictionary<int, LocalizedString> stats = new Dictionary<int, LocalizedString>
            {
                {0, @"Attack: {00}"},
                {1, @"Ability Power: {00}"},
                {2, @"Defense: {00}"},
                {3, @"Magic Resist: {00}"},
                {4, @"Speed: {00}"}
            };

            public static Dictionary<int, LocalizedString> targettypes = new Dictionary<int, LocalizedString>
            {
                {0, @"Self Cast"},
                {1, @"Targetted - Range: {00} Tiles"},
                {2, @"AOE"},
                {3, @"Projectile - Range: {00} Tiles"},
                {4, @"On Hit"},
                {5, @"Trap"},
            };

            public static Dictionary<int, LocalizedString> vitals = new Dictionary<int, LocalizedString>
            {
                {0, @"HP: {00}{01}"},
                {1, @"MP: {00}{01}"},
            };

            public static Dictionary<int, LocalizedString> vitalcosts = new Dictionary<int, LocalizedString>
            {
                {0, @"HP Cost: {00}"},
                {1, @"MP Cost: {00}"},
            };

        }

        public struct Spells
        {

            public static LocalizedString cooldown = "{00}s";

            public static LocalizedString forgetspell = @"Forget Spell";

            public static LocalizedString forgetspellprompt = @"Are you sure you want to forget {00}?";

            public static LocalizedString title = @"Spells";

        }

        public struct Trading
        {

            public static LocalizedString accept = @"Accept";

            public static LocalizedString infight = @"You are currently fighting!";

            public static LocalizedString offeritem = @"Offer Item";

            public static LocalizedString offeritemprompt = @"How many/much {00} would you like to offer?";

            public static LocalizedString pending = @"Pending";

            public static LocalizedString requestprompt =
                @"{00} has invited you to trade items with them. Do you accept?";

            public static LocalizedString revokeitem = @"Revoke Item";

            public static LocalizedString revokeitemprompt = @"How many/much {00} would you like to revoke?";

            public static LocalizedString theiroffer = @"Their Offer:";

            public static LocalizedString title = @"Trading with {00}";

            public static LocalizedString traderequest = @"Trading Invite";

            public static LocalizedString value = @"Value: {00}";

            public static LocalizedString youroffer = @"Your Offer:";

        }

        public struct EscapeMenu
        {

            public static LocalizedString Title = @"Menu";

            public static LocalizedString Options = @"Options";

            public static LocalizedString CharacterSelect = @"Characters";

            public static LocalizedString Logout = @"Logout";

            public static LocalizedString ExitToDesktop = @"Desktop";

            public static LocalizedString Close = @"Close";

        }

        public struct Numbers
        {

            public static LocalizedString thousands = "k";

            public static LocalizedString millions = "m";

            public static LocalizedString billions = "b";

            public static LocalizedString dec = ".";

            public static LocalizedString comma = ",";

        }

        public struct Update
        {

            public static LocalizedString Checking = @"Checking for updates, please wait!";

            public static LocalizedString Updating = @"Downloading updates, please wait!";

            public static LocalizedString Restart = @"Update complete! Relaunch {00} to play!";

            public static LocalizedString Done = @"Update complete! Launching game!";

            public static LocalizedString Error = @"Update Error! Check logs for more info!";

            public static LocalizedString Files = @"{00} Files Remaining";

            public static LocalizedString Size = @"{00} Left";

            public static LocalizedString Percent = @"{00}%";

        }

        public struct GameWindow
        {
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public static LocalizedString EntityNameAndLevel = @"{00} [Lv. {01}]";
        }

    }

}
