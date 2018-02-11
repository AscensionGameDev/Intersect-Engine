using System;
using System.Windows.Forms;
using Intersect.GameObjects.Events;
using Intersect.Localization;

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
            grpSetAccess.Text = Strings.Get("eventsetaccess", "title");
            lblAccess.Text = Strings.Get("eventsetaccess", "label");
            cmbAccess.Items.Clear();
            cmbAccess.Items.Add(Strings.Get("eventsetaccess", "access0"));
            cmbAccess.Items.Add(Strings.Get("eventsetaccess", "access1"));
            cmbAccess.Items.Add(Strings.Get("eventsetaccess", "access2"));
            btnSave.Text = Strings.Get("eventsetaccess", "okay");
            btnCancel.Text = Strings.Get("eventsetaccess", "cancel");
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