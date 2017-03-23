using System;
using System.Windows.Forms;
using Intersect;
using Intersect.GameObjects.Events;
using Intersect.Localization;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_ChangeLevel : UserControl
    {
        private readonly FrmEvent _eventEditor;
        private EventCommand _myCommand;

        public EventCommand_ChangeLevel(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            if (_myCommand.Ints[0] <= 0 || _myCommand.Ints[0] > Options.MaxLevel) _myCommand.Ints[0] = 1;
            nudLevel.Maximum = Options.MaxLevel;
            nudLevel.Value = _myCommand.Ints[0];
            InitLocalization();
        }

        private void InitLocalization()
        {
            grpChangeLevel.Text = Strings.Get("eventchangelevel", "title");
            lblLevel.Text = Strings.Get("eventchangelevel", "label");
            btnSave.Text = Strings.Get("eventchangelevel", "okay");
            btnCancel.Text = Strings.Get("eventchangelevel", "cancel");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _myCommand.Ints[0] = (int) nudLevel.Value;
            _eventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _eventEditor.CancelCommandEdit();
        }
    }
}