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
using System.IO;
using System.Xml;

namespace Intersect_Client.Classes
{
    public static class Database
    {
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
                var settings = new XmlWriterSettings {Indent = true};
                var writer = XmlWriter.Create("Resources\\config.xml", settings);
                writer.WriteStartDocument();
                writer.WriteComment("Config.xml generated automatically by Intersect Client.");
                writer.WriteStartElement("Config");
                writer.WriteElementString("Host", "localhost");
                writer.WriteElementString("Port", "4500");
                writer.WriteElementString("DisplayMode", "0");
                writer.WriteElementString("FullScreen", "False");
                writer.WriteElementString("Sound", "True");
                writer.WriteElementString("Music", "True");
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
                    options.Load("Resources\\config.xml");
                    var selectSingleNode = options.SelectSingleNode("//Config/Port");
                    if (selectSingleNode != null)
                        Globals.ServerPort = Int32.Parse(selectSingleNode.InnerText);
                    selectSingleNode = options.SelectSingleNode("//Config/Host");
                    if (selectSingleNode != null)
                        Globals.ServerHost = selectSingleNode.InnerText;
                    selectSingleNode = options.SelectSingleNode("//Config/DisplayMode");
                    if (selectSingleNode != null)
                        Graphics.DisplayMode = Int32.Parse(selectSingleNode.InnerText);
                    selectSingleNode = options.SelectSingleNode("//Config/FullScreen");
                    if (selectSingleNode != null)
                        Graphics.FullScreen = Boolean.Parse(selectSingleNode.InnerText);
                    selectSingleNode = options.SelectSingleNode("//Config/Sound");
                    if (selectSingleNode != null)
                        Globals.SoundEnabled = Boolean.Parse(selectSingleNode.InnerText);
                    selectSingleNode = options.SelectSingleNode("//Config/Music");
                    if (selectSingleNode != null)
                        Globals.MusicEnabled = Boolean.Parse(selectSingleNode.InnerText);
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
                writer.WriteElementString("FullScreen", Graphics.FullScreen.ToString());
                writer.WriteElementString("Sound", Globals.SoundEnabled.ToString());
                writer.WriteElementString("Music", Globals.MusicEnabled.ToString());
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();
        }
    }
}
