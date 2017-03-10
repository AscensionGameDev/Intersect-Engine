
using System;
using System.Windows.Forms;
using Intersect_Library.GameObjects.Events;
using Intersect_Library.Localization;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_GotoLabel : UserControl
    {
        private EventCommand _myCommand;
        private readonly FrmEvent _eventEditor;
        public EventCommand_GotoLabel(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            InitLocalization();
            txtGotoLabel.Text = _myCommand.Strs[0];
        }

        private void InitLocalization()
        {
            grpGotoLabel.Text = Strings.Get("eventgotolabel", "title");
            lblGotoLabel.Text = Strings.Get("eventgotolabel", "label");
            btnSave.Text = Strings.Get("eventgotolabel", "okay");
            btnCancel.Text = Strings.Get("eventgotolabel", "cancel");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _myCommand.Strs[0] = txtGotoLabel.Text;
            _eventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _eventEditor.CancelCommandEdit();
        }
    }
}
