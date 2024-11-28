using System.Reflection;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.MonoGame.NativeInterop;
using Intersect.Logging;
using Steamworks;

namespace Intersect.Client.ThirdParty;

public static partial class Steam
{
    private static readonly FunctionLoader Loader = new(KnownLibrary.steam_api);

    private delegate bool SteamAPI_InitSafe_d();

    private static readonly SteamAPI_InitSafe_d SteamAPI_InitSafe =
        Loader.LoadFunction<SteamAPI_InitSafe_d>(nameof(SteamAPI_InitSafe));

    private delegate bool CSteamAPIContext_Init_d();

    private static readonly CSteamAPIContext_Init_d CSteamAPIContext_Init =
        typeof(SteamAPI).Assembly.GetType("Steamworks.CSteamAPIContext")
            ?.GetMethod("Init", BindingFlags.NonPublic | BindingFlags.Static)
            ?.CreateDelegate<CSteamAPIContext_Init_d>() ??
        throw new MissingMethodException("Steamworks.CSteamAPIContext", "Init");

    private delegate void CallbackDispatcher_Initialize_d();

    private static readonly CallbackDispatcher_Initialize_d CallbackDispatcher_Initialize =
        typeof(CallbackDispatcher).GetMethod("Initialize", BindingFlags.NonPublic | BindingFlags.Static)
            ?.CreateDelegate<CallbackDispatcher_Initialize_d>() ??
        throw new MissingMethodException(
            typeof(CallbackDispatcher).FullName ?? nameof(CallbackDispatcher),
            "Initialize"
        );

    static Steam()
    {
        if (!SupportedAttribute.IsPresent())
        {
            return;
        }

        try
        {
            var initializing = SteamAPI_InitSafe();
            initializing &= CSteamAPIContext_Init();
            if (initializing)
            {
                CallbackDispatcher_Initialize();
            }

            Language = SteamApps.GetCurrentGameLanguage();
            Languages = SteamApps.GetAvailableGameLanguages().Split(',');
            SteamDeck = SteamUtils.IsSteamRunningOnSteamDeck();

            Initialized = initializing;
        }
        catch (Exception exception)
        {
            Log.Error(exception);
            Initialized = false;
        }
    }

    public static bool Initialized { get; }

    public static string Language { get; } = string.Empty;

    public static string[] Languages { get; } = Array.Empty<string>();

    public static bool SteamDeck { get; }

    private static Callback<GamepadTextInputDismissed_t>? _callbackGamepadTextInputDismissed;
    private static Callback<FloatingGamepadTextInputDismissed_t>? _callbackFloatingGamepadTextInputDismissed;

    public static bool ShowKeyboard(
        Action<string?> inputHandler,
        string description,
        string? existingInput,
        bool password,
        uint maxLength = 1024,
        Rectangle? inputBounds = default
    )
    {
        if (!Initialized)
        {
            return true;
        }

        bool inputVisible;
        if (inputBounds.HasValue)
        {
            var bounds = inputBounds.Value;
            inputVisible = SteamUtils.ShowFloatingGamepadTextInput(
                EFloatingGamepadTextInputMode.k_EFloatingGamepadTextInputModeModeSingleLine,
                bounds.X,
                bounds.Y,
                bounds.Width,
                bounds.Height
            );

            if (!inputVisible)
            {
                return false;
            }

            _callbackFloatingGamepadTextInputDismissed = Callback<FloatingGamepadTextInputDismissed_t>.Create(
                data =>
                {
                    _callbackFloatingGamepadTextInputDismissed = default;

                    var length = SteamUtils.GetEnteredGamepadTextLength();
                    if (length > 0 && SteamUtils.GetEnteredGamepadTextInput(out var enteredText, length))
                    {
                        inputHandler(enteredText);
                    }
                    else
                    {
                        inputHandler(default);
                    }
                }
            );
        }
        else
        {
            inputVisible = SteamUtils.ShowGamepadTextInput(
                password
                    ? EGamepadTextInputMode.k_EGamepadTextInputModePassword
                    : EGamepadTextInputMode.k_EGamepadTextInputModeNormal,
                EGamepadTextInputLineMode.k_EGamepadTextInputLineModeSingleLine,
                description,
                Math.Max(1, maxLength),
                existingInput ?? string.Empty
            );

            if (!inputVisible)
            {
                return false;
            }

            _callbackGamepadTextInputDismissed = Callback<GamepadTextInputDismissed_t>.Create(
                data =>
                {
                    _callbackGamepadTextInputDismissed = default;

                    if (data.m_bSubmitted)
                    {
                        var length = SteamUtils.GetEnteredGamepadTextLength();
                        if (SteamUtils.GetEnteredGamepadTextInput(out var enteredText, length))
                        {
                            inputHandler(enteredText);
                            return;
                        }
                    }

                    inputHandler(default);
                }
            );
        }

        return true;
    }

    public static void PumpEvents()
    {
        if (!Initialized)
        {
            return;
        }

        SteamAPI.RunCallbacks();
    }
}