using System.Collections.Generic;
using System.IO;
using System.Xml;
using IntersectClientExtras.Gwen;

namespace Intersect.Client.Classes.UI.Menu
{
    class CreditsParser
    {
        public List<CreditsLine> Credits = new List<CreditsLine>();

        public CreditsParser()
        {
            LoadCredits();
        }

        private void LoadCredits()
        {
            var xml = "";
            Credits.Clear();
            if (File.Exists(Path.Combine("resources", "credits.xml")))
            {
                xml = File.ReadAllText(Path.Combine("resources", "credits.xml"));
            }
            else
            {
                //TODO Load XML from Resources
            }
            if (xml.Trim().Length <= 0) return;
            var readerSettings = new XmlReaderSettings
            {
                IgnoreWhitespace = true,
                IgnoreComments = true
            };
            using (var reader = XmlReader.Create(new StringReader(xml), readerSettings))
            {
                while (reader.Read())
                {
                    if (reader.Name != "Line" || !reader.IsStartElement()) continue;
                    CreditsLine cl;
                    var alignment = reader.GetAttribute("alignment");
                    switch (alignment)
                    {
                        case "center":
                            cl.alignment = Alignments.CenterH;
                            break;
                        case "right":
                            cl.alignment = Alignments.Right;
                            break;
                        default:
                            cl.alignment = Alignments.Left;
                            break;
                    }
                    cl.font = reader.GetAttribute("font");
                    cl.size = int.Parse(reader.GetAttribute("size"));
                    var colors = reader.GetAttribute("color").Split(',');
                    cl.clr = new Color(int.Parse(colors[0]), int.Parse(colors[1]), int.Parse(colors[2]),
                        int.Parse(colors[3]));
                    reader.Read();
                    cl.text = reader.Value;
                    Credits.Add(cl);
                }
            }
        }

        public struct CreditsLine
        {
            public string text;
            public string font;
            public int size;
            public Alignments alignment;
            public Color clr;
        }
    }
}