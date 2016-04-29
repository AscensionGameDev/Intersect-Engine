/*
    Intersect Game Engine (Editor)
    Copyright (C) 2015  JC Snider, Joe Bridges
    
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com 

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/
using System;
using System.IO;
using System.Xml;
using Intersect_Editor.Classes.General;
using Intersect_Library;
using Intersect_Library.GameObjects;
using Intersect_Library.GameObjects.Events;

namespace Intersect_Editor.Classes
{
    public static class Database
    {
        public static void InitDatabase()
        {
            Globals.GameItems = new ItemStruct[Options.MaxItems];
            Globals.GameNpcs = new NpcStruct[Options.MaxNpcs];
            Globals.GameSpells = new SpellStruct[Options.MaxSpells];
            Globals.GameAnimations = new AnimationStruct[Options.MaxAnimations];
            Globals.GameResources = new ResourceStruct[Options.MaxResources];
            Globals.GameClasses = new ClassStruct[Options.MaxClasses];
            Globals.GameQuests = new QuestStruct[Options.MaxQuests];
            Globals.GameProjectiles = new ProjectileStruct[Options.MaxProjectiles];
            Globals.CommonEvents = new EventStruct[Options.MaxCommonEvents];
            Globals.GameShops = new ShopStruct[Options.MaxShops];

            Globals.ServerSwitches = new string[Options.MaxServerSwitches];
            Globals.ServerSwitchValues = new bool[Options.MaxServerSwitches];
            Globals.ServerVariables = new string[Options.MaxServerVariables];
            Globals.ServerVariableValues = new int[Options.MaxServerVariables];
            Globals.PlayerSwitches = new string[Options.MaxPlayerSwitches];
            Globals.PlayerVariables = new string[Options.MaxPlayerVariables];
        }

        //Options File
        public static bool LoadOptions()
        {
            if (!Directory.Exists("resources")) Directory.CreateDirectory("resources");
            if (!File.Exists("resources/config.xml"))
            {
                var settings = new XmlWriterSettings { Indent = true };
                using (var writer = XmlWriter.Create("resources/config.xml", settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteComment("Config.xml generated automatically by Intersect Game Engine.");
                    writer.WriteStartElement("Config");
                    writer.WriteElementString("Host", "localhost");
                    writer.WriteElementString("Port", "4500");
                    writer.WriteElementString("RenderCache", "true"); //Not used by the editor, but created here just in case we ever want to share a resource folder with a client.
                    writer.WriteElementString("MenuBGM", ""); //Not used by the editor, but created here just in case we ever want to share a resource folder with a client.
                    writer.WriteElementString("MenuBG", ""); //Not used by the editor, but created here just in case we ever want to share a resource folder with a client.
                    writer.WriteElementString("IntroBG", ""); //Not used by the editor, but created here just in case we ever want to share a resource folder with a client.
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Flush();
                    writer.Close();
                }
            }
            else
            {
                XmlDocument xmlDoc = new XmlDocument();
                try
                {
                    xmlDoc.LoadXml(File.ReadAllText("resources/config.xml"));
                    Globals.ServerHost = xmlDoc.SelectSingleNode("//Config/Host").InnerText;
                    Globals.ServerPort = int.Parse(xmlDoc.SelectSingleNode("//Config/Port").InnerText);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }

    }
}
