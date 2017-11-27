using System;
using System.Windows.Forms;
using Intersect.GameObjects.Events;
using Intersect.Editor.Classes.Localization;

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
            grpChangeGender.Text = Strings.eventchangegender.title;
            cmbGender.Items.Clear();
            for (int i = 0; i < Strings.eventchangegender.genders.Length; i++)
            {
                cmbGender.Items.Add(Strings.eventchangegender.genders[i]);
            }
            lblGender.Text = Strings.eventchangegender.label;
            btnSave.Text = Strings.eventchangegender.okay;
            btnCancel.Text = Strings.eventchangegender.cancel;
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