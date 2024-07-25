using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Interface.Game;
using Intersect.Client.Localization;

namespace Intersect.Client.Interface.Shared;

public partial class ErrorWindow
{
    private readonly List<InputBox> _errorWindows = [];

    public ErrorWindow(Canvas gameCanvas, Canvas menuCanvas, string error, string header)
    {
        CreateErrorWindow(gameCanvas, error, header, GameContentManager.UI.InGame);
        CreateErrorWindow(menuCanvas, error, header, GameContentManager.UI.Menu);
    }

    private void CreateErrorWindow(Canvas canvas, string error, string header, GameContentManager.UI stage)
    {
        var window = new InputBox(
            title: header,
            prompt: error,
            modal: false,
            inputType: InputBox.InputType.OkayOnly,
            onSuccess: (sender, e) =>
            {
                foreach (var window in _errorWindows)
                {
                    window.Dispose();
                }
            },
            maxQuantity: 0,
            parent: canvas,
            stage: stage
        );

        _errorWindows.Add(window);
    }
}

public partial class ErrorHandler
{
    private readonly List<ErrorWindow> _windows = [];
    private readonly Canvas _gameCanvas;
    private readonly Canvas _menuCanvas;

    public ErrorHandler(Canvas menuCanvas, Canvas gameCanvas)
    {
        _gameCanvas = gameCanvas;
        _menuCanvas = menuCanvas;
    }

    public void Update()
    {
        while (Interface.TryDequeueErrorMessage(out var message))
        {
            _windows.Add(
                new ErrorWindow(
                    _gameCanvas,
                    _menuCanvas,
                    message.Value,
                    string.IsNullOrWhiteSpace(message.Key) ? Strings.Errors.Title.ToString() : message.Key
                )
            );
        }

        _windows.Clear();
    }
}
