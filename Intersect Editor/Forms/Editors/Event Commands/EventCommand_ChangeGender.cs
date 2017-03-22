
using System;
using System.Windows.Forms;
using Intersect.GameObjects.Events;
using Intersect.Localization;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_ChangeGender : UserControl
    {
        private EventCommand _myCommand;
        private readonly FrmEvent _eventEditor;
        public EventCommand_ChangeGender(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            InitLocalization();
            cmbGender.SelectedIndex = _myCommand.Ints[0];
        }

        private void InitLocalization()
        {
            grpChangeGender.Text = Strings.Get("eventchangegender", "title");
            cmbGender.Items.Clear();
            for (int i = 0; i < 2; i++)
            {
                cmbGender.Items.Add(Strings.Get("eventchangegender", "gender" + i));
            }
            lblGender.Text = Strings.Get("eventchangegender", "label");
            btnSave.Text = Strings.Get("eventchangegender", "okay");
            btnCancel.Text = Strings.Get("eventchangegender", "cancel");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _myCommand.Ints[0] = cmbGender.SelectedIndex;
            _eventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _eventEditor.CancelCommandEdit();
        }
    }
}
