using System;
using System.Windows.Forms;
using Intersect.GameObjects.Events;
using Intersect.Localization;

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
            lblWait.Text = Strings.Get("eventwait", "label");
        }

        private void InitLocalization()
        {
            grpWait.Text = Strings.Get("eventwait", "title");
            lblWait.Text = Strings.Get("eventwait", "label");
            btnSave.Text = Strings.Get("eventwait", "okay");
            btnCancel.Text = Strings.Get("eventwait", "cancel");
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