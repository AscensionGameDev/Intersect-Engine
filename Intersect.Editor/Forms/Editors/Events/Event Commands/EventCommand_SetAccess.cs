using System;
using System.Windows.Forms;

using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventCommandSetAccess : UserControl
    {

        private readonly FrmEvent mEventEditor;

        private SetAccessCommand mMyCommand;

        public EventCommandSetAccess(SetAccessCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            InitLocalization();
            cmbAccess.SelectedIndex = (int) mMyCommand.Access;
        }

        private void InitLocalization()
        {
            grpSetAccess.Text = Strings.EventSetAccess.title;
            lblAccess.Text = Strings.EventSetAccess.label;
            cmbAccess.Items.Clear();
            cmbAccess.Items.Add(Strings.EventSetAccess.access0);
            cmbAccess.Items.Add(Strings.EventSetAccess.access1);
            cmbAccess.Items.Add(Strings.EventSetAccess.access2);
            btnSave.Text = Strings.EventSetAccess.okay;
            btnCancel.Text = Strings.EventSetAccess.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Access = (Access) cmbAccess.SelectedIndex;
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }

    }

}
