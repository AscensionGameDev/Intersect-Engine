using System;
using System.Windows.Forms;

using Intersect.Editor.Localization;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventCommandWait : UserControl
    {

        private readonly FrmEvent mEventEditor;

        private WaitCommand mMyCommand;

        public EventCommandWait(WaitCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            InitLocalization();
            nudWait.Value = mMyCommand.Time;
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
            mMyCommand.Time = (int) nudWait.Value;
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
