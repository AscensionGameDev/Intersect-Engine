using System;
using System.Windows.Forms;

using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventCommandChangeGender : UserControl
    {

        private readonly FrmEvent mEventEditor;

        private ChangeGenderCommand mMyCommand;

        public EventCommandChangeGender(ChangeGenderCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            InitLocalization();
            cmbGender.SelectedIndex = (int) mMyCommand.Gender;
        }

        private void InitLocalization()
        {
            grpChangeGender.Text = Strings.EventChangeGender.title;
            cmbGender.Items.Clear();
            for (var i = 0; i < Strings.EventChangeGender.genders.Count; i++)
            {
                cmbGender.Items.Add(Strings.EventChangeGender.genders[i]);
            }

            lblGender.Text = Strings.EventChangeGender.label;
            btnSave.Text = Strings.EventChangeGender.okay;
            btnCancel.Text = Strings.EventChangeGender.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Gender = (Gender) cmbGender.SelectedIndex;
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }

    }

}
