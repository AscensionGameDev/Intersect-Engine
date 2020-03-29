using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Intersect.Localization;
using Intersect.Logging;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace Intersect.Utilities
{

    public static class Strings
    {

        public sealed class AccountNamespace : LocaleNamespace
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString adminonly =
                @"The server is currently allowing only admins to connect. Come back later!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString badaccess = @"Access denied! Invalid power level!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString badlogin = @"Username or password incorrect.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString banned = @"{00} has been banned!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString banstatus =
                @"Your account has been banned since: {00} (UTC) by {01}. Ban expires: {02} (UTC). Reason for ban: {03}";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString characterexists =
                @"An account with this character name exists. Please choose another.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString deletechar = @"The character has been deleted.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString deleted = @"Delete Character";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString doesnotexist = @"Account does not exist.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString emailexists = @"An account with this email address already exists.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString exists = @"Account already exists!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString invalidclass = @"Invalid class selected. Try again.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString invalidemail =
                @"The chosen email does not meet requirements set by the server.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString invalidname =
                @"The chosen name does not meet requirements set by the server.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString loadfail = @"Failed to load account. Please try logging in again.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString maxchars =
                @"You have already created the maximum number of characters. Delete one before creating a new one.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString muted = @"{00} has been muted!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString mutestatus =
                @"Your account has been muted since: {00} by {01}. Mute expires: {02}. Reason for mute: {03}";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString notfound = @"Error: Account {00} was not found!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString unbanned = @"Account {00} has been unbanned!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString unmuted = @"{00} has been unmuted!";

        }

        public sealed class BagsNamespace : LocaleNamespace
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString baginbag = @"You cannot store a bag inside another bag!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString baginself = @"You cannot store a bag in within itself!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString bagnospace = @"There is no space left in your bag for that item!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString depositinvalid = @"Invalid item selected to store!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString dropnotempty = @"You cannot drop a bag unless it's empty!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString inventorynospace =
                @"There is no space left in your inventory for that item!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString onlysellempty = @"Cannot sell bag unless it's empty!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString onlytradeempty = @"Cannot trade bag unless it's empty!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString withdrawinvalid = @"Invalid item selected to retreive!";

        }

        public sealed class BanksNamespace : LocaleNamespace
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString banknospace = @"There is no space left in your bank for that item!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString depositinvalid = @"Invalid item selected to deposit!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString inventorynospace =
                @"There is no space left in your inventory for that item!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString withdrawinvalid = @"Invalid item selected to withdraw!";

        }

        public sealed class ChatNamespace : LocaleNamespace
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString admin = @"[ADMIN] {00}: {01}";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString admincmd = @"/admin";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString allcmd = @"/all";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString announcement = @"[ANNOUNCEMENT] {00}: {01}";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString announcementcmd = @"/announcement";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString Global = @"[GLOBAL] {00}: {01}";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString globalcmd = @"/global";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString local = @"{00}: {01}";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString localcmd = @"/local";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString messagecmd = @"/message";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString party = @"[PARTY] {00}: {01}";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString partycmd = @"/party";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString pmcmd = @"/pm";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString Private = @"[PM] {00}: {01}";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString rcmd = @"/r";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString replycmd = @"/reply";

        }

        public sealed class ClassesNamespace : LocaleNamespace
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString lastclass = @"Last Class";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString lastclasserror =
                @"Failed to delete class, you must have at least one class at all times!";

        }

        public sealed class ColorsNamespace : LocaleNamespace
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocaleDictionary<int, LocalizedString> presets = new LocaleDictionary<int, LocalizedString>(
                new Dictionary<int, LocalizedString>
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
                }
            );

        }

        public sealed class CombatNamespace : LocaleNamespace
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString addsymbol = @"+";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString blocked = @"BLOCKED!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString channeling = @"You are currently channeling another skill.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString channelingnoattack =
                @"You are currently channeling a spell, you cannot attack.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString cooldown = @"This skill is on cooldown.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString critical = @"CRITICAL HIT!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocaleDictionary<int, LocalizedString> damagetypes =
                new LocaleDictionary<int, LocalizedString>(
                    new Dictionary<int, LocalizedString>
                    {
                        {0, @"Physical"},
                        {1, @"Magic"},
                        {2, @"True"}
                    }
                );

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString dash = @"DASH!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString dynamicreq = @"You do not meet the requirements to cast the spell!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString exp = @"Experience";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString invulnerable = @"INVULNERABLE!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString levelreq = @"You are not a high enough level to use this ability.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString levelup = @"LEVEL UP!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString lowhealth = @"Not enough health.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString lowmana = @"Not enough mana.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString miss = @"MISS!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString notarget = @"No Target!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString removesymbol = @"-";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString
                resourcereqs = @"You do not meet the requirements to harvest this resource!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString silenced = @"You cannot cast this ability whilst silenced.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString statreq =
                @"You do not possess the correct combat stats to use this ability.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocaleDictionary<int, LocalizedString> stats = new LocaleDictionary<int, LocalizedString>(
                new Dictionary<int, LocalizedString>
                {
                    {0, @"Attack"},
                    {1, @"Ability Power"},
                    {2, @"Defense"},
                    {3, @"Magic Resist"},
                    {4, @"Speed"}
                }
            );

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocaleDictionary<int, LocalizedString> status = new LocaleDictionary<int, LocalizedString>(
                new Dictionary<int, LocalizedString>
                {
                    {0, @"NONE!"},
                    {1, @"SILENCED!"},
                    {2, @"STUNNED!"},
                    {3, @"SNARED!"},
                    {4, @"BLINDED!"},
                    {5, @"STEALTH!"},
                    {6, @"TRANSFORMED!"},
                    {7, @"CLEANSED!"},
                    {8, @"INVULNERABLE!"}
                }
            );

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString stunattacking = @"You are stunned and can't attack.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString stunblocking = @"You are stunned and can't block.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString stunned = @"You cannot cast this ability whilst stunned.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString targetoutsiderange = @"Target is out of range!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString toolrequired = @"You require a {00} to interact with this resource!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocaleDictionary<int, LocalizedString> vitals = new LocaleDictionary<int, LocalizedString>(
                new Dictionary<int, LocalizedString>
                {
                    {0, @"Health"},
                    {1, @"Mana"}
                }
            );

        }

        public sealed class CommandoutputNamespace : LocaleNamespace
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString apigranted = @"{00} now has api access!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString apirevoked = @"{00} has had their api access revoked!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString cps = @"Current CPS: {00}";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString cpslocked = @"CPS Locked";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString cpsunlocked = @"CPS Unlocked";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString gametime = @"Game time is now: {00}";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString helpfooter =
                @"Type in any command followed by {00} for parameters and usage information.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString helpheader = @"List of available commands:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString invalidparameters =
                @"Invalid parameters provided! Use {00} to get more info about a command.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString killsuccess = @"{00} has been killed!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString listaccount = @"Account";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString listcharacter = @"Character";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString listid = @"ID";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString notfound =
                @"Command not recoginized. Enter help for a list of commands. Remember console commands are case sensitive!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString parseerror =
                @"Parse Error: Parameter could not be read. Type {00} {01} for usage information.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString playercount = @"Server has {00} registered players.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString powerchanged = @"{00} has had their power updated!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString powerlevel = @"{00}'s power has been set to {01}!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString syntaxerror =
                @"Syntax Error: Expected parameter not found. Type {00} {01} for usage information.";

        }

        public sealed class CommandsNamespace : LocaleNamespace
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
            public readonly ArgumentsNamespace Arguments = new ArgumentsNamespace();

            public sealed class ArgumentsNamespace : LocaleNamespace
            {

                [NotNull] public readonly LocaleArgument Help = new LocaleArgument
                {
                    Name = @"help",
                    ShortName = 'h',
                    Description = @"Shows help information for this command"
                };

            }

        }

        public sealed class CraftingNamespace : LocaleNamespace
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString crafted = @"You successfully crafted {00}!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString nospace = @"You do not have enough inventory space to craft {00}!";

        }

        public sealed class DatabaseNamespace : LocaleNamespace
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString Default = @"Default";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString gamedboutofdate =
                @"Game Database is out of date! Version: {00} Expected Version: {01}. Please run the included migration tool!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString noclasses = @"No classes found! - Creating a default class!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString nomaps = @"No maps found! - Creating an empty map!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString nullerror = @"Tried to load one or more null game objects!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString nullfound = @"Tried to load null value for index {00} of {01}";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString playerdboutofdate =
                @"Player Database is out of date! Version: {00} Expected Version: {01}. Please run the included migration tool!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString usingsqlite = @"Using SQLite Database for account and data storage.";

        }

        public sealed class ErrorsNamespace : LocaleNamespace
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString errorloadingconfig =
                @"Failed to load server options! Press any key to shut down.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString errorlogged = @"An error was logged into errors.log";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString errorservercrash =
                @"The Intersect server has encountered an error and must close. Error information can be found in resources/logs/errors.log. Press any key to exit.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString errorservercrashnohalt =
                @"The Intersect server has encountered an error and must close. Error information can be found in resources/logs/errors.log.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString warpfail = @"Failed to warp player to new map -- warping to spawn.";

        }

        public sealed class EventsNamespace : LocaleNamespace
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString commandparameter = @"\param";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString eventnamecommand = @"\en";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString globalswitch = @"\gs";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString globalvar = @"\gv";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString militaryhour = @"\24hour";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString onlinecountcommand = @"\onlinecount";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString onlinelistcommand = @"\onlinelist";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString periodevening = @"PM";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString periodmorning = @"AM";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString playernamecommand = @"\pn";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString playerswitch = @"\ps";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString playervar = @"\pv";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString timehour = @"\hour";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString timeminute = @"\minute";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString timeperiod = @"\period";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString timesecond = @"\second";

        }

        public sealed class FormulasNamespace : LocaleNamespace
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString loadfailed = @"Failed to load formulas! Press any key to shut down.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString missing = @"Formulas.json missing. Generated default formulas file.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString syntax =
                @"Error loading formulas! Please make sure the file exists and is free on syntax errors.";

        }

        public sealed class FriendsNamespace : LocaleNamespace
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString accept = @"{00} has accepted your friend request!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString alreadyfriends = @"You are already friends with {00}.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString busy = @"{00} is busy. Please try again later!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString notification = @"You are now friends with {00}!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString remove = @"Friend removed.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString sent = @"Friend request sent.";

        }

        public sealed class GeneralNamespace : LocaleNamespace
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString none = @"None";

        }

        public sealed class IntroNamespace : LocaleNamespace
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString api = @"Starting API on TCP Port #{00}";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString consoleactive =
                @"Type exit to shutdown the server, or help for a list of commands.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString exit = @"Press any key to exit.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString loading = @"Loading, please wait.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString started = @"Server Started. Using UDP Port #{00}";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString support =
                @"For help, support, and updates visit: https://www.ascensiongamedev.com";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString tagline = @"                          free 2d orpg engine";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString title = @"Intersect Server";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString version = @"Version {00}";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString websocketstarted =
                @"Websocket listener started for Unity WebGL Clients using Port #{00}";

        }

        public sealed class ItemsNamespace : LocaleNamespace
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString bound = @"You cannot drop this item.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString cannotuse = @"You cannot use this item!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString cooldown = @"You must wait before using this item again!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString dynamicreq = @"You do not meet the requirements to use this item!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString equipped = @"You cannot drop equipped items.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString notenough = @"Not enough {00}s!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString notimplemented = @"Use of this item type is not yet implemented.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString statreq = @"You do not possess the correct combat stats to use this item.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString stunned = @"You cannot use this item whilst stunned.";

        }

        public sealed class MappingNamespace : LocaleNamespace
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString lastmap = @"Last Map";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString lastmaperror =
                @"Failed to delete map, you must have at least one map at all times!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString linkfail = @"Map Link Failure";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString linkfailerror =
                @"Failed to link map {00} to map {01}. If this merge were to happen, maps {02} and {03} would occupy the same space in the world.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString newfolder = @"New Folder";

        }

        public sealed class MigrationNamespace : LocaleNamespace
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString alreadyusingengine =
                @"   Migration Error: {00} database is already using {01}!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString cancel = @"   Press any other key to cancel migration.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString currentlymysql = @"currently using MySql";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString currentlysqlite = @"currently using Sqlite";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString entermysqlinfo = @"Please enter your Mysql connection parameters:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString gamedb = @"Game";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString migratetomysql = @"   [2] Mysql";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString migratetosqlite = @"   [1] Sqlite";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString migrationcancelled = @"Migration Cancelled";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString migrationcomplete = @"Migration complete! Press any key to exit.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString mysql = @"Mysql";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString mysqlconnecting = @"Please wait, attempting to connect to database...";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString mysqlconnectionerror = @"Error opening db connection! Error: {00}";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString mysqldatabase = @"Database: ";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString mysqlhost = @"Host: ";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString mysqlnotempty =
                @"Database must be empty before migration! Please delete any tables before proceeding! Migration Cancelled.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString mysqlpass = @"Password: ";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString mysqlport = @"Port: ";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString mysqltryagain =
                @"Would you like to try entering your connection info again? (y/n)  ";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString mysqluser = @"User: ";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString overwritecharacter = @"y";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString playerdb = @"Player";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString selectdb = @"Which database would you like to migrate:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString selectdbengine = @"Select which engine to migrate the {00} database to:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString selectgamedb =
                "   [1] Game Database ({00})  -  Sqlite Strongly Recommended!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString selectgamedbkey = @"1";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString selectmysqlkey = @"2";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString selectplayerdb = "   [2] Player Database ({00})";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString selectplayerdbkey = @"2";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString selectsqlitekey = @"1";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString sqlite = @"Sqlite";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString sqlitealreadyexists = @"{00} already exists, overwrite? (y/n)  ";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString startingmigration =
                @"Starting migration, please wait! (This could take several minutes depending on the size of your game)";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString stoppingserver =
                @"Please wait, stopping server, and saving current database...";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString tryagaincharacter = @"y";

        }

        public sealed class NetDebugNamespace : LocaleNamespace
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString hastebin =
                @"Network Debug information uploaded to {00} (copied to clipboard) share this link with AGD when requesting for help getting your game online!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString pleasewait = @"Please wait while network diagnostics run....";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString savedtofile =
                @"Network Debug information saved to netdebug.txt! Upload that file and share it with AGD when requesting for help getting your game online!";

        }

        public sealed class NetworkingNamespace : LocaleNamespace
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString badpacket =
                @"Error handling client packet. Disconnecting client. More info logged to errors.log";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString disconnected = @"Client disconnected.";

        }

        public sealed class PartiesNamespace : LocaleNamespace
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString alreadydenied = @"Your party invitation has already been rejected!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString busy = @"{00} is busy. Please try again later!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString declined = @"{00} has declined your party invitation!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString disbanded = @"The party has been disbanded.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString joined = @"{00} has joined the party!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString kicked = @"You have been kicked from the party!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString
                leaderinvonly = @"Only the party leader can send invitations to your party.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString left = @"You have left the party.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString limitreached =
                @"You have reached the maximum limit of party members. Kick another member before adding more.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString memberkicked = @"{00} has been kicked from the party!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString memberleft = @"{00} has left the party!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString notinparty = @"You are not in a party.";

        }

        public sealed class PlayerNamespace : LocaleNamespace
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString admin = @"{00} has been given administrative powers!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString adminjoined =
                @"You are an administrator! Press Insert at any time to access the administration menu or F2 for debug information.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString adminsetpower = @"Only admins can set power!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString beenwarpedto = @"You have been warped to {00}.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString changeownpower = @"You cannot alter your own power!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString deadmin = @"{00} has had their administrative powers revoked!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString demod = @"{00} has had their moderation powers revoked!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString haswarpedto = @"{00} has been warped to you.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString joined = @"{00} has joined {01}.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString kicked = @"{00} has been kicked by {01}!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString killed = @"{00} has been killed by {01}!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString left = @"{00} has left {01}.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString levelup = @"You have leveled up! You are now level {00}!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString mod = @"{00} has been given moderation powers!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString modjoined =
                @"You are a moderator! Press Insert at any time to access the administration menu or F2 for debug information.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString notarget = @"You need to select a valid target.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString offline = @"User not online!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString powerchanged = @"Your power has been modified!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString serverkicked = @"{00} has been kicked by the server!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString serverkilled = @"{00} has been killed by the server!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString spelltaughtlevelup = @"You've learned the {00} spell!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString statpoints = @"You have {00} stat points available to be spent!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString targetoutsiderange = @"Target not in range.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString warpedto = @"Warped to {00}.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString warpedtoyou = @"{00} warped to you.";

        }

        public sealed class PortcheckingNamespace : LocaleNamespace
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString accessible = @"Your game is accesible to the public!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString checkantivirus =
                @"   2. Antivirus programs might also be blocking connections and you may need to add Intersect Server.exe to your antivirus exclusions.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString checkfirewalls =
                @"   1. Firewalls might be blocking connections to your server. Check firewalls on your system. (i.e. iptables, FirewallD, Windows Firewall)";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString checkrouterupnp =
                @"It appears that UPnP Failed. Your might need to enable UPnP on your router or manually port forward to allow connections to your server.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString connectioninfo = @"Connection Information:";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString debuggingsteps = @"Debugging Steps (To allow public access):";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString letothersjoin =
                @"Enter your public ip and port into the client/editor config for others to join!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString notaccessible =
                @"It does not appear that your game is accessible to the outside world.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString notconnected =
                @"   Could not retreive connection information from AGD servers. Do you have an internet connection?";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString publicip = @"   Public IP: {00}";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString publicport = @"   Public Port: {00}";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString screwed =
                @"   3. If on a college campus, or within a business network you likely do not have permission to open ports or host games in which case you are screwed!";

        }

        public sealed class QuestsNamespace : LocaleNamespace
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString abandoned = @"Quest Abandoned: {00}!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString completed = @"Quest: {00} completed!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString declined = @"Quest Declined: {00}!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString itemtask = @"{00}  updated! {01}/{02} {03}(s) gathered!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString npctask = @"{00}  updated! {01}/{02} {03}(s) slain!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString started = @"Quest Started: {00}!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString taskcompleted = @"Task Completed!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString updated = @"Quest: {00} updated!";

        }

        public sealed class RegexNamespace : LocaleNamespace
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString email =
                @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString
                password = @"^[-_=\+`~!@#\$%\^&\*()\[\]{}\\|;\:'"",<\.>/\?a-zA-Z0-9]{4,64}$";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString username = @"^[a-zA-Z0-9]{2,20}$";

        }

        public sealed class ShopsNamespace : LocaleNamespace
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString bound = @"This item is bound to you and cannot be sold!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString cantafford = @"Transaction failed due to insufficent funds.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString doesnotaccept = @"This shop does not accept that item!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString inventoryfull = @"You do not have space to purchase that item!";

        }

        public sealed class TradingNamespace : LocaleNamespace
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString accepted = @"The trade was successful!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString alreadydenied = @"Your trade request has already been denied!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString bound = @"This item is bound to you and cannot be traded!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString busy = @"{00} is busy. Please try again later!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString declined = @"The trade was declined!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString inventorynospace =
                @"There is no space left in your inventory for that item!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString itemsdropped =
                @"Out of inventory space. Some of your items have been dropped on the ground!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString offerinvalid = @"Invalid item selected to offer!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString revokeinvalid = @"Invalid item selected to revoke!";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString tradenospace = @"There is no space left in the trade window for that item!";

        }

        public sealed class UpnpNamespace : LocaleNamespace
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString failedforwardingtcp =
                @"Failed to automatically port forward tcp port {00} using UPnP. (UPnP possibly disabled in your router settings, or this port might already be forwarded!)";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString failedforwardingudp =
                @"Failed to automatically port forward udp port {00} using UPnP. (UPnP possibly disabled in your router settings, or this port might already be forwarded!)";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString
                forwardedtcp = @"Successfully auto port forwarded tcp port {00} using UPnP.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString
                forwardedudp = @"Successfully auto port forwarded udp port {00} using UPnP.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString initializationfailed =
                @"UPnP Service Initialization Failed. You might not have a router, or UPnP on your router might be disabled.";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public readonly LocalizedString initialized = @"UPnP Service Initialization Succeeded";

        }

        #region Serialization

        public static bool Load()
        {
            var filepath = Path.Combine("server_strings.json");

            // Really don't want two JsonSave() return points...
            // ReSharper disable once InvertIf
            if (File.Exists(filepath))
            {
                var json = File.ReadAllText(filepath, Encoding.UTF8);
                try
                {
                    Root = JsonConvert.DeserializeObject<RootNamespace>(json) ?? Root;
                }
                catch (Exception exception)
                {
                    Log.Error(exception);

                    return false;
                }
            }

            return Save();
        }

        public static bool Save()
        {
            try
            {
                var filepath = Path.Combine("server_strings.json");
                var json = JsonConvert.SerializeObject(Root, Formatting.Indented, new LocalizedStringConverter());
                File.WriteAllText(filepath, json, Encoding.UTF8);

                return true;
            }
            catch (Exception exception)
            {
                Log.Error(exception);

                return false;
            }
        }

        #endregion

        #region Root Namespace

        static Strings()
        {
            Root = new RootNamespace();
        }

        [NotNull]
        private static RootNamespace Root { get; set; }

        private sealed class RootNamespace : LocaleNamespace
        {

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
            public readonly AccountNamespace AccountNamespace = new AccountNamespace();

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
            public readonly BagsNamespace BagsNamespace = new BagsNamespace();

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
            public readonly BanksNamespace BanksNamespace = new BanksNamespace();

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
            public readonly ChatNamespace ChatNamespace = new ChatNamespace();

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
            public readonly ClassesNamespace ClassesNamespace = new ClassesNamespace();

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
            public readonly ColorsNamespace ColorsNamespace = new ColorsNamespace();

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
            public readonly CombatNamespace CombatNamespace = new CombatNamespace();

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
            public readonly CommandoutputNamespace CommandoutputNamespace = new CommandoutputNamespace();

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
            public readonly CommandsNamespace CommandsNamespace = new CommandsNamespace();

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
            public readonly CraftingNamespace CraftingNamespace = new CraftingNamespace();

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
            public readonly DatabaseNamespace DatabaseNamespace = new DatabaseNamespace();

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
            public readonly ErrorsNamespace ErrorsNamespace = new ErrorsNamespace();

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
            public readonly EventsNamespace EventsNamespace = new EventsNamespace();

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
            public readonly FormulasNamespace FormulasNamespace = new FormulasNamespace();

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
            public readonly FriendsNamespace FriendsNamespace = new FriendsNamespace();

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
            public readonly GeneralNamespace GeneralNamespace = new GeneralNamespace();

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
            public readonly IntroNamespace IntroNamespace = new IntroNamespace();

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
            public readonly ItemsNamespace ItemsNamespace = new ItemsNamespace();

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
            public readonly MappingNamespace MappingNamespace = new MappingNamespace();

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
            public readonly MigrationNamespace MigrationNamespace = new MigrationNamespace();

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
            public readonly NetDebugNamespace NetDebugNamespace = new NetDebugNamespace();

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
            public readonly NetworkingNamespace NetworkingNamespace = new NetworkingNamespace();

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
            public readonly PartiesNamespace PartiesNamespace = new PartiesNamespace();

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
            public readonly PlayerNamespace PlayerNamespace = new PlayerNamespace();

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
            public readonly PortcheckingNamespace PortcheckingNamespace = new PortcheckingNamespace();

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
            public readonly QuestsNamespace QuestsNamespace = new QuestsNamespace();

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
            public readonly RegexNamespace RegexNamespace = new RegexNamespace();

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
            public readonly ShopsNamespace ShopsNamespace = new ShopsNamespace();

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
            public readonly TradingNamespace TradingNamespace = new TradingNamespace();

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] [NotNull]
            public readonly UpnpNamespace UpnpNamespace = new UpnpNamespace();

        }

        #endregion

        #region Namespace Exposure

        [NotNull]
        public static AccountNamespace Account => Root.AccountNamespace;

        [NotNull]
        public static BagsNamespace Bags => Root.BagsNamespace;

        [NotNull]
        public static BanksNamespace Banks => Root.BanksNamespace;

        [NotNull]
        public static ChatNamespace Chat => Root.ChatNamespace;

        [NotNull]
        public static ClassesNamespace Classes => Root.ClassesNamespace;

        [NotNull]
        public static ColorsNamespace Colors => Root.ColorsNamespace;

        [NotNull]
        public static CombatNamespace Combat => Root.CombatNamespace;

        [NotNull]
        public static CommandoutputNamespace Commandoutput => Root.CommandoutputNamespace;

        [NotNull]
        public static CommandsNamespace Commands => Root.CommandsNamespace;

        [NotNull]
        public static CraftingNamespace Crafting => Root.CraftingNamespace;

        [NotNull]
        public static DatabaseNamespace Database => Root.DatabaseNamespace;

        [NotNull]
        public static ErrorsNamespace Errors => Root.ErrorsNamespace;

        [NotNull]
        public static EventsNamespace Events => Root.EventsNamespace;

        [NotNull]
        public static FormulasNamespace Formulas => Root.FormulasNamespace;

        [NotNull]
        public static FriendsNamespace Friends => Root.FriendsNamespace;

        [NotNull]
        public static GeneralNamespace General => Root.GeneralNamespace;

        [NotNull]
        public static IntroNamespace Intro => Root.IntroNamespace;

        [NotNull]
        public static ItemsNamespace Items => Root.ItemsNamespace;

        [NotNull]
        public static MappingNamespace Mapping => Root.MappingNamespace;

        [NotNull]
        public static MigrationNamespace Migration => Root.MigrationNamespace;

        [NotNull]
        public static NetDebugNamespace NetDebug => Root.NetDebugNamespace;

        [NotNull]
        public static NetworkingNamespace Networking => Root.NetworkingNamespace;

        [NotNull]
        public static PartiesNamespace Parties => Root.PartiesNamespace;

        [NotNull]
        public static PlayerNamespace Player => Root.PlayerNamespace;

        [NotNull]
        public static PortcheckingNamespace Portchecking => Root.PortcheckingNamespace;

        [NotNull]
        public static QuestsNamespace Quests => Root.QuestsNamespace;

        [NotNull]
        public static RegexNamespace Regex => Root.RegexNamespace;

        [NotNull]
        public static ShopsNamespace Shops => Root.ShopsNamespace;

        [NotNull]
        public static TradingNamespace Trading => Root.TradingNamespace;

        [NotNull]
        public static UpnpNamespace Upnp => Root.UpnpNamespace;

        #endregion

    }

}
