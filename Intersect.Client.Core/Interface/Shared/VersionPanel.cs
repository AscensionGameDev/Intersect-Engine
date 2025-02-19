using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.Core;

namespace Intersect.Client.Interface.Shared;

public partial class VersionPanel : Panel
{
    private readonly VersionLabel _engineVersionLabel;

    public VersionPanel(Base parent, string name = nameof(VersionPanel)) : base(parent: parent, name: name)
    {
        Alignment = [Alignments.Bottom, Alignments.Right];
        BackgroundColor = new Color(0x7f, 0, 0, 0);
        RestrictToParent = true;
        // TODO: Remove this when showing a game version is added
        IsVisibleInTree = ApplicationContext.CurrentContext.IsDeveloper;

        var font = GameContentManager.Current.GetFont("sourcesansproblack", 10);

        _engineVersionLabel = new VersionLabel(this, name: nameof(_engineVersionLabel))
        {
            Font = font,
            Padding = new Padding(8, 4),
            Text = ApplicationContext.CurrentContext.VersionName,
            IsVisibleInTree = ApplicationContext.CurrentContext.IsDeveloper,
        };
    }

    protected override void Layout(Framework.Gwen.Skin.Base skin)
    {
        base.Layout(skin);

        SizeToChildren();
    }
}