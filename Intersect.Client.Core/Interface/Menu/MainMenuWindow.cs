using System.Security.Cryptography;
using System.Text;
using Intersect.Client.Core;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Core;
using Intersect.Network;
using Intersect.Network.Events;
using Microsoft.Extensions.Logging;

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
        _buttonCredits.Clicked += _buttonCredits_Clicked;

        _buttonExit = new Button(this, nameof(_buttonExit))
        {
            IsTabable = true,
            Text = Strings.MainMenu.Exit,
        };
        _buttonExit.Clicked += _buttonExit_Clicked;

        _buttonLogin = new Button(this, nameof(_buttonLogin))
        {
            IsDisabled = MainMenu.ActiveNetworkStatus != NetworkStatus.Online,
            IsHidden = ClientContext.IsSinglePlayer,
            IsTabable = true,
            Text = Strings.MainMenu.Login,
        };
        _buttonLogin.Clicked += _buttonLogin_Clicked;

        _buttonRegister = new Button(this, nameof(_buttonRegister))
        {
            IsDisabled = MainMenu.ActiveNetworkStatus != NetworkStatus.Online || (Options.IsLoaded && Options.Instance.BlockClientRegistrations),
            IsHidden = ClientContext.IsSinglePlayer,
            IsTabable = true,
            Text = Strings.MainMenu.Register,
        };
        _buttonRegister.Clicked += _buttonRegister_Clicked;

        _buttonSettings = new Button(this, nameof(_buttonSettings))
        {
            IsTabable = true,
            Text = Strings.MainMenu.Settings,
        };
        _buttonSettings.Clicked += _buttonSettings_Clicked;

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
        _buttonStart.Clicked += _buttonStart_Clicked;
    }

    private void _buttonCredits_Clicked(Base sender, ClickedEventArgs arguments) => _mainMenu.SwitchToWindow<CreditsWindow>();

    private static void _buttonExit_Clicked(Base sender, ClickedEventArgs arguments)
    {
        ApplicationContext.Context.Value?.Logger.LogInformation("User clicked exit button.");
        Globals.IsRunning = false;
    }

    #region Login

    private void _buttonLogin_Clicked(Base sender, ClickedEventArgs arguments)
    {
        if (Networking.Network.InterruptDisconnectsIfConnected())
        {
            _mainMenu.SwitchToWindow<LoginWindow>();
        }
        else
        {
            _buttonLogin.IsDisabled = Globals.WaitingOnServer;
            _addLoginEvents();
            Networking.Network.TryConnect();
        }
    }

    private void _addLoginEvents()
    {
        MainMenu.ReceivedConfiguration += _loginConnected;
        Networking.Network.Socket.ConnectionFailed += _loginConnectionFailed;
        Networking.Network.Socket.Disconnected += _loginDisconnected;
    }

    private void _removeLoginEvents()
    {
        MainMenu.ReceivedConfiguration -= _loginConnected;
        Networking.Network.Socket.ConnectionFailed -= _loginConnectionFailed;
        Networking.Network.Socket.Disconnected -= _loginDisconnected;
    }

    private void _loginConnectionFailed(INetworkLayerInterface nli, ConnectionEventArgs args, bool denied) => _removeLoginEvents();

    private void _loginDisconnected(INetworkLayerInterface nli, ConnectionEventArgs args) => _removeLoginEvents();

    private void _loginConnected(object? sender, EventArgs eventArgs)
    {
        _removeLoginEvents();
        _mainMenu.SwitchToWindow<LoginWindow>();
    }

    #endregion Login

    #region Register

    private void _buttonRegister_Clicked(Base sender, ClickedEventArgs arguments)
    {
        if (Networking.Network.InterruptDisconnectsIfConnected())
        {
            _mainMenu.SwitchToWindow<RegistrationWindow>();
        }
        else
        {
            _buttonRegister.IsDisabled = Globals.WaitingOnServer;
            _addRegisterEvents();
            Networking.Network.TryConnect();
        }
    }

    private void _addRegisterEvents()
    {
        MainMenu.ReceivedConfiguration += _registerConnected;
        Networking.Network.Socket.ConnectionFailed += _registerConnectionFailed;
        Networking.Network.Socket.Disconnected += _registerDisconnected;
    }

    private void _removeRegisterEvents()
    {
        MainMenu.ReceivedConfiguration -= _registerConnected;
        Networking.Network.Socket.ConnectionFailed -= _registerConnectionFailed;
        Networking.Network.Socket.Disconnected -= _registerDisconnected;
    }

    private void _registerConnectionFailed(INetworkLayerInterface nli, ConnectionEventArgs args, bool denied) => _removeRegisterEvents();

    private void _registerDisconnected(INetworkLayerInterface nli, ConnectionEventArgs args) => _removeRegisterEvents();

    private void _registerConnected(object? sender, EventArgs eventArgs)
    {
        _removeRegisterEvents();
        _mainMenu.SwitchToWindow<RegistrationWindow>();
    }

    #endregion Register

    private void _buttonSettings_Clicked(Base sender, ClickedEventArgs arguments) => _mainMenu.SettingsButton_Clicked();

    private void _buttonStart_Clicked(Base sender, ClickedEventArgs arguments)
    {
        Hide();
        Networking.Network.TryConnect();
        const string singleplayer = "singleplayer";
        PacketSender.SendLogin(singleplayer, Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(singleplayer))));
    }

    internal void Reset() => _buttonSettings.Show();

    internal void Update()
    {
        if (Networking.Network.IsConnected)
        {
            _buttonLogin.IsDisabled = Globals.WaitingOnServer;
            _buttonRegister.IsDisabled = Globals.WaitingOnServer;
        }
        else
        {
            UpdateDisabled();
        }
    }


    internal void UpdateDisabled()
    {
        var networkStatus = MainMenu.ActiveNetworkStatus;
        var isOffline = networkStatus != NetworkStatus.Online;
        _buttonLogin.IsDisabled = isOffline;
        _buttonRegister.IsDisabled = isOffline || (Options.IsLoaded && Options.Instance.BlockClientRegistrations);
    }
}