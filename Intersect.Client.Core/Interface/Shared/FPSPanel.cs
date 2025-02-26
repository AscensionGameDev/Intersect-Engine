using System.Numerics;
using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.Control.Layout;
using Intersect.Client.General;
using Intersect.Client.Interface.Data;
using Intersect.Client.Localization;
using Intersect.Core;

namespace Intersect.Client.Interface.Shared;

public partial class FPSPanel : Panel
{
    private readonly Label _label;

    public FPSPanel(Base parent, string name = nameof(FPSPanel)) : base(parent: parent, name: name)
    {
        Alignment = [Alignments.Top, Alignments.Right];
        BackgroundColor = new Color(0x7f, 0, 0, 0);
        IsVisibleInParent = Globals.Database?.ShowFPSCounter ?? false;
        RestrictToParent = true;

        var font = GameContentManager.Current.GetFont("sourcesansproblack");

        _label = new Label(this, name: nameof(_label))
        {
            AutoSizeToContents = false,
            Dock = Pos.Fill,
            Font = font,
            FontSize = 10,
            Padding = new Padding(8, 4),
            Text = ApplicationContext.CurrentContext.VersionName,
            TextAlign = Pos.Center,
        };

        MinimumSize = Graphics.Renderer.MeasureText(
                          Strings.General.FpsLabelFormat.ToString(10_000),
                          font,
                          size: 10,
                          fontScale: 1
                      ) +
                      new Vector2(16, 8);

        DelegateDataProvider<int> fpsProvider = new(() => Graphics.Renderer.FPS)
        {
            UserData = _label,
        };
        AddDataProvider(fpsProvider);
        fpsProvider.ValueChanged += OnFPSChanged;
    }

    private static void OnFPSChanged(IDataProvider dataProvider, ValueChangedEventArgs<int> args)
    {
        if (dataProvider is not DataProvider<int> typedProvider)
        {
            throw new InvalidOperationException("Received event from invalid data provider");
        }

        if (typedProvider.UserData is not Label label)
        {
            throw new InvalidOperationException("Data provider's user data is not a label as expected");
        }

        label.Text = Strings.General.FpsLabelFormat.ToString(args.Value);
    }

    protected override void Layout(Framework.Gwen.Skin.Base skin)
    {
        base.Layout(skin);

        SizeToChildren();
    }
}