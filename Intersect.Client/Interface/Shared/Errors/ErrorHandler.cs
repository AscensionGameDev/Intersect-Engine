using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Localization;

namespace Intersect.Client.Interface.Shared.Errors
{

    public partial class ErrorHandler
    {

        //Controls
        private readonly List<ErrorWindow> _windows = new();

        //Canvasses
        private readonly Canvas _gameCanvas;
        private readonly Canvas _menuCanvas;

        //Init
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
                        string.IsNullOrWhiteSpace(message.Key) ? Strings.Errors.title.ToString() : message.Key
                    )
                );
            }

            var windowsToRemove = _windows.Where(window => !window.Update()).ToArray();
            foreach (var window in windowsToRemove)
            {
                _windows.Remove(window);
            }
        }

    }

}
