using System.Collections.Concurrent;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Localization;

namespace Intersect.Client.Interface.Shared;

public enum AlertType
{
    Unknown,
    Information,
    Error,
    Warning,
}

public record struct Alert(string Message, string? Title = default, AlertType Type = AlertType.Unknown);

public class AlertWindow : InputBox
{
    private static readonly HashSet<AlertWindow> _instances = [];
    private readonly GwenEventHandler<EventArgs>? _handleCancel;

    private AlertWindow(
        string title,
        string message,
        AlertType alertType,
        object? userData = default,
        GwenEventHandler<InputSubmissionEventArgs>? handleSubmit = default,
        GwenEventHandler<EventArgs>? handleCancel = default,
        InputType inputType = InputType.Okay
    ) : base(
        name: nameof(AlertWindow),
        title: title,
        prompt: message,
        inputType: inputType,
        onSubmit: handleSubmit,
        onCancel: handleCancel,
        userData: userData
    )
    {
        _handleCancel = handleCancel;

        Icon = GameContentManager.Current.GetTexture(
            TextureType.Gui,
            $"icon.alert.{alertType.ToString().ToLowerInvariant()}.png"
        );

        var iconContainerMargin = IconContainer.Margin;
        IconContainer.Margin = iconContainerMargin with { Left = iconContainerMargin.Left + 4 };

        var titleLabelMargin = TitleLabel.Margin;
        TitleLabel.Margin = titleLabelMargin with { Left = titleLabelMargin.Left + 4 };

        // Subscribe to the Closed event to handle X button clicks
        Closed += OnWindowClosed;
    }

    private void OnWindowClosed(Base sender, EventArgs args)
    {
        // Trigger the cancel handler when window is closed via X button
        _handleCancel?.Invoke(this, args);
    }

    protected override void OnCanceled(Base sender, EventArgs args)
    {
        Remove();
    }

    protected override void OnSubmitted(Base sender, InputSubmissionEventArgs args)
    {
        Remove();
    }

    private void Remove()
    {
        _instances.Remove(this);
        if (Parent is { } parent)
        {
            parent.RemoveChild(this, true);
        }
        else
        {
            Dispose();
        }
    }

    public static void OpenPendingAlertWindowsFrom(IProducerConsumerCollection<Alert> source)
    {
        while (source.TryTake(out var alert))
        {
            Open(message: alert.Message, title: alert.Title, alertType: alert.Type);
        }
    }

    public static void Open(
        string message,
        string? title = default,
        AlertType alertType = AlertType.Unknown,
        object? userData = default,
        GwenEventHandler<InputSubmissionEventArgs>? handleSubmit = default,
        GwenEventHandler<EventArgs>? handleCancel = default,
        InputType inputType = InputType.Okay
    )
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            if (Strings.Alerts.Titles.TryGetValue(alertType, out var alertTitle))
            {
                title = alertTitle;
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                title = Strings.Alerts.FallbackTitle;
            }
        }

        _instances.Add(
            new AlertWindow(
                title: string.IsNullOrWhiteSpace(title) ? Strings.Errors.Title.ToString() : title,
                message: message,
                alertType: alertType,
                userData: userData,
                handleSubmit: handleSubmit,
                handleCancel: handleCancel,
                inputType: inputType
            )
        );
    }

    public static void CloseAll()
    {
        var errorWindowsToClose = _instances.ToArray();
        _instances.Clear();

        foreach (var errorWindow in errorWindowsToClose)
        {
            errorWindow.Remove();
        }
    }
}