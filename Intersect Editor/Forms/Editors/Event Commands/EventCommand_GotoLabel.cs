using System;
using System.Windows.Forms;
using Intersect.GameObjects.Events;
using Intersect.Editor.Classes.Localization;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommandGotoLabel : UserControl
    {
        private readonly FrmEvent mEventEditor;
        private EventCommand mMyCommand;

        public EventCommandGotoLabel(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            InitLocalization();
            txtGotoLabel.Text = mMyCommand.Strs[0];
        }

        private void InitLocalization()
        {
            grpGotoLabel.Text = Strings.eventgotolabel.title;
            lblGotoLabel.Text = Strings.eventgotolabel.label;
            btnSave.Text = Strings.eventgotolabel.okay;
            btnCancel.Text = Strings.eventgotolabel.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Strs[0] = txtGotoLabel.Text;
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }
    }
}