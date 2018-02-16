using System;
using System.Windows.Forms;
using Intersect.GameObjects.Events;
using Intersect.Editor.Classes.Localization;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommandLabel : UserControl
    {
        private readonly FrmEvent mEventEditor;
        private EventCommand mMyCommand;

        public EventCommandLabel(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            InitLocalization();
            txtLabel.Text = mMyCommand.Strs[0];
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
            mMyCommand.Strs[0] = txtLabel.Text;
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }
    }
}