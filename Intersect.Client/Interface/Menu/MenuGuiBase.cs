using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Localization;
using Intersect.Client.Networking;

namespace Intersect.Client.Interface.Menu;

public partial class MenuGuiBase : IMutableInterface
{
    private readonly Canvas _menuCanvas;
    private readonly ImagePanel _serverStatusArea;
    private readonly Label _serverStatusLabel;

    public MainMenu MainMenu { get; }

    private bool _shouldReset;

    public MenuGuiBase(Canvas myCanvas)
    {
        _menuCanvas = myCanvas;

        MainMenu = new MainMenu(_menuCanvas);
        _serverStatusArea = new ImagePanel(_menuCanvas, "ServerStatusArea");
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

        _serverStatusArea.IsHidden = ClientContext.IsSinglePlayer;
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
