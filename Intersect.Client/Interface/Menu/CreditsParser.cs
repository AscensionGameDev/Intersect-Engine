using Intersect.Client.Framework.Gwen;

namespace Intersect.Client.Interface.Menu;

partial class Credits
{
    public List<CreditsLine> Lines = [];

    public partial struct CreditsLine
    {
        public string Text;
        public string Font;
        public int Size;
        public string Alignment;
        public Color TextColor;

        public Alignments GetAlignment()
        {
            return Alignment switch
            {
                "center" => Alignments.CenterH,
                "right" => Alignments.Right,
                _ => Alignments.Left,
            };
        }
    }
}
