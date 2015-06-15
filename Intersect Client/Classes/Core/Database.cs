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
