using System;
using System.Windows.Forms;

using Intersect.Editor.Localization;
using Intersect.GameObjects;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventCommandSetClass : UserControl
    {

        private readonly FrmEvent mEventEditor;

        private SetClassCommand mMyCommand;

        public EventCommandSetClass(SetClassCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            InitLocalization();
            cmbClass.Items.Clear();
            cmbClass.Items.AddRange(ClassBase.Names);
            cmbClass.SelectedIndex = ClassBase.ListIndex(mMyCommand.ClassId);
        }

        private void InitLocalization()
        {
            grpSetClass.Text = Strings.EventSetClass.title;
            lblClass.Text = Strings.EventSetClass.label;
            btnSave.Text = Strings.EventSetClass.okay;
            btnCancel.Text = Strings.EventSetClass.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbClass.SelectedIndex > -1)
            {
                mMyCommand.ClassId = ClassBase.IdFromList(cmbClass.SelectedIndex);
            }

            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }

    }

}
