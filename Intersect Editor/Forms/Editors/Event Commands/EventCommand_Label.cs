
using System;
using System.Windows.Forms;
using Intersect_Library.GameObjects.Events;
using Intersect_Library.Localization;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_Label : UserControl
    {
        private EventCommand _myCommand;
        private readonly FrmEvent _eventEditor;
        public EventCommand_Label(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            InitLocalization();
            txtLabel.Text = _myCommand.Strs[0];
        }

        private void InitLocalization()
        {
            grpLabel.Text = Strings.Get("eventlabel", "title");
            lblLabel.Text = Strings.Get("eventlabel", "label");
            btnSave.Text = Strings.Get("eventlabel", "okay");
            btnCancel.Text = Strings.Get("eventlabel", "cancel");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _myCommand.Strs[0] = txtLabel.Text;
            _eventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _eventEditor.CancelCommandEdit();
        }
    }
}
