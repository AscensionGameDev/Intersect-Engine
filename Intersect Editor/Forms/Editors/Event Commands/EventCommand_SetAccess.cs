using System;
using System.Windows.Forms;
using Intersect.GameObjects.Events;
using Intersect.Editor.Classes.Localization;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommandSetAccess : UserControl
    {
        private readonly FrmEvent mEventEditor;
        private EventCommand mMyCommand;

        public EventCommandSetAccess(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            InitLocalization();
            cmbAccess.SelectedIndex = mMyCommand.Ints[0];
        }

        private void InitLocalization()
        {
            grpSetAccess.Text = Strings.eventsetaccess.title;
            lblAccess.Text = Strings.eventsetaccess.label;
            cmbAccess.Items.Clear();
            cmbAccess.Items.Add(Strings.eventsetaccess.access0);
            cmbAccess.Items.Add(Strings.eventsetaccess.access1);
            cmbAccess.Items.Add(Strings.eventsetaccess.access2);
            btnSave.Text = Strings.eventsetaccess.okay;
            btnCancel.Text = Strings.eventsetaccess.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Ints[0] = cmbAccess.SelectedIndex;
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }
    }
}