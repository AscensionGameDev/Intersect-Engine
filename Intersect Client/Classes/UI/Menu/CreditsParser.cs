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
                            cl.Alignment = Alignments.CenterH;
                            break;
                        case "right":
                            cl.Alignment = Alignments.Right;
                            break;
                        default:
                            cl.Alignment = Alignments.Left;
                            break;
                    }
                    cl.Font = reader.GetAttribute("font");
                    cl.Size = int.Parse(reader.GetAttribute("size"));
                    var colors = reader.GetAttribute("color").Split(',');
                    cl.Clr = new Color(int.Parse(colors[0]), int.Parse(colors[1]), int.Parse(colors[2]),
                        int.Parse(colors[3]));
                    reader.Read();
                    cl.Text = reader.Value;
                    Credits.Add(cl);
                }
            }
        }

        public struct CreditsLine
        {
            public string Text;
            public string Font;
            public int Size;
            public Alignments Alignment;
            public Color Clr;
        }
    }
}