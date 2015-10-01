/*
    The MIT License (MIT)

    Copyright (c) 2015 JC Snider, Joe Bridges
  
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Intersect_Client.Classes
{
    public static class Database
    {
        public static MapList MapStructure = new MapList();
        public static List<FolderMap> OrderedMaps = new List<FolderMap>();

        public static void CheckDirectories()
        {
            if (!Directory.Exists("Resources")) { Directory.CreateDirectory("Resources"); }
            if (!Directory.Exists("Resources/Tilesets")) { Directory.CreateDirectory("Resources/Tilesets"); }
            if (!Directory.Exists("Resources/Entities")) { Directory.CreateDirectory("Resources/Entities"); }
        }

        //Options File
        public static bool LoadOptions()
        {

            if (!File.Exists("Resources\\config.xml"))
            {
                SaveOptions();
            }
            else
            {
                var options = new XmlDocument();
                try
                {
                    options.Load("Resources\\config.xml");
                    var selectSingleNode = options.SelectSingleNode("//Config/Port");
                    if (selectSingleNode != null && selectSingleNode.InnerText != "")
                        Globals.ServerPort = Int32.Parse(selectSingleNode.InnerText);
                    selectSingleNode = options.SelectSingleNode("//Config/Host");
                    if (selectSingleNode != null && selectSingleNode.InnerText != "")
                        Globals.ServerHost = selectSingleNode.InnerText;
                    selectSingleNode = options.SelectSingleNode("//Config/DisplayMode");
                    if (selectSingleNode != null && selectSingleNode.InnerText != "")
                        Graphics.DisplayMode = Int32.Parse(selectSingleNode.InnerText);
                    selectSingleNode = options.SelectSingleNode("//Config/FullScreen");
                    if (selectSingleNode != null && selectSingleNode.InnerText != "")
                        selectSingleNode = options.SelectSingleNode("//Config/FPS");
                    if (selectSingleNode != null && selectSingleNode.InnerText != "")
                        Graphics.FPS = Int32.Parse(selectSingleNode.InnerText);
                    selectSingleNode = options.SelectSingleNode("//Config/Sound");
                    if (selectSingleNode != null && selectSingleNode.InnerText != "")
                        Globals.SoundVolume = Int32.Parse(selectSingleNode.InnerText);
                    selectSingleNode = options.SelectSingleNode("//Config/Music");
                    if (selectSingleNode != null && selectSingleNode.InnerText != "")
                        Globals.MusicVolume = Int32.Parse(selectSingleNode.InnerText);
                    selectSingleNode = options.SelectSingleNode("//Config/MenuBGM");
                    if (selectSingleNode != null && selectSingleNode.InnerText != "")
                        Globals.MenuBGM = selectSingleNode.InnerText;
                    selectSingleNode = options.SelectSingleNode("//Config/MenuBG");
                    if (selectSingleNode != null && selectSingleNode.InnerText != "")
                        Globals.MenuBG = selectSingleNode.InnerText;
                    selectSingleNode = options.SelectSingleNode("//Config/IntroBG");
                    if (selectSingleNode != null && selectSingleNode.InnerText != "")
                    {
                        Globals.IntroBG.AddRange(selectSingleNode.InnerText.Split(','));
                        Globals.IntroBGString = selectSingleNode.InnerText;
                    }
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

        public static void SaveOptions()
        {
            var settings = new XmlWriterSettings {Indent = true};
            var writer = XmlWriter.Create("Resources\\config.xml", settings);
                writer.WriteStartDocument();
                writer.WriteComment("Config.xml generated automatically by Intersect Client.");
                writer.WriteStartElement("Config");
                writer.WriteElementString("Host", Globals.ServerHost);
                writer.WriteElementString("Port", Globals.ServerPort.ToString());
                writer.WriteElementString("DisplayMode", Graphics.DisplayMode.ToString());
                writer.WriteElementString("FPS", Graphics.FPS.ToString());
                writer.WriteElementString("FullScreen", Graphics.FullScreen.ToString());
                writer.WriteElementString("Sound", Globals.SoundVolume.ToString());
                writer.WriteElementString("Music", Globals.MusicVolume.ToString());
                writer.WriteElementString("MenuBGM", Globals.MenuBGM);
                writer.WriteElementString("MenuBG", Globals.MenuBG);
                writer.WriteElementString("IntroBG", Globals.IntroBGString);
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
    }
}
