using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Core;
using Intersect.Framework.Core.Security;

namespace Intersect.Client.Interface.Shared;

public partial class VersionPanel : Panel
{
    private readonly VersionLabel _engineVersionLabel;

    public VersionPanel(Base parent, string name = nameof(VersionPanel)) : base(parent: parent, name: name)
    {
        var applicationContext = ApplicationContext.GetCurrentContext<ClientContext>();

        Alignment = [Alignments.Bottom, Alignments.Right];
        BackgroundColor = new Color(0x7f, 0, 0, 0);
        DockChildSpacing = new Padding(0, 4);
        RestrictToParent = true;

        var font = GameContentManager.Current.GetFont("sourcesansproblack");

        _engineVersionLabel = new VersionLabel(this, name: nameof(_engineVersionLabel))
        {
            Font = font,
            FontSize = 10,
            Padding = new Padding(8, 4),
            Text = applicationContext.VersionName,
            IsVisibleInParent = applicationContext.IsDeveloper,
        };

        applicationContext.PermissionsChanged += ApplicationContextOnPermissionsChanged;
    }

    private void ApplicationContextOnPermissionsChanged(PermissionSet permissionSet)
    {
        _engineVersionLabel.IsVisibleInParent = permissionSet.IsGranted(Permission.EngineVersion) || ApplicationContext.CurrentContext.IsDeveloper;
    }

    protected override void Layout(Framework.Gwen.Skin.Base skin)
    {
        base.Layout(skin);

        SizeToChildren();
    }

    protected override void Dispose(bool disposing)
    {
        var applicationContext = ApplicationContext.GetCurrentContext<ClientContext>();
        applicationContext.PermissionsChanged -= ApplicationContextOnPermissionsChanged;

        base.Dispose(disposing);
    }
}