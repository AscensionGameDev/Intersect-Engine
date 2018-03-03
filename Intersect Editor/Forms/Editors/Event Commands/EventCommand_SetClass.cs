using System;
using System.Windows.Forms;
using Intersect.Editor.Core;
using Intersect.Editor.Localization;
using Intersect.Enums;
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
            cmbClass.Items.AddRange(Database.GetGameObjectList(GameObjectType.Class));
            cmbClass.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Class, mMyCommand.Ints[0]);
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
                mMyCommand.Ints[0] = Database.GameObjectIdFromList(GameObjectType.Class, cmbClass.SelectedIndex);
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }
    }
}