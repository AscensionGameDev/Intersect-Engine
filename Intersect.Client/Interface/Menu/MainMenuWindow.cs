using System.Security.Cryptography;
using System.Text;
using Intersect.Client.Core;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Network.Events;

namespace Intersect.Client.Interface.Menu;

public partial class MainMenuWindow : Window
{
    private readonly Button _buttonCredits;
    private readonly Button _buttonExit;
    private readonly Button _buttonLogin;
    private readonly Button _buttonRegister;
    private readonly Button _buttonSettings;
    private readonly Button _buttonStart;
    private readonly MainMenu _mainMenu;

    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    public MainMenuWindow(Canvas canvas, MainMenu mainMenu) : base(canvas, Strings.MainMenu.Title, false, $"{nameof(MainMenuWindow)}_{(ClientContext.IsSinglePlayer ? "singleplayer" : "online")}")
    {
        _mainMenu = mainMenu;

        _buttonCredits = new Button(this, nameof(_buttonCredits))
        {
            IsTabable = true,
            Text = Strings.MainMenu.Credits,
        };
        _buttonCredits.Clicked += ButtonCreditsOnClicked;

        _buttonExit = new Button(this, nameof(_buttonExit))
        {
            IsTabable = true,
            Text = Strings.MainMenu.Exit,
        };
        _buttonExit.Clicked += ButtonExitOnClicked;

        _buttonLogin = new Button(this, nameof(_buttonLogin))
        {
            IsDisabled = MainMenu.ActiveNetworkStatus != NetworkStatus.Online,
            IsHidden = ClientContext.IsSinglePlayer,
            IsTabable = true,
            Text = Strings.MainMenu.Login,
        };
        _buttonLogin.Clicked += ButtonLoginOnClicked;

        _buttonRegister = new Button(this, nameof(_buttonRegister))
        {
            IsDisabled = MainMenu.ActiveNetworkStatus != NetworkStatus.Online || Options.Loaded && Options.BlockClientRegistrations,
            IsHidden = ClientContext.IsSinglePlayer,
            IsTabable = true,
            Text = Strings.MainMenu.Register,
        };
        _buttonRegister.Clicked += ButtonRegisterOnClicked;

        _buttonSettings = new Button(this, nameof(_buttonSettings))
        {
            IsTabable = true,
            Text = Strings.MainMenu.Settings,
        };
        _buttonSettings.Clicked += ButtonSettingsOnClicked;
        if (!string.IsNullOrEmpty(Strings.MainMenu.SettingsTooltip))
        {
            _buttonSettings.SetToolTipText(Strings.MainMenu.SettingsTooltip);
        }

        _buttonStart = new Button(this, nameof(_buttonStart))
        {
            IsTabable = true,
            IsVisible = ClientContext.IsSinglePlayer,
            Text = Strings.MainMenu.Start,
        };
        _buttonStart.Clicked += ButtonStartOnClicked;
    }

    private void ButtonCreditsOnClicked(Base sender, ClickedEventArgs arguments) =>
        _mainMenu.SwitchToWindow<CreditsWindow>();

    private static void ButtonExitOnClicked(Base sender, ClickedEventArgs arguments)
    {
        Log.Info("User clicked exit button.");
        Globals.IsRunning = false;
    }

    #region Login

    private void ButtonLoginOnClicked(Base sender, ClickedEventArgs arguments)
    {
        if (Networking.Network.InterruptDisconnectsIfConnected())
        {
            _mainMenu.SwitchToWindow<LoginWindow>();
        }
        else
        {
            _buttonLogin.IsDisabled = Globals.WaitingOnServer;
            AddLoginEvents();
            Networking.Network.TryConnect();
        }
    }

    private void AddLoginEvents()
    {
        MainMenu.ReceivedConfiguration += LoginConnected;
        Networking.Network.Socket.ConnectionFailed += LoginConnectionFailed;
        Networking.Network.Socket.Disconnected += LoginDisconnected;
    }

    private void RemoveLoginEvents()
    {
        MainMenu.ReceivedConfiguration -= LoginConnected;
        Networking.Network.Socket.ConnectionFailed -= LoginConnectionFailed;
        Networking.Network.Socket.Disconnected -= LoginDisconnected;
    }

    private void LoginConnectionFailed(INetworkLayerInterface nli, ConnectionEventArgs args, bool denied) => RemoveLoginEvents();
    private void LoginDisconnected(INetworkLayerInterface nli, ConnectionEventArgs args) => RemoveLoginEvents();
    private void LoginConnected(object? sender, EventArgs eventArgs)
    {
        RemoveLoginEvents();
        _mainMenu.SwitchToWindow<LoginWindow>();
    }

    #endregion Login

    #region Register

    private void ButtonRegisterOnClicked(Base sender, ClickedEventArgs arguments)
    {
        if (Networking.Network.InterruptDisconnectsIfConnected())
        {
            _mainMenu.SwitchToWindow<RegisterWindow>();
        }
        else
        {
            _buttonRegister.IsDisabled = Globals.WaitingOnServer;
            AddRegisterEvents();
            Networking.Network.TryConnect();
        }
    }

    private void AddRegisterEvents()
    {
        MainMenu.ReceivedConfiguration += RegisterConnected;
        Networking.Network.Socket.ConnectionFailed += RegisterConnectionFailed;
        Networking.Network.Socket.Disconnected += RegisterDisconnected;
    }

    private void RemoveRegisterEvents()
    {
        MainMenu.ReceivedConfiguration -= RegisterConnected;
        Networking.Network.Socket.ConnectionFailed -= RegisterConnectionFailed;
        Networking.Network.Socket.Disconnected -= RegisterDisconnected;
    }

    private void RegisterConnectionFailed(INetworkLayerInterface nli, ConnectionEventArgs args, bool denied) => RemoveRegisterEvents();
    private void RegisterDisconnected(INetworkLayerInterface nli, ConnectionEventArgs args) => RemoveRegisterEvents();
    private void RegisterConnected(object? sender, EventArgs eventArgs)
    {
        RemoveRegisterEvents();
        _mainMenu.SwitchToWindow<RegisterWindow>();
    }

    #endregion Register

    private void ButtonSettingsOnClicked(Base sender, ClickedEventArgs arguments) =>
        _mainMenu.SettingsButton_Clicked(sender, arguments);

    private void ButtonStartOnClicked(Base sender, ClickedEventArgs arguments)
    {
        Hide();
        Networking.Network.TryConnect();
        const string singleplayer = "singleplayer";
        PacketSender.SendLogin(singleplayer, Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(singleplayer))));
    }

    internal void Reset()
    {
        _buttonSettings.Show();
    }

    internal void Update()
    {
        if (Networking.Network.IsConnected)
        {
            _buttonLogin.IsDisabled = Globals.WaitingOnServer;
            _buttonRegister.IsDisabled = Globals.WaitingOnServer;
        }
    }

    internal void UpdateDisabled()
    {
        _buttonLogin.IsDisabled = MainMenu.ActiveNetworkStatus != NetworkStatus.Online;
        _buttonRegister.IsDisabled = MainMenu.ActiveNetworkStatus != NetworkStatus.Online ||
                                     Options.Loaded && Options.BlockClientRegistrations;
    }
}