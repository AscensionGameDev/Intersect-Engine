using System;
using System.Windows.Forms;
using DarkUI.Controls;
using Intersect.GameObjects.Events;
using Intersect.Localization;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_Wait : UserControl
    {
        private readonly FrmEvent _eventEditor;
        private EventCommand _myCommand;

        public EventCommand_Wait(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            InitLocalization();
            nudWait.Value = _myCommand.Ints[0];
            lblWait.Text = Strings.Get("eventwait", "label");
        }

        private void InitLocalization()
        {
            grpWait.Text = Strings.Get("eventwait", "title");
            lblWait.Text = Strings.Get("eventwait", "label");
            btnSave.Text = Strings.Get("eventwait", "okay");
            btnCancel.Text = Strings.Get("eventwait", "cancel");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _myCommand.Ints[0] = (int)nudWait.Value;
            _eventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _eventEditor.CancelCommandEdit();
        }

        private void nudWait_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}