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
using System.Collections.Generic;
using System.IO;
using System.Xml;
namespace Intersect_Editor.Classes
{
    public static class Database
    {
        public static MapList MapStructure = new MapList();
        public static List<FolderMap> OrderedMaps = new List<FolderMap>();
        public static void InitDatabase()
        {
            Globals.GameItems = new ItemStruct[Constants.MaxItems];
            Globals.GameNpcs = new NpcStruct[Constants.MaxNpcs];
            Globals.GameSpells = new SpellStruct[Constants.MaxSpells];
            Globals.GameAnimations = new AnimationStruct[Constants.MaxAnimations];
            Globals.GameResources = new ResourceStruct[Constants.MaxResources];
            Globals.GameClasses = new ClassStruct[Constants.MaxClasses];
            Globals.GameQuests = new QuestStruct[Constants.MaxQuests];
            LoadOptions();
        }

        //Options File
        public static bool LoadOptions()
        {

            if (!File.Exists("resources\\config.xml"))
            {
                var settings = new XmlWriterSettings { Indent = true };
                var writer = XmlWriter.Create("resources\\config.xml", settings);
                writer.WriteStartDocument();
                writer.WriteComment("Config.xml generated automatically by Intersect Editor.");
                writer.WriteStartElement("Config");
                writer.WriteElementString("Host", "localhost");
                writer.WriteElementString("Port", "4500");
                writer.WriteElementString("TileWidth", Globals.TileWidth.ToString());
                writer.WriteElementString("TileHeight", Globals.TileHeight.ToString());
                writer.WriteComment("Do NOT touch these values will resize the maps in the engine. If you have existing maps and change these values you MUST delete them or else the engine will crash on launch.");
                writer.WriteElementString("MapWidth", Globals.MapWidth.ToString());
                writer.WriteElementString("MapHeight", Globals.MapHeight.ToString());
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();
            }
            else
            {
                var options = new XmlDocument();
                try
                {
                    options.Load("resources\\config.xml");
                    var selectSingleNode = options.SelectSingleNode("//Config/Port");
                    if (selectSingleNode != null && selectSingleNode.InnerText != "")
                        Globals.ServerPort = Int32.Parse(selectSingleNode.InnerText);
                    selectSingleNode = options.SelectSingleNode("//Config/Host");
                    if (selectSingleNode != null && selectSingleNode.InnerText != "")
                        Globals.ServerHost = selectSingleNode.InnerText;
                    selectSingleNode = options.SelectSingleNode("//Config/TileWidth");
                    if (selectSingleNode != null && selectSingleNode.InnerText != "")
                        Globals.TileWidth = Int32.Parse(selectSingleNode.InnerText);
                    selectSingleNode = options.SelectSingleNode("//Config/TileHeight");
                    if (selectSingleNode != null && selectSingleNode.InnerText != "")
                        Globals.TileHeight = Int32.Parse(selectSingleNode.InnerText);
                    selectSingleNode = options.SelectSingleNode("//Config/MapWidth");
                    if (selectSingleNode != null && selectSingleNode.InnerText != "")
                        Globals.MapWidth = Int32.Parse(selectSingleNode.InnerText);
                    selectSingleNode = options.SelectSingleNode("//Config/MapHeight");
                    if (selectSingleNode != null && selectSingleNode.InnerText != "")
                        Globals.MapHeight = Int32.Parse(selectSingleNode.InnerText);
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
