using System.Numerics;
using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Interface.Data;
using Intersect.Client.Localization;

namespace Intersect.Client.Interface.Shared;

public partial class StatisticsPanel : Panel
{
    private readonly Label _labelFPS;
    private readonly Label _labelPing;

    public StatisticsPanel(Base parent, string name = nameof(StatisticsPanel)) : base(parent: parent, name: name)
    {
        Alignment = [Alignments.Top, Alignments.Right];
        BackgroundColor = new Color(0x7f, 0, 0, 0);
        DockChildSpacing = new Padding(0, 4);
        Padding = new Padding(8, 4);
        RestrictToParent = true;

        var font = GameContentManager.Current.GetFont("sourcesansproblack");

        _labelFPS = new Label(this, name: nameof(_labelFPS))
        {
            AutoSizeToContents = false,
            Dock = Pos.Top,
            Font = font,
            FontSize = 10,
            Padding = new Padding(8, 4),
            Text = Strings.General.FpsLabelFormat.ToString(Graphics.Renderer.FPS),
            TextAlign = Pos.Center,
        };
        var sizeFPS = Graphics.Renderer.MeasureText(
            Strings.General.FpsLabelFormat.ToString(10_000),
            font,
            size: 10,
            fontScale: 1
        );
        _labelFPS.MinimumSize = sizeFPS;

        _labelPing = new Label(this, name: nameof(_labelPing))
        {
            AutoSizeToContents = false,
            Dock = Pos.Top,
            Font = font,
            FontSize = 10,
            Padding = new Padding(8, 4),
            Text = Strings.General.PingLabelFormat.ToString(Networking.Network.Ping),
            TextAlign = Pos.Center,
        };
        var sizePing = Graphics.Renderer.MeasureText(
            Strings.General.PingLabelFormat.ToString(10_000),
            font,
            size: 10,
            fontScale: 1
        );
        _labelPing.MinimumSize = sizePing;

        MinimumSize = new Vector2(
            16 + Math.Max(sizeFPS.X, sizePing.X),
            8 + Math.Max(sizeFPS.Y, sizePing.Y)
        );

        DelegateDataProvider<int> fpsProvider = new(() => Graphics.Renderer.FPS)
        {
            UserData = _labelFPS,
        };
        AddDataProvider(fpsProvider);
        fpsProvider.ValueChanged += OnFPSChanged;

        DelegateDataProvider<int> pingProvider = new(() => Networking.Network.Ping)
        {
            UserData = _labelPing,
        };
        AddDataProvider(pingProvider);
        pingProvider.ValueChanged += OnPingChanged;
    }

    public bool IsFPSEnabled
    {
        get => _labelFPS.IsVisibleInParent;
        set => _labelFPS.IsVisibleInParent = value;
    }

    public bool IsPingEnabled
    {
        get => _labelPing.IsVisibleInParent;
        set => _labelPing.IsVisibleInParent = value;
    }

    protected override void OnChildVisibilityChanged(object? sender, VisibilityChangedEventArgs eventArgs)
    {
        base.OnChildVisibilityChanged(sender, eventArgs);

        IsVisibleInParent = IsFPSEnabled || IsPingEnabled;
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

    private static void OnPingChanged(IDataProvider dataProvider, ValueChangedEventArgs<int> args)
    {
        if (dataProvider is not DataProvider<int> typedProvider)
        {
            throw new InvalidOperationException("Received event from invalid data provider");
        }

        if (typedProvider.UserData is not Label label)
        {
            throw new InvalidOperationException("Data provider's user data is not a label as expected");
        }

        label.Text = Strings.General.PingLabelFormat.ToString(args.Value);
    }

    protected override void Layout(Framework.Gwen.Skin.Base skin)
    {
        base.Layout(skin);

        SizeToChildren();
    }
}