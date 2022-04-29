using System;
using System.Windows.Forms;

using Intersect.Editor.Localization;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventCommandGotoLabel : UserControl
    {

        private readonly FrmEvent mEventEditor;

        private GoToLabelCommand mMyCommand;

        public EventCommandGotoLabel(GoToLabelCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            InitLocalization();
            txtGotoLabel.Text = mMyCommand.Label;
        }

        private void InitLocalization()
        {
            grpGotoLabel.Text = Strings.EventGotoLabel.title;
            lblGotoLabel.Text = Strings.EventGotoLabel.label;
            btnSave.Text = Strings.EventGotoLabel.okay;
            btnCancel.Text = Strings.EventGotoLabel.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Label = txtGotoLabel.Text;
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }

    }

}
