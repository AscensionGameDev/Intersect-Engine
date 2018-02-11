using System;
using System.Windows.Forms;
using Intersect.GameObjects.Events;
using Intersect.Localization;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommandChangeGender : UserControl
    {
        private readonly FrmEvent mEventEditor;
        private EventCommand mMyCommand;

        public EventCommandChangeGender(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            InitLocalization();
            cmbGender.SelectedIndex = mMyCommand.Ints[0];
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
            mMyCommand.Ints[0] = cmbGender.SelectedIndex;
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }
    }
}