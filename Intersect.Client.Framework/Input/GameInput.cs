using System.Collections.Immutable;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Core;
using Intersect.Framework.Reflection;
using Microsoft.Extensions.Logging;

namespace Intersect.Client.Framework.Input;

public abstract partial class GameInput : IGameInput
{
    private static GameInput? _current;

    public static GameInput Current
    {
        get => _current ?? throw new InvalidOperationException($"{nameof(GameInput)} not initialized");
    }

    private readonly HashSet<IControlsProvider> _controlsProviders =
        [new BuiltinControlsProvider(), new HotkeyControlsProvider()];

    protected GameInput(bool forceGlobal = false)
    {
        if (forceGlobal)
        {
            _current = this;
        }
        else
        {
            _current ??= this;
        }

        Options.OptionsLoaded += OnOptionsOnOptionsLoaded;
    }

    private void OnOptionsOnOptionsLoaded(Options options)
    {
        foreach (var controlsProvider in _controlsProviders)
        {
            controlsProvider.ReloadFromOptions(options);
        }

        ControlSet.ReloadFromOptions(options);
    }

    public abstract IControlSet ControlSet { get; set; }

    public IReadOnlySet<IControlsProvider> ControlsProviders => _controlsProviders.ToImmutableHashSet();

    public Control[] AllControls => ControlsProviders.SelectMany(provider => provider.Controls)
        .Distinct()
        .ToArray();

    public abstract bool MouseHitInterface { get; }

    public bool AddControlsProviders(params IControlsProvider[] controlsProviders)
    {
        var success = true;

        foreach (var controlsProvider in controlsProviders)
        {
            try
            {
                if (!_controlsProviders.Add(controlsProvider))
                {
                    success &= _controlsProviders.Contains(controlsProvider);
                }
            }
            catch (Exception exception)
            {
                ApplicationContext.Context.Value?.Logger.LogError(
                    exception,
                    "Failed to add controls provider of type {ControlsProviderType} to input instance",
                    controlsProvider.GetType().GetName(qualified: true)
                );
                success = false;
            }
        }

        return success;
    }

    public bool RemoveControlsProviders(params IControlsProvider[] controlsProviders)
    {
        var success = true;

        foreach (var controlsProvider in controlsProviders)
        {
            try
            {
                if (!_controlsProviders.Remove(controlsProvider))
                {
                    success &= !_controlsProviders.Contains(controlsProvider);
                }
            }
            catch (Exception exception)
            {
                ApplicationContext.Context.Value?.Logger.LogError(
                    exception,
                    "Failed to remove controls provider of type {ControlsProviderType} to input instance",
                    controlsProvider.GetType().GetName(qualified: true)
                );
                success = false;
            }
        }

        return success;
    }

    public abstract bool MouseButtonDown(MouseButton mb);

    public abstract bool IsKeyDown(Keys key);

    public Pointf MousePosition => GetMousePosition();

    public abstract Pointf GetMousePosition();

    public abstract void Update(TimeSpan elapsed);

    public abstract void OpenKeyboard(
        KeyboardType type,
        string text,
        bool autoCorrection,
        bool multiLine,
        bool secure
    );

    public abstract void OpenKeyboard(
        KeyboardType keyboardType,
        Action<string?> inputHandler,
        string description,
        string text,
        bool multiline = false,
        uint maxLength = 1024,
        Rectangle? inputBounds = default
    );

}
