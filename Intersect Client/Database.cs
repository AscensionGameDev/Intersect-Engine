using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;

namespace Intersect_Client
{
    public static class Database
    {
        public static void CheckDirectories()
        {
            if (!Directory.Exists("data")) { Directory.CreateDirectory("data"); }
            if (!Directory.Exists("data/graphics")) { Directory.CreateDirectory("data/graphics"); }
            if (!Directory.Exists("data/graphics/tilesets")) { Directory.CreateDirectory("data/graphics/tilesets"); }
            if (!Directory.Exists("data/graphics/entities")) { Directory.CreateDirectory("data/graphics/entities"); }
        }

        //Options File
        public static bool LoadOptions()
        {

            if (!File.Exists("data\\config.xml"))
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                XmlWriter writer = XmlWriter.Create("data\\config.xml", settings);
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
                XmlDocument options = new XmlDocument();
                try
                {
                    options.Load("data\\config.xml");
                    Globals.ServerPort = Int32.Parse(options.SelectSingleNode("//Config/Port").InnerText);
                    Globals.ServerHost = options.SelectSingleNode("//Config/Host").InnerText;
                    Graphics.DisplayMode = Int32.Parse(options.SelectSingleNode("//Config/DisplayMode").InnerText);
                    Graphics.FullScreen = Boolean.Parse(options.SelectSingleNode("//Config/FullScreen").InnerText);
                    Globals.SoundEnabled = Boolean.Parse(options.SelectSingleNode("//Config/Sound").InnerText);
                    Globals.MusicEnabled = Boolean.Parse(options.SelectSingleNode("//Config/Music").InnerText);
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
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                XmlWriter writer = XmlWriter.Create("data\\config.xml", settings);
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
