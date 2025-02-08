using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Core;

namespace Intersect.Client.Interface.Menu;

public partial class MenuGuiBase : IMutableInterface
{
    private readonly Canvas _menuCanvas;
    private readonly ImagePanel _serverStatusArea;
    private readonly Label _serverStatusLabel;
    private readonly Panel _versionPanel;
    private readonly Label _versionLabel;

    public MainMenu MainMenu { get; }

    private bool _shouldReset;

    public MenuGuiBase(Canvas myCanvas)
    {
        _menuCanvas = myCanvas;

        MainMenu = new MainMenu(_menuCanvas);

        _versionPanel = new Panel(_menuCanvas, name: nameof(_versionPanel))
        {
            Alignment = [Alignments.Bottom, Alignments.Right],
            BackgroundColor = new Color(0x7f, 0, 0, 0),
            Padding = new Padding(8, 4),
            RestrictToParent = true,
            IsVisible = ApplicationContext.CurrentContext.IsDeveloper, // TODO: Remove this when showing a game version is added
        };

        _versionLabel = new Label(_versionPanel, name: nameof(_versionLabel))
        {
            Alignment = [Alignments.Center],
            AutoSizeToContents = true,
            Font = GameContentManager.Current.GetFont("sourcesansproblack", 10),
            Text = ApplicationContext.CurrentContext.VersionName,
            TextPadding = new Padding(2, 0),
            IsVisible = ApplicationContext.CurrentContext.IsDeveloper,
        };

        _versionLabel.SizeToContents();

        _versionPanel.SizeToChildren();

        _serverStatusArea = new ImagePanel(_menuCanvas, "ServerStatusArea")
        {
            IsHidden = ClientContext.IsSinglePlayer,
        };
        _serverStatusLabel = new Label(_serverStatusArea, "ServerStatusLabel")
        {
            IsHidden = ClientContext.IsSinglePlayer,
            Text = Strings.Server.StatusLabel.ToString(MainMenu.ActiveNetworkStatus.ToLocalizedString()),
        };

        _serverStatusArea.LoadJsonUi(GameContentManager.UI.Menu, Graphics.Renderer?.GetResolutionString());
        MainMenu.NetworkStatusChanged += HandleNetworkStatusChanged;
    }

    ~MenuGuiBase()
    {
        // ReSharper disable once DelegateSubtraction
        MainMenu.NetworkStatusChanged -= HandleNetworkStatusChanged;
    }

    private void HandleNetworkStatusChanged()
    {
        _serverStatusLabel.Text = Strings.Server.StatusLabel.ToString(MainMenu.ActiveNetworkStatus.ToLocalizedString());
    }

    public void Update()
    {
        if (_shouldReset)
        {
            MainMenu.Reset();
            _shouldReset = false;
        }

        MainMenu.Update();
    }

    public void Draw()
    {
        _menuCanvas.RenderCanvas();
    }

    public void Reset()
    {
        _shouldReset = true;
    }

    //Dispose
    public void Dispose()
    {
        _menuCanvas.Dispose();
    }

    /// <inheritdoc />
    public List<Base> Children => MainMenu.Children;

    /// <inheritdoc />
    public TElement Create<TElement>(params object[] parameters) where TElement : Base =>
        MainMenu.Create<TElement>(parameters);

    /// <inheritdoc />
    public TElement? Find<TElement>(string? name = null, bool recurse = false) where TElement : Base =>
        MainMenu.Find<TElement>(name, recurse);

    /// <inheritdoc />
    public IEnumerable<TElement?> FindAll<TElement>(bool recurse = false) where TElement : Base =>
        MainMenu.FindAll<TElement>(recurse);

    /// <inheritdoc />
    public void Remove<TElement>(TElement element, bool dispose = false) where TElement : Base =>
        MainMenu.Remove(element, dispose);
}
