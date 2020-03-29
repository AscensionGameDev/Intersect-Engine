using System;
using System.Windows.Forms;

using Intersect.Editor.Localization;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventCommandLabel : UserControl
    {

        private readonly FrmEvent mEventEditor;

        private LabelCommand mMyCommand;

        public EventCommandLabel(LabelCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            InitLocalization();
            txtLabel.Text = mMyCommand.Label;
        }

        private void InitLocalization()
        {
            grpLabel.Text = Strings.EventLabel.title;
            lblLabel.Text = Strings.EventLabel.label;
            btnSave.Text = Strings.EventLabel.okay;
            btnCancel.Text = Strings.EventLabel.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Label = txtLabel.Text;
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }

    }

}
