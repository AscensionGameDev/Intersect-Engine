using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Intersect_Library.Localization
{
    class Language
    {
        private bool _loaded = false;
        private Dictionary<string,Dictionary<string,string>> loadedStrings = new Dictionary<string, Dictionary<string, string>>();
        public Language(string filename)
        {
            if (File.Exists(filename))
            {
                XmlDocument xmlDoc = new XmlDocument(); // Create an XML document object
                xmlDoc.Load(filename); // Load the XML document from the specified file
                XmlNodeList nodes = xmlDoc.SelectNodes("//Strings").Item(0).ChildNodes;
                foreach (XmlNode node in nodes)
                {
                    if (node.NodeType != XmlNodeType.Comment)
                    {
                        if (!loadedStrings.ContainsKey(node.Name.ToLower()))
                        {
                            loadedStrings.Add(node.Name.ToLower(), new Dictionary<string, string>());
                        }
                        foreach (XmlNode childNode in node.ChildNodes)
                        {
                            if (childNode.NodeType != XmlNodeType.Comment)
                            {
                                if (
                                    !loadedStrings[node.Name.ToLower()].ContainsKey(
                                        childNode.Attributes["id"].Value.ToLower()))
                                {
                                    if (childNode.FirstChild == null)
                                    {
                                        loadedStrings[node.Name.ToLower()].Add(childNode.Attributes["id"].Value.ToLower(),"");
                                    }
                                    else
                                    {
                                        loadedStrings[node.Name.ToLower()].Add(childNode.Attributes["id"].Value.ToLower(),
                                        childNode.FirstChild.Value);
                                    }
                                    
                                }
                            }
                        }
                    }
                }
                //Try to load it into dictionaries.
                _loaded = true;
            }
        }

        public bool Loaded()
        {
            return _loaded;
        }

        public bool HasString(string section, string id)
        {
            if (loadedStrings.ContainsKey(section.ToLower()))
            {
                if (loadedStrings[section.ToLower()].ContainsKey(id.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }

        public string GetString(string section, string id, params object[] args)
        {
            try
            {
                return string.Format(loadedStrings[section.ToLower()][id.ToLower()], args);
            }
            catch (FormatException)
            {
                return "Format Exception!";
            }
        }
    }
}
