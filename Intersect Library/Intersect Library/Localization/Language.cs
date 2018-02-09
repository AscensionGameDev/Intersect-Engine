using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Intersect.Localization
{
    class Language
    {
        private Dictionary<string, Dictionary<string, string>> mLoadedStrings =
            new Dictionary<string, Dictionary<string, string>>();

        public Language(string data, bool isRaw)
        {
            if (isRaw)
            {
                LoadLanguage(data);
                return;
            }
            if (File.Exists(data))
            {
                LoadLanguage(File.ReadAllText(data));
            }
        }

        public bool IsLoaded { get; private set; }

        private void LoadLanguage(string xmlData)
        {
            XmlDocument xmlDoc = new XmlDocument(); // Create an XML document object
            xmlDoc.LoadXml(xmlData); // Load the XML document from the specified file
            XmlNodeList nodes = xmlDoc.SelectNodes("//Strings").Item(0).ChildNodes;
            foreach (XmlNode node in nodes)
            {
                if (node.NodeType != XmlNodeType.Comment)
                {
                    if (!mLoadedStrings.ContainsKey(node.Name.ToLower()))
                    {
                        mLoadedStrings.Add(node.Name.ToLower(), new Dictionary<string, string>());
                    }
                    foreach (XmlNode childNode in node.ChildNodes)
                    {
                        if (childNode.NodeType != XmlNodeType.Comment)
                        {
                            if (
                                !mLoadedStrings[node.Name.ToLower()].ContainsKey(
                                    childNode.Attributes["id"].Value.ToLower()))
                            {
                                if (childNode.FirstChild == null)
                                {
                                    mLoadedStrings[node.Name.ToLower()].Add(
                                        childNode.Attributes["id"].Value.ToLower(), "");
                                }
                                else
                                {
                                    mLoadedStrings[node.Name.ToLower()].Add(
                                        childNode.Attributes["id"].Value.ToLower(),
                                        childNode.FirstChild.Value);
                                }
                            }
                        }
                    }
                }
            }
            //Try to load it into dictionaries.
            IsLoaded = true;
        }

        public bool HasString(string section, string id)
        {
            return mLoadedStrings.ContainsKey(section.ToLower()) &&
                   mLoadedStrings[section.ToLower()].ContainsKey(id.ToLower());
        }

        public string GetString(string section, string id, params object[] args)
        {
            try
            {
                return string.Format(mLoadedStrings[section.ToLower()][id.ToLower()], args);
            }
            catch (FormatException)
            {
                return "Format Exception!";
            }
        }

        public string GetString(string section, string id)
        {
            return mLoadedStrings[section.ToLower()][id.ToLower()];
        }
    }
}