using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Intersect.Server.Classes.Localization
{
    public static class Strings
    {
        public struct Account
        {
            public static LocalizedString badaccess = @"Access denied! Invalid power level!";
            public static LocalizedString badlogin = @"Username or password incorrect.";
            public static LocalizedString banned = @"{00} has been banned!";
            public static LocalizedString banstatus = @"Your account has been banned since: {00} by {01}. Ban expires: {02}. Reason for ban: {03}";
            public static LocalizedString characterexists = @"An account with this character name exists. Please choose another.";
            public static LocalizedString deletechar = @"The character has been deleted.";
            public static LocalizedString deleted = @"Delete Character";
            public static LocalizedString doesnotexist = @"Account does not exist.";
            public static LocalizedString emailexists = @"An account with this email address already exists.";
            public static LocalizedString exists = @"Account already exists!";
            public static LocalizedString invalidclass = @"Invalid class selected. Try again.";
            public static LocalizedString invalidemail = @"The chosen email does not meet requirements set by the server.";
            public static LocalizedString invalidname = @"The chosen name does not meet requirements set by the server.";
            public static LocalizedString loadfail = @"Failed to load account. Please try logging in again.";
            public static LocalizedString maxchars = @"You have already created the maximum number of characters. Delete one before creating a new one.";
            public static LocalizedString muted = @"{00} has been muted!";
            public static LocalizedString mutestatus = @"Your account has been muted since: {00} by {01}. Mute expires: {02}. Reason for mute: {03}";
            public static LocalizedString notfound = @"Error: Account {00} was not found!";
            public static LocalizedString unbanned = @"Account {00} has been unbanned!";
            public static LocalizedString unmuted = @"{00} has been unmuted!";
        }

        public struct Bags
        {
            public static LocalizedString baginself = @"You cannot store a bag in within itself!";
            public static LocalizedString bagnospace = @"There is no space left in your bag for that item!";
            public static LocalizedString depositinvalid = @"Invalid item selected to store!";
            public static LocalizedString inventorynospace = @"There is no space left in your inventory for that item!";
            public static LocalizedString onlysellempty = @"Cannot sell bag unless it's empty!";
            public static LocalizedString onlytradeempty = @"Cannot trade bag unless it's empty!";
            public static LocalizedString withdrawinvalid = @"Invalid item selected to retreive!";
        }

        public struct Banks
        {
            public static LocalizedString banknospace = @"There is no space left in your bank for that item!";
            public static LocalizedString depositinvalid = @"Invalid item selected to deposit!";
            public static LocalizedString inventorynospace = @"There is no space left in your inventory for that item!";
            public static LocalizedString withdrawinvalid = @"Invalid item selected to withdraw!";
        }

        public struct Chat
        {
            public static LocalizedString admin = @"[ADMIN] {00}: {01}";
            public static LocalizedString admincmd = @"/admin";
            public static LocalizedString allcmd = @"/all";
            public static LocalizedString announcement = @"[ANNOUNCEMENT] {00}: {01}";
            public static LocalizedString announcementcmd = @"/announcement";
            public static LocalizedString Global = @"[GLOBAL] {00}: {01}";
            public static LocalizedString globalcmd = @"/global";
            public static LocalizedString local = @"{00}: {01}";
            public static LocalizedString localcmd = @"/local";
            public static LocalizedString messagecmd = @"/message";
            public static LocalizedString party = @"[PARTY] {00}: {01}";
            public static LocalizedString partycmd = @"/party";
            public static LocalizedString pmcmd = @"/pm";
            public static LocalizedString Private = @"[PM] {00}: {01}";
            public static LocalizedString rcmd = @"/r";
            public static LocalizedString replycmd = @"/reply";
        }

        public struct Classes
        {
            public static LocalizedString lastclass = @"Last Class";
            public static LocalizedString lastclasserror = @"Failed to delete class, you must have at least one class at all times!";
        }

        public struct Combat
        {
            public static LocalizedString exp = @"Experience";
            public static LocalizedString addsymbol = @"+";
            public static LocalizedString blocked = @"BLOCKED!";
            public static LocalizedString channeling = @"You are currently channeling another skill.";
            public static LocalizedString channelingnoattack = @"You are currently channeling a spell, you cannot attack.";
            public static LocalizedString cooldown = @"This skill is on cooldown.";
            public static LocalizedString critical = @"CRITICAL HIT!";
            public static LocalizedString[] damagetypes = new LocalizedString[]
            {
                @"Physical",
                @"Magic",
                @"True",
            };
            public static LocalizedString dash = @"DASH!";
            public static LocalizedString dynamicreq = @"You do not meet the requirements to cast the spell!";
            public static LocalizedString levelreq = @"You are not a high enough level to use this ability.";
            public static LocalizedString levelup = @"LEVEL UP!";
            public static LocalizedString lowhealth = @"Not enough health.";
            public static LocalizedString lowmana = @"Not enough mana.";
            public static LocalizedString miss = @"MISS!";
            public static LocalizedString notarget = @"No Target!";
            public static LocalizedString removesymbol = @"-";
            public static LocalizedString resourcereqs = @"You do not meet the requirements to harvest this resource!";
            public static LocalizedString silenced = @"You cannot cast this ability whilst silenced.";
            public static LocalizedString statreq = @"You do not possess the correct combat stats to use this ability.";
            public static LocalizedString[] stats = new LocalizedString[]
            {
                @"Attack",
                @"Ability Power",
                @"Defense",
                @"Magic Resist",
                @"Speed",
            };
            public static LocalizedString[] status =
            {
                @"NONE!",
                @"SILENCED!",
                @"STUNNED!",
                @"SNARED!",
                @"BLINDED!",
                @"STEALTH!",
                @"TRANSFORMED!"
            };
            public static LocalizedString stunattacking = @"You are stunned and can't attack.";
            public static LocalizedString stunblocking = @"You are stunned and can't block.";
            public static LocalizedString stunned = @"You cannot cast this ability whilst stunned.";
            public static LocalizedString targetoutsiderange = @"Target is out of range!";
            public static LocalizedString toolrequired = @"You require a {00} to interact with this resource!";
            public static LocalizedString[] vitals = new LocalizedString[]
            {
                @"Health",
                @"Mana",
            };
        }

        public struct Commandoutput
        {
            public static LocalizedString cps = @"Current CPS: {00}";
            public static LocalizedString cpslocked = @"CPS Locked";
            public static LocalizedString cpsunlocked = @"CPS Unlocked";
            public static LocalizedString gametime = @"Game time is now: {00}";
            public static LocalizedString helpfooter = @"Type in any command followed by {00} for parameters and usage information.";
            public static LocalizedString helpheader = @"List of available commands:";
            public static LocalizedString invalidparameters = @"Invalid parameters provided! Use {00} to get more info about a command.";
            public static LocalizedString killsuccess = @"{00} has been killed!";
            public static LocalizedString listaccount = @"Account";
            public static LocalizedString listcharacter = @"Character";
            public static LocalizedString listid = @"ID";
            public static LocalizedString notfound = @"Command not recoginized. Enter help for a list of commands. Remember console commands are case sensitive!";
            public static LocalizedString parseerror = @"Parse Error: Parameter could not be read. Type {00} {01} for usage information.";
            public static LocalizedString playercount = @"Server has {00} registered players.";
            public static LocalizedString powerchanged = @"{00} has had their power updated!";
            public static LocalizedString powerlevel = @"{00}'s power has been set to {01}!";
            public static LocalizedString syntaxerror = @"Syntax Error: Expected parameter not found. Type {00} {01} for usage information.";
        }

        public struct Commands
        {
            public static LocalizedString announcement = @"announcement";
            public static LocalizedString announcementdesc = @"Desc: Sends a global message to all users playing the game.";
            public static LocalizedString announcementhelp = @"sends a global message to all players";
            public static LocalizedString announcementusage = @"Usage: announcement [message] {00}";
            public static LocalizedString ban = @"ban";
            public static LocalizedString bandesc = @"Desc: Bans a player from the server.";
            public static LocalizedString banhelp = @"bans a player from the server";
            public static LocalizedString banusage = @"Usage: ban [username] [duration (days)] [IP Ban? ({00}/{01})] [reason] {02}";
            public static LocalizedString banuser = @"console";
            public static LocalizedString commandinfo = @"/?";
            public static LocalizedString cps = @"cps";
            public static LocalizedString cpsdesc = @"Desc: Prints the current CPS. The status flag tells if the server loop is locked or unlocked. The lock flag locks the cps while the unlock flag unlocks it.";
            public static LocalizedString cpshelp = @"prints the current server cps";
            public static LocalizedString cpslock = @"lock";
            public static LocalizedString cpsstatus = @"status";
            public static LocalizedString cpsunlock = @"unlock";
            public static LocalizedString cpsusage = @"Usage: cps [status] [lock] [unlock] {00}";
            public static LocalizedString exit = @"exit";
            public static LocalizedString exitdesc = @"Desc: Closes down the server.";
            public static LocalizedString exithelp = @"closes the server";
            public static LocalizedString exitusage = @"Usage: exit {00}";
            public static LocalizedString False = @"false";
            public static LocalizedString help = @"help";
            public static LocalizedString helpdesc = @"help";
            public static LocalizedString helphelp = @"displays list of available commands";
            public static LocalizedString helpusage = @"help";
            public static LocalizedString invalid = @"Invalid /command.";
            public static LocalizedString kick = @"kick";
            public static LocalizedString kickdesc = @"Desc: Kicks a player from the server.";
            public static LocalizedString kickhelp = @"kicks a player from the server";
            public static LocalizedString kickusage = @"Usage: kick [username] {00}";
            public static LocalizedString kill = @"kill";
            public static LocalizedString killdesc = @"Desc: Kills a player on the server.";
            public static LocalizedString killhelp = @"kills a player on the server";
            public static LocalizedString killusage = @"Usage: kill [username] {00}";
            public static LocalizedString mute = @"mute";
            public static LocalizedString mutedesc = @"Desc: mutes a player preventing them from talking.";
            public static LocalizedString mutehelp = @"mutes a player preventing them from talking";
            public static LocalizedString muteusage = @"Usage: mute [username] [duration (days)] [IP Ban? ({00}/{01})] [reason] {02}";
            public static LocalizedString muteuser = @"console";
            public static LocalizedString onlinelist = @"onlinelist";
            public static LocalizedString onlinelisthelp = @"shows all players online";
            public static LocalizedString power = @"power";
            public static LocalizedString poweracc = @"poweracc";
            public static LocalizedString poweraccdesc = @"Desc: Sets the power or access of a selected account. Power 0 is regular user. Power 1 is in-game moderator. Power 2 is owner/designer and allows editor access.";
            public static LocalizedString poweracchelp = @"sets the administrative access of an account";
            public static LocalizedString poweraccusage = @"Usage: power [login] [level] {00}";
            public static LocalizedString powerdesc = @"Desc: Sets the power or access of a selected account. Power 0 is regular user. Power 1 is in-game moderator. Power 2 is owner/designer and allows editor access.";
            public static LocalizedString powerhelp = @"sets the administrative access of a user";
            public static LocalizedString powerusage = @"Usage: power [username] [level] {00}";
            public static LocalizedString True = @"true";
            public static LocalizedString unban = @"unban";
            public static LocalizedString unbandesc = @"Desc: Unbans a player from the server.";
            public static LocalizedString unbanhelp = @"unbans a player from the server";
            public static LocalizedString unbanusage = @"Usage: unban [account] {00}";
            public static LocalizedString unmute = @"unmute";
            public static LocalizedString unmutedesc = @"Desc: unmutes a player allowing them to talk.";
            public static LocalizedString unmutehelp = @"unmutes a player allowing them to talk";
            public static LocalizedString unmuteusage = @"Usage: unmute [username] {00}";
        }

        public struct Crafting
        {
            public static LocalizedString crafted = @"You successfully crafted {00}!";
            public static LocalizedString nospace = @"You do not have enough inventory space to craft {00}!";
        }

        public struct Database
        {
            public static LocalizedString Default = @"Default";
            public static LocalizedString noclasses = @"No classes found! - Creating a default class!";
            public static LocalizedString nomaps = @"No maps found! - Creating an empty map!";
            public static LocalizedString nullerror = @"Tried to load one or more null game objects!";
            public static LocalizedString nullfound = @"Tried to load null value for index {00} of {01}";
            public static LocalizedString outofdate = @"Database is out of date! Version: {00} Expected Version: {01}. Please run the included migration tool!";
            public static LocalizedString usingsqlite = @"Using SQLite Database for account and data storage.";
        }

        public struct Errors
        {
            public static LocalizedString errorlogged = @"An error was logged into errors.log";
            public static LocalizedString errorservercrash = @"The Intersect server has encountered an error and must close. Error information can be found in resources/logs/errors.log. Press any key to exit.";
            public static LocalizedString errorservercrashnohalt = @"The Intersect server has encountered an error and must close. Error information can be found in resources/logs/errors.log.";
            public static LocalizedString warpfail = @"Failed to warp player to new map -- warping to spawn.";
        }

        public struct Events
        {
            public static LocalizedString commandparameter = @"\param";
            public static LocalizedString eventnamecommand = @"\en";
            public static LocalizedString globalswitch = @"\gs";
            public static LocalizedString globalvar = @"\gv";
            public static LocalizedString militaryhour = @"\24hour";
            public static LocalizedString onlinecountcommand = @"\onlinecount";
            public static LocalizedString onlinelistcommand = @"\onlinelist";
            public static LocalizedString periodevening = @"PM";
            public static LocalizedString periodmorning = @"AM";
            public static LocalizedString playernamecommand = @"\pn";
            public static LocalizedString playerswitch = @"\ps";
            public static LocalizedString playervar = @"\pv";
            public static LocalizedString timehour = @"\hour";
            public static LocalizedString timeminute = @"\minute";
            public static LocalizedString timeperiod = @"\period";
            public static LocalizedString timesecond = @"\second";
        }

        public struct Formulas
        {
            public static LocalizedString loadfailed = @"Failed to load formulas! Press any key to shut down.";
            public static LocalizedString missing = @"Formulas.xml missing. Generated default formulas file.";
            public static LocalizedString syntax = @"Error loading formulas! Please make sure the file exists and is free on syntax errors.";
        }

        public struct Friends
        {
            public static LocalizedString accept = @"{00} has accepted your friend request!";
            public static LocalizedString alreadyfriends = @"You are already friends with {00}.";
            public static LocalizedString busy = @"{00} is busy. Please try again later!";
            public static LocalizedString notification = @"You are now friends with {00}!";
            public static LocalizedString remove = @"Friend removed.";
            public static LocalizedString sent = @"Friend request sent.";
        }

        public struct General
        {
            public static LocalizedString none = @"None";
        }

        public struct Intro
        {
            public static LocalizedString consoleactive = @"Type exit to shutdown the server, or help for a list of commands.";
            public static LocalizedString exit = @"Press any key to exit.";
            public static LocalizedString loading = @"Loading, please wait.";
            public static LocalizedString started = @"Server Started. Using Port #{00}";
            public static LocalizedString support = @"For help, support, and updates visit: http://ascensiongamedev.com";
            public static LocalizedString tagline = @"                          free 2d orpg engine";
            public static LocalizedString title = @"Intersect Server";
            public static LocalizedString version = @"Version {00}";
            public static LocalizedString websocketstarted = @"Websocket listener started for Unity WebGL Clients using Port #{00}";
        }

        public struct Items
        {
            public static LocalizedString bound = @"You cannot drop this item.";
            public static LocalizedString cannotuse = @"You cannot use this item!";
            public static LocalizedString dynamicreq = @"You do not meet the requirements to use this item!";
            public static LocalizedString equipped = @"You cannot drop equipped items.";
            public static LocalizedString notenough = @"Not enough {00}s!";
            public static LocalizedString notimplemented = @"Use of this item type is not yet implemented.";
            public static LocalizedString statreq = @"You do not possess the correct combat stats to use this item.";
            public static LocalizedString stunned = @"You cannot use this item whilst stunned.";
        }

        public struct Mapping
        {
            public static LocalizedString lastmap = @"Last Map";
            public static LocalizedString lastmaperror = @"Failed to delete map, you must have at least one map at all times!";
            public static LocalizedString linkfail = @"Map Link Failure";
            public static LocalizedString linkfailerror = @"Failed to link map {00} to map {01}. If this merge were to happen, maps {02} and {03} would occupy the same space in the world.";
            public static LocalizedString newfolder = @"New Folder";
        }

        public struct Networking
        {
            public static LocalizedString badpacket = @"Error handling client packet. Disconnecting client. More info logged to errors.log";
            public static LocalizedString disconnected = @"Client disconnected.";
        }

        public struct Parties
        {
            public static LocalizedString alreadydenied = @"Your party invitation has already been rejected!";
            public static LocalizedString busy = @"{00} is busy. Please try again later!";
            public static LocalizedString declined = @"{00} has declined your party invitation!";
            public static LocalizedString disbanded = @"The party has been disbanded.";
            public static LocalizedString joined = @"{00} has joined the party!";
            public static LocalizedString kicked = @"You have been kicked from the party!";
            public static LocalizedString leaderinvonly = @"Only the party leader can send invitations to your party.";
            public static LocalizedString left = @"You have left the party.";
            public static LocalizedString limitreached = @"You have reached the maximum limit of party members. Kick another member before adding more.";
            public static LocalizedString memberkicked = @"{00} has been kicked from the party!";
            public static LocalizedString memberleft = @"{00} has left the party!";
            public static LocalizedString notinparty = @"You are not in a party.";
        }

        public struct Player
        {
            public static LocalizedString admin = @"{00} has been given administrative powers!";
            public static LocalizedString adminjoined = @"You are an administrator! Press Insert at any time to access the administration menu or F2 for debug information.";
            public static LocalizedString adminsetpower = @"Only admins can set power!";
            public static LocalizedString beenwarpedto = @"You have been warped to {00}.";
            public static LocalizedString changeownpower = @"You cannot alter your own power!";
            public static LocalizedString deadmin = @"{00} has had their administrative powers revoked!";
            public static LocalizedString demod = @"{00} has had their moderation powers revoked!";
            public static LocalizedString haswarpedto = @"{00} has been warped to you.";
            public static LocalizedString joined = @"{00} has joined {01}.";
            public static LocalizedString kicked = @"{00} has been kicked by {01}!";
            public static LocalizedString killed = @"{00} has been killed by {01}!";
            public static LocalizedString left = @"{00} has left {01}.";
            public static LocalizedString levelup = @"You have leveled up! You are now level {00}!";
            public static LocalizedString mod = @"{00} has been given moderation powers!";
            public static LocalizedString modjoined = @"You are a moderator! Press Insert at any time to access the administration menu or F2 for debug information.";
            public static LocalizedString notarget = @"You need to select a valid target.";
            public static LocalizedString offline = @"User not online!";
            public static LocalizedString powerchanged = @"Your power has been modified!";
            public static LocalizedString saved = @"Your progress has been automatically saved.";
            public static LocalizedString serverkicked = @"{00} has been kicked by the server!";
            public static LocalizedString serverkilled = @"{00} has been killed by the server!";
            public static LocalizedString spelltaughtlevelup = @"You've learned the {00} spell!";
            public static LocalizedString statpoints = @"You have {00} stat points available to be spent!";
            public static LocalizedString targetoutsiderange = @"Target not in range.";
            public static LocalizedString warpedto = @"Warped to {00}.";
            public static LocalizedString warpedtoyou = @"{00} warped to you.";
        }

        public struct Portchecking
        {
            public static LocalizedString accessible = @"Your game is accesible to the public!";
            public static LocalizedString checkantivirus = @"   2. Antivirus programs might also be blocking connections and you may need to add Intersect Server.exe to your antivirus exclusions.";
            public static LocalizedString checkfirewalls = @"   1. Firewalls might be blocking connections to your server. Check firewalls on your system. (i.e. iptables, FirewallD, Windows Firewall)";
            public static LocalizedString checkrouterupnp = @"It appears that UPnP Failed. Your might need to enable UPnP on your router or manually port forward to allow connections to your server.";
            public static LocalizedString connectioninfo = @"Connection Information:";
            public static LocalizedString debuggingsteps = @"Debugging Steps (To allow public access):";
            public static LocalizedString letothersjoin = @"Enter your public ip and port into the client/editor config for others to join!";
            public static LocalizedString notaccessible = @"It does not appear that your game is accessible to the outside world.";
            public static LocalizedString notconnected = @"   Could not retreive connection information from AGD servers. Do you have an internet connection?";
            public static LocalizedString publicip = @"   Public IP: {00}";
            public static LocalizedString publicport = @"   Public Port: {00}";
            public static LocalizedString screwed = @"   3. If on a college campus, or within a business network you likely do not have permission to open ports or host games in which case you are screwed!";
        }

        public struct Quests
        {
            public static LocalizedString abandoned = @"Quest Abandoned: {00}!";
            public static LocalizedString completed = @"Quest: {00} completed!";
            public static LocalizedString declined = @"Quest Declined: {00}!";
            public static LocalizedString itemtask = @"{00}  updated! {01}/{02} {03}(s) gathered!";
            public static LocalizedString npctask = @"{00}  updated! {01}/{02} {03}(s) slain!";
            public static LocalizedString started = @"Quest Started: {00}!";
            public static LocalizedString taskcompleted = @"Task Completed!";
            public static LocalizedString updated = @"Quest: {00} updated!";
        }

        public struct Regex
        {
            public static LocalizedString email = @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";
            public static LocalizedString password = @"^[-_=\+`~!@#\$%\^&\*()\[\]{}\\|;\:'"",<\.>/\?a-zA-Z0-9]{4,64}$";
            public static LocalizedString username = @"^[a-zA-Z0-9]{2,20}$";
        }

        public struct Shops
        {
            public static LocalizedString bound = @"This item is bound to you and cannot be sold!";
            public static LocalizedString cantafford = @"Transaction failed due to insufficent funds.";
            public static LocalizedString doesnotaccept = @"This shop does not accept that item!";
            public static LocalizedString inventoryfull = @"You do not have space to purchase that item!";
        }

        public struct Trading
        {
            public static LocalizedString accepted = @"The trade was successful!";
            public static LocalizedString alreadydenied = @"Your trade request has already been denied!";
            public static LocalizedString busy = @"{00} is busy. Please try again later!";
            public static LocalizedString declined = @"The trade was declined!";
            public static LocalizedString inventorynospace = @"There is no space left in your inventory for that item!";
            public static LocalizedString itemsdropped = @"Out of inventory space. Some of your items have been dropped on the ground!";
            public static LocalizedString offerinvalid = @"Invalid item selected to offer!";
            public static LocalizedString revokeinvalid = @"Invalid item selected to revoke!";
            public static LocalizedString tradenospace = @"There is no space left in the trade window for that item!";
        }

        public struct Upnp
        {
            public static LocalizedString failedforwardingtcp = @"Failed to automatically port forward tcp port {00} using UPnP. (UPnP possibly disabled in your router settings, or this port might already be forwarded!)";
            public static LocalizedString failedforwardingudp = @"Failed to automatically port forward udp port {00} using UPnP. (UPnP possibly disabled in your router settings, or this port might already be forwarded!)";
            public static LocalizedString forwardedtcp = @"Successfully auto port forwarded tcp port {00} using UPnP.";
            public static LocalizedString forwardedudp = @"Successfully auto port forwarded udp port {00} using UPnP.";
            public static LocalizedString initializationfailed = @"UPnP Service Initialization Failed. You might not have a router, or UPnP on your router might be disabled.";
            public static LocalizedString initialized = @"UPnP Service Initialization Succeeded";
        }



        public static void Load(string language)
        {
            if (File.Exists(Path.Combine("resources","languages", "Server." + language + ".json")))
            {
                Dictionary<string, Dictionary<string, object>> strings =
                    new Dictionary<string, Dictionary<string, object>>();
                strings = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(
                    File.ReadAllText(Path.Combine("resources", "languages", "Server." + language + ".json")));
                Type type = typeof(Strings);

                var fields = type.GetNestedTypes(System.Reflection.BindingFlags.Static |
                                                 System.Reflection.BindingFlags.Public);
                foreach (var p in fields)
                {
                    if (strings.ContainsKey(p.Name))
                    {
                        var dict = strings[p.Name];
                        foreach (var p1 in p.GetFields(System.Reflection.BindingFlags.Static |
                                                       System.Reflection.BindingFlags.Public))
                        {
                            if (dict.ContainsKey(p1.Name))
                            {
                                if (p1.GetValue(null).GetType() == typeof(LocalizedString))
                                {
                                    p1.SetValue(null, new LocalizedString((string)dict[p1.Name]));
                                }
                                else if (p1.GetValue(null).GetType() == typeof(LocalizedString[]))
                                {
                                    string[] values = ((JArray) dict[p1.Name]).ToObject<string[]>();
                                    List<LocalizedString> list = new List<LocalizedString>();
                                    for (int i = 0; i < values.Length; i++)
                                        list.Add(new LocalizedString(values[i]));
                                    p1.SetValue(null, list.ToArray());
                                }
                            }
                        }
                    }
                }
            }
            Save(language);
        }

        public static void Save(string language)
        {
            Dictionary<string, Dictionary<string, object>> strings =
                new Dictionary<string, Dictionary<string, object>>();
            Type type = typeof(Strings);
            var fields = type.GetNestedTypes(System.Reflection.BindingFlags.Static |
                                        System.Reflection.BindingFlags.Public);
            foreach (var p in fields)
            {
                var dict = new Dictionary<string, object>();
                foreach (var p1 in p.GetFields(System.Reflection.BindingFlags.Static |
                                               System.Reflection.BindingFlags.Public))
                {
                    if (p1.GetValue(null).GetType() == typeof(LocalizedString))
                    {
                        dict.Add(p1.Name, ((LocalizedString)p1.GetValue(null)).ToString());
                    }
                    else if (p1.GetValue(null).GetType() == typeof(LocalizedString[]))
                    {
                        string[] values = ((LocalizedString[]) p1.GetValue(null)).Select(x => x.ToString()).ToArray();
                        dict.Add(p1.Name, values);
                    }
                        
                }
                strings.Add(p.Name, dict);
            }
            File.WriteAllText(Path.Combine("resources", "languages", "Server." + language + ".json"), JsonConvert.SerializeObject(strings, Formatting.Indented));
        }
    }

    public struct LocalizedString
    {
        private string _mValue;

        public LocalizedString(string value)
        {
            _mValue = value;
        }

        public static implicit operator LocalizedString(string value)
        {
            return new LocalizedString(value);
        }

        public static implicit operator string(LocalizedString str)
        {
            return str._mValue;
        }

        public override string ToString()
        {
            return _mValue;
        }

        public string ToString(params object[] args)
        {
            try
            {
                if (args.Length == 0) return _mValue;
                return string.Format(_mValue, args);
            }
            catch (FormatException)
            {
                return "Format Exception!";
            }
        }
    }
}

