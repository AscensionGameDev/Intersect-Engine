using Gwen.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intersect_Client.Classes.UI.Game
{
    public class InputBox
    {
        private WindowControl _myWindow;
        private event EventHandler _okayEventHandler;
        private event EventHandler _cancelEventHandler;
        public float Value;
        public int Slot;
        private TextBoxNumeric _textbox;

        public InputBox(string title, string prompt, bool modal, EventHandler okayClicked, EventHandler cancelClicked, int slot, bool textInput)
        {

            _okayEventHandler = okayClicked;
            _cancelEventHandler = cancelClicked;
            Slot = slot;

            _myWindow = new WindowControl(Gui._GameGui.GameCanvas, title, modal);
            _myWindow.SetSize(300, 120);
            _myWindow.SetPosition(Graphics.ScreenWidth / 2 - 380 / 2, Graphics.ScreenHeight / 2 - 200 / 2);
            _myWindow.IsClosable = false;
            _myWindow.DisableResizing();

            Label promptLabel = new Label(_myWindow);
            promptLabel.SetText(prompt);
            promptLabel.SetPosition(300 / 2 - promptLabel.TextWidth / 2, 10);

            int y = promptLabel.Y + promptLabel.Height + 20;
            if (textInput)
            {
                _textbox = new TextBoxNumeric(_myWindow);
                _textbox.SetSize(150, _textbox.Height);
                _textbox.SetPosition(300 / 2 - 150 / 2, promptLabel.Y + promptLabel.TextHeight + 6);
                y = _textbox.Y + _textbox.Height + 8;
            }

            Button okayBtn =  new Button(_myWindow);
            okayBtn.SetSize(50, 24);
            okayBtn.SetText("Okay");
            okayBtn.SetPosition(300/2 - 108 /2, y);
            okayBtn.Clicked += okayBtn_Clicked; ;

            Button cancelBtn = new Button(_myWindow);
            cancelBtn.SetSize(50, 24);
            cancelBtn.SetText("Cancel");
            cancelBtn.SetPosition(300 / 2 - 108 / 2 + 58, y);
            cancelBtn.Clicked += cancelBtn_Clicked; ;

        }

        void cancelBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (_textbox != null) { Value = _textbox.Value; }
            if (_cancelEventHandler != null) { _cancelEventHandler(this, EventArgs.Empty); }
            Dispose();
        }

        void okayBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (_textbox != null) { Value = _textbox.Value; }
            if (_okayEventHandler != null) { _okayEventHandler(this, EventArgs.Empty); }
            Dispose();
        }

        private void Dispose()
        {
            _myWindow.Close();
            Gui._GameGui.GameCanvas.RemoveChild(_myWindow,false);
            _myWindow.Dispose();
        }


    }
}
