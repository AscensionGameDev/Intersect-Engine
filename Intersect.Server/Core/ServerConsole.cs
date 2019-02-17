using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Config;
using Intersect.Enums;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.General;
using Intersect.Server.Localization;
using Intersect.Server.Networking;
using Intersect.Server.Networking.Helpers;
using Intersect.Threading;

namespace Intersect.Server.Core
{
    internal sealed class ServerConsole : Threaded
    {
        protected override void ThreadStart()
        {
            Console.WriteLine(Strings.Intro.consoleactive);
            Console.Write("> ");
            var command = Console.ReadLine();
            while (ServerContext.Instance.IsRunning)
            {
                var userFound = false;
                var ip = "";
                if (command == null)
                {
                    ServerContext.Instance.Dispose();
                    //ServerStatic.Shutdown();
                    return;
                }

                command = command.Trim();
                var commandsplit = command.Split(' ');

                if (commandsplit[0] == Strings.Commands.announcement) //Announcement Command
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(@"    " + Strings.Commands.announcementusage.ToString(Strings.Commands.commandinfo));
                            Console.WriteLine(@"    " + Strings.Commands.announcementdesc);
                        }
                        else
                        {
                            PacketSender.SendGlobalMsg(command.Remove(0, 12));
                        }
                    }
                    else
                    {
                        Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                    }
                }
                else if (commandsplit[0] == Strings.Commands.netdebug) //Output network debug info
                {
                    NetDebug.GenerateDebugFile();
                }
                else if (commandsplit[0] == Strings.Commands.onlinelist) //Online List Command
                {
                    Console.WriteLine(string.Format("{0,-10}", Strings.Commandoutput.listid) + string.Format("{0,-28}", Strings.Commandoutput.listaccount) + string.Format("{0,-28}", Strings.Commandoutput.listcharacter));
                    Console.WriteLine(new string('-', 66));
                    for (var i = 0; i < Globals.Clients.Count; i++)
                        if (Globals.Clients[i] != null)
                        {
                            var name = Globals.Clients[i].Entity != null ? Globals.Clients[i].Entity.Name : "";
                            Console.WriteLine(string.Format("{0,-10}", "#" + i) + string.Format("{0,-28}", Globals.Clients[i].Name) + string.Format("{0,-28}", name));
                        }
                }
                else if (commandsplit[0] == Strings.Commands.kill) //Kill Command
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(@"    " + Strings.Commands.killusage.ToString(Strings.Commands.commandinfo));
                            Console.WriteLine(@"    " + Strings.Commands.killdesc);
                        }
                        else
                        {
                            for (var i = 0; i < Globals.Clients.Count; i++)
                                if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                                {
                                    var user = Globals.Clients[i].Entity.Name.ToLower();
                                    if (user == commandsplit[1].ToLower())
                                    {
                                        Globals.Clients[i].Entity.Die();
                                        PacketSender.SendGlobalMsg(@"    " + Strings.Player.serverkilled.ToString(Globals.Clients[i].Entity.Name));
                                        Console.WriteLine(@"    " + Strings.Commandoutput.killsuccess.ToString(Globals.Clients[i].Entity.Name));
                                        userFound = true;
                                        break;
                                    }
                                }

                            if (userFound == false) Console.WriteLine(@"    " + Strings.Player.offline);
                        }
                    }
                    else
                    {
                        Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                    }
                }
                else if (commandsplit[0] == Strings.Commands.kick) //Kick Command
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(@"    " + Strings.Commands.kickusage.ToString(Strings.Commands.commandinfo));
                            Console.WriteLine(@"    " + Strings.Commands.kickdesc);
                        }
                        else
                        {
                            for (var i = 0; i < Globals.Clients.Count; i++)
                                if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                                {
                                    var user = Globals.Clients[i].Entity.Name.ToLower();
                                    if (user == commandsplit[1].ToLower())
                                    {
                                        PacketSender.SendGlobalMsg(Strings.Player.serverkicked.ToString(Globals.Clients[i].Entity.Name));
                                        Console.WriteLine(@"    " + Strings.Player.serverkicked.ToString(Globals.Clients[i].Entity.Name));
                                        Globals.Clients[i].Disconnect(); //Kick em'
                                        userFound = true;
                                        break;
                                    }
                                }

                            if (userFound == false) Console.WriteLine(@"    " + Strings.Player.offline);
                        }
                    }
                    else
                    {
                        Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                    }
                }
                else if (commandsplit[0] == Strings.Commands.unban) //Unban Command
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(@"    " + Strings.Commands.unbanusage.ToString(Strings.Commands.commandinfo));
                            Console.WriteLine(@"    " + Strings.Commands.unbandesc);
                        }
                        else
                        {
                            var unbannedUser = LegacyDatabase.GetUser(commandsplit[1]);
                            if (unbannedUser != null)
                            {
                                Ban.DeleteBan(unbannedUser);
                                Console.WriteLine(@"    " + Strings.Account.unbanned.ToString(commandsplit[1]));
                            }
                            else
                            {
                                Console.WriteLine("    " + Strings.Account.notfound.ToString(commandsplit[1]));
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                    }
                }
                else if (commandsplit[0] == Strings.Commands.ban) //Ban Command
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(@"    " + Strings.Commands.banusage.ToString(Strings.Commands.True, Strings.Commands.False, Strings.Commands.commandinfo));
                            Console.WriteLine(@"    " + Strings.Commands.bandesc);
                        }
                        else
                        {
                            if (commandsplit.Length > 3)
                            {
                                for (var i = 0; i < Globals.Clients.Count; i++)
                                    if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                                    {
                                        var user = Globals.Clients[i].Entity.Name.ToLower();
                                        if (user == commandsplit[1].ToLower())
                                        {
                                            var reason = "";
                                            for (var n = 4; n < commandsplit.Length; n++) reason += commandsplit[n] + " ";
                                            if (commandsplit[3] == Strings.Commands.True) ip = Globals.Clients[i].GetIp();
                                            Ban.AddBan(Globals.Clients[i], Convert.ToInt32(commandsplit[2]), reason, Strings.Commands.banuser, ip);
                                            PacketSender.SendGlobalMsg(Strings.Account.banned.ToString(Globals.Clients[i].Entity.Name));
                                            Console.WriteLine(@"    " + Strings.Account.banned.ToString(Globals.Clients[i].Entity.Name));
                                            Globals.Clients[i].Disconnect(); //Kick em'
                                            userFound = true;
                                            break;
                                        }
                                    }

                                if (userFound == false) Console.WriteLine(@"    " + Strings.Player.offline);
                            }
                            else
                            {
                                Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                    }
                }
                else if (commandsplit[0] == Strings.Commands.unmute) //Unmute Command
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(@"    " + Strings.Commands.unmuteusage.ToString(Strings.Commands.commandinfo));
                            Console.WriteLine(@"    " + Strings.Commands.unmutedesc);
                        }
                        else
                        {
                            var unmutedUser = LegacyDatabase.GetUser(commandsplit[1]);
                            if (unmutedUser != null)
                            {
                                Mute.DeleteMute(unmutedUser);
                                Console.WriteLine(@"    " + Strings.Account.unmuted.ToString(unmutedUser.Name));
                            }
                            else
                            {
                                Console.WriteLine(@"    " + Strings.Account.notfound);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                    }
                }
                else if (commandsplit[0] == Strings.Commands.mute) //Mute Command
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(@"    " + Strings.Commands.muteusage.ToString(Strings.Commands.True, Strings.Commands.False, Strings.Commands.commandinfo));
                            Console.WriteLine(@"    " + Strings.Commands.mutedesc);
                        }
                        else
                        {
                            if (commandsplit.Length > 3)
                            {
                                for (var i = 0; i < Globals.Clients.Count; i++)
                                    if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                                    {
                                        var user = Globals.Clients[i].Entity.Name.ToLower();
                                        if (user == commandsplit[1].ToLower())
                                        {
                                            var reason = "";
                                            for (var n = 4; n < commandsplit.Length; n++) reason += commandsplit[n] + " ";
                                            if (commandsplit[3] == Strings.Commands.True) ip = Globals.Clients[i].GetIp();
                                            Mute.AddMute(Globals.Clients[i], Convert.ToInt32(commandsplit[2]), reason, Strings.Commands.muteuser, ip);
                                            PacketSender.SendGlobalMsg(Strings.Account.muted.ToString(Globals.Clients[i].Entity.Name));
                                            Console.WriteLine(@"    " + Strings.Account.muted.ToString(Globals.Clients[i].Entity.Name));
                                            userFound = true;
                                            break;
                                        }
                                    }

                                if (userFound == false) Console.WriteLine(@"    " + Strings.Player.offline);
                            }
                            else
                            {
                                Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                    }
                }
                else if (commandsplit[0] == Strings.Commands.power) //Power Command
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(@"    " + Strings.Commands.powerusage.ToString(Strings.Commands.commandinfo));
                            Console.WriteLine(@"    " + Strings.Commands.powerdesc);
                        }
                        else
                        {
                            if (commandsplit.Length > 2)
                            {
                                for (var i = 0; i < Globals.Clients.Count; i++)
                                    if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                                    {
                                        var user = Globals.Clients[i].Entity.Name.ToLower();
                                        if (user == commandsplit[1].ToLower())
                                        {
                                            var power = UserRights.None;
                                            switch ((Access)int.Parse(commandsplit[2]))
                                            {
                                                case Access.Moderator:
                                                    power = UserRights.Moderation;
                                                    break;
                                                case Access.Admin:
                                                    power = UserRights.Admin;
                                                    break;
                                            }
                                            LegacyDatabase.SetPlayerPower(Globals.Clients[i].Name, power);
                                            PacketSender.SendEntityDataToProximity(Globals.Clients[i].Entity);
                                            if (power != UserRights.None)
                                                PacketSender.SendGlobalMsg(Strings.Player.admin.ToString(Globals.Clients[i].Entity.Name));
                                            else
                                                PacketSender.SendGlobalMsg(Strings.Player.deadmin.ToString(Globals.Clients[i].Entity.Name));
                                            Console.WriteLine(@"    " + Strings.Commandoutput.powerchanged.ToString(Globals.Clients[i].Entity.Name));

                                            userFound = true;
                                            break;
                                        }
                                    }

                                if (userFound == false) Console.WriteLine(@"    " + Strings.Player.offline);
                            }
                            else
                            {
                                Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                    }
                }
                else if (commandsplit[0] == Strings.Commands.makeprivate)
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(@"    " + Strings.Commands.api.ToString(Strings.Commands.commandinfo));
                            Console.WriteLine(@"    " + Strings.Commands.makeprivatedesc);
                        }
                        else
                        {
                            Console.WriteLine(@"    " + Strings.Commands.makeprivateusage);
                        }
                    }
                    else
                    {
                        Console.WriteLine(@"    " + Strings.Commands.madeprivate);
                        Options.AdminOnly = true;
                        Options.SaveToDisk();
                    }
                }
                else if (commandsplit[0] == Strings.Commands.makepublic)
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(@"    " + Strings.Commands.makepublicdesc);
                        }
                        else
                        {
                            Console.WriteLine(@"    " + Strings.Commands.makepublicusage);
                        }
                    }
                    else
                    {
                        Console.WriteLine(@"    " + Strings.Commands.madepublic);
                        Options.AdminOnly = false;
                        Options.SaveToDisk();
                    }
                }
                else if (commandsplit[0] == Strings.Commands.api) //API Command
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(@"    " + Strings.Commands.apidesc);
                        }
                        else
                        {
                            if (commandsplit.Length > 2)
                            {
                                if (commandsplit.Length > 2)
                                    try
                                    {
                                        if (LegacyDatabase.AccountExists(commandsplit[1]))
                                        {
                                            var access = Convert.ToBoolean(int.Parse(commandsplit[2]));
                                            var account = LegacyDatabase.GetUser(commandsplit[1]);
                                            account.Power.Api = access;
                                            if (access)
                                            {
                                                Console.WriteLine(@"    " + Strings.Commandoutput.apigranted.ToString(commandsplit[1]));
                                            }
                                            else
                                            {
                                                Console.WriteLine(@"    " + Strings.Commandoutput.apirevoked.ToString(commandsplit[1]));
                                            }
                                            LegacyDatabase.SavePlayerDatabaseAsync();
                                        }
                                        else
                                        {
                                            Console.WriteLine(@"    " + Strings.Account.notfound.ToString(commandsplit[1]));
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        Console.WriteLine(@"    " + Strings.Commandoutput.parseerror.ToString(commandsplit[0], Strings.Commands.commandinfo));
                                    }
                                else
                                    Console.WriteLine(@"    " + Strings.Commandoutput.syntaxerror.ToString(commandsplit[0], Strings.Commands.commandinfo));
                            }
                            else
                            {
                                Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                    }
                }
                else if (commandsplit[0] == Strings.Commands.poweracc) //Power Account Command
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(@"    " + Strings.Commands.poweraccusage.ToString(Strings.Commands.commandinfo));
                            Console.WriteLine(@"    " + Strings.Commands.poweraccdesc);
                        }
                        else
                        {
                            if (commandsplit.Length > 2)
                            {
                                if (commandsplit.Length > 2)
                                    try
                                    {
                                        if (LegacyDatabase.AccountExists(commandsplit[1]))
                                        {
                                            var power = UserRights.None;
                                            switch ((Access)int.Parse(commandsplit[2]))
                                            {
                                                case Access.Moderator:
                                                    power = UserRights.Moderation;
                                                    break;
                                                case Access.Admin:
                                                    power = UserRights.Admin;
                                                    break;
                                            }
                                            LegacyDatabase.SetPlayerPower(commandsplit[1], power);
                                            Console.WriteLine(@"    " + Strings.Commandoutput.powerchanged.ToString(commandsplit[1]));
                                        }
                                        else
                                        {
                                            Console.WriteLine(@"    " + Strings.Account.notfound.ToString(commandsplit[1]));
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        Console.WriteLine(@"    " + Strings.Commandoutput.parseerror.ToString(commandsplit[0], Strings.Commands.commandinfo));
                                    }
                                else
                                    Console.WriteLine(@"    " + Strings.Commandoutput.syntaxerror.ToString(commandsplit[0], Strings.Commands.commandinfo));
                            }
                            else
                            {
                                Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                    }
                }
                else if (commandsplit[0] == Strings.Commands.cps) //CPS Command
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(@"    " + Strings.Commands.cpsusage.ToString(Strings.Commands.commandinfo));
                            Console.WriteLine(@"    " + Strings.Commands.cpsdesc);
                        }
                        else if (commandsplit[1] == Strings.Commands.cpslock)
                        {
                            Globals.CpsLock = true;
                        }
                        else if (commandsplit[1] == Strings.Commands.cpsunlock)
                        {
                            Globals.CpsLock = false;
                        }
                        else if (commandsplit[1] == Strings.Commands.cpsstatus)
                        {
                            if (Globals.CpsLock)
                                Console.WriteLine(Strings.Commandoutput.cpslocked);
                            else
                                Console.WriteLine(Strings.Commandoutput.cpsunlocked);
                        }
                        else
                        {
                            Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                        }
                    }
                    else
                    {
                        Console.WriteLine(Strings.Commandoutput.cps.ToString(Globals.Cps));
                    }
                }
                else if (commandsplit[0] == Strings.Commands.exit) //Exit Command
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(@"    " + Strings.Commands.exitusage.ToString(Strings.Commands.commandinfo));
                            Console.WriteLine(@"    " + Strings.Commands.exitdesc);
                        }
                        else
                        {
                            Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                        }
                    }
                    else
                    {
                        ServerContext.Instance.Dispose();
                        //ServerStatic.Shutdown();
                        return;
                    }
                }
                else if (commandsplit[0] == Strings.Commands.migrate) //Migrate Command
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(@"    " + Strings.Commands.migrateusage.ToString(Strings.Commands.commandinfo));
                            Console.WriteLine(@"    " + Strings.Commands.migratedesc);
                        }
                        else
                        {
                            Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                        }
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine(Strings.Migration.selectdb);
                        Console.WriteLine();
                        Console.WriteLine(Strings.Migration.selectgamedb.ToString(Options.GameDb.Type == DatabaseOptions.DatabaseType.sqlite ? Strings.Migration.currentlysqlite : Strings.Migration.currentlymysql));
                        Console.WriteLine(Strings.Migration.selectplayerdb.ToString(Options.PlayerDb.Type == DatabaseOptions.DatabaseType.sqlite ? Strings.Migration.currentlysqlite : Strings.Migration.currentlymysql));
                        Console.WriteLine();
                        Console.WriteLine(Strings.Migration.cancel);
                        Console.Write("> ");
                        var selection = Console.ReadKey().KeyChar;
                        Console.WriteLine();
                        DatabaseOptions db = null;
                        if (selection.ToString() == Strings.Migration.selectgamedbkey.ToString())
                        {
                            db = Options.GameDb;
                        }
                        else if (selection.ToString() == Strings.Migration.selectplayerdbkey.ToString())
                        {
                            db = Options.PlayerDb;
                        }
                        if (db != null)
                        {
                            var dbString = db == Options.GameDb ? Strings.Migration.gamedb : Strings.Migration.playerdb;
                            Console.WriteLine();
                            Console.WriteLine(Strings.Migration.selectdbengine.ToString(dbString));
                            Console.WriteLine(Strings.Migration.migratetosqlite);
                            Console.WriteLine(Strings.Migration.migratetomysql);
                            Console.WriteLine();
                            Console.WriteLine(Strings.Migration.cancel);
                            Console.Write("> ");
                            selection = Console.ReadKey().KeyChar;
                            Console.WriteLine();
                            DatabaseOptions.DatabaseType dbengine = DatabaseOptions.DatabaseType.sqlite;
                            if (selection.ToString() == Strings.Migration.selectsqlitekey.ToString() || selection.ToString() == Strings.Migration.selectmysqlkey.ToString())
                            {
                                if (selection.ToString() == Strings.Migration.selectmysqlkey.ToString()) dbengine = DatabaseOptions.DatabaseType.mysql;
                                if (db.Type == dbengine)
                                {
                                    var engineString = dbengine == DatabaseOptions.DatabaseType.sqlite ? Strings.Migration.sqlite : Strings.Migration.mysql;
                                    Console.WriteLine();
                                    Console.WriteLine(Strings.Migration.alreadyusingengine.ToString(dbString, engineString));
                                    Console.WriteLine();
                                }
                                else
                                {
                                    LegacyDatabase.Migrate(db, dbengine);
                                }
                            }
                        }


                    }
                }
                else if (commandsplit[0] == Strings.Commands.help) //Help Command
                {
                    if (commandsplit.Length > 1)
                    {
                        if (commandsplit[1] == Strings.Commands.commandinfo)
                        {
                            Console.WriteLine(@"    " + Strings.Commands.helpusage.ToString(Strings.Commands.commandinfo));
                            Console.WriteLine(@"    " + Strings.Commands.helpdesc);
                        }
                        else
                        {
                            Console.WriteLine(Strings.Commandoutput.invalidparameters.ToString(Strings.Commands.commandinfo));
                        }
                    }
                    else
                    {
                        Console.WriteLine(@"    " + Strings.Commandoutput.helpheader);
                        Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.help) + " - " + Strings.Commands.helphelp);
                        Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.exit) + " - " + Strings.Commands.exithelp);
                        Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.api) + " - " + Strings.Commands.apihelp);
                        Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.announcement) + " - " + Strings.Commands.announcementhelp);
                        Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.cps) + " - " + Strings.Commands.cpshelp);
                        Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.power) + " - " + Strings.Commands.powerhelp);
                        Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.poweracc) + " - " + Strings.Commands.poweracchelp);
                        Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.onlinelist) + " - " + Strings.Commands.onlinelisthelp);
                        Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.kick) + " - " + Strings.Commands.kickhelp);
                        Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.ban) + " - " + Strings.Commands.banhelp);
                        Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.unban) + " - " + Strings.Commands.unbanhelp);
                        Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.mute) + " - " + Strings.Commands.mutehelp);
                        Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.unmute) + " - " + Strings.Commands.unmutehelp);
                        Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.kill) + " - " + Strings.Commands.killhelp);
                        Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.makeprivate) + " - " + Strings.Commands.makeprivatehelp);
                        Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.makepublic) + " - " + Strings.Commands.makepublichelp);
                        Console.WriteLine(@"    " + string.Format("{0,-20}", Strings.Commands.migrate) + " - " + Strings.Commands.migratehelp);
                        Console.WriteLine(@"    " + Strings.Commandoutput.helpfooter.ToString(Strings.Commands.commandinfo));
                    }
                }
                else
                {
                    Console.WriteLine(@"    " + Strings.Commandoutput.notfound);
                }

                Console.Write("> ");
                command = Console.ReadLine();
            }
        }
    }
}
