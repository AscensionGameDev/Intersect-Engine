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

using System.IO;
using System.Security.Cryptography;
using IntersectClientExtras.Database;
using Intersect_Client.Classes.General;
using Microsoft.Win32;

namespace Intersect_Client.Classes.Bridges_and_Interfaces.SFML.Database
{
    public class SfmlDatabase : GameDatabase
    {
        public override void SavePreference(string key, string value)
        {
            var sha = new SHA256Managed();
            RegistryKey regkey = Registry.CurrentUser.OpenSubKey("Software", true);

            regkey.CreateSubKey("IntersectEngine");
            regkey = regkey.OpenSubKey("IntersectEngine", true);
            regkey.CreateSubKey(ServerHost + ServerPort);
            regkey = regkey.OpenSubKey(ServerHost + ServerPort, true);
            regkey.SetValue(key, value);
        }

        public override string LoadPreference(string key)
        {
            RegistryKey regkey = Registry.CurrentUser.OpenSubKey("Software", false);
            regkey = regkey.OpenSubKey("IntersectEngine", false);
            if (regkey == null) { return ""; }
            regkey = regkey.OpenSubKey(ServerHost + ServerPort);
            if (regkey == null) { return ""; }
            string value = (string)regkey.GetValue(key);
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }
            return value;
        }

        public override bool LoadConfig()
        {
            if (!File.Exists("Resources/config.xml"))
            {
                File.WriteAllText("Resources/config.xml",base.GetDefaultConfig());
                return LoadConfig();
            }
            else
            {
                string xmldata = File.ReadAllText("Resources/config.xml");
                return base.LoadConfigFromXml(xmldata);
            }
        }
    }
}
