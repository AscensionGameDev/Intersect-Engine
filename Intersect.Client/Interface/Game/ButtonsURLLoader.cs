using System.Diagnostics;
using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Configuration;

namespace Intersect.Client.Interface.Game;
public partial class ButtonsURLLoader
{ 
    private readonly ImagePanel _panelButtons;
    private readonly List<Button> _buttons;
    private readonly List<string> _urls;
    public ButtonsURLLoader(Canvas gameCanvas)
    {
        List<string> buttons = ClientConfiguration.Instance.ButtonsURL;
        _buttons = new List<Button>();
        _buttons.Clear();
        _urls = new List<string>();
        _urls.Clear();

        _panelButtons = new ImagePanel(gameCanvas, "ButtonsURL");
        _panelButtons.RenderColor = new Color(0, 0, 0, 0);
        _panelButtons.Show();

        for (int i=0; i<buttons.Count; i++)
        {
            string[] tokens = buttons[i].Split('-');
            Button _button = new Button(_panelButtons, tokens[0]);
            _button.Clicked += buttonURL_Clicked;
            _button.Text = tokens[0];
            _button.Show();
            _buttons.Add(_button);
            _urls.Add(tokens[1]);
        }

        _panelButtons.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
    }

    void buttonURL_Clicked(Base sender, ClickedEventArgs arguments)
    {
        for (var i = 0; i < _buttons.Count; i++)
        {
            if (_buttons[i] == sender)
            {
                ProcessStartInfo psInfo = new ProcessStartInfo
                {
                    FileName = _urls[i],
                    UseShellExecute = true
                };
                Process.Start(psInfo);
                return;
            }
        }
    }
}