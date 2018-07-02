using System;
using System.Windows.Forms;
using Intersect.Editor.Core;
using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommandSetClass : UserControl
    {
        private readonly FrmEvent mEventEditor;
        private EventCommand mMyCommand;

        public EventCommandSetClass(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            InitLocalization();
            cmbClass.Items.Clear();
            cmbClass.Items.AddRange(ClassBase.Names);
            cmbClass.SelectedIndex = ClassBase.ListIndex(mMyCommand.Guids[0]);
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
                mMyCommand.Guids[0] = ClassBase.IdFromList(cmbClass.SelectedIndex);
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }
    }
}