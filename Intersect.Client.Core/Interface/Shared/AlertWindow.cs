using System.Collections.Concurrent;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
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

    private AlertWindow(
        string title,
        string message,
        AlertType alertType,
        object? userData = default,
        EventHandler? handleSubmit = default,
        EventHandler? handleCancel = default,
        InputType inputType = InputType.OkayOnly
    ) : base(
        title,
        message,
        inputType,
        handleSubmit,
        handleCancel,
        userData
    )
    {
        Icon = GameContentManager.Current.GetTexture(
            TextureType.Gui,
            $"icon.alert.{alertType.ToString().ToLowerInvariant()}.png"
        );

        var iconContainerMargin = IconContainer.Margin;
        IconContainer.Margin = iconContainerMargin with { Left = iconContainerMargin.Left + 4 };

        var titleLabelMargin = TitleLabel.Margin;
        TitleLabel.Margin = titleLabelMargin with { Left = titleLabelMargin.Left + 4 };
    }

    protected override void OnCanceled(Base sender, EventArgs args)
    {
        Remove();
    }

    protected override void OnSubmitted(Base sender, EventArgs args)
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
        EventHandler? handleSubmit = default,
        EventHandler? handleCancel = default,
        InputType inputType = InputType.OkayOnly
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