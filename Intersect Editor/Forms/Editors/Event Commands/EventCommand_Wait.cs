using System;
using System.Windows.Forms;
using Intersect.Editor.Localization;
using Intersect.GameObjects.Events;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommandWait : UserControl
    {
        private readonly FrmEvent mEventEditor;
        private EventCommand mMyCommand;

        public EventCommandWait(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            InitLocalization();
            nudWait.Value = mMyCommand.Ints[0];
            lblWait.Text = Strings.EventWait.label;
        }

        private void InitLocalization()
        {
            grpWait.Text = Strings.EventWait.title;
            lblWait.Text = Strings.EventWait.label;
            btnSave.Text = Strings.EventWait.okay;
            btnCancel.Text = Strings.EventWait.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Ints[0] = (int) nudWait.Value;
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }

        private void nudWait_ValueChanged(object sender, EventArgs e)
        {
        }
    }
}